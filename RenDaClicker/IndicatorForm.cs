using System.Drawing.Imaging;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.Graphics.Gdi;
using Windows.Win32.UI.HiDpi;
using Windows.Win32.UI.WindowsAndMessaging;

namespace RenDaClicker {
	/// <summary>
	/// 連打中にマウスカーソルの近くに表示する目印(クリック透過・非アクティブ)。
	/// 透明色(TransparencyKey)ではなくピクセル単位のアルファで描画するため、
	/// アンチエイリアスの縁が残らず輪郭が滑らかになる。
	/// </summary>
	internal sealed class IndicatorForm : Form {

		private const int WS_EX_LAYERED = 0x00080000;
		private const int WS_EX_TRANSPARENT = 0x00000020;
		private const int WS_EX_NOACTIVATE = 0x08000000;
		private const int WS_EX_TOOLWINDOW = 0x00000080;

		/// <summary>目印の論理サイズ(96DPI基準)。</summary>
		private const int BaseSize = 16;

		/// <summary>カーソルからの論理オフセット(96DPI基準)。</summary>
		private const int BaseOffset = 18;

		/// <summary>現在の描画倍率(表示先モニタの DPI / 96)。</summary>
		private float scale = 1f;

		public IndicatorForm() {
			FormBorderStyle = FormBorderStyle.None;
			ShowInTaskbar = false;
			StartPosition = FormStartPosition.Manual;
			TopMost = true;
			// モニタごとに自前で拡大するため WinForms の自動スケーリングは無効化する
			AutoScaleMode = AutoScaleMode.None;
			Size = new Size( BaseSize, BaseSize );
		}

		/// <summary>目印の色を設定する。左クリックは赤・右クリックは青。</summary>
		public void SetMark( bool leftClick ) => MarkColor = MarkColorOf( leftClick );

		protected override bool ShowWithoutActivation => true;

		protected override CreateParams CreateParams {
			get {
				var cp = base.CreateParams;
				// クリック透過+フォーカスを奪わない
				cp.ExStyle |= WS_EX_LAYERED | WS_EX_TRANSPARENT | WS_EX_NOACTIVATE | WS_EX_TOOLWINDOW;
				return cp;
			}
		}

		/// <summary>目印の色(左クリックは赤・右クリックは青)。</summary>
		[System.ComponentModel.Browsable( false )]
		[System.ComponentModel.DesignerSerializationVisibility( System.ComponentModel.DesignerSerializationVisibility.Hidden )]
		public Color MarkColor {
			get;
			set {
				if( field != value ) {
					field = value;
					UpdateLayered();
				}
			}
		} = Color.Red;

		protected override void OnHandleCreated( EventArgs e ) {
			base.OnHandleCreated( e );
			UpdateLayered();
		}

		/// <summary>目印をピクセル単位のアルファ付きでウインドウへ転送する。</summary>
		private void UpdateLayered() {
			if( !IsHandleCreated ) {
				return;
			}

			using var bitmap = new Bitmap( Width, Height, PixelFormat.Format32bppArgb );
			using( var g = Graphics.FromImage( bitmap ) ) {
				g.Clear( Color.Transparent );
				DrawMark( g, new Point( 0, 0 ), MarkColor, scale );
			}
			Premultiply( bitmap );

			nint hBitmap = bitmap.GetHbitmap( Color.FromArgb( 0 ) );
			HDC screenDc = PInvoke.GetDC( HWND.Null );
			HDC memDc = PInvoke.CreateCompatibleDC( screenDc );
			HGDIOBJ oldBitmap = PInvoke.SelectObject( memDc, (HGDIOBJ)hBitmap );
			try {
				var blend = new BLENDFUNCTION {
					BlendOp = 0,               // AC_SRC_OVER
					BlendFlags = 0,
					SourceConstantAlpha = 255,
					AlphaFormat = 1,           // AC_SRC_ALPHA
				};
				var size = new SIZE( Width, Height );
				var source = new Point( 0, 0 );
				PInvoke.UpdateLayeredWindow( (HWND)Handle, screenDc, null, size, memDc, source,
					default, blend, UPDATE_LAYERED_WINDOW_FLAGS.ULW_ALPHA );
			} finally {
				PInvoke.SelectObject( memDc, oldBitmap );
				PInvoke.DeleteObject( (HGDIOBJ)hBitmap );
				PInvoke.DeleteDC( memDc );
				PInvoke.ReleaseDC( HWND.Null, screenDc );
			}
		}

		/// <summary>UpdateLayeredWindow が要求する乗算済みアルファへ変換する。</summary>
		private static void Premultiply( Bitmap bitmap ) {
			var rect = new Rectangle( 0, 0, bitmap.Width, bitmap.Height );
			var data = bitmap.LockBits( rect, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb );
			try {
				unsafe {
					for( int y = 0; y < data.Height; y++ ) {
						byte* line = (byte*)data.Scan0 + y * data.Stride;
						for( int x = 0; x < data.Width; x++ ) {
							byte* px = line + x * 4; // B, G, R, A
							int alpha = px[3];
							px[0] = (byte)( px[0] * alpha / 255 );
							px[1] = (byte)( px[1] * alpha / 255 );
							px[2] = (byte)( px[2] * alpha / 255 );
						}
					}
				}
			} finally {
				bitmap.UnlockBits( data );
			}
		}

		/// <summary>
		/// 目印を描画する(設定画面のプレビューと共用)。左上を <paramref name="location"/> とする16x16。
		/// 外側から順に、0.5pxの円 → 白いリング → 中央の色付きの丸。
		/// </summary>
		public static void DrawMark( Graphics g, Point location, Color markColor, float scale = 1f ) {
			g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
			g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
			var oldTransform = g.Transform;
			g.TranslateTransform( location.X, location.Y );
			g.ScaleTransform( scale, scale );
			float x = 0f;
			float y = 0f;

			// 小さい円はペンで描くと歪むため、大きい順に塗りつぶして重ねる
			using var brush = new SolidBrush( markColor );
			using var white = new SolidBrush( Color.White );

			// 一番外側:0.5pxの円(直径12px)
			g.FillEllipse( brush, x + 2f, y + 2f, 12f, 12f );
			// 白いリング(幅2.5px)
			g.FillEllipse( white, x + 2.5f, y + 2.5f, 11f, 11f );
			// 中央の色付きの丸(直径6px)
			g.FillEllipse( brush, x + 5f, y + 5f, 6f, 6f );

			g.Transform = oldTransform;
		}

		/// <summary>クリックボタンに応じた目印の色。左クリックは赤・右クリックは青。</summary>
		public static Color MarkColorOf( bool leftClick ) => leftClick ? Color.Red : Color.Blue;

		/// <summary>カーソルの右下に移動する。表示先モニタの DPI に合わせて大きさと位置を調整する。</summary>
		public void MoveNear( Point cursorPos ) {
			float newScale = GetScaleAt( cursorPos );
			int size = (int)MathF.Round( BaseSize * newScale );
			int offset = (int)MathF.Round( BaseOffset * newScale );
			var bounds = new Rectangle( cursorPos.X + offset, cursorPos.Y + offset, size, size );

			if( newScale != scale ) {
				scale = newScale;
				Bounds = bounds;
				UpdateLayered();
			} else {
				Bounds = bounds;
			}
		}

		/// <summary>指定した画面座標のあるモニタの拡大率(DPI / 96)を返す。</summary>
		private static float GetScaleAt( Point screenPos ) {
			var monitor = PInvoke.MonitorFromPoint( screenPos, MONITOR_FROM_FLAGS.MONITOR_DEFAULTTONEAREST );
			if( PInvoke.GetDpiForMonitor( monitor, MONITOR_DPI_TYPE.MDT_EFFECTIVE_DPI, out uint dpiX, out _ ).Succeeded && dpiX > 0 ) {
				return dpiX / 96f;
			}
			return 1f;
		}

		/// <summary>フォーカスを奪わずに表示する。</summary>
		public void ShowNoActivate() {
			if( !Visible ) {
				Show();
			}
		}
	}
}

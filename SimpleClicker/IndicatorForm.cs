namespace SimpleClicker {
	/// <summary>
	/// 連打中にマウスカーソルの近くに表示する目印(クリック透過・非アクティブ)。
	/// </summary>
	internal sealed class IndicatorForm : Form {

		private const int WS_EX_LAYERED = 0x00080000;
		private const int WS_EX_TRANSPARENT = 0x00000020;
		private const int WS_EX_NOACTIVATE = 0x08000000;
		private const int WS_EX_TOOLWINDOW = 0x00000080;

		public IndicatorForm() {
			FormBorderStyle = FormBorderStyle.None;
			ShowInTaskbar = false;
			StartPosition = FormStartPosition.Manual;
			TopMost = true;
			Size = new Size( 16, 16 );
			BackColor = Color.Magenta;
			TransparencyKey = Color.Magenta;
		}

		protected override bool ShowWithoutActivation => true;

		protected override CreateParams CreateParams {
			get {
				var cp = base.CreateParams;
				// クリック透過+フォーカスを奪わない
				cp.ExStyle |= WS_EX_LAYERED | WS_EX_TRANSPARENT | WS_EX_NOACTIVATE | WS_EX_TOOLWINDOW;
				return cp;
			}
		}

		protected override void OnPaint( PaintEventArgs e ) {
			e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
			using var brush = new SolidBrush( Color.Red );
			e.Graphics.FillEllipse( brush, 2, 2, 12, 12 );
			using var pen = new Pen( Color.White, 2 );
			e.Graphics.DrawEllipse( pen, 2, 2, 12, 12 );
		}

		/// <summary>カーソルの右下に移動する。</summary>
		public void MoveNear( Point cursorPos ) {
			Location = new Point( cursorPos.X + 18, cursorPos.Y + 18 );
		}

		/// <summary>フォーカスを奪わずに表示する。</summary>
		public void ShowNoActivate() {
			if( !Visible ) {
				Show();
			}
		}
	}
}

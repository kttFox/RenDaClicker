using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.WindowsAndMessaging;

namespace RenDaClicker {
	/// <summary>
	/// 縦スクロールバーを常に表示する <see cref="ListView"/>。
	/// (項目数が少ないときは無効状態で表示されるため、一覧の幅が変わらない)
	/// ListView は列幅を DPI に追従させないため、96DPI 基準の幅を保持して自前で拡大する。
	/// </summary>
	internal sealed class ListViewEx : ListView {

		private const int WM_NCCALCSIZE = 0x0083;

		/// <summary>96DPI 基準の列幅(デザイナで設定された値)。</summary>
		private int[]? baseColumnWidths;

		protected override void OnHandleCreated( EventArgs e ) {
			base.OnHandleCreated( e );
			ShowVerticalScrollBar();
			ScaleColumns();
		}

		protected override void OnDpiChangedAfterParent( EventArgs e ) {
			base.OnDpiChangedAfterParent( e );
			ScaleColumns();
		}

		/// <summary>列幅を現在の DPI に合わせて設定する。</summary>
		private void ScaleColumns() {
			if( Columns.Count == 0 ) {
				return;
			}

			// 最初の一回だけ、デザイナで設定された 96DPI 基準の幅を記憶する
			if( baseColumnWidths == null || baseColumnWidths.Length != Columns.Count ) {
				baseColumnWidths = new int[Columns.Count];
				for( int i = 0; i < Columns.Count; i++ ) {
					baseColumnWidths[i] = Columns[i].Width;
				}
			}

			float scale = DeviceDpi / 96f;
			for( int i = 0; i < Columns.Count; i++ ) {
				Columns[i].Width = (int)MathF.Round( baseColumnWidths[i] * scale );
			}
		}

		protected override void WndProc( ref Message m ) {
			// ListView は自前でスクロールバーの表示を制御するため、
			// 非クライアント領域の再計算のたびに表示し直す
			if( m.Msg == WM_NCCALCSIZE ) {
				ShowVerticalScrollBar();
			}
			base.WndProc( ref m );
		}

		private void ShowVerticalScrollBar() {
			if( IsHandleCreated ) {
				PInvoke.ShowScrollBar( (HWND)Handle, SCROLLBAR_CONSTANTS.SB_VERT, true );
			}
		}
	}
}

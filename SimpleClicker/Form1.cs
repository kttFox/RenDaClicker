using System.Runtime.InteropServices;

namespace SimpleClicker {
	public partial class Form1 : Form {

		[DllImport( "user32.dll" )]
		private static extern short GetAsyncKeyState( int vKey );

		[DllImport( "user32.dll" )]
		private static extern uint SendInput( uint nInputs, INPUT[] pInputs, int cbSize );

		[DllImport( "user32.dll" )]
		private static extern bool SetCursorPos( int X, int Y );

		[DllImport( "user32.dll" )]
		private static extern IntPtr WindowFromPoint( Point pt );

		[DllImport( "user32.dll" )]
		private static extern IntPtr GetAncestor( IntPtr hwnd, uint gaFlags );

		private const uint GA_ROOT = 2;

		[StructLayout( LayoutKind.Sequential )]
		private struct INPUT {
			public uint type;
			public MOUSEINPUT mi;
		}

		[StructLayout( LayoutKind.Sequential )]
		private struct MOUSEINPUT {
			public int dx;
			public int dy;
			public uint mouseData;
			public uint dwFlags;
			public uint time;
			public IntPtr dwExtraInfo;
		}

		private const uint INPUT_MOUSE = 0;
		private const uint MOUSEEVENTF_LEFTDOWN = 0x0002;
		private const uint MOUSEEVENTF_LEFTUP = 0x0004;
		private const uint MOUSEEVENTF_RIGHTDOWN = 0x0008;
		private const uint MOUSEEVENTF_RIGHTUP = 0x0010;

		private readonly System.Windows.Forms.Timer clickTimer = new();
		private readonly IndicatorForm indicator = new();
		private readonly System.Windows.Forms.Timer hotkeyTimer = new();
		private bool hotkeyWasDown = false;
		private bool toggledOn = false;
		private bool buttonOn = false;
		private int hotkeyVk = (int)Keys.F6;
		private Point lockedPos;
		private IntPtr targetWindow = IntPtr.Zero;
		private bool capturing = false;
		private bool emergencyStopped = false;
		private bool hotkeySuspended = false;

		private const int VK_XBUTTON1 = 0x05;
		private const int VK_XBUTTON2 = 0x06;

		public Form1() {
			InitializeComponent();

			LoadSettings();
			textHotkey.Text = HotkeyName( hotkeyVk );
			UpdateSuspendButton();

			clickTimer.Tick += ClickTimer_Tick;

			// 設定変更のたびにリアルタイム保存
			radioLeft.CheckedChanged += ( s, e ) => SaveSettings();
			radioHold.CheckedChanged += ( s, e ) => SaveSettings();
			numInterval.ValueChanged += ( s, e ) => SaveSettings();
			checkLockPos.CheckedChanged += ( s, e ) => SaveSettings();
			checkIndicator.CheckedChanged += ( s, e ) => SaveSettings();

			hotkeyTimer.Interval = 15;
			hotkeyTimer.Tick += HotkeyTimer_Tick;
			hotkeyTimer.Start();

			numInterval.ValueChanged += ( s, e ) => clickTimer.Interval = (int)numInterval.Value;
			// 連打中に目印設定を切り替えた場合も即時反映
			checkIndicator.CheckedChanged += ( s, e ) => {
				if( clickTimer.Enabled && checkIndicator.Checked ) {
					indicator.MoveNear( Cursor.Position );
					indicator.ShowNoActivate();
				}
				else {
					indicator.Hide();
				}
			};
			clickTimer.Interval = (int)numInterval.Value;
		}

		private void HotkeyTimer_Tick( object? sender, EventArgs e ) {
			// 連打中は目印をカーソルに追従させる
			if( indicator.Visible ) {
				indicator.MoveNear( Cursor.Position );
			}

			if( capturing || hotkeySuspended ) {
				return;
			}
			bool isDown = ( GetAsyncKeyState( hotkeyVk ) & 0x8000 ) != 0;

			// 緊急停止中:明示的な停止操作があるまで再開しない
			if( emergencyStopped ) {
				if( radioHold.Checked ) {
					// キーを一度離したら解除
					if( !isDown ) {
						ClearEmergency();
					}
				}
				else {
					// ホットキーを1回押したら解除(停止操作扱い。再開はもう1回)
					if( isDown && !hotkeyWasDown ) {
						ClearEmergency();
					}
				}
				hotkeyWasDown = isDown;
				return;
			}

			if( radioHold.Checked ) {
				// 押している間だけ連打(開始ボタンでの連打中は維持)
				SetClicking( buttonOn || isDown );
			}
			else {
				// トグル:押した瞬間にON/OFF切替
				if( isDown && !hotkeyWasDown ) {
					toggledOn = !toggledOn;
					buttonOn = false;
					SetClicking( toggledOn );
				}
			}
			hotkeyWasDown = isDown;
		}

		private void SetClicking( bool on ) {
			if( on && !clickTimer.Enabled ) {
				lockedPos = Cursor.Position; // 連打開始時点の位置を記憶
				// 位置保持時はクリック先ウインドウを記憶(終了検知用)
				targetWindow = checkLockPos.Checked
					? GetAncestor( WindowFromPoint( lockedPos ), GA_ROOT )
					: IntPtr.Zero;
				clickTimer.Start();
				labelStatus.Text = "状態: 連打中";
				labelStatus.ForeColor = Color.Red;
				if( checkIndicator.Checked ) {
					indicator.MoveNear( Cursor.Position );
					indicator.ShowNoActivate();
				}
			}
			else if( !on && clickTimer.Enabled ) {
				clickTimer.Stop();
				labelStatus.Text = "状態: 停止";
				labelStatus.ForeColor = SystemColors.ControlText;
				indicator.Hide();
			}
		}

		private void ClickTimer_Tick( object? sender, EventArgs e ) {
			// カーソル位置固定が有効なら記憶した位置へ戻す
			if( checkLockPos.Checked ) {
				// クリック先ウインドウが変わっていたら緊急停止
				if( targetWindow != IntPtr.Zero
					&& GetAncestor( WindowFromPoint( lockedPos ), GA_ROOT ) != targetWindow ) {
					buttonOn = false;
					toggledOn = false;
					emergencyStopped = true;
					SetClicking( false );
					labelStatus.Text = "状態: 緊急停止";
					labelStatus.ForeColor = Color.Red;
					return;
				}
				SetCursorPos( lockedPos.X, lockedPos.Y );
			}

			uint down = radioLeft.Checked ? MOUSEEVENTF_LEFTDOWN : MOUSEEVENTF_RIGHTDOWN;
			uint up = radioLeft.Checked ? MOUSEEVENTF_LEFTUP : MOUSEEVENTF_RIGHTUP;

			var inputs = new INPUT[] {
				new() { type = INPUT_MOUSE, mi = new MOUSEINPUT { dwFlags = down } },
				new() { type = INPUT_MOUSE, mi = new MOUSEINPUT { dwFlags = up } },
			};
			SendInput( (uint)inputs.Length, inputs, Marshal.SizeOf<INPUT>() );
		}

		private void RadioMode_CheckedChanged( object? sender, EventArgs e ) {
			// モード切替時は停止状態にリセット
			toggledOn = false;
			buttonOn = false;
			SetClicking( false );
		}

		private void ButtonSuspend_Click( object? sender, EventArgs e ) {
			hotkeySuspended = !hotkeySuspended;
			if( hotkeySuspended ) {
				// 中止時はキー押下状態をリセット(再開直後の誤発動防止)
				hotkeyWasDown = true;
			}
			UpdateSuspendButton();
		}

		private void UpdateSuspendButton() {
			if( hotkeySuspended ) {
				buttonSuspend.Text = "⏸ ホットキー受付:停止中(クリックで再開)";
				buttonSuspend.BackColor = Color.Gold;
			}
			else {
				buttonSuspend.Text = "▶ ホットキー受付:有効(クリックで一時中止)";
				buttonSuspend.BackColor = Color.PaleGreen;
			}
		}

		private void ClearEmergency() {
			emergencyStopped = false;
			labelStatus.Text = "状態: 停止";
			labelStatus.ForeColor = SystemColors.ControlText;
		}

		private void ButtonStart_Click( object? sender, EventArgs e ) {
			if( emergencyStopped ) {
				return; // 緊急停止中は先に停止操作が必要
			}
			buttonOn = true;
			toggledOn = true;
			SetClicking( true );
		}

		private void ButtonStop_Click( object? sender, EventArgs e ) {
			buttonOn = false;
			toggledOn = false;
			SetClicking( false );
			if( emergencyStopped ) {
				ClearEmergency();
			}
		}

		// ---- ホットキーのキャプチャ ----

		private void ButtonCapture_Click( object? sender, EventArgs e ) {
			capturing = true;
			buttonOn = false;
			toggledOn = false;
			SetClicking( false );
			textHotkey.Text = "キー入力待ち…";
			textHotkey.Focus();
		}

		private void TextHotkey_Leave( object? sender, EventArgs e ) {
			// フォーカスが外れたらキャプチャ中止
			capturing = false;
			textHotkey.Text = HotkeyName( hotkeyVk );
		}

		private void TextHotkey_KeyDown( object? sender, KeyEventArgs e ) {
			if( !capturing ) {
				return;
			}
			e.SuppressKeyPress = true;
			// 修飾キー単体は無視しない(Shift等も設定可能)
			SetHotkey( (int)e.KeyCode );
		}

		private void TextHotkey_PreviewKeyDown( object? sender, PreviewKeyDownEventArgs e ) {
			// Tab・矢印キーなどもKeyDownへ渡す
			e.IsInputKey = true;
		}

		private void TextHotkey_MouseDown( object? sender, MouseEventArgs e ) {
			if( !capturing ) {
				return;
			}
			// マウスのサイドボタンを設定
			if( e.Button == MouseButtons.XButton1 ) {
				SetHotkey( VK_XBUTTON1 );
			}
			else if( e.Button == MouseButtons.XButton2 ) {
				SetHotkey( VK_XBUTTON2 );
			}
		}

		private void SetHotkey( int vk ) {
			hotkeyVk = vk;
			textHotkey.Text = HotkeyName( vk );
			// キャプチャ終了(フォーカスを外す)
			ActiveControl = null;
			capturing = false;
			hotkeyWasDown = true; // 設定直後の誤発動防止
			SaveSettings();
		}

		private static string HotkeyName( int vk ) {
			return vk switch {
				VK_XBUTTON1 => "サイドボタン1",
				VK_XBUTTON2 => "サイドボタン2",
				_ => ( (Keys)vk ).ToString(),
			};
		}

		// ---- 設定の保存・復元 ----

		private sealed class AppSettings {
			public bool LeftClick { get; set; } = true;
			public int Interval { get; set; } = 100;
			public bool HoldMode { get; set; } = true;
			public int HotkeyVk { get; set; } = (int)Keys.F6;
			public bool LockPos { get; set; } = false;
			public bool ShowIndicator { get; set; } = true;
		}

		private static string SettingsPath => Path.Combine( AppContext.BaseDirectory, "settings.json" );

		private void LoadSettings() {
			try {
				if( !File.Exists( SettingsPath ) ) {
					return;
				}
				var s = System.Text.Json.JsonSerializer.Deserialize<AppSettings>( File.ReadAllText( SettingsPath ) );
				if( s == null ) {
					return;
				}
				radioLeft.Checked = s.LeftClick;
				radioRight.Checked = !s.LeftClick;
				numInterval.Value = Math.Clamp( s.Interval, numInterval.Minimum, numInterval.Maximum );
				radioHold.Checked = s.HoldMode;
				radioToggle.Checked = !s.HoldMode;
				hotkeyVk = s.HotkeyVk;
				checkLockPos.Checked = s.LockPos;
				checkIndicator.Checked = s.ShowIndicator;
			}
			catch {
				// 読み込み失敗時は既定値のまま起動
			}
		}

		private void SaveSettings() {
			try {
				var s = new AppSettings {
					LeftClick = radioLeft.Checked,
					Interval = (int)numInterval.Value,
					HoldMode = radioHold.Checked,
					HotkeyVk = hotkeyVk,
					LockPos = checkLockPos.Checked,
					ShowIndicator = checkIndicator.Checked,
				};
				File.WriteAllText( SettingsPath, System.Text.Json.JsonSerializer.Serialize(
					s, new System.Text.Json.JsonSerializerOptions { WriteIndented = true } ) );
			}
			catch {
				// 保存失敗は無視(次回は既定値)
			}
		}
	}
}

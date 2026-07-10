using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.Input.KeyboardAndMouse;
using Windows.Win32.UI.WindowsAndMessaging;

namespace SimpleClicker {
	public partial class Form1 : Form {

		private const MOUSE_EVENT_FLAGS MouseEventLeftDown = MOUSE_EVENT_FLAGS.MOUSEEVENTF_LEFTDOWN;
		private const MOUSE_EVENT_FLAGS MouseEventLeftUp = MOUSE_EVENT_FLAGS.MOUSEEVENTF_LEFTUP;
		private const MOUSE_EVENT_FLAGS MouseEventRightDown = MOUSE_EVENT_FLAGS.MOUSEEVENTF_RIGHTDOWN;
		private const MOUSE_EVENT_FLAGS MouseEventRightUp = MOUSE_EVENT_FLAGS.MOUSEEVENTF_RIGHTUP;

		private readonly System.Windows.Forms.Timer clickTimer = new();
		private bool buttonHeld = false;
		private CancellationTokenSource? holdCts;
		private readonly IndicatorForm indicator = new();
		private readonly System.Windows.Forms.Timer hotkeyTimer = new();
		private bool hotkeyWasDown = false;
		private bool toggledOn = false;
		private bool buttonOn = false;
		private int hotkeyVk = (int)Keys.F6;
		private Point lockedPos;
		private nint targetWindow = 0;
		private bool capturing = false;
		private bool emergencyStopped = false;
		private bool hotkeySuspended = false;

		private const int VK_XBUTTON1 = 0x05;
		private const int VK_XBUTTON2 = 0x06;

		public Form1() {
			InitializeComponent();

			RefreshWindowList();
			LoadSettings();
			textHotkey.Text = HotkeyName( hotkeyVk );
			UpdateSuspendButton();
			UpdateTargetControls();

			clickTimer.Tick += ClickTimer_Tick;

			// 設定変更のたびにリアルタイム保存
			radioLeft.CheckedChanged += ( s, e ) => SaveSettings();
			radioHold.CheckedChanged += ( s, e ) => SaveSettings();
			numInterval.ValueChanged += ( s, e ) => SaveSettings();
			numHold.ValueChanged += ( s, e ) => SaveSettings();
			checkLockPos.CheckedChanged += ( s, e ) => SaveSettings();
			checkIndicator.CheckedChanged += ( s, e ) => SaveSettings();
			checkTargetWindow.CheckedChanged += ( s, e ) => { UpdateTargetControls(); SaveSettings(); };
			comboWindows.SelectedIndexChanged += ( s, e ) => SaveSettings();
			buttonRefreshWindows.Click += ( s, e ) => RefreshWindowList();

			hotkeyTimer.Interval = 15;
			hotkeyTimer.Tick += HotkeyTimer_Tick;
			hotkeyTimer.Start();

			numInterval.ValueChanged += ( s, e ) => clickTimer.Interval = (int)numInterval.Value;
			// 連打中に目印設定を切り替えた場合も即時反映
			checkIndicator.CheckedChanged += ( s, e ) => {
				if( clickTimer.Enabled && checkIndicator.Checked ) {
					indicator.MoveNear( Cursor.Position );
					indicator.ShowNoActivate();
				} else {
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
			bool isDown = ( PInvoke.GetAsyncKeyState( hotkeyVk ) & 0x8000 ) != 0;

			// 緊急停止中:明示的な停止操作があるまで再開しない
			if( emergencyStopped ) {
				if( radioHold.Checked ) {
					// キーを一度離したら解除
					if( !isDown ) {
						ClearEmergency();
					}
				} else {
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
			} else {
				// トグル:押した瞬間にON/OFF切替
				if( isDown && !hotkeyWasDown ) {
					toggledOn = !toggledOn;
					buttonOn = false;
					SetClicking( toggledOn );
				}
			}
			hotkeyWasDown = isDown;
		}

		// ---- 対象ウインドウの限定 ----

		private sealed record WindowItem( nint Handle, string Title ) {
			public override string ToString() => Title;
		}

		private void RefreshWindowList() {
			var items = new List<WindowItem>();
			nint self = Handle;
			PInvoke.EnumWindows( ( hwnd, _ ) => {
				if( hwnd != self && PInvoke.IsWindowVisible( hwnd ) ) {
					Span<char> buf = stackalloc char[256];
					int len = PInvoke.GetWindowText( hwnd, buf );
					if( len > 0 ) {
						items.Add( new WindowItem( hwnd, buf[..len].ToString() ) );
					}
				}
				return true;
			}, 0 );

			var prev = comboWindows.SelectedItem as WindowItem;
			comboWindows.Items.Clear();
			foreach( var item in items ) {
				comboWindows.Items.Add( item );
			}
			// 更新前の選択を可能な限り維持(ハンドル一致→タイトル一致の順)
			if( prev != null ) {
				var match = items.FirstOrDefault( i => i.Handle == prev.Handle )
					?? items.FirstOrDefault( i => i.Title == prev.Title );
				comboWindows.SelectedItem = match;
			}
		}

		private void UpdateTargetControls() {
			comboWindows.Enabled = checkTargetWindow.Checked;
			buttonRefreshWindows.Enabled = checkTargetWindow.Checked;
		}

		// 指定位置が対象ウインドウ上かどうか(限定が無効なら常にtrue)
		private bool IsOnTargetWindow( Point pos ) {
			if( !checkTargetWindow.Checked ) {
				return true;
			}
			if( comboWindows.SelectedItem is not WindowItem item || !PInvoke.IsWindow( new HWND( item.Handle ) ) ) {
				return false; // 未選択・対象が閉じられた場合は連打しない
			}
			return PInvoke.GetAncestor( PInvoke.WindowFromPoint( pos ), GET_ANCESTOR_FLAGS.GA_ROOT ) == item.Handle;
		}

		private void SetClicking( bool on ) {
			// 対象ウインドウ限定時:対象上でなければ連打を開始しない
			if( on && !clickTimer.Enabled && !IsOnTargetWindow( Cursor.Position ) ) {
				return;
			}
			if( on && !clickTimer.Enabled ) {
				lockedPos = Cursor.Position; // 連打開始時点の位置を記憶
											 // 位置保持時はクリック先ウインドウを記憶(終了検知用)
				targetWindow = checkLockPos.Checked ? PInvoke.GetAncestor( PInvoke.WindowFromPoint( lockedPos ), GET_ANCESTOR_FLAGS.GA_ROOT ) : 0;
				clickTimer.Start();
				labelStatus.Text = "状態: 連打中";
				labelStatus.ForeColor = Color.Red;
				if( checkIndicator.Checked ) {
					indicator.MoveNear( Cursor.Position );
					indicator.ShowNoActivate();
				}
			} else if( !on && clickTimer.Enabled ) {
				clickTimer.Stop();
				// 押下中に停止した場合は待機をキャンセル(継続処理側で即時解放される)
				holdCts?.Cancel();
				labelStatus.Text = "状態: 停止";
				labelStatus.ForeColor = SystemColors.ControlText;
				indicator.Hide();
			}
		}

		private async void ClickTimer_Tick( object? sender, EventArgs e ) {
			// まだ押下中(押下時間 > クリック間隔)なら今回はスキップ
			if( buttonHeld ) {
				return;
			}

			// 対象ウインドウ限定時:クリック先が対象から外れたら停止
			if( !IsOnTargetWindow( checkLockPos.Checked ? lockedPos : Cursor.Position ) ) {
				buttonOn = false;
				toggledOn = false;
				SetClicking( false );
				return;
			}

			// カーソル位置固定が有効なら記憶した位置へ戻す
			if( checkLockPos.Checked ) {
				// クリック先ウインドウが変わっていたら緊急停止
				if( targetWindow != 0 && PInvoke.GetAncestor( PInvoke.WindowFromPoint( lockedPos ), GET_ANCESTOR_FLAGS.GA_ROOT ) != targetWindow ) {
					buttonOn = false;
					toggledOn = false;
					emergencyStopped = true;
					SetClicking( false );
					labelStatus.Text = "状態: 緊急停止";
					labelStatus.ForeColor = Color.Red;
					return;
				}
				PInvoke.SetCursorPos( lockedPos.X, lockedPos.Y );
			}

			MOUSE_EVENT_FLAGS down = radioLeft.Checked ? MouseEventLeftDown : MouseEventRightDown;
			MOUSE_EVENT_FLAGS up = radioLeft.Checked ? MouseEventLeftUp : MouseEventRightUp;

			// 押下を送信し、指定した押下時間の経過後に解放を送信する
			// (停止操作でキャンセルされた場合も必ず解放する)
			SendMouse( down );
			buttonHeld = true;
			holdCts = new CancellationTokenSource();
			try {
				await Task.Delay( (int)numHold.Value, holdCts.Token );
			} catch( TaskCanceledException ) {
				// 停止によるキャンセル:即時解放へ進む
			} finally {
				holdCts.Dispose();
				holdCts = null;
				SendMouse( up );
				buttonHeld = false;
			}
		}

		private static unsafe void SendMouse( MOUSE_EVENT_FLAGS flags ) {
			Span<INPUT> inputs = stackalloc INPUT[1];
			inputs[0].type = INPUT_TYPE.INPUT_MOUSE;
			inputs[0].Anonymous.mi.dwFlags = flags;
			PInvoke.SendInput( inputs, sizeof( INPUT ) );
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
			} else {
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
			} else if( e.Button == MouseButtons.XButton2 ) {
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
			public int HoldMs { get; set; } = 50;
			public bool HoldMode { get; set; } = true;
			public int HotkeyVk { get; set; } = (int)Keys.F6;
			public bool LockPos { get; set; } = false;
			public bool ShowIndicator { get; set; } = true;
			public bool LimitToWindow { get; set; } = false;
			public string TargetWindowTitle { get; set; } = "";
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
				numHold.Value = Math.Clamp( s.HoldMs, numHold.Minimum, numHold.Maximum );
				radioHold.Checked = s.HoldMode;
				radioToggle.Checked = !s.HoldMode;
				hotkeyVk = s.HotkeyVk;
				checkLockPos.Checked = s.LockPos;
				checkIndicator.Checked = s.ShowIndicator;
				checkTargetWindow.Checked = s.LimitToWindow;
				// 前回の対象ウインドウをタイトル一致で復元
				if( !string.IsNullOrEmpty( s.TargetWindowTitle ) ) {
					comboWindows.SelectedItem = comboWindows.Items.OfType<WindowItem>()
						.FirstOrDefault( i => i.Title == s.TargetWindowTitle );
				}
			} catch {
				// 読み込み失敗時は既定値のまま起動
			}
		}

		private void SaveSettings() {
			try {
				var s = new AppSettings {
					LeftClick = radioLeft.Checked,
					Interval = (int)numInterval.Value,
					HoldMs = (int)numHold.Value,
					HoldMode = radioHold.Checked,
					HotkeyVk = hotkeyVk,
					LockPos = checkLockPos.Checked,
					ShowIndicator = checkIndicator.Checked,
					LimitToWindow = checkTargetWindow.Checked,
					TargetWindowTitle = ( comboWindows.SelectedItem as WindowItem )?.Title ?? "",
				};
				File.WriteAllText( SettingsPath, System.Text.Json.JsonSerializer.Serialize(
					s, new System.Text.Json.JsonSerializerOptions { WriteIndented = true } ) );
			} catch {
				// 保存失敗は無視(次回は既定値)
			}
		}
	}
}

using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.Input.KeyboardAndMouse;
using Windows.Win32.UI.WindowsAndMessaging;

namespace RenDaClicker {
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
		private bool toggledOn = false;
		private Point lockedPos;
		private nint targetWindow = 0;
		private bool emergencyStopped = false;
		private bool emergencyFromHold = false;
		private bool hotkeySuspended = false;
		private bool editing = false;

		/// <summary>登録済みプロファイル。ホットキーはこの順で判定する。</summary>
		private readonly List<Profile> profiles = [];

		/// <summary>連打中のプロファイル。停止中はnull。</summary>
		private Profile? activeProfile = null;

		public Form1() {
			InitializeComponent();

			RefreshWindowList();
			LoadSettings();
			if( profiles.Count == 0 ) {
				profiles.Add( new Profile { Name = "連打1", HotkeyVk = (int)Keys.F6 } );
			}
			RefreshProfileList();
			UpdateSuspendButton();
			UpdateTargetControls();

			clickTimer.Tick += ClickTimer_Tick;

			panelMarkPreviewLeft.Paint += PanelMarkPreviewLeft_Paint;
			panelMarkPreviewRight.Paint += PanelMarkPreviewRight_Paint;

			buttonAdd.Click += ButtonAdd_Click;
			buttonEdit.Click += ButtonEdit_Click;
			buttonDelete.Click += ButtonDelete_Click;
			buttonUp.Click += ( s, e ) => MoveProfile( -1 );
			buttonDown.Click += ( s, e ) => MoveProfile( +1 );
			listProfiles.DoubleClick += ButtonEdit_Click;
			listProfiles.SelectedIndexChanged += ( s, e ) => UpdateProfileButtons();

			// 設定変更のたびにリアルタイム保存
			checkTargetWindow.CheckedChanged += ( s, e ) => { UpdateTargetControls(); SaveSettings(); };
			comboWindows.SelectedIndexChanged += ( s, e ) => SaveSettings();
			buttonRefreshWindows.Click += ( s, e ) => RefreshWindowList();

			hotkeyTimer.Interval = 15;
			hotkeyTimer.Tick += HotkeyTimer_Tick;
			hotkeyTimer.Start();

			// 連打中に目印設定を切り替えた場合も即時反映
			checkIndicator.CheckedChanged += ( s, e ) => {
				if( clickTimer.Enabled && checkIndicator.Checked ) {
					indicator.SetMark( activeProfile == null || activeProfile.LeftClick );
					indicator.MoveNear( Cursor.Position );
					indicator.ShowNoActivate();
				} else {
					indicator.Hide();
				}
				SaveSettings();
			};
		}

		/// <summary>目印の実際の見た目(左クリック=赤・右クリック=青)をプレビュー表示する。</summary>
		private void PanelMarkPreviewLeft_Paint( object? sender, PaintEventArgs e ) {
			DrawMarkPreview( sender, e, true );
		}

		private void PanelMarkPreviewRight_Paint( object? sender, PaintEventArgs e ) {
			DrawMarkPreview( sender, e, false );
		}

		/// <summary>プレビューを描画する。パネルの DPI に合わせて拡大する。</summary>
		private static void DrawMarkPreview( object? sender, PaintEventArgs e, bool leftClick ) {
			float scale = sender is Control c ? c.DeviceDpi / 96f : 1f;
			var origin = new Point( 0, (int)MathF.Round( 2 * scale ) );
			IndicatorForm.DrawMark( e.Graphics, origin, IndicatorForm.MarkColorOf( leftClick ), scale );
		}

		private void HotkeyTimer_Tick( object? sender, EventArgs e ) {
			// 連打中は目印をカーソルに追従させる
			if( indicator.Visible ) {
				indicator.MoveNear( Cursor.Position );
			}

			if( hotkeySuspended || editing ) {
				return;
			}

			// 各プロファイルの押下状態を更新し、「押した瞬間」のプロファイルを拾う
			Profile? pressed = null;
			bool anyDown = false;
			foreach( var p in profiles ) {
				bool isDown = p.IsHotkeyDown();
				if( isDown && !p.HotkeyWasDown ) {
					// 同時に成立した場合は修飾キーの多い方を優先(Tab より Shift+Tab)
					if( pressed == null || ModCount( pressed.Mods ) < ModCount( p.Mods ) ) {
						pressed = p;
					}
				}
				p.HotkeyWasDown = isDown;
				anyDown |= isDown;
			}

			// 緊急停止中:明示的な停止操作があるまで再開しない
			if( emergencyStopped ) {
				if( emergencyFromHold ) {
					// キーを一度離したら解除
					if( !anyDown ) {
						ClearEmergency();
					}
				} else {
					// ホットキーを1回押したら解除(停止操作扱い。再開はもう1回)
					if( pressed != null ) {
						ClearEmergency();
					}
				}
				return;
			}

			if( pressed != null ) {
				if( pressed.HoldMode ) {
					// 押している間だけ連打(他プロファイルの連打中でも押した方へ切り替え)
					toggledOn = false;
					SetClicking( true, pressed );
				} else if( ReferenceEquals( pressed, activeProfile ) && clickTimer.Enabled ) {
					// トグル:同じプロファイルの再押下で停止
					StopAll();
				} else {
					// トグル:開始(他プロファイルの連打中なら切り替え)
					toggledOn = true;
					SetClicking( true, pressed );
				}
			}

			// 押している間モードで連打中のキーが離されたら停止
			if( activeProfile != null && clickTimer.Enabled && activeProfile.HoldMode
				&& !activeProfile.HotkeyIsDown && !toggledOn ) {
				SetClicking( false, activeProfile );
			}
		}

		/// <summary>修飾キーの個数。ホットキーの優先順位付けに使う。</summary>
		private static int ModCount( Keys mods ) {
			int n = 0;
			if( mods.HasFlag( Keys.Control ) ) n++;
			if( mods.HasFlag( Keys.Shift ) ) n++;
			if( mods.HasFlag( Keys.Alt ) ) n++;
			return n;
		}

		// ---- プロファイルの一覧・追加・変更・削除 ----

		private void RefreshProfileList() {
			int selected = listProfiles.SelectedIndices.Count > 0 ? listProfiles.SelectedIndices[0] : 0;
			listProfiles.BeginUpdate();
			listProfiles.Items.Clear();
			foreach( var p in profiles ) {
				listProfiles.Items.Add( new ListViewItem( [
					p.Name,
					p.HotkeyText,
					p.LeftClick ? "左" : "右",
					p.HoldMode ? "押下中" : "トグル",
					$"{p.Interval}ms",
					p.LockPos ? "保持" : "",
				] ) );
			}
			listProfiles.EndUpdate();
			if( listProfiles.Items.Count > 0 ) {
				int index = Math.Clamp( selected, 0, listProfiles.Items.Count - 1 );
				listProfiles.Items[index].Selected = true;
			}
			UpdateProfileButtons();
			UpdateActiveHighlight();
		}

		/// <summary>連打中のプロファイルの行を黄色で示す。</summary>
		private void UpdateActiveHighlight() {
			for( int i = 0; i < listProfiles.Items.Count && i < profiles.Count; i++ ) {
				bool active = ReferenceEquals( profiles[i], activeProfile );
				listProfiles.Items[i].BackColor = active ? Color.Gold : listProfiles.BackColor;
			}
		}

		private void UpdateProfileButtons() {
			bool has = listProfiles.SelectedIndices.Count > 0;
			buttonEdit.Enabled = has;
			buttonDelete.Enabled = has && profiles.Count > 1;
			int index = has ? listProfiles.SelectedIndices[0] : -1;
			buttonUp.Enabled = index > 0;
			buttonDown.Enabled = index >= 0 && index < profiles.Count - 1;
		}

		/// <summary>選択中のプロファイルを上下に移動する(並び順=ホットキーの判定順)。</summary>
		private void MoveProfile( int delta ) {
			if( listProfiles.SelectedIndices.Count == 0 ) {
				return;
			}
			int index = listProfiles.SelectedIndices[0];
			int moved = index + delta;
			if( moved < 0 || moved >= profiles.Count ) {
				return;
			}
			StopAll();
			( profiles[index], profiles[moved] ) = ( profiles[moved], profiles[index] );
			RefreshProfileList();
			listProfiles.Items[moved].Selected = true;
			listProfiles.Items[moved].Focused = true;
			SaveSettings();
		}

		private void ButtonAdd_Click( object? sender, EventArgs e ) {
			StopAll();
			var draft = new Profile { Name = $"連打{profiles.Count + 1}", HotkeyVk = 0 };
			if( ShowEditor( draft ) is Profile added ) {
				profiles.Add( added );
				RefreshProfileList();
				listProfiles.Items[^1].Selected = true;
				SaveSettings();
			}
		}

		private void ButtonEdit_Click( object? sender, EventArgs e ) {
			if( listProfiles.SelectedIndices.Count == 0 ) {
				return;
			}
			StopAll();
			int index = listProfiles.SelectedIndices[0];
			if( ShowEditor( profiles[index] ) is Profile edited ) {
				profiles[index] = edited;
				RefreshProfileList();
				SaveSettings();
			}
		}

		private void ButtonDelete_Click( object? sender, EventArgs e ) {
			if( listProfiles.SelectedIndices.Count == 0 || profiles.Count <= 1 ) {
				return;
			}
			StopAll();
			int index = listProfiles.SelectedIndices[0];
			var result = MessageBox.Show( this, $"「{profiles[index].Name}」を削除しますか?", "RenDaClicker",
				MessageBoxButtons.OKCancel, MessageBoxIcon.Question );
			if( result != DialogResult.OK ) {
				return;
			}
			profiles.RemoveAt( index );
			RefreshProfileList();
			SaveSettings();
		}

		/// <summary>編集ダイアログを表示する。OKなら編集後のプロファイル、キャンセルならnull。</summary>
		private Profile? ShowEditor( Profile source ) {
			// ダイアログ表示中はホットキーを受け付けない(キー割り当て入力と衝突するため)
			editing = true;
			try {
				using var dialog = new ProfileEditForm( source );
				return dialog.ShowDialog( this ) == DialogResult.OK ? dialog.Result : null;
			} finally {
				editing = false;
				foreach( var p in profiles ) {
					p.HotkeyWasDown = true; // 設定直後の誤発動防止
				}
			}
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
			// 更新前の選択を可能な限り維持(判定はタイトルのみのためタイトル一致で復元)
			if( prev != null ) {
				comboWindows.SelectedItem = items.FirstOrDefault( i => i.Title == prev.Title );
			}
		}

		private void UpdateTargetControls() {
			comboWindows.Enabled = checkTargetWindow.Checked;
			buttonRefreshWindows.Enabled = checkTargetWindow.Checked;
		}

		// 指定位置が対象ウインドウ上かどうか(限定が無効なら常にtrue)
		// タイトルのみで判定するため、対象が一度閉じられて再度開かれた(ハンドルが変わった)場合も有効
		private bool IsOnTargetWindow( Point pos ) {
			if( !checkTargetWindow.Checked ) {
				return true;
			}
			if( comboWindows.SelectedItem is not WindowItem item ) {
				return false; // 未選択の場合は連打しない
			}
			HWND root = PInvoke.GetAncestor( PInvoke.WindowFromPoint( pos ), GET_ANCESTOR_FLAGS.GA_ROOT );
			Span<char> buf = stackalloc char[256];
			int len = PInvoke.GetWindowText( root, buf );
			return len > 0 && buf[..len].SequenceEqual( item.Title );
		}

		/// <summary>連打の開始・停止。開始時は使用するプロファイルを指定する。</summary>
		private void SetClicking( bool on, Profile profile ) {
			if( on ) {
				// 対象ウインドウ限定時:対象上でなければ連打を開始しない
				if( !clickTimer.Enabled && !IsOnTargetWindow( Cursor.Position ) ) {
					return;
				}
				// 他プロファイルの連打中なら、いったん停止してから切り替える
				if( clickTimer.Enabled && !ReferenceEquals( activeProfile, profile ) ) {
					StopClicking();
				}
				if( !clickTimer.Enabled ) {
					activeProfile = profile;
					lockedPos = Cursor.Position; // 連打開始時点の位置を記憶
												 // 位置保持時はクリック先ウインドウを記憶(終了検知用)
					targetWindow = profile.LockPos ? PInvoke.GetAncestor( PInvoke.WindowFromPoint( lockedPos ), GET_ANCESTOR_FLAGS.GA_ROOT ) : 0;
					clickTimer.Interval = profile.Interval;
					clickTimer.Start();
					if( checkIndicator.Checked ) {
						indicator.SetMark( profile.LeftClick );
						indicator.MoveNear( Cursor.Position );
						indicator.ShowNoActivate();
					}
					UpdateActiveHighlight();
				}
			} else if( clickTimer.Enabled ) {
				StopClicking();
			}
		}

		private void StopClicking() {
			clickTimer.Stop();
			// 押下中に停止した場合は待機をキャンセル(継続処理側で即時解放される)
			holdCts?.Cancel();
			activeProfile = null;
			indicator.Hide();
			UpdateActiveHighlight();
		}

		/// <summary>ボタン・トグルの状態も含めて完全に停止する。</summary>
		private void StopAll() {
			toggledOn = false;
			if( activeProfile != null ) {
				SetClicking( false, activeProfile );
			}
		}

		private async void ClickTimer_Tick( object? sender, EventArgs e ) {
			// まだ押下中(押下時間 > クリック間隔)なら今回はスキップ
			if( buttonHeld ) {
				return;
			}
			if( activeProfile is not Profile profile ) {
				return;
			}

			// 対象ウインドウ限定時:クリック先が対象から外れたら停止
			if( !IsOnTargetWindow( profile.LockPos ? lockedPos : Cursor.Position ) ) {
				StopAll();
				return;
			}

			// カーソル位置固定が有効なら記憶した位置へ戻す
			if( profile.LockPos ) {
				// クリック先ウインドウが変わっていたら緊急停止
				if( targetWindow != 0 && PInvoke.GetAncestor( PInvoke.WindowFromPoint( lockedPos ), GET_ANCESTOR_FLAGS.GA_ROOT ) != targetWindow ) {
					emergencyStopped = true;
					emergencyFromHold = profile.HoldMode;
					StopAll();
					return;
				}
				PInvoke.SetCursorPos( lockedPos.X, lockedPos.Y );
			}

			MOUSE_EVENT_FLAGS down = profile.LeftClick ? MouseEventLeftDown : MouseEventRightDown;
			MOUSE_EVENT_FLAGS up = profile.LeftClick ? MouseEventLeftUp : MouseEventRightUp;

			// 押下を送信し、指定した押下時間の経過後に解放を送信する
			// (停止操作でキャンセルされた場合も必ず解放する)
			SendMouse( down );
			buttonHeld = true;
			holdCts = new CancellationTokenSource();
			try {
				await Task.Delay( profile.HoldMs, holdCts.Token );
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

		private void ButtonSuspend_Click( object? sender, EventArgs e ) {
			hotkeySuspended = !hotkeySuspended;
			if( hotkeySuspended ) {
				// 中止時はキー押下状態をリセット(再開直後の誤発動防止)
				foreach( var p in profiles ) {
					p.HotkeyWasDown = true;
				}
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
		}

		// ---- 設定の保存・復元 ----

		private sealed class AppSettings {
			public List<Profile> Profiles { get; set; } = [];
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
				profiles.Clear();
				profiles.AddRange( s.Profiles );
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
					Profiles = profiles,
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

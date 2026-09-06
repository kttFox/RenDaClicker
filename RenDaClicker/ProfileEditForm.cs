namespace RenDaClicker {
	/// <summary>
	/// プロファイル1件の編集ダイアログ(名前・クリックボタン・間隔・押下時間・動作モード・ホットキー・位置保持)。
	/// </summary>
	internal partial class ProfileEditForm : Form {

		private int hotkeyVk;
		private Keys hotkeyMods;
		private bool capturing = false;

		/// <summary>編集結果(OKで閉じた場合のみ有効)。</summary>
		public Profile Result { get; private set; }

		/// <summary>デザイナ用。</summary>
		public ProfileEditForm() : this( new Profile() ) {
		}

		public ProfileEditForm( Profile source ) {
			InitializeComponent();

			Result = source.Clone();
			hotkeyVk = source.HotkeyVk;
			hotkeyMods = source.Mods;

			textName.Text = source.Name;
			radioLeft.Checked = source.LeftClick;
			radioRight.Checked = !source.LeftClick;
			radioHold.Checked = source.HoldMode;
			radioToggle.Checked = !source.HoldMode;
			numInterval.Value = Math.Clamp( source.Interval, numInterval.Minimum, numInterval.Maximum );
			numHold.Value = Math.Clamp( source.HoldMs, numHold.Minimum, numHold.Maximum );
			checkLockPos.Checked = source.LockPos;
			textHotkey.Text = Hotkey.Name( hotkeyVk, hotkeyMods );
		}

		private void ButtonOk_Click( object? sender, EventArgs e ) {
			string name = textName.Text.Trim();
			if( name.Length == 0 ) {
				MessageBox.Show( this, "名前を入力してください。", "RenDaClicker",
					MessageBoxButtons.OK, MessageBoxIcon.Information );
				textName.Focus();
				return;
			}
			Result.Name = name;
			Result.LeftClick = radioLeft.Checked;
			Result.HoldMode = radioHold.Checked;
			Result.Interval = (int)numInterval.Value;
			Result.HoldMs = (int)numHold.Value;
			Result.HotkeyVk = hotkeyVk;
			Result.HotkeyMods = (int)hotkeyMods;
			Result.LockPos = checkLockPos.Checked;
			DialogResult = DialogResult.OK;
		}

		// ---- ホットキーのキャプチャ ----

		private void ButtonCapture_Click( object? sender, EventArgs e ) {
			capturing = true;
			textHotkey.Text = "キー入力待ち…";
			textHotkey.Focus();
		}

		private void TextHotkey_Leave( object? sender, EventArgs e ) {
			// フォーカスが外れたらキャプチャ中止
			capturing = false;
			textHotkey.Text = Hotkey.Name( hotkeyVk, hotkeyMods );
		}

		private void TextHotkey_PreviewKeyDown( object? sender, PreviewKeyDownEventArgs e ) {
			// Tab・矢印キーなどもKeyDownへ渡す
			e.IsInputKey = true;
		}

		private void TextHotkey_KeyDown( object? sender, KeyEventArgs e ) {
			if( !capturing ) {
				return;
			}
			e.SuppressKeyPress = true;
			// 修飾キー単体の押下ではまだ確定しない(組み合わせ入力の途中の可能性)
			// → 押下中の修飾キーを表示だけ更新し、確定は KeyUp か通常キー押下で行う
			if( e.KeyCode is Keys.ShiftKey or Keys.ControlKey or Keys.Menu ) {
				textHotkey.Text = Hotkey.Name( 0, e.Modifiers ) + "…";
				return;
			}
			// 通常キー:押されている修飾キーとの組み合わせで確定
			SetHotkey( (int)e.KeyCode, e.Modifiers );
		}

		private void TextHotkey_KeyUp( object? sender, KeyEventArgs e ) {
			if( !capturing ) {
				return;
			}
			// 修飾キーを他のキーと組み合わせず離した場合は、修飾キー単体として確定
			if( e.KeyCode is Keys.ShiftKey or Keys.ControlKey or Keys.Menu ) {
				SetHotkey( (int)e.KeyCode );
			}
		}

		private void TextHotkey_MouseDown( object? sender, MouseEventArgs e ) {
			if( !capturing ) {
				return;
			}
			// マウスのサイドボタンを設定(押下中の修飾キーとの組み合わせも可)
			Keys mods = ModifierKeys & ( Keys.Control | Keys.Shift | Keys.Alt );
			if( e.Button == MouseButtons.XButton1 ) {
				SetHotkey( Hotkey.VK_XBUTTON1, mods );
			} else if( e.Button == MouseButtons.XButton2 ) {
				SetHotkey( Hotkey.VK_XBUTTON2, mods );
			}
		}

		private void SetHotkey( int vk, Keys mods = Keys.None ) {
			hotkeyVk = vk;
			hotkeyMods = mods;
			textHotkey.Text = Hotkey.Name( vk, mods );
			// キャプチャ終了(フォーカスを外す)
			ActiveControl = null;
			capturing = false;
		}
	}
}

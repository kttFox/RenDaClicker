using System.Text.Json.Serialization;
using Windows.Win32;

namespace RenDaClicker {
	/// <summary>
	/// 連打プロファイル1件分の設定。ホットキー・クリック内容ごとに複数登録できる。
	/// </summary>
	internal sealed class Profile {
		public string Name { get; set; } = "連打";
		public bool LeftClick { get; set; } = true;
		public int Interval { get; set; } = 100;
		public int HoldMs { get; set; } = 50;
		public bool HoldMode { get; set; } = true;
		public int HotkeyVk { get; set; } = (int)Keys.F6;
		public int HotkeyMods { get; set; } = 0;
		public bool LockPos { get; set; } = false;

		/// <summary>直前のTickでホットキーが押されていたか(押した瞬間の検出用)。</summary>
		[JsonIgnore]
		public bool HotkeyWasDown = true;

		/// <summary>直近に判定したホットキーの押下状態。</summary>
		[JsonIgnore]
		public bool HotkeyIsDown;

		[JsonIgnore]
		public Keys Mods => (Keys)HotkeyMods & ( Keys.Control | Keys.Shift | Keys.Alt );

		public string HotkeyText => Hotkey.Name( HotkeyVk, Mods );

		public Profile Clone() => (Profile)MemberwiseClone();

		/// <summary>ホットキーの押下判定。結果は <see cref="HotkeyIsDown"/> にも記録する。</summary>
		public bool IsHotkeyDown() {
			HotkeyIsDown = Hotkey.IsDown( HotkeyVk, Mods );
			return HotkeyIsDown;
		}
	}

	/// <summary>ホットキーの押下判定と表示名。</summary>
	internal static class Hotkey {

		public const int VK_XBUTTON1 = 0x05;
		public const int VK_XBUTTON2 = 0x06;

		public static bool IsDown( int vk, Keys mods ) {
			if( ( PInvoke.GetAsyncKeyState( vk ) & 0x8000 ) == 0 ) {
				return false;
			}
			if( mods == Keys.None ) {
				return true;
			}
			// 設定した修飾キーがすべて押されていること(不要な修飾キーが押されていても成立)
			// ただしホットキー本体が修飾キー自身の場合はそのキーを判定から除外
			foreach( var (mod, modVk) in new[] {
				(Keys.Shift, 0x10 /*VK_SHIFT*/),
				(Keys.Control, 0x11 /*VK_CONTROL*/),
				(Keys.Alt, 0x12 /*VK_MENU*/),
			} ) {
				if( !mods.HasFlag( mod ) ) {
					continue;
				}
				int left = 0xA0 + ( modVk - 0x10 ) * 2;  // VK_LSHIFT / VK_LCONTROL / VK_LMENU
				if( vk == modVk || vk == left || vk == left + 1 ) {
					continue; // 本体が該当修飾キー(左右キー含む)なら除外
				}
				if( ( PInvoke.GetAsyncKeyState( modVk ) & 0x8000 ) == 0 ) {
					return false;
				}
			}
			return true;
		}

		public static string Name( int vk, Keys mods ) {
			string name = vk switch {
				0 => "",
				VK_XBUTTON1 => "サイドボタン1",
				VK_XBUTTON2 => "サイドボタン2",
				_ => ( (Keys)vk ).ToString(),
			};
			string prefix = "";
			if( mods.HasFlag( Keys.Control ) ) prefix += "Ctrl+";
			if( mods.HasFlag( Keys.Shift ) ) prefix += "Shift+";
			if( mods.HasFlag( Keys.Alt ) ) prefix += "Alt+";
			return prefix + name;
		}
	}
}

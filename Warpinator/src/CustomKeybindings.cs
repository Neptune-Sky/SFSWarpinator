using ModLoader;
using ModLoader.Helpers;
using UnityEngine;
using static SFS.Input.KeybindingsPC;

namespace Warpinator
{
    public class DefaultKeys
    {
        public readonly Key openMenu = KeyCode.BackQuote;
    }

    public class WarpKeybindings : ModKeybindings
    {
        static readonly DefaultKeys DefaultKeys = new();

        #region Keys

        public Key openMenu = DefaultKeys.openMenu;

        #endregion

        private static WarpKeybindings main;

        public static void LoadKeybindings()
        {
            main = SetupKeybindings<WarpKeybindings>(Main.main);
            SceneHelper.OnWorldSceneLoaded += OnWorldLoad;
        }

        static void OnWorldLoad() => AddOnKeyDown_World(main.openMenu, Menu.Open);

        public override void CreateUI()
        {
            CreateUI_Text("Warpinator Keybindings");
            CreateUI_Keybinding(openMenu, DefaultKeys.openMenu, "Open Menu");
        }
    }
}
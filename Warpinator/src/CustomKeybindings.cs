using System;
using ModLoader;
using ModLoader.Helpers;
using UnityEngine;
using static SFS.Input.KeybindingsPC;

namespace Warpinator
{
    public class DefaultKeys
    {
        public Key openMenu = KeyCode.BackQuote;
    }
    public class Warp_Keybindings : ModKeybindings
    {
        static readonly DefaultKeys defaultKeys = new DefaultKeys();

        #region Keys

        public Key openMenu = defaultKeys.openMenu;

        #endregion

        public static Warp_Keybindings main;

        public static void LoadKeybindings()
        {
            main = SetupKeybindings<Warp_Keybindings>(Main.main);

            SceneHelper.OnWorldSceneLoaded += OnWorldLoad;
        }

        static void OnWorldLoad()
        {
            AddOnKeyDown_World(main.openMenu, Menu.Open);
        }

        public override void CreateUI()
        {
            CreateUI_Text("Warpinator Keybindings");
        }
    }
}

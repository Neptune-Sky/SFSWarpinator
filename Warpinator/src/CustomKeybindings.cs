﻿using ModLoader;
using ModLoader.Helpers;
using UnityEngine;
using static SFS.Input.KeybindingsPC;
// ReSharper disable MemberCanBePrivate.Global

namespace Warpinator
{
    public class DefaultKeys
    {
        public readonly Key openMenu = KeyCode.BackQuote;
    }

    public class WarpKeybindings : ModKeybindings
    {
        private static readonly DefaultKeys DefaultKeys = new();

        #region Keys

        public Key openMenu = DefaultKeys.openMenu;

        #endregion

        private static WarpKeybindings main;

        public static void LoadKeybindings()
        {
            main = SetupKeybindings<WarpKeybindings>(Main.main);
            SceneHelper.OnWorldSceneLoaded += OnWorldLoad;
        }

        private static void OnWorldLoad() => AddOnKeyDown_World(main.openMenu, PlanetSelectMenu.Open);

        public override void CreateUI()
        {
            CreateUI_Text("Warpinator Keybindings");
            CreateUI_Keybinding(openMenu, DefaultKeys.openMenu, "Open Menu");
        }
    }
}
using System;
using System.Collections.Generic;
using HarmonyLib;
using ModLoader;
using ModLoader.Helpers;
using SFS.Input;
using SFS.IO;
using SFS.UI;
using SFS.UI.ModGUI;
using TMPro;
using UITools;
using UnityEngine;
using Type = SFS.UI.ModGUI.Type;

namespace Warpinator
{
    public class Main : Mod, IUpdatable
    {
        // Properties with brief comments.
        public override string ModNameID => "Warpinator";
        public override string DisplayName => "Warpinator";
        public override string Author => "NeptuneSky";
        public override string MinimumGameVersionNecessary => "1.6";
        public override string ModVersion => "v0.8.4-alpha";
        public override string Description => "A teleporter for rockets.";
        public Dictionary<string, FilePath> UpdatableFiles => new()
        {
            // Dictionary with the link to the mod update and file path.
            {
                "https://github.com/Neptune-Sky/SFSWarpinator/releases/latest/download/Warpinator.dll",
                new FolderPath(ModFolder).ExtendToFile("Warpinator.dll")
            }
        };

        // Changelog for the current version.
        private readonly string[] changelog =
        {
            "Fixed landmark teleportation breaking the mod in update 1.6."
        };

        // Fields.
        public static FolderPath modFolder;
        private static Harmony patcher;
        public static Main main;

        // LoadKeybindings property to load keybindings when the mod is loaded.
        public override Action LoadKeybindings => WarpKeybindings.LoadKeybindings;

        // Called when the mod is loaded.
        public override void Load()
        {
            // Attach the TeleportButton.Create method to the WorldSceneLoaded event.
            SceneHelper.OnWorldSceneLoaded += TeleportButton.Create;

            // Show the changelog if there are updates and the user hasn't disabled it.
            if (Config.settings.configVersion != ModVersion && !Config.settings.noChangelogs)
            {
                var menuElement = new MenuElement(delegate(GameObject root)
                {
                    // Create the changelog window.
                    var containerObject = new GameObject("ModGUI Container");
                    var rectTransform = containerObject.AddComponent<RectTransform>();
                    rectTransform.sizeDelta = new Vector2(0, 0);

                    Window scroll = Builder.CreateWindow(rectTransform, Builder.GetRandomID(), 500, 350, 0, 0, false, false, 1, "Warpinator Changelog");
                    scroll.Position = new Vector2(0, scroll.Size.y / 2);

                    // Populate the changelog window with the changelog entries.
                    scroll.CreateLayoutGroup(Type.Vertical, TextAnchor.MiddleCenter, 25);
                    scroll.EnableScrolling(Type.Vertical);
                    containerObject.transform.SetParent(root.transform);
                    Builder.CreateSpace(scroll, 0, 50);
                    foreach (string line in changelog)
                    {
                        var label = Builder.CreateLabel(scroll, 470, 32, text: "- " + line);
                        label.TextAlignment = TextAlignmentOptions.BottomLeft;
                        label.AutoFontResize = false;
                        label.FontSize = 22;
                    }

                    Builder.CreateSpace(scroll, 0, 40);
                    Container okayButton = Builder.CreateContainer(scroll.gameObject.transform, 0, (int)-scroll.Size.y + 45);
                    okayButton.CreateLayoutGroup(Type.Horizontal);

                    // Create a toggle to disable showing the changelog again.
                    Builder.CreateToggleWithLabel(okayButton, 300, 40,
                        () => Config.settings.noChangelogs,
                        () => Config.settings.noChangelogs = !Config.settings.noChangelogs,
                        labelText: "Don't show this again"
                    );

                    // Create the "Okay" button to close the changelog window.
                    Builder.CreateButton(okayButton, 100, 50, onClick: () =>
                    {
                        Config.Save();
                        ScreenManager.main.CloseCurrent();
                    }, text: "Okay");
                });

                // Open the changelog menu.
                MenuGenerator.OpenMenu(CancelButton.Cancel, CloseMode.None, menuElement);
            }

            // Update the config version.
            Config.settings.configVersion = ModVersion;
        }

        // Called before anything from the game is loaded.
        public override void Early_Load()
        {
            // Set modFolder and main references.
            modFolder = new FolderPath(ModFolder);
            main = this;

            // Load config settings.
            Config.Load();

            // Create the Harmony patcher.
            patcher = new Harmony("Neptune.Warpinator.Mod");

            // Apply Harmony patches from the namespace.
            patcher.PatchAll();
        }
    }
}



using System;
using System.Collections.Generic;
using ModLoader;
using HarmonyLib;
using JetBrains.Annotations;
using ModLoader.Helpers;
using SFS.Input;
using SFS.IO;
using SFS.UI;
using SFS.UI.ModGUI;
using SFS.WorldBase;
using TMPro;
using UITools;
using UnityEngine;
using UnityEngine.UI;
using Type = SFS.UI.ModGUI.Type;

namespace Warpinator
{
    [UsedImplicitly]
    public class Main : Mod, IUpdatable
    {
        public override string ModNameID => "Warpinator";
        public override string DisplayName => "Warpinator";
        public override string Author => "NeptuneSky";
        public override string MinimumGameVersionNecessary => "1.5.9.8";
        public override string ModVersion => "v0.7.0-alpha";
        public override string Description => "A teleporter for rockets.";
        public Dictionary<string, FilePath> UpdatableFiles => new()
        {
            {
                "https://github.com/Neptune-Sky/SFSWarpinator/releases/latest/download/Warpinator.dll",
                new FolderPath(ModFolder).ExtendToFile("Warpinator.dll")
            }
        };

        private string[] changelog = 
        {
            "Fixed a typo in orbit selection menu",
            "Added surface teleporter prototype",
            "Counterclockwise toggle for orbit teleporter",
            "Added this changelog window"
        };
        public static FolderPath modFolder;
        public override Action LoadKeybindings => WarpKeybindings.LoadKeybindings;
    
        // This initializes the patcher. This is required if you use any Harmony patches.
        private static Harmony patcher;
    
        public static Main main;
    
        public override void Load()
        {
            // This tells the loader what to run when your mod is loaded.
            SceneHelper.OnWorldSceneLoaded += TeleportButton.Create;
            if (Config.settings.configVersion != ModVersion && !Config.settings.noChangelogs)
            {
                var menuElement = new MenuElement(delegate(GameObject root)
                {
                    var containerObject = new GameObject("ModGUI Container");
                    var rectTransform = containerObject.AddComponent<RectTransform>();
                    rectTransform.sizeDelta = new Vector2(0, 0);
                    
                    var scroll = Builder.CreateWindow(rectTransform, Builder.GetRandomID(), 500, 350, 0, 0, false, false, 1, "Warpinator Changelog");
                    
                    scroll.Position = new Vector2(0, scroll.Size.y / 2);
            
                    var layout = scroll.CreateLayoutGroup(Type.Vertical, TextAnchor.MiddleCenter, 7);
                    scroll.EnableScrolling(Type.Vertical);
                    containerObject.transform.SetParent(root.transform);
                    foreach (var line in changelog)
                    {
                        Builder.CreateLabel(scroll, 470, 32, text: "- " + line).TextAlignment = TextAlignmentOptions.Left;
                    }

                    Builder.CreateSpace(scroll, 0, 40);
                    Container okayButton = Builder.CreateContainer(scroll.gameObject.transform, 0, (int)-scroll.Size.y + 45);
                    okayButton.CreateLayoutGroup(Type.Horizontal);

                    Builder.CreateToggleWithLabel(okayButton, 300, 40,
                        () => Config.settings.noChangelogs,
                        () => Config.settings.noChangelogs = !Config.settings.noChangelogs,
                        labelText: "Don't show this again"
                    );//.toggle.gameObject.transform.localScale = new Vector3(0.9f, 0.9f);
                    Builder.CreateButton(okayButton, 100, 50, onClick: () =>
                    {
                        Config.Save();
                        ScreenManager.main.CloseCurrent();
                    }, text: "Okay");
                });
                MenuGenerator.OpenMenu(CancelButton.Cancel, CloseMode.None, menuElement);
                
            }

            Config.settings.configVersion = ModVersion;
        }
    
        public override void Early_Load()
        {
            // This method runs before anything from the game is loaded. This is where you should apply your patches, as shown below.
            modFolder = new FolderPath(ModFolder);
            main = this;
            Config.Load();
            // The patcher uses an ID formatted like a web domain.
            patcher = new Harmony("Neptune.Warpinator.Mod");
    
            // This pulls your Harmony patches from everywhere in the namespace and applies them.
            patcher.PatchAll();
        }
    }
}


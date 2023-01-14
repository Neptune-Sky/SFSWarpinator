﻿using System;
using System.Collections.Generic;
using HarmonyLib;
using ModLoader;
using ModLoader.Helpers;
using SFS.IO;
using UITools;

namespace Warpinator
{
    public class Main : Mod, IUpdatable
    {

        public override string ModNameID => "Warpinator";
        public override string DisplayName => "Warpinator";
        public override string Author => "NeptuneSky";
        public override string MinimumGameVersionNecessary => "1.5.9.8";
        public override string ModVersion => "v1.0.0";
        public override string Description => "A simple teleport menu for rockets.";
        public Dictionary<string, FilePath> UpdatableFiles
        {
            get
            {
                return new Dictionary<string, FilePath> { { "https://github.com/Neptune-Sky/SFSWarpinator/releases/latest/download/Warpinator.dll", new FolderPath(ModFolder).ExtendToFile("Warpinator.dll") } };
            }
        }
        public override Action LoadKeybindings => Warp_Keybindings.LoadKeybindings;

        // This initializes the patcher. This is required if you use any Harmony patches.
        public static Harmony patcher;

        public static Main main;

        public override void Load()
        {
            // This tells the loader what to run when your mod is loaded.
        }

        public override void Early_Load()
        {
            // This method runs before anything from the game is loaded. This is where you should apply your patches, as shown below.

            main = this;

            // The patcher uses an ID formatted like a web domain.
            patcher = new Harmony("Neptune.Warpinator.Mod");

            // This pulls your Harmony patches from everywhere in the namespace and applies them.
            patcher.PatchAll();
        }
    }
}
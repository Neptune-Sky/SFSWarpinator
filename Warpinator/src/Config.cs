using System;
using SFS.IO;
using SFS.Variables;
using UITools;
using UnityEngine;

namespace Warpinator
{
    [Serializable]
    public class SettingsData
    {
        public string configVersion = "e";
        public bool noChangelogs;
    }

    public class Config : ModSettings<SettingsData>
    {
        private static Config main;

        private Action saveAction;

        protected override FilePath SettingsFile { get; } = Main.modFolder.ExtendToFile("Config.txt");

        public static void Load()
        {
            main = new Config();
            main.Initialize();
        }

        public static void Save()
        {
            main.saveAction?.Invoke();
        }

        protected override void RegisterOnVariableChange(Action onChange)
        {
            saveAction = onChange;
            Application.quitting += onChange;
        }
    }
}


using System;
using System.Collections.Generic;
using System.Linq;
using SFS.Input;
using SFS.UI;
using SFS.UI.ModGUI;
using SFS.World;
using SFS.World.Maps;
using SFS.WorldBase;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static SFS.UI.MenuGenerator;
using static SFS.UI.ModGUI.Builder;
using Button = SFS.UI.ModGUI.Button;
using GUIElement = SFS.UI.ModGUI.GUIElement;
using Type = SFS.UI.ModGUI.Type;


namespace Warpinator
{
    public static class MoveMapCamera
    {
        public static void ToPlanet(Planet planet)
        {
            Map.manager.mapMode.Value = true;
            ScreenManager.main.CloseStack();
            Map.view.SetViewSmooth(new MapView.View(planet.mapPlanet, Double2.zero, (MoveRocket.GetOrbitParameters(planet, planet.Radius).x + planet.Radius) * 1.3));
        }

        public static void ToRocket(bool mapOpen = true)
        {
            Map.manager.mapMode.Value = mapOpen;
            ScreenManager.main.CloseStack();
            Map.view.SetViewSmooth(new MapView.View(PlayerController.main.player.Value.mapPlayer, Double2.zero, PlayerController.main.player.Value.GetSizeRadius() + 100));
        }
    }
}
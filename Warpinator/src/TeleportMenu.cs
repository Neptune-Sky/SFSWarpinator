using System;
using System.Collections.Generic;
using System.Net.Mime;
using SFS.UI;
using static SFS.UI.MenuGenerator;
using static SFS.UI.ButtonBuilder;
using SFS.WorldBase;
using SFS;
using SFS.Audio;
using SFS.Input;
using SFS.UI.ModGUI;
using SFS.World;
using SFS.World.Maps;
using UnityEngine;
using Button = SFS.UI.Button;
using Type = SFS.UI.ModGUI.Type;

namespace Warpinator
{
    public class TeleportMenu
    {
        static double DefaultHeight(Planet planet)
        {
            return planet.HasAtmospherePhysics ? planet.AtmosphereHeightPhysics * 1.1 : (planet.TimewarpRadius_Descend - planet.Radius) * 1.1;
        }
        public static void Open(Planet planet)
        {
            List<MenuElement> menuElements = new();

            const float scale = 0.75f;
            
            var output = new MenuElement(delegate(GameObject root)
            {
                GameObject containerObject = new GameObject("ModGUI Container");
                containerObject.transform.SetParent(root.transform);
                RectTransform rectTransform = containerObject.AddComponent<RectTransform>();
                rectTransform.sizeDelta = new Vector2(0, 0);
                
                Window window = Builder.CreateWindow(rectTransform, Builder.GetRandomID(), 500, 220, 0, 0, false, false, 1, "Teleport to " + planet.name);

                window.Position = new Vector2(0, window.Size.y * scale / 2);
                var layout = window.CreateLayoutGroup(Type.Vertical);
                layout.spacing = 20;
                layout.childAlignment = TextAnchor.MiddleCenter;
                Builder.CreateSpace(window, 0, 0);
                Container container = Builder.CreateContainer(window);
                container.CreateLayoutGroup(Type.Horizontal, TextAnchor.MiddleLeft, 10);
                Builder.CreateLabel(container, 200, 35, text: "Orbit Height:");
                var input = CustomUI.CreateNumberInput(container, 200, 45, DefaultHeight(planet), 0, planet.SOI * 0.99);

                Container container2 = Builder.CreateContainer(window);
                container2.CreateLayoutGroup(Type.Horizontal, TextAnchor.MiddleCenter, 10);
                Builder.CreateButton(container2, 200, 45, onClick: () => ScreenManager.main.CloseCurrent(),
                    text: "Back");
                Builder.CreateButton(container2, 200, 45, 0, 0, () =>
                {
                    MoveRocket.PerfectOrbit(planet, input.currentVal);
                }, "Go");
                window.gameObject.transform.localScale = new Vector3(scale, scale);
            });
            
            menuElements.Add(output);
            
            OpenMenu(CancelButton.Cancel, CloseMode.Stack, menuElements.ToArray());
        }
    }
}
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
    public static class Menu
    {
        static List<MenuElement> menuElements = new();
        static List<Planet> planets = new();

        static void Create()
        {
            menuElements.Clear();
            planets.Clear();
            planets.AddRange(Base.planetLoader.planets.Values);
            int columns = Math.Clamp((int)Math.Ceiling((double)(planets.Count / 12)), 2, 4);
            int rows = Math.Clamp((int)Math.Ceiling((double)planets.Count / columns), 1, 13);
            
            var output = new MenuElement(delegate(GameObject root)
            {
                var containerObject = new GameObject("ModGUI Container");
                var rectTransform = containerObject.AddComponent<RectTransform>();
                rectTransform.sizeDelta = new Vector2(0, 0);
                
                var scroll = Builder.CreateWindow(rectTransform, Builder.GetRandomID(), 275 * columns, 50 + 58 * rows, 0, 0, false, false, 1, "Warpinator");

                float scale = 0.75f;

                scroll.Position = new Vector2(0, scroll.Size.y * scale / 2);
                var layout = scroll.CreateLayoutGroup(Type.Vertical);
                layout.spacing = 7;
                layout.childAlignment = TextAnchor.MiddleCenter;
                scroll.EnableScrolling(Type.Vertical);

                Container horizontal = Builder.CreateContainer(scroll);
                horizontal.CreateLayoutGroup(Type.Horizontal);
                for (int i = 0; i < planets.Count; i++)
                {
                    Planet planet = planets[i];

                    if (i % columns == 0 && i != 0)
                    {
                        horizontal = Builder.CreateContainer(scroll);
                        horizontal.CreateLayoutGroup(Type.Horizontal);
                    }
                    var button = Builder.CreateButton(horizontal, 250, 50, 0, 0, () =>
                    {
                        MoveRocket.Teleport(planet);
                        ScreenManager.main.CloseCurrent();
                    }, planet.name);
                }

                scroll.gameObject.transform.localScale = new Vector3(scale, scale);
                containerObject.transform.SetParent(root.transform);
            });
            
            menuElements.Add(output);
        }

        public static void Open()
        {
            if (PlayerController.main.player.Value == null)
            {
                SoundPlayer.main.denySound.Play();
                MsgDrawer.main.Log("You aren't controlling a rocket!");
                return;
            }
            List<Planet> planets2 = new List<Planet>();
            planets2.AddRange(Base.planetLoader.planets.Values);

            if (planets2 != planets) Create();
            
            OpenMenu(CancelButton.Cancel, SFS.Input.CloseMode.Current, menuElements.ToArray());
        }
    }
}
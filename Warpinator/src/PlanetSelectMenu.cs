using System;
using System.Collections.Generic;
using SFS.UI;
using static SFS.UI.MenuGenerator;
using SFS.WorldBase;
using SFS;
using SFS.Audio;
using SFS.UI.ModGUI;
using SFS.World;
using UnityEngine;
using Button = SFS.UI.ModGUI.Button;
using Type = SFS.UI.ModGUI.Type;

namespace Warpinator
{
    public static class PlanetSelectMenu
    {
        static List<MenuElement> menuElements = new();
        public static List<Planet> planets = new();
        const float windowScale = 0.75f;
        const float buttonTextScale = 0.875f;
        
        static void CreateDefault()
        {
            menuElements.Clear();
            planets.Clear();
            planets.AddRange(Base.planetLoader.planets.Values);
            int columns = Mathf.Clamp((int)Math.Ceiling((double)(planets.Count / 13)), 2, 4);
            int rows = Mathf.Clamp((int)Math.Ceiling((double)planets.Count / columns), 1, 13);
            
            var output = new MenuElement(delegate(GameObject root)
            {
                var containerObject = new GameObject("ModGUI Container");
                var rectTransform = containerObject.AddComponent<RectTransform>();
                rectTransform.sizeDelta = new Vector2(0, 0);
                
                var scroll = Builder.CreateWindow(rectTransform, Builder.GetRandomID(), 275 * columns, 50 + 58 * rows, 0, 0, false, false, 1, "Warpinator");

                scroll.Position = new Vector2(0, scroll.Size.y * windowScale / 2);

                var searchButton = Builder.CreateButton(scroll.gameObject.transform, 120, 42, 0, 0, SearchHandler.OpenMenu, "Search...");
                searchButton.gameObject.GetComponentInChildren<TextAdapter>().gameObject.transform.localScale =
                    new Vector3(buttonTextScale, buttonTextScale);
                searchButton.gameObject.transform.localPosition = new Vector3(-(scroll.Size.x / 2) + searchButton.Size.x / 2 + 15, -(searchButton.Size.y / 2) - 5);
                
                var layout = scroll.CreateLayoutGroup(Type.Vertical);
                layout.spacing = 7;
                layout.childAlignment = TextAnchor.MiddleCenter;
                scroll.EnableScrolling(Type.Vertical);

                CreatePlanetButtons(planets, scroll);
                /*
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
                        PlanetTeleportMenu.Open(planet);
                    }, planet.name);
                    button.gameObject.GetComponentInChildren<TextAdapter>().gameObject.transform.localScale =
                        new Vector3(buttonTextScale, buttonTextScale);
                }
                */
                scroll.gameObject.transform.localScale = new Vector3(windowScale, windowScale);
                containerObject.transform.SetParent(root.transform);
            });
            
            menuElements.Add(output);
        }

        public static void CreatePlanetButtons(List<Planet> planetList, Transform parent)
        {
            int columns = Mathf.Clamp((int)Math.Ceiling((double)(planetList.Count / 13)), 2, 4);
            
            Container horizontal = Builder.CreateContainer(parent);
            horizontal.CreateLayoutGroup(Type.Horizontal);
            for (int i = 0; i < planetList.Count; i++)
            {
                Planet planet = planetList[i];

                if (i % columns == 0 && i != 0)
                {
                    horizontal = Builder.CreateContainer(parent);
                    horizontal.CreateLayoutGroup(Type.Horizontal);
                }
                var button = Builder.CreateButton(horizontal, 250, 50, 0, 0, () =>
                {
                    PlanetTeleportMenu.Open(planet);
                }, planet.name);
                button.gameObject.GetComponentInChildren<TextAdapter>().gameObject.transform.localScale =
                    new Vector3(buttonTextScale, buttonTextScale);
            }
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

            if (planets2 != planets) CreateDefault();
            
            OpenMenu(CancelButton.Cancel, SFS.Input.CloseMode.Current, menuElements.ToArray());
        }
    }
}
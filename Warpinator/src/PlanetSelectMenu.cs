using System;
using System.Collections.Generic;
using System.Linq;
using SFS.UI;
using static SFS.UI.MenuGenerator;
using SFS.WorldBase;
using SFS;
using SFS.Audio;
using SFS.Input;
using SFS.UI.ModGUI;
using SFS.World;
using SFS.World.Maps;
using UnityEngine;
using UnityEngine.UI;
using Button = SFS.UI.ModGUI.Button;
using Type = SFS.UI.ModGUI.Type;

namespace Warpinator
{
    public class GenericPropertyComparer<T, TKey> : IComparer<T>
    {
        private readonly Func<T, TKey> _keySelector;
        private readonly IComparer<TKey> _keyComparer;

        public GenericPropertyComparer(Func<T, TKey> keySelector, IComparer<TKey> keyComparer = null)
        {
            _keySelector = keySelector;
            _keyComparer = keyComparer ?? Comparer<TKey>.Default;
        }

        public int Compare(T x, T y)
        {
            TKey keyX = _keySelector(x);
            TKey keyY = _keySelector(y);
            return _keyComparer.Compare(keyX, keyY);
        }
    }
    public static class PlanetSelectMenu
    {
        private static readonly List<MenuElement> menuElements = new();
        public static List<Planet> planets = new();
        private const float windowScale = 0.9f;
        private const float buttonTextScale = 0.875f;

        private static void CreateDefault()
        {
            menuElements.Clear();
            planets.Clear();
            planets.AddRange(Base.planetLoader.planets.Values);

            var comparer = new GenericPropertyComparer<Planet, string>(obj =>obj.DisplayName, StringComparer.OrdinalIgnoreCase);
            planets = planets.OrderBy(planet => planet, comparer).ToList();
            
            Utilities.FindRowsAndColumns((2, 13), (2, 4), planets.Count, out int rows, out int columns);
            
            var output = new MenuElement(delegate(GameObject root)
            {
                var containerObject = new GameObject("ModGUI Container");
                var rectTransform = containerObject.AddComponent<RectTransform>();
                rectTransform.sizeDelta = new Vector2(0, 0);
                
                Window scroll = Builder.CreateWindow(rectTransform, Builder.GetRandomID(), 275 * columns, 50 + 58 * rows, 0, 0, false, false, 1, "Warpinator");

                scroll.Position = new Vector2(0, scroll.Size.y * windowScale / 2);

                Button searchButton = CustomUI.UnboundedButton(scroll, 120, 42, 15, -5, SearchHandler.OpenMenu, "Search...");
                Utilities.ButtonTextScale(searchButton, buttonTextScale);
                
                HorizontalOrVerticalLayoutGroup layout = scroll.CreateLayoutGroup(Type.Vertical);
                layout.spacing = 7;
                layout.childAlignment = TextAnchor.MiddleCenter;
                scroll.EnableScrolling(Type.Vertical);

                CreateButtons(planets, scroll);
                
                scroll.gameObject.transform.localScale = new Vector3(windowScale, windowScale);
                containerObject.transform.SetParent(root.transform);
            });
            
            menuElements.Add(output);
        }

        public static void CreateButtons<T>(List<T> list, Transform parent, int minColumns = 2, int maxColumns = 4)
        {
            // Calculate the number of columns based on the list size and clamped by minColumns and maxColumns.
            int columns = Mathf.Clamp((int)Math.Ceiling((decimal)list.Count / 13), minColumns, maxColumns);

            // Create horizontal container and layout group to organize buttons.
            Container horizontal = Builder.CreateContainer(parent);
            horizontal.CreateLayoutGroup(Type.Horizontal);

            // Iterate through the list and create buttons accordingly.
            for (var i = 0; i < list.Count; i++)
            {
                // If 'i' is a multiple of 'columns', create a new horizontal container.
                if (i % columns == 0 && i != 0)
                {
                    horizontal = Builder.CreateContainer(parent);
                    horizontal.CreateLayoutGroup(Type.Horizontal);
                }

                // Get the item from the list.
                T item = list[i];

                // Create button and handle button-specific actions.
                Button button;
                switch (item)
                {
                    case Planet planet:
                        // Create a button for the Planet.
                        button = Builder.CreateButton(horizontal, 250, 50, 0, 0, () =>
                        {
                            PlanetTeleportMenus.Open(planet);
                        }, planet.DisplayName);
                        break;
                    case Landmark landmark:
                        // Create a button for the Landmark.
                        button = Builder.CreateButton(horizontal, 250, 50, 0, 0, () =>
                        {
                            MoveRocket.PlanetSurface(landmark.gameObject.GetComponentInChildren<Planet>(), landmark.data.angle - 90, landmark.displayName);
                        }, landmark.displayName);
                        break;
                    default:
                        // Handle other types if needed.
                        continue;
                }

                // Adjust button text scale.
                Utilities.ButtonTextScale(button, buttonTextScale);
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
            var planets2 = new List<Planet>();
            planets2.AddRange(Base.planetLoader.planets.Values);

            if (planets2 != planets) CreateDefault();
            
            OpenMenu(CancelButton.Cancel, SFS.Input.CloseMode.Current, menuElements.ToArray());
        }
        public static void LandmarkSelect(List<Landmark> landmarks, Planet planet)
        {
            Utilities.FindRowsAndColumns((2, 13), (2, 4), landmarks.Count, out int rows, out int columns);
            int windowWidth = 275 * columns;
            int windowHeight = 50 + 58 * rows;

            var containerObject = new GameObject("ModGUI Container");
            var rectTransform = containerObject.AddComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(0, 0);

            var output = new MenuElement(delegate(GameObject root)
            {
                // Use the shared container object.
                rectTransform.SetParent(root.transform, false);

                Window scroll = Builder.CreateWindow(rectTransform, Builder.GetRandomID(), windowWidth, windowHeight, 0, 0, false, false, 1, planet.DisplayName + " Landmarks");

                scroll.Position = new Vector2(0, scroll.Size.y * windowScale / 2);

                HorizontalOrVerticalLayoutGroup layout = scroll.CreateLayoutGroup(Type.Vertical);
                layout.spacing = 7;
                layout.childAlignment = TextAnchor.MiddleCenter;
                scroll.EnableScrolling(Type.Vertical);

                Button backButton = CustomUI.UnboundedButton(scroll, 120, 42, 0, -5, () =>
                {
                    ScreenManager.main.CloseCurrent();
                    ScreenManager.main.CloseCurrent();
                    PlanetTeleportMenus.Open(planet, true);
                }, "Back", true);
                Utilities.ButtonTextScale(backButton, buttonTextScale);

                if (landmarks.Count <= 2) Builder.CreateSpace(scroll, 0, 25);
                CreateButtons(landmarks, scroll);

                scroll.gameObject.transform.localScale = new Vector3(windowScale, windowScale);
            });

            OpenMenu(CancelButton.Cancel, CloseMode.Current, output);
        }
    }
}
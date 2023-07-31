using System;
using System.Collections.Generic;
using System.Linq;
using SFS.Input;
using SFS.UI;
using SFS.UI.ModGUI;
using SFS.WorldBase;
using UnityEngine;
using UnityEngine.UI;
using Type = SFS.UI.ModGUI.Type;

namespace Warpinator
{
    public static class SearchHandler
    {
        private const float windowScale = 0.75f;
        public static void OpenMenu()
        {
            Menu.textInput.Open("Back", "Search", OnSearch, CloseMode.Current, TextInputMenu.Element("Planet Search", string.Empty));

            void OnSearch(string[] value)
            {
                var searchResults = FindMatches(value[0]);
                
                switch (searchResults.Count)
                {
                    case 0:
                        MenuGenerator.ShowChoices(() => "No results found.", ButtonBuilder.CreateButton(null, () => "Okay", null, CloseMode.Current));
                        break;
                    case 1:
                        PlanetTeleportMenus.Open(searchResults[0]);
                        break;
                    default:
                        OpenResultsMenu(searchResults);
                        break;
                }
            }
        }

        private static List<Planet> FindMatches(string input)
        {
            input = input.ToLowerInvariant();
            List<Planet> matches = new();
            if (string.IsNullOrWhiteSpace(input)) return matches;

            matches.AddRange(PlanetSelectMenu.planets.Where(planet => planet.DisplayName.Inject("", "").GetText().ToLowerInvariant().Contains(input)));
            return matches;
        }

        private static void OpenResultsMenu(List<Planet> planets)
        {
            int columns = Mathf.Clamp((int)Math.Ceiling((double)(planets.Count / 13)), 2, 4);
            int rows = Mathf.Clamp((int)Math.Ceiling((double)planets.Count / columns), 2, 13);
            
            var menuElement = new MenuElement(delegate(GameObject root)
            {
                var containerObject = new GameObject("ModGUI Container");
                var rectTransform = containerObject.AddComponent<RectTransform>();
                rectTransform.sizeDelta = new Vector2(0, 0);
                
                Window scroll = Builder.CreateWindow(rectTransform, Builder.GetRandomID(), 275 * columns, 50 + 58 * rows, 0, 0, false, false, 1, "Search Results");

                scroll.Position = new Vector2(0, scroll.Size.y * windowScale / 2);
                
                HorizontalOrVerticalLayoutGroup layout = scroll.CreateLayoutGroup(Type.Vertical);
                layout.spacing = 7;
                layout.childAlignment = TextAnchor.MiddleCenter;
                scroll.EnableScrolling(Type.Vertical);
                if (planets.Count <= 2) Builder.CreateSpace(scroll, 0, 25);
                PlanetSelectMenu.CreateButtons(planets, scroll);
                
                scroll.gameObject.transform.localScale = new Vector3(windowScale, windowScale);
                containerObject.transform.SetParent(root.transform);
            });

            MenuElement[] elements = { menuElement };
            MenuGenerator.OpenMenu(CancelButton.Cancel, CloseMode.Current, elements);
        }
    }
}
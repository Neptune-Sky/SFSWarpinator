using System;
using System.Collections.Generic;
using System.Linq;
using SFS.Input;
using SFS.UI;
using SFS.UI.ModGUI;
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
    internal static class PlanetTeleportMenus
    {
        private static double DefaultHeight(Planet planet)
        {
            return (planet.HasAtmospherePhysics ? planet.AtmosphereHeightPhysics * 1.1 : (planet.TimewarpRadius_Descend - planet.Radius) * 1.1).Round(0.001);
        }
        public static void Open(Planet planet, bool startAtSurface = false)
        {
            var menuElements = new List<MenuElement>();

            const float scale = 0.8f;

            var output = new MenuElement(delegate (GameObject root)
            {
            // Container setup
                var containerObject = new GameObject("ModGUI Container");
                containerObject.transform.SetParent(root.transform);
                var rectTransform = containerObject.AddComponent<RectTransform>();
                rectTransform.sizeDelta = new Vector2(0, 0);

                // Window setup
                Window window = CreateWindow(rectTransform, GetRandomID(), 500, 550, 0, 0, false, false, 1, "Teleport to " + planet.DisplayName);
                window.Position = new Vector2(0, window.Size.y * scale / 2);
                HorizontalOrVerticalLayoutGroup layout = window.CreateLayoutGroup(Type.Vertical);
                layout.spacing = 20;
                layout.childAlignment = TextAnchor.MiddleCenter;

                // Orbit and Surface Buttons
                Container menuSelect = CreateContainer(window);
                menuSelect.CreateLayoutGroup(Type.Horizontal, TextAnchor.UpperCenter, spacing: 10);
                Button orbitButton = CreateButton(menuSelect, 200, 45, text: "Orbit");
                Button surfaceButton = CreateButton(menuSelect, 200, 45, text: "Surface");

                // Orbit and Surface Menu Containers
                Container orbitMenu = OrbitMenu(window, planet);
                Container surfaceMenu = SurfaceMenu(window, planet);
                surfaceMenu.Active = false;

                // ButtonPC components
                var orbitButtonPC = orbitButton.gameObject.GetComponent<ButtonPC>();

                var surfaceButtonPC = surfaceButton.gameObject.GetComponent<ButtonPC>();

                // Shared click handler for Orbit and Surface Buttons
                void OnButtonClick(GUIElement menu, GUIElement otherMenu, ButtonPC buttonPC)
                {
                    otherMenu.Active = false;
                    menu.Active = true;
                    buttonPC.SetSelected(true);
                    (buttonPC == orbitButtonPC ? surfaceButtonPC : orbitButtonPC).SetSelected(false);

                    // Adjust window size and position
                    window.Size = new Vector2(500, buttonPC == orbitButtonPC ? 550 : 445);
                    window.Position = new Vector2(0, window.Size.y * scale / 2);
                }

                orbitButton.OnClick += () => OnButtonClick(orbitMenu, surfaceMenu, orbitButtonPC);
                surfaceButton.OnClick += () => OnButtonClick(surfaceMenu, orbitMenu, surfaceButtonPC);

                // Optionally start at Surface
                if (startAtSurface) OnButtonClick(surfaceMenu, orbitMenu, surfaceButtonPC);
                else orbitButtonPC.SetSelected(true);

                window.gameObject.transform.localScale = new Vector3(scale, scale);
            });

                menuElements.Add(output);

                OpenMenu(CancelButton.Cancel, CloseMode.Stack, menuElements.ToArray());
        }

        private static Container OrbitMenu(Window window, Planet planet)
        {
            Container ToReturn = CreateContainer(window);
            ToReturn.CreateLayoutGroup(Type.Vertical);

            Box parameters = CreateBox(ToReturn, 450, 150);
            parameters.CreateLayoutGroup(Type.Vertical, TextAnchor.MiddleLeft, 10, new RectOffset(20, 0, 0, 0));

            Container orbitHeightContainer = CreateContainer(parameters);
            orbitHeightContainer.CreateLayoutGroup(Type.Horizontal, TextAnchor.MiddleCenter, 10);
            
            Label heightLabel = CreateLabel(orbitHeightContainer, 200, 35, text: "Orbit Height:");
            heightLabel.TextAlignment = TextAlignmentOptions.Left;
            NumberInput orbitInput = CustomUI.CreateNumberInput(orbitHeightContainer, 200, 45, DefaultHeight(planet), 0, (planet.SOI - planet.Radius) * 0.999);

            CreateSeparator(parameters, 410);
            
            var counterclockwise = false;
            ToggleWithLabel toggle = CreateToggleWithLabel(parameters, 410, 35, () => counterclockwise, () => counterclockwise = !counterclockwise, labelText: "Counterclockwise:");
            toggle.label.TextAlignment = TextAlignmentOptions.Left;
            
            Box statBox = CreateBox(ToReturn, 450, 180);
            statBox.CreateLayoutGroup(Type.Vertical, TextAnchor.MiddleLeft, 10);
            
            CreateLabel(statBox, 450, 35, text: "Projected Orbit");
            
            Container stats = CreateContainer(statBox);
            stats.CreateLayoutGroup(Type.Vertical, TextAnchor.MiddleCenter, 10, new RectOffset(10, 0, 0, 0));

            Label velocity = CreateLabel(stats, 425, 30, text: "Velocity: " + MoveRocket.GetOrbitParameters(planet, orbitInput.currentVal).y.ToVelocityString());
            velocity.TextAlignment = TextAlignmentOptions.Left;

            Label apoapsis = CreateLabel(stats, 425, 30, text: "Apoapsis: " + orbitInput.currentVal.ToDistanceString());
            apoapsis.TextAlignment = TextAlignmentOptions.Left;

            Label periapsis = CreateLabel(stats, 425, 30, text: "Periapsis: " + orbitInput.currentVal.ToDistanceString());
            periapsis.TextAlignment = TextAlignmentOptions.Left;
            
            orbitInput.textInput.OnChange += _ =>
            {
                velocity.Text = "Velocity: " + MoveRocket.GetOrbitParameters(planet, orbitInput.currentVal).y.ToVelocityString();
                apoapsis.Text = "Apoapsis: " + orbitInput.currentVal.ToDistanceString();
                periapsis.Text = "Periapsis: " + orbitInput.currentVal.ToDistanceString();
            };

            Container cancelContinueButtons = CreateContainer(ToReturn);
            cancelContinueButtons.CreateLayoutGroup(Type.Horizontal, TextAnchor.MiddleCenter, 10);
            CreateButton(cancelContinueButtons, 200, 45, onClick: () => ScreenManager.main.CloseCurrent(),
                text: "Back");
            CreateButton(cancelContinueButtons, 200, 45, 0, 0, () =>
            {
                MoveRocket.PerfectOrbit(planet, orbitInput.currentVal, counterclockwise);
            }, "Go");

            return ToReturn;
        }

        private static Container SurfaceMenu(Window window, Planet planet)
        {
            Container ToReturn = CreateContainer(window);
            ToReturn.CreateLayoutGroup(Type.Vertical);
            
            Box parameters = CreateBox(ToReturn, 450, 120);
            parameters.CreateLayoutGroup(Type.Vertical, TextAnchor.MiddleLeft, 10, new RectOffset(20, 0, 0, 0));
            
            Container parametersContainer = CreateContainer(parameters);
            parametersContainer.CreateLayoutGroup(Type.Horizontal, TextAnchor.MiddleCenter, 10);
            
            CreateLabel(parametersContainer, 200, 35, text: "Surface Angle:").TextAlignment = TextAlignmentOptions.Left;
            NumberInput input = CustomUI.CreateNumberInput(parametersContainer, 200, 45, 0, -360, 360);
            
            var comparer = new GenericPropertyComparer<Landmark, string>(obj =>obj.displayName, StringComparer.OrdinalIgnoreCase);
            Button landmarkButton = CreateButton(parameters, 400, 50, text: "Use Landmark");
            if (planet.landmarks.Length == 0) landmarkButton.gameObject.GetComponent<ButtonPC>().SetEnabled(false);
            else
                landmarkButton.OnClick = () =>
                {
                    PlanetSelectMenu.LandmarkSelect(planet.landmarks.OrderBy(landmark => landmark, comparer).ToList(), planet);
                };
            
            Box box = CreateBox(ToReturn, 450, 100);
            box.CreateLayoutGroup(Type.Vertical, TextAnchor.MiddleCenter, 10);
            
            CreateLabel(box, 425, 90, text: "NOTE: This function is in an early state. Use at your own risk.");
            
            Container cancelContinueButtons = CreateContainer(ToReturn);
            cancelContinueButtons.CreateLayoutGroup(Type.Horizontal, TextAnchor.MiddleCenter, 10);
            CreateButton(cancelContinueButtons, 200, 45, onClick: () => ScreenManager.main.CloseCurrent(),
                text: "Back");
            CreateButton(cancelContinueButtons, 200, 45, 0, 0, () =>
            {
                MoveRocket.PlanetSurface(planet, (float)input.currentVal);
            }, "Go");

            return ToReturn;
        }
    }
}
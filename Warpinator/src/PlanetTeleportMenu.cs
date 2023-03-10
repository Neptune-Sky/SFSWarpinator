using System.Collections.Generic;
using SFS.Input;
using SFS.UI;
using SFS.UI.ModGUI;
using SFS.WorldBase;
using TMPro;
using UnityEngine;
using static SFS.UI.MenuGenerator;
using static SFS.UI.ModGUI.Builder;

namespace Warpinator
{
    static class PlanetTeleportMenu
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
                
                Window window = CreateWindow(rectTransform, GetRandomID(), 500, 550, 0, 0, false, false, 1, "Teleport to " + planet.name);

                window.Position = new Vector2(0, window.Size.y * scale / 2);
                var layout = window.CreateLayoutGroup(Type.Vertical);
                layout.spacing = 20;
                layout.childAlignment = TextAnchor.MiddleCenter;
                Container menuSelect = CreateContainer(window);
                menuSelect.CreateLayoutGroup(Type.Horizontal, TextAnchor.UpperCenter, spacing: 10);
                var orbitButton = CreateButton(menuSelect, 200, 45, text: "Orbit");
                var surfaceButton = CreateButton(menuSelect, 200, 45, text: "Surface");

                Container orbitMenu = OrbitMenu(window, planet);
                Container surfaceMenu = SurfaceMenu(window, planet);
                surfaceMenu.Active = false;
                orbitButton.gameObject.GetComponent<ButtonPC>().SetSelected(true);
                
                orbitButton.OnClick += () =>
                {
                    surfaceMenu.Active = false;
                    orbitMenu.Active = true;
                    orbitButton.gameObject.GetComponent<ButtonPC>().SetSelected(true);
                    surfaceButton.gameObject.GetComponent<ButtonPC>().SetSelected(false);
                    
                    window.Size = new Vector2(500, 550);
                    window.Position = new Vector2(0, window.Size.y * scale / 2);
                };

                surfaceButton.OnClick += () =>
                {
                    orbitMenu.Active = false;
                    surfaceMenu.Active = true;
                    surfaceButton.gameObject.GetComponent<ButtonPC>().SetSelected(true);
                    orbitButton.gameObject.GetComponent<ButtonPC>().SetSelected(false);

                    window.Size = new Vector2(500, 445);
                    window.Position = new Vector2(0, window.Size.y * scale / 2);
                };
                
                window.gameObject.transform.localScale = new Vector3(scale, scale);
            });
            
            menuElements.Add(output);
            
            OpenMenu(CancelButton.Cancel, CloseMode.Stack, menuElements.ToArray());
        }

        static Container OrbitMenu(Window window, Planet planet)
        {
            Container ToReturn = CreateContainer(window);
            ToReturn.CreateLayoutGroup(Type.Vertical);

            Box parameters = CreateBox(ToReturn, 450, 150);
            parameters.CreateLayoutGroup(Type.Vertical, TextAnchor.MiddleLeft, 10, new RectOffset(20, 0, 0, 0));

            Container orbitHeightContainer = CreateContainer(parameters);
            orbitHeightContainer.CreateLayoutGroup(Type.Horizontal, TextAnchor.MiddleCenter, 10);
            
            var heightLabel = CreateLabel(orbitHeightContainer, 200, 35, text: "Orbit Height:");
            heightLabel.TextAlignment = TextAlignmentOptions.Left;
            var orbitInput = CustomUI.CreateNumberInput(orbitHeightContainer, 200, 45, DefaultHeight(planet), 0, (planet.SOI - planet.Radius) * 0.999);

            CreateSeparator(parameters, 410);
            
            bool counterclockwise = false;
            var toggle = CreateToggleWithLabel(parameters, 410, 35, () => counterclockwise, () => counterclockwise = !counterclockwise, labelText: "Counterclockwise:");
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

        static Container SurfaceMenu(Window window, Planet planet)
        {
            Container ToReturn = CreateContainer(window);
            ToReturn.CreateLayoutGroup(Type.Vertical);
            
            Box parameters = CreateBox(ToReturn, 450, 100);
            parameters.CreateLayoutGroup(Type.Vertical, TextAnchor.MiddleLeft, 10, new RectOffset(20, 0, 0, 0));
            
            Container parametersContainer = CreateContainer(parameters);
            parametersContainer.CreateLayoutGroup(Type.Horizontal, TextAnchor.MiddleCenter, 10);
            
            CreateLabel(parametersContainer, 200, 35, text: "Surface Angle:").TextAlignment = TextAlignmentOptions.Left;
            var input = CustomUI.CreateNumberInput(parametersContainer, 200, 45, 0, -360, 360);
            
            Box box = CreateBox(ToReturn, 450, 100);
            box.CreateLayoutGroup(Type.Vertical, TextAnchor.MiddleCenter, 10);
            
            CreateLabel(box, 425, 70, text: "NOTE: This function is in an early state. Use at your own risk.");
            
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
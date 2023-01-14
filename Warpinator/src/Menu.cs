using System;
using System.Collections.Generic;
using SFS.UI;
using static SFS.UI.MenuGenerator;
using static SFS.UI.ButtonBuilder;
using SFS.WorldBase;
using SFS;
using SFS.World;

namespace Warpinator
{
    public static class Menu
    {
        static List<MenuElement> menuElements = new List<MenuElement>();
        static List<Planet> planets = new List<Planet>();

        static void Create()
        {
            menuElements.Clear();
            TextBuilder title = TextBuilder.CreateText();
            title.Text(() => "Warpinator");
            menuElements.Add(title);
            menuElements.Add(ElementGenerator.VerticalSpace(20));

            planets.Clear();
            planets.AddRange(Base.planetLoader.planets.Values);

            List<MenuElement> horizontalElements = new List<MenuElement>();
            int columns = (int)Math.Ceiling((double)(planets.Count / 12));
            if (columns == 0) columns++;

            for (int i = 0; i < planets.Count; i++)
            {

                Planet planet = planets[i];

                ButtonBuilder button = CreateButton(null, () => planet.DisplayName, () =>
                {
                    MsgDrawer.main.Log(planet.DisplayName);
                    Rocket rocket = PlayerController.main.player.Value as Rocket;
                    var parameters = GetOrbitParameters(planet);
                    rocket.location.position.Value = new Double2(planet.GetLocation(WorldTime.main.worldTime).position.x, planet.GetLocation(WorldTime.main.worldTime).position.y + parameters.x);
                    rocket.location.planet.Value = planet;

                }, SFS.Input.CloseMode.Current);

                horizontalElements.Add(button);

                if ((horizontalElements.Count % columns == 0 && horizontalElements.Count > 0) || i == planets.Count - 1)
                {
                    menuElements.Add(ElementGenerator.DefaultHorizontalGroup(horizontalElements.ToArray()));
                    horizontalElements.Clear();
                }
            }
        }

        public static void Open()
        {
            List<Planet> planets2 = new List<Planet>();
            planets2.AddRange(Base.planetLoader.planets.Values);

            if (planets2 != planets) Create();

            OpenMenu(CancelButton.Cancel, SFS.Input.CloseMode.Current, menuElements.ToArray());
        }

        static Double2 GetOrbitParameters(Planet planet)
        {
            double distance = planet.HasAtmospherePhysics ? planet.AtmosphereHeightPhysics * 1.05 : planet.TimewarpRadius_Descend * 1.05;

            double G = 6.6743 * Math.Pow(10, -11);

            double velocity = Math.Sqrt(G * planet.mass / distance);

            return new Double2(distance, velocity);
        }
    }
}

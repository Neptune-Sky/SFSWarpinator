using System;
using System.Collections.Generic;
using HarmonyLib;
using SFS.UI;
using static SFS.UI.MenuGenerator;
using static SFS.UI.ButtonBuilder;
using SFS.WorldBase;
using SFS;
using SFS.Audio;
using SFS.World;
using SFS.World.Maps;
using UnityEngine;

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
                    MsgDrawer.main.Log("Teleporting to " + planet.DisplayName);
                    var indexOf = GameManager.main.rockets.IndexOf(PlayerController.main.player.Value as Rocket);
                    PlayerController.main.player.Value = null;
                    Double2 parameters = GetOrbitParameters(planet);
                    Debug.Log(parameters);
                    Location nullLocation = new Location(0, planet, new Double2(0, 0), new Double2(0, 0));
                    Location location = new Location(0, planet, new Double2(0, planet.Radius + parameters.x), new Double2(parameters.y, 0));
                    GameManager.main.rockets[indexOf].physics.SetLocationAndState(nullLocation, false);
                    GameManager.main.rockets[indexOf].physics.SetLocationAndState(location, false);
                    PlayerController.main.player.Value = GameManager.main.rockets[indexOf];
                    Map.view.ToggleFocus(GameManager.main.rockets[indexOf].mapPlayer);
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

        static Double2 GetOrbitParameters(Planet planet)
        {
            double distance = planet.HasAtmospherePhysics
                ? planet.AtmosphereHeightPhysics * 1.1
                : planet.TimewarpRadius_Descend * 1.1 - planet.Radius;

            double velocity = Math.Sqrt(planet.mass / (distance + planet.Radius));

            return new Double2(distance, velocity);
        }
    }
}
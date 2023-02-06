using System;
using System.Collections.Generic;
using SFS.UI;
using static SFS.UI.MenuGenerator;
using static SFS.UI.ButtonBuilder;
using SFS.WorldBase;
using SFS;
using SFS.Audio;
using SFS.World;
using SFS.World.Maps;

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
                    MoveRocket.Teleport(planet);
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
    }
}
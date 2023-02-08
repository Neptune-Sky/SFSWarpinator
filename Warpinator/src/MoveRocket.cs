using System;
using System.Media;
using SFS.Input;
using SFS.UI;
using SFS.World;
using SFS.World.Maps;
using SFS.WorldBase;
using SoundPlayer = SFS.Audio.SoundPlayer;

namespace Warpinator
{
    public class MoveRocket
    {
        enum InvalidType
        {
            None,
            BelowTerrain,
            InsideAtmosphere
        }
        static InvalidType ValidityCheck(Planet planet, double distance)
        {
            double rocketHeight = ((Rocket)PlayerController.main.player.Value).GetSizeRadius();
            if (distance < planet.maxTerrainHeight + rocketHeight)
            {
                return InvalidType.BelowTerrain;
            }
            if (planet.HasAtmospherePhysics && distance < planet.AtmosphereHeightPhysics + rocketHeight)
            {
                return InvalidType.InsideAtmosphere;
            }
            return InvalidType.None;
        }
        public static void PerfectOrbit(Planet planet, double distance)
        {
            switch (ValidityCheck(planet, distance))
            {
                case InvalidType.None:
                    ExecutePerfectOrbit(planet, distance);
                    break;
                case InvalidType.BelowTerrain:
                    ScreenManager.main.CloseStack();
                    SoundPlayer.main.denySound.Play();
                    MsgDrawer.main.Log("Cannot teleport below planet surface");
                    break;
                case InvalidType.InsideAtmosphere:
                    MenuGenerator.OpenConfirmation(CloseMode.Current, () => "The height you set is within this planet's atmosphere, and may destroy your rocket from heating.\n\nAre you sure?", () => "Yes", () =>
                    {
                        ExecutePerfectOrbit(planet, distance);
                        WorldTime.main.StopTimewarp(false);
                    }, () => "No");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        static void ExecutePerfectOrbit(Planet planet, double distance)
        {
            WorldTime.main.SetState(0, false, false);
            MsgDrawer.main.Log("Teleporting to " + planet.DisplayName);
            var indexOf = GameManager.main.rockets.IndexOf(PlayerController.main.player.Value as Rocket);
            PlayerController.main.player.Value = null;
            Double2 parameters = GetOrbitParameters(planet, distance);
            Location nullLocation = new Location(0, planet, new Double2(0, 0), new Double2(0, 0));
            Location location = new Location(0, planet, new Double2(0, parameters.x), new Double2(parameters.y, 0));
                    
            GameManager.main.rockets[indexOf].physics.SetLocationAndState(nullLocation, false);
                    
            GameManager.main.rockets[indexOf].physics.SetLocationAndState(location, false);
            PlayerController.main.player.Value = GameManager.main.rockets[indexOf];
            Map.view.ToggleFocus(GameManager.main.rockets[indexOf].mapPlayer);
            ScreenManager.main.CloseStack();
            WorldTime.main.StopTimewarp(false);
        }

        public static Double2 GetOrbitParameters(Planet planet, double distance)
        {
            distance += planet.Radius;

            double velocity = Math.Sqrt(planet.mass / (distance));

            return new Double2(distance, velocity);
        }
    }
}
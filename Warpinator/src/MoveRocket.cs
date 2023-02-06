using System;
using SFS.UI;
using SFS.World;
using SFS.World.Maps;
using SFS.WorldBase;

namespace Warpinator
{
    public class MoveRocket
    {
        public static void Teleport(Planet planet)
        {
            MsgDrawer.main.Log("Teleporting to " + planet.DisplayName);
            var indexOf = GameManager.main.rockets.IndexOf(PlayerController.main.player.Value as Rocket);
            PlayerController.main.player.Value = null;
            Double2 parameters = GetOrbitParameters(planet);
            Location nullLocation = new Location(0, planet, new Double2(0, 0), new Double2(0, 0));
            Location location = new Location(0, planet, new Double2(0, planet.Radius + parameters.x), new Double2(parameters.y, 0));
                    
            GameManager.main.rockets[indexOf].physics.SetLocationAndState(nullLocation, false);
                    
            GameManager.main.rockets[indexOf].physics.SetLocationAndState(location, false);
            PlayerController.main.player.Value = GameManager.main.rockets[indexOf];
            Map.view.ToggleFocus(GameManager.main.rockets[indexOf].mapPlayer);
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
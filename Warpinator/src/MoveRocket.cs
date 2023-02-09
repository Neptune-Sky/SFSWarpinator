using System;
using System.Media;
using SFS.Input;
using SFS.UI;
using SFS.World;
using SFS.World.Maps;
using SFS.WorldBase;
using UnityEngine;
using SoundPlayer = SFS.Audio.SoundPlayer;

namespace Warpinator
{
    public static class MoveRocket
    {
        enum InvalidType
        {
            None,
            BelowTerrain,
            InsideAtmosphere
        }
        static InvalidType OrbitCheck(Planet planet, double distance)
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
            switch (OrbitCheck(planet, distance))
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
            MsgDrawer.main.Log("Teleporting to " + planet.DisplayName);
            Double2 parameters = GetOrbitParameters(planet, distance);
            Location location = new Location(0, planet, new Double2(0, parameters.x), new Double2(parameters.y, 0));
            
            Execute(location);
            
            Map.view.SetViewSmooth(new MapView.View(PlayerController.main.player.Value.location.planet.Value.mapPlanet, Double2.zero, (parameters.x + planet.Radius) * 1.3));
        }

        public static Double2 GetOrbitParameters(Planet planet, double distance)
        {
            distance += planet.Radius;

            double velocity = Math.Sqrt(planet.mass / (distance));

            return new Double2(distance, velocity);
        }
        
        static Location GetNearRocket(Rocket rocket)
        {
            Double2 addPos = new Double2(0, rocket.GetSizeRadius() * 1.1 +  PlayerController.main.player.Value.GetSizeRadius() * 1.1).Rotate(270 * Mathf.Deg2Rad);
            double rocketAngle = rocket.location.position.Value.AngleRadians;
            addPos = addPos.Rotate(rocketAngle);
            return new Location(rocket.location.Value.time, rocket.location.planet.Value, rocket.location.position.Value + addPos, rocket.location.velocity);
        }
        public static void ToRocket(Rocket rocket)
        {
            string name = String.IsNullOrEmpty(rocket.rocketName) ? "Rocket" : rocket.rocketName;
            MsgDrawer.main.Log("Teleporting to " + name);
            Execute(GetNearRocket(rocket));
            
            Map.manager.mapMode.Value = false;
            Map.view.SetViewSmooth(new MapView.View(PlayerController.main.player.Value.mapPlayer, Double2.zero, rocket.GetSizeRadius() + 100));
        }
        
        static void Execute(Location location)
        {
            ScreenManager.main.CloseStack();
            
            WorldTime.main.SetState(0, false, false);
            var indexOf = GameManager.main.rockets.IndexOf(PlayerController.main.player.Value as Rocket);
            PlayerController.main.player.Value = null;
            
            Location nullLocation = new Location(0, location.planet, new Double2(0, 0), new Double2(0, 0));
            
            GameManager.main.rockets[indexOf].physics.SetLocationAndState(nullLocation, false);
                    
            GameManager.main.rockets[indexOf].physics.SetLocationAndState(location, false);
            PlayerController.main.player.Value = GameManager.main.rockets[indexOf];
            
            WorldTime.main.StopTimewarp(false);
        }
    }
}
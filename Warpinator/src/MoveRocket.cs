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
        private enum InvalidType
        {
            None,
            BelowTerrain,
            InsideAtmosphere
        }

        private static InvalidType OrbitCheck(Planet planet, double distance)
        {
            double rocketHeight = ((Rocket)PlayerController.main.player.Value).GetSizeRadius();
            if (distance < planet.maxTerrainHeight + rocketHeight && planet.data.terrain.collider)
            {
                return InvalidType.BelowTerrain;
            }
            if (planet.HasAtmospherePhysics && distance < planet.AtmosphereHeightPhysics + rocketHeight)
            {
                return InvalidType.InsideAtmosphere;
            }
            return InvalidType.None;
        }
        public static void PerfectOrbit(Planet planet, double distance, bool counterclockwise = false)
        {
            switch (OrbitCheck(planet, distance))
            {
                case InvalidType.None:
                    ExecutePerfectOrbit(planet, distance, counterclockwise);
                    break;
                case InvalidType.BelowTerrain:
                    SoundPlayer.main.denySound.Play();
                    MenuGenerator.ShowChoices(() => "Cannot teleport below planet's surface.", ButtonBuilder.CreateButton(null, () => "Okay", null, CloseMode.Current));
                    break;
                case InvalidType.InsideAtmosphere:
                    MenuGenerator.OpenConfirmation(CloseMode.Current, () => "The height you set is within this planet's atmosphere, and may destroy your rocket from heating.\n\nAre you sure?", () => "Yes", () =>
                    {
                        ExecutePerfectOrbit(planet, distance, counterclockwise);
                        WorldTime.main.StopTimewarp(false);
                    }, () => "No");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static void ExecutePerfectOrbit(Planet planet, double distance, bool counterclockwise = false)
        {
            MsgDrawer.main.Log("Teleporting to " + planet.DisplayName);
            Double2 parameters = GetOrbitParameters(planet, distance);
            double velocity = counterclockwise ? -parameters.y : parameters.y;
            var location = new Location(0, planet, new Double2(0, parameters.x), new Double2(velocity, 0));
            
            Execute(location);
            
            Map.view.SetViewSmooth(new MapView.View(PlayerController.main.player.Value.location.planet.Value.mapPlanet, Double2.zero, (parameters.x + planet.Radius) * 1.3));
        }

        public static Double2 GetOrbitParameters(Planet planet, double distance)
        {
            distance += planet.Radius;

            double velocity = Math.Sqrt(planet.mass / (distance));

            return new Double2(distance, velocity);
        }

        private static Location GetNearRocket(Player rocket)
        {
            Double2 addPos = new Double2(0, rocket.GetSizeRadius() * 1.1 +  PlayerController.main.player.Value.GetSizeRadius() * 1.1).Rotate(270 * Mathf.Deg2Rad);
            double rocketAngle = rocket.location.position.Value.AngleRadians;
            addPos = addPos.Rotate(rocketAngle);
            return new Location(rocket.location.Value.time, rocket.location.planet.Value, rocket.location.position.Value + addPos, rocket.location.velocity);
        }
        public static void ToRocket(Rocket rocket)
        {
            string name = string.IsNullOrEmpty(rocket.rocketName) ? "Rocket" : rocket.rocketName;
            MsgDrawer.main.Log("Teleporting to " + name);
            Execute(GetNearRocket(rocket));
            
            Map.manager.mapMode.Value = false;
            Map.view.SetViewSmooth(new MapView.View(PlayerController.main.player.Value.mapPlayer, Double2.zero, rocket.GetSizeRadius() + 100));
        }

        public static void PlanetSurface(Planet planet, double degrees)
        {
            double angle = degrees * Mathf.Deg2Rad;
            MsgDrawer.main.Log("Teleporting to " + planet.DisplayName);
            Double2 position = new Double2(
                0,
                planet.GetTerrainHeightAtAngle(angle + 90 * Mathf.Deg2Rad) + planet.Radius + PlayerController.main.player.Value.GetSizeRadius() / 2 + 3
                ).Rotate(angle);

            var location = new Location(0, planet, position, new Double2(0, 0));
            Execute(location);
            
            var rocket = PlayerController.main.player.Value as Rocket;
            if (rocket == null) return;
            rocket.EnableCollisionImmunity(6);
            rocket.partHolder.transform.eulerAngles = new Vector3(0, 0, (float)location.position.AngleDegrees - 90);
            
            Map.view.SetViewSmooth(new MapView.View(rocket.mapPlayer, Double2.zero, PlayerController.main.player.Value.GetSizeRadius() + 100));
        }

        private static void Execute(Location location)
        {
            ScreenManager.main.CloseStack();
            
            WorldTime.main.SetState(0, false, false);
            int indexOf = GameManager.main.rockets.IndexOf(PlayerController.main.player.Value as Rocket);
            PlayerController.main.player.Value = null;
            
            var nullLocation = new Location(0, location.planet, new Double2(0, 0), new Double2(0, 0));
            
            GameManager.main.rockets[indexOf].physics.SetLocationAndState(nullLocation, false);
                    
            GameManager.main.rockets[indexOf].physics.SetLocationAndState(location, false);
            PlayerController.main.player.Value = GameManager.main.rockets[indexOf];
            
            WorldTime.main.StopTimewarp(false);
        }
    }
}
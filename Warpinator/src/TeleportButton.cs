using SFS.Audio;
using SFS.UI;
using SFS.World;
using SFS.World.Maps;
using UnityEngine;

namespace Warpinator
{
    public static class TeleportButton
    {
        private static readonly IconButton teleport = new();
        public static void Create()
        {
            
            GameObject parent = GameSelector.main.focusButton.button.gameObject;
            teleport.button = Object.Instantiate(parent).GetComponent<Button>();
            GameObject gameObject = teleport.button.gameObject;
            gameObject.transform.SetParent(parent.transform.parent);
            gameObject.transform.localScale = Vector3.one;
            teleport.text = teleport.button.gameObject.GetComponentInChildren<TextAdapter>();
            teleport.text.Text = "Teleport";
            teleport.Show = false;
            GameSelector.main.selected.OnChange += selected =>
            {
                if (PlayerController.main.player.Value == null)
                {
                    teleport.Show = false;
                    return;
                }
                switch (selected)
                {
                    case MapRocket mapRocket when !mapRocket.Player.isPlayer:
                    case MapPlanet:
                        teleport.Show = true;
                        break;
                    default:
                        teleport.Show = false;
                        break;
                }
            };
            teleport.button.onClick += Clicked;
        }
        public static void Clicked()
        {
            if (PlayerController.main.player.Value == null)
            {
                SoundPlayer.main.denySound.Play();
                MsgDrawer.main.Log("You aren't controlling a rocket!");
                return;
            }
            SelectableObject mapObject;
            if (GameSelector.main.selected.Value != null)
                mapObject = GameSelector.main.selected.Value;
            else if (Map.manager.mapMode.Value)
            {
                mapObject = Map.view.view.target.Value;
                if (mapObject == PlayerController.main.player.Value.mapPlayer)
                    mapObject = PlayerController.main.player.Value.location.planet.Value.mapPlanet;
            }
            else
                mapObject = PlayerController.main.player.Value.location.planet.Value.mapPlanet;

            switch (mapObject)
            {
                case MapPlanet mapPlanet:
                    PlanetTeleportMenus.Open(mapPlanet.planet);
                    break;
                case MapRocket mapRocket:
                    MoveRocket.ToRocket(mapRocket.rocket);
                    break;
            }
        }
    }
}
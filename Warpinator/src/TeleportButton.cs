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
            teleport.button.onClick += () =>
            {
                switch (GameSelector.main.selected.Value)
                {
                    case MapPlanet mapPlanet:
                        PlanetTeleportMenu.Open(mapPlanet.planet);
                        break;
                    case MapRocket mapRocket:
                        MoveRocket.ToRocket(mapRocket.rocket);
                        break;
                }
            };

        }
    }
}
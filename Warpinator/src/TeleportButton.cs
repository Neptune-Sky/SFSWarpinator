using HarmonyLib;
using SFS.UI;
using SFS.World;
using SFS.World.Maps;
using UnityEngine;

namespace Warpinator
{
    public static class TeleportButton
    {
        private static readonly GameSelector.SelectMenuButton teleport = new();
        public static void Create()
        {
            
            var parent = GameSelector.main.focusButton.button.gameObject;
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
                if (selected is MapPlayer mapPlayer && mapPlayer.Player.isPlayer)
                {
                    teleport.Show = false;
                    return;
                }

                teleport.Show = true;
            };
            teleport.button.onClick += () =>
            {
                if (GameSelector.main.selected.Value is MapPlanet mapPlanet)
                {
                    PlanetTeleportMenu.Open(mapPlanet.planet);
                }
                if (GameSelector.main.selected.Value is MapPlayer mapPlayer)
                {
                    MoveRocket.ToRocket(mapPlayer.Player as Rocket);
                }
            };

        }
    }
}
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
                switch (selected)
                {
                    case MapRocket mapRocket when mapRocket.Player.isPlayer:
                    case not MapPlanet or MapRocket:
                        teleport.Show = false;
                        return;
                    default:
                        teleport.Show = true;
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
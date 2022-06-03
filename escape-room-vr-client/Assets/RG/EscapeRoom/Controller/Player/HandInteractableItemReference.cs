using UnityEngine;

namespace RG.EscapeRoom.Controller.Player
{
    public class HandInteractableItemReference : MonoBehaviour, HandInteractableItem
    {
        private string networkId;

        public string NetworkId()
        {
            return networkId;
        }

        public void SetNetworkId(string id)
        {
            this.networkId = id;
        }
    }
}
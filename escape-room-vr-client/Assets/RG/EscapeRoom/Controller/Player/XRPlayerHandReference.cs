using System.Collections.Generic;
using UnityEngine;

namespace RG.EscapeRoom.Controller.Player
{
    public class XRPlayerHandReference : MonoBehaviour
    {
        public GameObject gameObject;
        public HashSet<HandInteractableItem> interactableItemsInContactWithHand = new HashSet<HandInteractableItem>();

        public void OnTriggerEnter(Collider other)
        {
            var handInteractableItemReference = other.gameObject.GetComponent<HandInteractableItemReference>();
            if (handInteractableItemReference) interactableItemsInContactWithHand.Add(handInteractableItemReference);
        }

        public void OnTriggerExit(Collider other)
        {
            var handInteractableItemReference = other.gameObject.GetComponent<HandInteractableItemReference>();
            if (handInteractableItemReference) interactableItemsInContactWithHand.Remove(handInteractableItemReference);
        }
    }

    public interface HandInteractableItem
    {
    }
}
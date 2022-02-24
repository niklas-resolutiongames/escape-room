using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class XRPlayerHandReference : MonoBehaviour
{
    public GameObject gameObject;
    public HashSet<HandInteractableItem> interactableItemsInContactWithHand = new HashSet<HandInteractableItem>();

    public void OnTriggerEnter(Collider other)
    {
        var handInteractableItemReference = other.gameObject.GetComponent<HandInteractableItemReference>();
        if (handInteractableItemReference) {
            interactableItemsInContactWithHand.Add(handInteractableItemReference);
        }
    }
    public void OnTriggerExit(Collider other)
    {
        
        var handInteractableItemReference = other.gameObject.GetComponent<HandInteractableItemReference>();
        if (handInteractableItemReference) {
            interactableItemsInContactWithHand.Remove(handInteractableItemReference);
            }
    }
    public void OnTriggerStay(Collider other)
    {
    }
}

public interface HandInteractableItem
{
}

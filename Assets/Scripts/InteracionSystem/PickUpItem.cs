using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpItem : Interactionable
{

    public Item item;

    public override void Interact(Interactionable interactionable)
    {
        base.Interact();

        PickUp(interactionable);
    }

    private void PickUp(Interactionable interactionable)
    {
        InventarySystem.instance.AddNewElement(item);
        ItemsNear.instance.DeleteElement(interactionable);
        Destroy(gameObject);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryCounter : BaseCounter
{
	// Customer pickup position
	public Transform pickupPoint;

	public override void Interact(Player player)
	{
		// Check if Player has a KitchenObject
		if (player.HasKitchenObject())
		{
			// Check if Player's KitchenObject is a PlateKitchenObject to delivery
			if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
			{
				// Only accepts plates
				DeliveryManager.Instance.DeliverRecipe(plateKitchenObject, this);

				// player.GetKitchenObject().DestroySelf();
			}
		}
	}
}

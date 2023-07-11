using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearCounter : BaseCounter
{
	public override void Interact(Player player)
	{
		// No KitchenObject in Counter
		if (!HasKitchenObject())
		{
			// Player has KitchenObject
			if (player.HasKitchenObject())
			{
				// Put KitchenObject on Counter
				player.GetKitchenObject().SetKitchenObjectParent(this);
			}
		}
		else
		{
			if (player.HasKitchenObject())
			{
				if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
				{
					// Player holding a Plate
					if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO()))
					{
						GetKitchenObject().DestroySelf();
					}
				}
				else
				{
					// Player not holding a plate but something else
					if (GetKitchenObject().TryGetPlate(out plateKitchenObject))
					{
						// Counter has a plate
						if (plateKitchenObject.TryAddIngredient(player.GetKitchenObject().GetKitchenObjectSO()))
						{
							player.GetKitchenObject().DestroySelf();

						}
					}
				}
			}
			else
			{
				// Give the player the KitchenObject
				GetKitchenObject().SetKitchenObjectParent(player);
			}
		}
	}
}

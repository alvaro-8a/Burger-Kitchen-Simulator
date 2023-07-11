using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainerCounter : BaseCounter
{
	// Event when Player grabs the KitchenObject from the ContainerCounter
	public event EventHandler OnPlayerGrabbedObject;

	// KitchenObject (as Scriptable Object) to spawn in this ContainerCounter
	[SerializeField] protected KitchenObjectSO kitchenObjectSO;

	public override void Interact(Player player)
	{
		if (!player.HasKitchenObject())
		{
			// Give KitchenObject only if not carrying anything
			KitchenObject.SpawnKitchenObject(kitchenObjectSO, player);


			OnPlayerGrabbedObject?.Invoke(this, EventArgs.Empty);
		}
	}

}

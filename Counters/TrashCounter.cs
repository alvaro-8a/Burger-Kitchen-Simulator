using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashCounter : BaseCounter
{
	public static event EventHandler onAnyObjectTrashed;
	new public static void ResetStaticData()
	{
		onAnyObjectTrashed = null;
	}

	public override void Interact(Player player)
	{
		if (player.HasKitchenObject())
		{
			player.GetKitchenObject().DestroySelf();

			onAnyObjectTrashed?.Invoke(this, EventArgs.Empty);
		}
	}
}

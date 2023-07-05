using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateKitchenObject : KitchenObject
{
	// Event when ingredient added to plate
	public event EventHandler<OnIngredientAddedEventArgs> OnIngredientAdded;
	public class OnIngredientAddedEventArgs : EventArgs
	{
		public KitchenObjectSO kitchenObjectSO;
	}

	// List of valid ingredients (KitchenObjects)
	[SerializeField] private List<KitchenObjectSO> validKitchenObjectSOList;

	// Ingredients on plate
	private List<KitchenObjectSO> kitchenObjectSOList;

	private void Awake()
	{
		kitchenObjectSOList = new List<KitchenObjectSO>();
	}

	public bool TryAddIngredient(KitchenObjectSO kitchenObjectSO)
	{
		// Check if ingredient is valid
		if (!validKitchenObjectSOList.Contains(kitchenObjectSO))
		{
			// KitchenObject Type not valid for plate
			return false;
		}

		// Check if Plate already has the ingredient
		if (kitchenObjectSOList.Contains(kitchenObjectSO))
		{
			// Already has this Type of KitchenObject
			return false;
		}
		else
		{
			// Add ingredient to Plate
			kitchenObjectSOList.Add(kitchenObjectSO);

			// Invoke Event to change the Plate Visual
			OnIngredientAdded?.Invoke(this, new OnIngredientAddedEventArgs
			{
				kitchenObjectSO = kitchenObjectSO
			});

			return true;
		}
	}

	public List<KitchenObjectSO> GetKitchenObjectSOList()
	{
		return kitchenObjectSOList;
	}
}

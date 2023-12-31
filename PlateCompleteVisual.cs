using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlateCompleteVisual : MonoBehaviour
{
	// Struct to link the ingredient (KitchenObjectSO) with the ingredient visual on the Plate (gameObject)
	[Serializable]
	public struct KitchenObjectSO_GameObject
	{
		public KitchenObjectSO kitchenObjectSO;
		public GameObject gameObject;
	}

	[SerializeField] private PlateKitchenObject plateKitchenObject;
	[SerializeField] private List<KitchenObjectSO_GameObject> kitchenObjectSO_GameObjectList;

	private void Start()
	{
		// Event subscription
		plateKitchenObject.OnIngredientAdded += PlateKitchenObject_OnIngredientAdded;

		// Hide all the Plate ingredients visuals
		foreach (KitchenObjectSO_GameObject kitchenObjectSO_GameObject in kitchenObjectSO_GameObjectList)
		{
			kitchenObjectSO_GameObject.gameObject.SetActive(false);

		}
	}

	private void PlateKitchenObject_OnIngredientAdded(object sender, PlateKitchenObject.OnIngredientAddedEventArgs e)
	{
		// Show the added ingredient visual on the Plate
		foreach (KitchenObjectSO_GameObject kitchenObjectSO_GameObject in kitchenObjectSO_GameObjectList)
		{
			if (kitchenObjectSO_GameObject.kitchenObjectSO == e.kitchenObjectSO)
			{
				kitchenObjectSO_GameObject.gameObject.SetActive(true);
			}
		}
	}
}

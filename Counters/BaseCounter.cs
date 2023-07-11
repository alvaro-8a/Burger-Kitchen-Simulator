using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseCounter : MonoBehaviour, IKitchenObjectParent
{
	// Global event when a KitchenObject is placed in any Counter
	public static event EventHandler OnAnyObjectPlacedHere;

	public static void ResetStaticData()
	{
		OnAnyObjectPlacedHere = null;
	}

	// KitchenObject placed position
	[SerializeField] private Transform counterTopPoint;

	// KitchenObject in the Counter
	private KitchenObject kitchenObject;


	public virtual void Interact(Player player)
	{
		Debug.LogError("BaseCounter.Interact();");
	}

	public virtual void InteractAlternate(Player player)
	{
		// Debug.LogError("BaseCounter.InteractAlternate();");
	}

	// Get KitchenObject place position 
	public Transform GetKitchenObjectFollowTransform()
	{
		return counterTopPoint;
	}

	// Put the KitchenObject in this Counter
	public void SetKitchenObject(KitchenObject kitchenObject)
	{
		this.kitchenObject = kitchenObject;

		if (kitchenObject != null)
		{
			OnAnyObjectPlacedHere?.Invoke(this, EventArgs.Empty);
		}
	}

	public KitchenObject GetKitchenObject()
	{
		return kitchenObject;
	}

	public void ClearKitchenObject()
	{
		kitchenObject = null;
	}

	public bool HasKitchenObject()
	{
		return kitchenObject != null;
	}
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Table : MonoBehaviour, IKitchenObjectParent
{
	// Position to place the KitchenObject
	[SerializeField] private Transform counterTopPoint;
	// Customer position to Eat
	[SerializeField] private Transform sitPosition;

	// KitchenObject on table
	private KitchenObject kitchenObject;

	public Transform GetSitPosition()
	{
		return sitPosition;
	}

	public Transform GetKitchenObjectFollowTransform()
	{
		return counterTopPoint;
	}

	public void SetKitchenObject(KitchenObject kitchenObject)
	{
		this.kitchenObject = kitchenObject;
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

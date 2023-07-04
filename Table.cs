using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Table : MonoBehaviour, IKitchenObjectParent
{
	[SerializeField] private Transform counterTopPoint;
	[SerializeField] private Transform sitPosition;

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

		// if (kitchenObject != null)
		// {
		// 	OnAnyObjectPlacedHere?.Invoke(this, EventArgs.Empty);
		// }
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

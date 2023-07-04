using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KitchenObject : MonoBehaviour
{
	[SerializeField] private KitchenObjectSO kitchenObjectSO;

	private IKitchenObjectParent kitchenObjectParent;

	public KitchenObjectSO GetKitchenObjectSO()
	{
		return kitchenObjectSO;
	}

	public void SetKitchenObjectParent(IKitchenObjectParent kitchenObjectParent)
	{
		// Clean old KitchenObjectParent if there was any
		if (this.kitchenObjectParent != null)
		{
			this.kitchenObjectParent.ClearKitchenObject();
		}

		this.kitchenObjectParent = kitchenObjectParent;

		if (this.kitchenObjectParent.HasKitchenObject())
			Debug.LogError("IKitchenObjectParent already have a KitchenObject");

		// Set the KitchenObject to the new KitchenObjectParent
		this.kitchenObjectParent.SetKitchenObject(this);

		// Move to the new KitchenObjectParent TopPosition
		transform.parent = this.kitchenObjectParent.GetKitchenObjectFollowTransform();
		transform.localPosition = Vector3.zero;
	}

	public IKitchenObjectParent GetKitchenObjectParent()
	{
		return kitchenObjectParent;
	}

	public void DestroySelf()
	{
		kitchenObjectParent.ClearKitchenObject();
		Destroy(gameObject);
	}

	public bool TryGetPlate(out PlateKitchenObject plateKitchenObject)
	{
		if (this is PlateKitchenObject)
		{
			plateKitchenObject = this as PlateKitchenObject;
			return true;
		}
		else
		{
			plateKitchenObject = null;
			return false;
		}
	}

	public static KitchenObject SpawnKitchenObject(KitchenObjectSO kitchenObjectSO, IKitchenObjectParent kitchenObjectParent)
	{
		Transform kitchenObjectTransform = Instantiate(kitchenObjectSO.prefab);
		KitchenObject kitchenObject = kitchenObjectTransform.GetComponent<KitchenObject>();
		kitchenObject.SetKitchenObjectParent(kitchenObjectParent);

		return kitchenObject;
	}
}

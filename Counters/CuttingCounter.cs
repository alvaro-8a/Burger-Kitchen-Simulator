using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuttingCounter : BaseCounter, IHasProgress
{
	public static event EventHandler OnAnyCut;

	new public static void ResetStaticData()
	{
		OnAnyCut = null;
	}

	public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;
	public event EventHandler OnCut;

	[SerializeField] private CuttingRecipeSO[] cuttingRecipeSOArray;

	private int cuttingProgress;

	public override void Interact(Player player)
	{
		if (!HasKitchenObject())
		{
			// Player has KitchenObject
			if (player.HasKitchenObject())
			{
				// KitchenObject can be cut
				if (HasRecipeWithInput(player.GetKitchenObject().GetKitchenObjectSO()))
				{
					// Put KitchenObject on Counter
					player.GetKitchenObject().SetKitchenObjectParent(this);
					cuttingProgress = 0;

					CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());

					OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
					{
						progressNormalized = (float)cuttingProgress / cuttingRecipeSO.cuttingProgressMax
					});
				}
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
			}
			else
			{
				// Give the player the KitchenObject
				GetKitchenObject().SetKitchenObjectParent(player);
			}
		}
	}

	public override void InteractAlternate(Player player)
	{
		if (HasKitchenObject())
		{
			if (HasRecipeWithInput(GetKitchenObject().GetKitchenObjectSO()))
			{
				// there is KitchenObject && can be cut
				cuttingProgress++;

				OnCut?.Invoke(this, EventArgs.Empty);
				OnAnyCut?.Invoke(this, EventArgs.Empty);

				CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());

				OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
				{
					progressNormalized = (float)cuttingProgress / cuttingRecipeSO.cuttingProgressMax
				});

				if (cuttingProgress >= cuttingRecipeSO.cuttingProgressMax)
				{
					KitchenObjectSO outputKitchenObjectSO = GetOutputForInput(GetKitchenObject().GetKitchenObjectSO());
					GetKitchenObject().DestroySelf();
					KitchenObject.SpawnKitchenObject(outputKitchenObjectSO, this);
				}
			}
		}
	}

	private bool HasRecipeWithInput(KitchenObjectSO inputKitchenObjectSO)
	{
		CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOWithInput(inputKitchenObjectSO);

		return cuttingRecipeSO != null;
	}

	private KitchenObjectSO GetOutputForInput(KitchenObjectSO inputKitchenObjectSO)
	{
		CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOWithInput(inputKitchenObjectSO);

		if (cuttingRecipeSO != null)
		{
			return cuttingRecipeSO.output;
		}

		return null;
	}

	private CuttingRecipeSO GetCuttingRecipeSOWithInput(KitchenObjectSO inputKitchenObjectSO)
	{
		foreach (CuttingRecipeSO cuttingRecipeSO in cuttingRecipeSOArray)
		{
			if (cuttingRecipeSO.input == inputKitchenObjectSO)
			{
				return cuttingRecipeSO;
			}
		}
		return null;
	}
}
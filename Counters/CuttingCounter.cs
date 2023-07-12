using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuttingCounter : BaseCounter, IHasProgress
{
	// Global event when Player cuts an ingredient on any CuttingCounter
	public static event EventHandler OnAnyCut;

	new public static void ResetStaticData()
	{
		OnAnyCut = null;
	}

	// Event when an ingredient cutting progresses
	public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;
	// Event when cut action is executed 
	public event EventHandler OnCut;

	// Reference to a List of cutting recipes. Ingrediente in (cut) => ingredient output
	[SerializeField] private CuttingRecipeSO[] cuttingRecipeSOArray;

	// Progress cutting the KitchenObject (ingredient)
	private int cuttingProgress;

	// Player interacts with Counter
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

					// Invoke event to update the progress bar of the cut action
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

	// Player interacts (to cut) with CuttingCounter
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

	// Check if we have a cutting recipe for the KitchenObject (ingredient)
	private bool HasRecipeWithInput(KitchenObjectSO inputKitchenObjectSO)
	{
		CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOWithInput(inputKitchenObjectSO);

		return cuttingRecipeSO != null;
	}

	// Get the KitchenObject (ingredient) output if the KitchenObject (ingredient) input is cut
	private KitchenObjectSO GetOutputForInput(KitchenObjectSO inputKitchenObjectSO)
	{
		CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOWithInput(inputKitchenObjectSO);

		if (cuttingRecipeSO != null)
		{
			return cuttingRecipeSO.output;
		}

		return null;
	}

	// Get the cuttingRecipe for the KitchenObject (ingredient) input 
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

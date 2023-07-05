using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryManager : MonoBehaviour
{
	public event EventHandler OnRecipeSpawned;
	public event EventHandler OnRecipeCompleted;
	public event EventHandler OnRecipeSuccess;
	public event EventHandler OnRecipeFailed;

	public static DeliveryManager Instance { get; private set; }
	public List<DeliveryCounter> pickupPoints;

	[SerializeField] private RecipeListSO recipeListSO;
	[SerializeField] private GameObject customer;
	[SerializeField] private Transform customerSpawnPoint;


	private List<RecipeSO> waitingRecipeSOList;
	private float spawnRecipeTimer;
	private float spawnRecipeTimerMax = 4f;
	private int waitingRecipeMax = 4;
	private int successfulRecipesAmount;
	private List<DeliveryCounter> availablePickupPoints;
	private Dictionary<DeliveryCounter, CustomerAI> customerInDeliveryCounter;

	private void Awake()
	{
		Instance = this;

		waitingRecipeSOList = new List<RecipeSO>();
		availablePickupPoints = new List<DeliveryCounter>();
		customerInDeliveryCounter = new Dictionary<DeliveryCounter, CustomerAI>();

		foreach (DeliveryCounter pickupPoint in pickupPoints)
		{
			availablePickupPoints.Add(pickupPoint);
		}
	}

	private void Update()
	{
		spawnRecipeTimer -= Time.deltaTime;
		if (spawnRecipeTimer <= 0f)
		{
			spawnRecipeTimer = spawnRecipeTimerMax;

			if (KitchenGameManager.Instance.IsGamePlaying() && waitingRecipeSOList.Count < waitingRecipeMax && availablePickupPoints.Count > 0)
			{
				GameObject customerSpawned = Instantiate(customer, customerSpawnPoint);
				customerSpawned.transform.parent = null;
			}
		}
	}

	public void DeliverRecipe(PlateKitchenObject plateKitchenObject, DeliveryCounter deliveryCounter)
	{
		for (int i = 0; i < waitingRecipeSOList.Count; i++)
		{
			RecipeSO waitingRecipeSO = waitingRecipeSOList[i];

			if (waitingRecipeSO.kitchenObjectSOList.Count == plateKitchenObject.GetKitchenObjectSOList().Count)
			{
				bool plateContentsMatchesRecipe = true;
				// Has same number of ingredients
				foreach (KitchenObjectSO recipeKitchenObjectSO in waitingRecipeSO.kitchenObjectSOList)
				{
					bool ingredientFound = false;
					// Cycling through ingredients in the Recipe
					foreach (KitchenObjectSO plateKitchenObjectSO in plateKitchenObject.GetKitchenObjectSOList())
					{
						// Cycling through ingredients in the Plate
						if (recipeKitchenObjectSO == plateKitchenObjectSO)
						{
							// Ingredient matches
							ingredientFound = true;
							break;
						}
					}

					if (!ingredientFound)
					{
						// Recipe ingredient not on Plate
						plateContentsMatchesRecipe = false;
						break;
					}
				}

				if (plateContentsMatchesRecipe)
				{
					// Player Delivered correct Recipe
					successfulRecipesAmount++;

					CustomerAI customer = customerInDeliveryCounter[deliveryCounter];

					Player.Instance.GetKitchenObject().SetKitchenObjectParent(customer);
					waitingRecipeSOList.RemoveAt(i);
					RemoveCustomerInDeliveryCounter(deliveryCounter);
					AddAvailablePickupPoint(deliveryCounter);

					OnRecipeCompleted?.Invoke(this, EventArgs.Empty);
					OnRecipeSuccess?.Invoke(this, EventArgs.Empty);
					return;
				}
			}
		}

		// No matches found
		// Player did not delivered the correct recipe
		OnRecipeFailed?.Invoke(this, EventArgs.Empty);

	}

	public void AddRecipeToWaitingList()
	{
		RecipeSO waitingRecipeSO = recipeListSO.recipeSOList[UnityEngine.Random.Range(0, recipeListSO.recipeSOList.Count)];

		// Debug.Log(waitingRecipeSO.recipeName);
		waitingRecipeSOList.Add(waitingRecipeSO);

		OnRecipeSpawned?.Invoke(this, EventArgs.Empty);
	}

	public List<RecipeSO> GetWaitingRecipeSOList()
	{
		return waitingRecipeSOList;
	}

	public int GetSuccessfulRecipesAmount()
	{
		return successfulRecipesAmount;
	}

	public List<DeliveryCounter> GetAvailablePickupPoints()
	{
		return availablePickupPoints;
	}

	public void RemoveAvailablePickupPoint(DeliveryCounter deliveryCounter)
	{
		if (availablePickupPoints == null) return;

		for (int i = 0; i < availablePickupPoints.Count; i++)
		{
			if (availablePickupPoints[i] == deliveryCounter)
			{
				availablePickupPoints.RemoveAt(i);
			}
		}
	}

	private void AddAvailablePickupPoint(DeliveryCounter deliveryCounter)
	{
		availablePickupPoints.Add(deliveryCounter);
	}

	public void AddCustomerToDeliveryCounter(DeliveryCounter deliveryCounter, CustomerAI customer)
	{
		customerInDeliveryCounter.Add(deliveryCounter, customer);
	}

	private void RemoveCustomerInDeliveryCounter(DeliveryCounter deliveryCounter)
	{
		customerInDeliveryCounter.Remove(deliveryCounter);
	}

	public Transform GetCustomerSpawnPoint()
	{
		return customerSpawnPoint;
	}
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryManager : MonoBehaviour
{
	// Event when a recipe is added to the waiting list
	public event EventHandler OnRecipeSpawned;
	// Event when a recipe has been completed
	public event EventHandler OnRecipeCompleted;
	// Event when recipe has been successfully delivered
	public event EventHandler OnRecipeSuccess;
	// Event when the recipe that tried to be delivered wasn't on the waiting list
	public event EventHandler OnRecipeFailed;

	/** SINGLETON PATTERN **/
	public static DeliveryManager Instance { get; private set; }

	// List of all Delivery counters on the scene
	public List<DeliveryCounter> pickupPoints;

	// Scriptable Object with the List of all the recipes
	[SerializeField] private RecipeListSO recipeListSO;
	// Customer (AI) GameObject
	[SerializeField] private GameObject customer;
	// Position to Spawn Customers ("Entry to the Burger")
	[SerializeField] private Transform customerSpawnPoint;

	// List of waiting recipes
	private List<RecipeSO> waitingRecipeSOList;
	// Time left to spawn another Customer
	private float spawnCustomerTimer;
	// Time between Customers spawn
	private float spawnCustomerTimerMax = 4f;
	// Max amount of recipes on waiting list
	private int waitingRecipeMax = 4;
	// Amount of recipes successfully delivered
	private int successfulRecipesAmount;
	// List of available delivery points
	private List<DeliveryCounter> availablePickupPoints;
	// Relation to know which customer is in each delivery counter
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
		spawnCustomerTimer -= Time.deltaTime;
		if (spawnCustomerTimer <= 0f)
		{
			spawnCustomerTimer = spawnCustomerTimerMax;

			if (KitchenGameManager.Instance.IsGamePlaying() && waitingRecipeSOList.Count < waitingRecipeMax && availablePickupPoints.Count > 0)
			{
				GameObject customerSpawned = Instantiate(customer, customerSpawnPoint);
				customerSpawned.transform.parent = null;
			}
		}
	}

	// This methods manages the delivery of a recipe by the player
	// It checks if the delivery is successful or failed and invokes the respectives events
	// @param plateKitchenObject: Plate that contains the recipe to deliver
	// @param deliveryCounter: DeliveryCounter that wants to deliver the recipe
	public void DeliverRecipe(PlateKitchenObject plateKitchenObject, DeliveryCounter deliveryCounter)
	{
		for (int i = 0; i < waitingRecipeSOList.Count; i++)
		{
			// Cycling through the waiting recipes list
			RecipeSO waitingRecipeSO = waitingRecipeSOList[i];

			// Check if the the waiting recipe has the same number of ingredients as the plate to deliver
			if (waitingRecipeSO.kitchenObjectSOList.Count == plateKitchenObject.GetKitchenObjectSOList().Count)
			{
				// Has same number of ingredients
				bool plateContentsMatchesRecipe = true;

				foreach (KitchenObjectSO recipeKitchenObjectSO in waitingRecipeSO.kitchenObjectSOList)
				{
					// Cycling through ingredients in the Recipe 
					bool ingredientFound = false;

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

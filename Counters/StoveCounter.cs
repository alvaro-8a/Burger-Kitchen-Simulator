using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class StoveCounter : BaseCounter, IHasProgress
{
	public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;
	public event EventHandler<OnStateChangedEventArgs> OnStateChanged;
	public class OnStateChangedEventArgs : EventArgs
	{
		public State state;
	}

	// State Machine states
	public enum State
	{
		Idle,
		Frying,
		Fried,
		Burned
	}

	[SerializeField] private FryingRecipeSO[] fryingRecipeSOArray;
	[SerializeField] private BurningRecipeSO[] burningRecipeSOArray;

	private float fryingTimer;
	private float burningTimer;
	private FryingRecipeSO fryingRecipeSO;
	private BurningRecipeSO burningRecipeSO;
	private State state;

	private void Start()
	{
		state = State.Idle;
	}

	private void Update()
	{
		if (HasKitchenObject())
		{
			switch (state)
			{
				case State.Idle:
					// Inital State
					// No KitchenObject => No Task
					break;
				case State.Frying:
					// Fry for the recipe time (FryingRecipeSO.fryingTimerMax)
					fryingTimer += Time.deltaTime;

					OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
					{
						progressNormalized = fryingTimer / fryingRecipeSO.fryingTimerMax
					});

					if (fryingTimer > fryingRecipeSO.fryingTimerMax)
					{
						// Fried
						GetKitchenObject().DestroySelf();
						KitchenObject.SpawnKitchenObject(fryingRecipeSO.output, this);

						// KitchenObject already fried => Change to Fried State
						state = State.Fried;
						burningTimer = 0f;
						burningRecipeSO = GetBurningRecipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());

						OnStateChanged?.Invoke(this, new OnStateChangedEventArgs
						{
							state = state
						});
					}

					break;
				case State.Fried:
					// Keep frying until burned (BurningRecipeSO.burningTimerMax)
					burningTimer += Time.deltaTime;

					OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
					{
						progressNormalized = burningTimer / burningRecipeSO.burningTimerMax
					});

					if (burningTimer > burningRecipeSO.burningTimerMax)
					{
						// Fried
						GetKitchenObject().DestroySelf();
						KitchenObject.SpawnKitchenObject(burningRecipeSO.output, this);

						// KitchenObject too cooked => Change to Burned State
						state = State.Burned;

						OnStateChanged?.Invoke(this, new OnStateChangedEventArgs
						{
							state = state
						});

						OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
						{
							progressNormalized = 0f
						});
					}

					break;
				case State.Burned:
					// KitchenObject Burned
					break;
			}
		}

	}

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

					fryingRecipeSO = GetFryingRecipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());

					state = State.Frying;
					fryingTimer = 0f;

					OnStateChanged?.Invoke(this, new OnStateChangedEventArgs
					{
						state = state
					});

					OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
					{
						progressNormalized = fryingTimer / fryingRecipeSO.fryingTimerMax
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

				state = State.Idle;

				OnStateChanged?.Invoke(this, new OnStateChangedEventArgs
				{
					state = state
				});

				OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
				{
					progressNormalized = 0f
				});
			}
			else
			{
				// Give the player the KitchenObject
				GetKitchenObject().SetKitchenObjectParent(player);

				// Reset Stove State
				state = State.Idle;

				OnStateChanged?.Invoke(this, new OnStateChangedEventArgs
				{
					state = state
				});

				OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
				{
					progressNormalized = 0f
				});
			}
		}
	}

	private bool HasRecipeWithInput(KitchenObjectSO inputKitchenObjectSO)
	{
		FryingRecipeSO fryingRecipeSO = GetFryingRecipeSOWithInput(inputKitchenObjectSO);

		return fryingRecipeSO != null;
	}

	private KitchenObjectSO GetOutputForInput(KitchenObjectSO inputKitchenObjectSO)
	{
		FryingRecipeSO fryingRecipeSO = GetFryingRecipeSOWithInput(inputKitchenObjectSO);

		if (fryingRecipeSO != null)
		{
			return fryingRecipeSO.output;
		}

		return null;
	}

	private FryingRecipeSO GetFryingRecipeSOWithInput(KitchenObjectSO inputKitchenObjectSO)
	{
		foreach (FryingRecipeSO fryingRecipeSO in fryingRecipeSOArray)
		{
			if (fryingRecipeSO.input == inputKitchenObjectSO)
			{
				return fryingRecipeSO;
			}
		}
		return null;
	}
	private BurningRecipeSO GetBurningRecipeSOWithInput(KitchenObjectSO inputKitchenObjectSO)
	{
		foreach (BurningRecipeSO burningRecipeSO in burningRecipeSOArray)
		{
			if (burningRecipeSO.input == inputKitchenObjectSO)
			{
				return burningRecipeSO;
			}
		}
		return null;
	}

	public bool IsFried()
	{
		return state == State.Fried;
	}
}

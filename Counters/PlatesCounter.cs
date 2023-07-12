using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatesCounter : BaseCounter
{
	// Event when Plate is spawned
	public event EventHandler OnPlateSpawned;
	// Event when Plate is taken
	public event EventHandler OnPlateRemoved;

	// Reference to the Plate KitchenObjectSO
	[SerializeField] private KitchenObjectSO plateKitchenObjectSO;

	// Time elapsed since last spawned plate
	private float spawnPlateTimer;
	// Time between spawned plates
	private float spawnPlateTimerMax = 4f;
	// Amount of plates on Counter
	private int platesSpawnedAmount;
	// Max amount of plates on Counter
	private int platesSpawnedAmountMax = 4;

	private void Update()
	{
		spawnPlateTimer += Time.deltaTime;

		if (spawnPlateTimer > spawnPlateTimerMax)
		{
			spawnPlateTimer = 0f;
			// Check if game is playing and the amount of plates on Counter is less than the maximum
			if (KitchenGameManager.Instance.IsGamePlaying() && platesSpawnedAmount < platesSpawnedAmountMax)
			{
				platesSpawnedAmount++;

				// Invoke event to update Counter visual with new plate on Counter
				OnPlateSpawned?.Invoke(this, EventArgs.Empty);
			}
		}
	}

	public override void Interact(Player player)
	{
		if (!player.HasKitchenObject())
		{
			// Player empty handed
			if (platesSpawnedAmount > 0)
			{
				// At least 1 plate in the Counter
				platesSpawnedAmount--;

				// Spawn PlateKitchenObject to the Player
				KitchenObject.SpawnKitchenObject(plateKitchenObjectSO, player);

				// Invoke event to update Counter visual when player pickup a plate
				OnPlateRemoved?.Invoke(this, EventArgs.Empty);
			}
		}
	}
}

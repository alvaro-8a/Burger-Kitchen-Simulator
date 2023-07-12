using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatesCounterVisual : MonoBehaviour
{
	// PlatesCounter reference
	[SerializeField] private PlatesCounter platesCounter;
	// Plate visual position
	[SerializeField] private Transform counterTopPoint;
	// Plate Visual prefab
	[SerializeField] private Transform plateVisualPrefab;

	// List of plates shown on top of the Counter
	private List<GameObject> plateVisualGameObjectList;

	private void Awake()
	{
		plateVisualGameObjectList = new List<GameObject>();
	}

	private void Start()
	{
		// Events subscription
		platesCounter.OnPlateSpawned += PlatesCounter_OnPlateSpawned;
		platesCounter.OnPlateRemoved += PlatesCounter_OnPlateRemoved;
	}

	private void PlatesCounter_OnPlateSpawned(object sender, System.EventArgs e)
	{
		// Add the plate visual to the Counter
		Transform plateVisualTransform = Instantiate(plateVisualPrefab, counterTopPoint);

		// Add an offset to the plate visual position to stack them up (based on the amount of plates on Counter)
		float plateOffsetY = .1f;
		plateVisualTransform.localPosition = new Vector3(0, plateOffsetY * plateVisualGameObjectList.Count, 0);

		// Add the plate visual to the List
		plateVisualGameObjectList.Add(plateVisualTransform.gameObject);
	}

	private void PlatesCounter_OnPlateRemoved(object sender, System.EventArgs e)
	{
		// Remove the last plate visual from list & Counter
		GameObject plateGameObject = plateVisualGameObjectList[plateVisualGameObjectList.Count - 1];
		plateVisualGameObjectList.Remove(plateGameObject);
		Destroy(plateGameObject);
	}
}

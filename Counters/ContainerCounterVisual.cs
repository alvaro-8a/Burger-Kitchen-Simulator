using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ContainerCounterVisual : MonoBehaviour
{
	// Constant reference to animator variable
	private const string OPEN_CLOSE = "OpenClose";

	// ContainerCounter Reference
	[SerializeField] private ContainerCounter containerCounter;
	private Animator animator;

	private void Awake()
	{
		animator = GetComponent<Animator>();
	}

	private void Start()
	{
		// Event subscription
		containerCounter.OnPlayerGrabbedObject += ContainerCounter_OnPlayerGrabbedObject;
	}

	private void ContainerCounter_OnPlayerGrabbedObject(object sender, System.EventArgs e)
	{
		// Set trigger to play animation
		animator.SetTrigger(OPEN_CLOSE);
	}
}

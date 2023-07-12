using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CuttingCounterVisual : MonoBehaviour
{
	// Constant reference to animator variable
	private const string CUT = "Cut";

	// CuttingCounter Reference
	[SerializeField] private CuttingCounter cuttingCounter;
	private Animator animator;

	private void Awake()
	{
		animator = GetComponent<Animator>();
	}

	private void Start()
	{
		// Event subscription
		cuttingCounter.OnCut += CuttingCounter_OnCut;
	}

	private void CuttingCounter_OnCut(object sender, System.EventArgs e)
	{
		// Set trigger to play animation
		animator.SetTrigger(CUT);
	}
}

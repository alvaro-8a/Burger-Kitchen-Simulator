using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerAIVisual : MonoBehaviour
{
	private const string IS_WALKING = "IsWalking";

	[SerializeField] private CustomerAI customer;

	private Animator animator;

	private void Awake()
	{
		animator = GetComponent<Animator>();
	}

	private void Update()
	{
		animator.SetBool(IS_WALKING, customer.IsWalking());
	}
}

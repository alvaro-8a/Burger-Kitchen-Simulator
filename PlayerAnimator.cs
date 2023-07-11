using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
	// Constant reference to animator variable
	private const string IS_WALKING = "IsWalking";

	// Player reference
	[SerializeField] private Player player;

	private Animator animator;

	private void Awake()
	{
		animator = GetComponent<Animator>();
	}

	private void Update()
	{
		animator.SetBool(IS_WALKING, player.IsWalking());
	}
}

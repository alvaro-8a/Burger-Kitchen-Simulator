using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSounds : MonoBehaviour
{
	// Player Reference
	private Player player;
	// Time left to play footstep sound
	private float footstepTimer;
	// Time between footstep sound
	private float footstepTimerMax = .1f;

	private void Awake()
	{
		player = GetComponent<Player>();
	}

	private void Update()
	{
		footstepTimer -= Time.deltaTime;

		if (footstepTimer < 0f)
		{
			footstepTimer = footstepTimerMax;

			// Check if player is walking
			if (player.IsWalking())
			{
				float volume = 1f;
				SoundManager.Instance.PlayFootstepsSound(transform.position, volume);
			}
		}
	}
}

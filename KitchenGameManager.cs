using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class KitchenGameManager : MonoBehaviour
{
	/** SINGLETON PATTERN **/
	public static KitchenGameManager Instance { get; private set; }

	// Event when Game State has changed
	public event EventHandler OnStateChanged;
	// Event when Game is Pause
	public event EventHandler OnGamePaused;
	// Event when Game is Unpaused
	public event EventHandler OnGameUnpaused;

	// State Type definition
	private enum State
	{
		WaitingToStart,
		CountdownToStart,
		GamePlaying,
		GameOver
	}

	// Game State
	private State state;
	// Countdown to start the GamePlaying state
	private float countdownToStartTimer = 3f;
	// Amount of time left on GamePlaying state
	private float gamePlayingTimer;
	// Max amount of time the game can be on GamePlaying state
	private float gamePlayingTimerMax = 180f;
	private bool isGamePaused = false;

	private void Awake()
	{
		Instance = this;

		state = State.WaitingToStart;
	}

	private void Start()
	{
		// Events subscription
		GameInput.Instance.OnPauseAction += GameInput_OnPauseAction;
		GameInput.Instance.OnInteractAction += GameInput_OnInteractAction;
	}

	private void Update()
	{
		// State Machine
		switch (state)
		{
			case State.WaitingToStart:
				break;

			case State.CountdownToStart:
				countdownToStartTimer -= Time.deltaTime;
				if (countdownToStartTimer < 0)
				{
					state = State.GamePlaying;
					gamePlayingTimer = gamePlayingTimerMax;
					OnStateChanged?.Invoke(this, EventArgs.Empty);
				}
				break;

			case State.GamePlaying:
				gamePlayingTimer -= Time.deltaTime;
				if (gamePlayingTimer < 0)
				{
					state = State.GameOver;
					OnStateChanged?.Invoke(this, EventArgs.Empty);
				}
				break;

			case State.GameOver:
				break;
		}
	}

	private void GameInput_OnPauseAction(object sender, System.EventArgs e)
	{
		TogglePauseGame();
	}

	private void GameInput_OnInteractAction(object sender, System.EventArgs e)
	{
		if (state == State.WaitingToStart)
		{
			state = State.CountdownToStart;
			OnStateChanged?.Invoke(this, EventArgs.Empty);
		}
	}

	public bool IsGamePlaying()
	{
		return state == State.GamePlaying;
	}

	public bool IsCountdownToStartActive()
	{
		return state == State.CountdownToStart;
	}

	public float GetCountdownToStartTimer()
	{
		return countdownToStartTimer;
	}

	public bool IsGameOver()
	{
		return state == State.GameOver;
	}

	public float GetGamePlayingTimerNormalized()
	{
		return 1 - (gamePlayingTimer / gamePlayingTimerMax);
	}

	public void TogglePauseGame()
	{
		isGamePaused = !isGamePaused;
		if (isGamePaused)
		{
			Time.timeScale = 0f;
			OnGamePaused?.Invoke(this, EventArgs.Empty);
		}
		else
		{
			Time.timeScale = 1f;
			OnGameUnpaused?.Invoke(this, EventArgs.Empty);
		}
	}
}

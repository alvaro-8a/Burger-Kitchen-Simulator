using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour
{
	// Constant to save Player Preferences
	private const string PLAYER_PREFS_BINDINGS = "InputBindings";

	/** SINGLETON PATTERN **/
	public static GameInput Instance { get; private set; }

	// Event when interact button is pressed
	public event EventHandler OnInteractAction;
	// Event when interact alternate button is pressed
	public event EventHandler OnInteractAlternateAction;
	// Event when pause button is pressed
	public event EventHandler OnPauseAction;
	// Event when key rebind is completed
	public event EventHandler OnBindingRebind;

	// Binding Type definition
	public enum Binding
	{
		Move_Up,
		Move_Down,
		Move_Left,
		Move_Right,
		Interact,
		InteractAlternate,
		Pause,
		Gamepad_Interact,
		Gamepad_InteractAlternate,
		Gamepad_Pause
	}

	// Player Input actions map
	private PlayerInputActions playerInputActions;

	private void Awake()
	{
		Instance = this;

		playerInputActions = new PlayerInputActions();

		// Check if there is a previous Player Preference for input actions map
		if (PlayerPrefs.HasKey(PLAYER_PREFS_BINDINGS))
		{
			// Load the Player Preference input actions map
			playerInputActions.LoadBindingOverridesFromJson(PlayerPrefs.GetString(PLAYER_PREFS_BINDINGS));
		}

		// Enable the player input actions map
		playerInputActions.Player.Enable();

		// Events subscription
		playerInputActions.Player.Interact.performed += Interact_performed;
		playerInputActions.Player.InteractAlternate.performed += InteractAlternate_performed;
		playerInputActions.Player.Pause.performed += Pause_performed;

	}

	private void OnDestroy()
	{
		// Events unsubscription
		playerInputActions.Player.Interact.performed -= Interact_performed;
		playerInputActions.Player.InteractAlternate.performed -= InteractAlternate_performed;
		playerInputActions.Player.Pause.performed -= Pause_performed;

		// Destroy the input actions map
		playerInputActions.Dispose();
	}


	private void Interact_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
	{
		OnInteractAction?.Invoke(this, EventArgs.Empty);
	}

	private void InteractAlternate_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
	{
		OnInteractAlternateAction?.Invoke(this, EventArgs.Empty);
	}
	private void Pause_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
	{
		OnPauseAction?.Invoke(this, EventArgs.Empty);
	}

	public Vector2 GetMovementVectorNormalized()
	{
		// Read the move input values
		Vector2 inputVector = playerInputActions.Player.Move.ReadValue<Vector2>();

		// Normalize the input
		inputVector = inputVector.normalized;

		return inputVector;
	}

	public string GetBindingText(Binding binding)
	{
		// Get the key name for the Binding Type
		switch (binding)
		{
			default:
			case Binding.Move_Up:
				return playerInputActions.Player.Move.bindings[1].ToDisplayString();
			case Binding.Move_Down:
				return playerInputActions.Player.Move.bindings[2].ToDisplayString();
			case Binding.Move_Left:
				return playerInputActions.Player.Move.bindings[3].ToDisplayString();
			case Binding.Move_Right:
				return playerInputActions.Player.Move.bindings[4].ToDisplayString();
			case Binding.Interact:
				return playerInputActions.Player.Interact.bindings[0].ToDisplayString();
			case Binding.InteractAlternate:
				return playerInputActions.Player.InteractAlternate.bindings[0].ToDisplayString();
			case Binding.Pause:
				return playerInputActions.Player.Pause.bindings[0].ToDisplayString();
			// GAMEPAD BINDINGS
			case Binding.Gamepad_Interact:
				return playerInputActions.Player.Interact.bindings[1].ToDisplayString();
			case Binding.Gamepad_InteractAlternate:
				return playerInputActions.Player.InteractAlternate.bindings[1].ToDisplayString();
			case Binding.Gamepad_Pause:
				return playerInputActions.Player.Pause.bindings[1].ToDisplayString();
		}
	}

	public void RebindBinding(Binding binding, Action onActionRebound)
	{
		// Disable the Player input actions map
		playerInputActions.Player.Disable();

		// Input action
		InputAction inputAction;
		// Position of the binding in the input action keys array
		int bindingIndex;

		// Set the inputAction and bindingIndex for the Binding Type
		switch (binding)
		{
			default:
			case Binding.Move_Up:
				inputAction = playerInputActions.Player.Move;
				bindingIndex = 1;
				break;
			case Binding.Move_Down:
				inputAction = playerInputActions.Player.Move;
				bindingIndex = 2;
				break;
			case Binding.Move_Left:
				inputAction = playerInputActions.Player.Move;
				bindingIndex = 3;
				break;
			case Binding.Move_Right:
				inputAction = playerInputActions.Player.Move;
				bindingIndex = 4;
				break;
			case Binding.Interact:
				inputAction = playerInputActions.Player.Interact;
				bindingIndex = 0;
				break;
			case Binding.InteractAlternate:
				inputAction = playerInputActions.Player.InteractAlternate;
				bindingIndex = 0;
				break;
			case Binding.Pause:
				inputAction = playerInputActions.Player.Pause;
				bindingIndex = 0;
				break;
			// GAMEPAD BINDINGS
			case Binding.Gamepad_Interact:
				inputAction = playerInputActions.Player.Interact;
				bindingIndex = 1;
				break;
			case Binding.Gamepad_InteractAlternate:
				inputAction = playerInputActions.Player.InteractAlternate;
				bindingIndex = 1;
				break;
			case Binding.Gamepad_Pause:
				inputAction = playerInputActions.Player.Pause;
				bindingIndex = 1;
				break;
		}

		// Rebind the Binding Type
		inputAction.PerformInteractiveRebinding(bindingIndex)
			.OnComplete(callback =>
			{
				callback.Dispose();
				// Enable the Player input actions
				playerInputActions.Player.Enable();
				// Call received Action
				onActionRebound();

				// Save Player Preference for the new Binding
				PlayerPrefs.SetString(PLAYER_PREFS_BINDINGS, playerInputActions.SaveBindingOverridesAsJson());
				PlayerPrefs.Save();

				// Invoke event to update the keys binding in the tutorialUI
				OnBindingRebind?.Invoke(this, EventArgs.Empty);
			})
			.Start();
	}
}

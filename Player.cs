using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Player : MonoBehaviour, IKitchenObjectParent
{
	/** SINGLETON PATTERN **/
	public static Player Instance { get; private set; }


	// Event when player picks a KitchenObject
	public event EventHandler OnPickedSomething;
	// Event when player looks/selects a Counter
	public event EventHandler<OnSelectedCounterChangedEventArgs> OnSelectedCounterChanged;
	// Event Arguments for OnSelectedCounterChanged Event
	public class OnSelectedCounterChangedEventArgs : EventArgs
	{
		public BaseCounter selectedCounter;
	}

	// Reference to the GameInput manager
	[SerializeField] private GameInput gameInput;
	[SerializeField] private float moveSpeed = 7f;
	[SerializeField] private float rotateSpeed = 10f;
	// LayerMask for interactive counters
	[SerializeField] private LayerMask countersLayerMask;
	// Position to hold KitchenObjects
	[SerializeField] private Transform kitchenObjectHoldPoint;


	private bool isWalking;
	// Interaction Direction to RayCast
	private Vector3 lastInteractDir;
	private BaseCounter selectedCounter;
	private KitchenObject kitchenObject;


	private void Awake()
	{
		if (Instance != null)
			Debug.LogError("There can't be more than one Player Instance");

		Instance = this;
	}

	private void Start()
	{
		// Interactions events subscription
		gameInput.OnInteractAction += GameInput_OnInteractAction;
		gameInput.OnInteractAlternateAction += GameInput_OnInteractAlternateAction;
	}

	private void GameInput_OnInteractAction(object sender, System.EventArgs e)
	{
		if (!KitchenGameManager.Instance.IsGamePlaying()) return;

		if (selectedCounter != null)
		{
			selectedCounter.Interact(this);
		}
	}
	private void GameInput_OnInteractAlternateAction(object sender, System.EventArgs e)
	{
		if (!KitchenGameManager.Instance.IsGamePlaying()) return;

		if (selectedCounter != null)
		{
			selectedCounter.InteractAlternate(this);
		}
	}

	private void Update()
	{
		HandleMovement();
		HandleInteractions();
	}

	public bool IsWalking()
	{
		return isWalking;
	}

	private void HandleMovement()
	{
		// Get Player Input
		Vector2 inputVector = gameInput.GetMovementVectorNormalized();

		// Remove movement in Y axis
		Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);

		float moveDistance = moveSpeed * Time.deltaTime;
		float playerRadius = .7f;
		float playerHeight = 2f;

		// Check if player is not blocked to move
		bool canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDir, moveDistance);

		if (!canMove)
		{
			// Cannot move torward moveDir

			// Check if Player is stucked in a diagonal movement (Keyboard or Gamepad)
			// If stucked in diagonal move => Try to move only on one axis (X or Z)

			// Attempt only X movement
			Vector3 moveDirX = new Vector3(moveDir.x, 0, 0).normalized;
			// Check if can move on X axis only
			canMove = (moveDir.x < -.5f || moveDir.x > .5f) && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirX, moveDistance);

			if (canMove)
			{
				moveDir = moveDirX;
			}
			else
			{
				// Attempt only Z Movement
				Vector3 moveDirZ = new Vector3(0, 0, moveDir.z).normalized;
				// Check if can move on Z axis only
				canMove = (moveDir.z < -.5f || moveDir.z > .5f) && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirZ, moveDistance);

				if (canMove)
					moveDir = moveDirZ;
			}
		}

		if (canMove)
		{
			transform.position += moveDir * moveDistance;
		}

		isWalking = moveDir != Vector3.zero;

		if (isWalking)
		{
			// Rotate player
			transform.forward = Vector3.Slerp(transform.forward, moveDir, Time.deltaTime * rotateSpeed);
		}
	}

	private void HandleInteractions()
	{
		// Get Player Input
		Vector2 inputVector = gameInput.GetMovementVectorNormalized();

		// Remove movement in Y axis
		Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);

		if (moveDir != Vector3.zero)
		{
			// Set new interaction direction to the Player looking position
			lastInteractDir = moveDir;
		}

		float interactDistance = 2f;
		// Check if raycast in the Interaction Direction hits something
		if (Physics.Raycast(transform.position, lastInteractDir, out RaycastHit raycastHit, interactDistance, countersLayerMask))
		{
			// If Counter is hit => select the counter
			if (raycastHit.transform.TryGetComponent(out BaseCounter baseCounter))
			{
				if (baseCounter != selectedCounter)
				{
					SetSelectedCounter(baseCounter);
				}
			}
			else
			{
				SetSelectedCounter(null);
			}
		}
		else
		{
			SetSelectedCounter(null);
		}
	}

	private void SetSelectedCounter(BaseCounter selectedCounter)
	{
		this.selectedCounter = selectedCounter;

		// Invoke Event to change the Counter Visual
		OnSelectedCounterChanged?.Invoke(this, new OnSelectedCounterChangedEventArgs
		{
			selectedCounter = selectedCounter
		});
	}

	public Transform GetKitchenObjectFollowTransform()
	{
		return kitchenObjectHoldPoint;
	}

	public void SetKitchenObject(KitchenObject kitchenObject)
	{
		this.kitchenObject = kitchenObject;

		if (kitchenObject != null)
		{
			OnPickedSomething?.Invoke(this, EventArgs.Empty);
		}
	}

	public KitchenObject GetKitchenObject()
	{
		return kitchenObject;
	}

	public void ClearKitchenObject()
	{
		kitchenObject = null;
	}

	public bool HasKitchenObject()
	{
		return kitchenObject != null;
	}
}

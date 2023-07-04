using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerAI : MonoBehaviour, IKitchenObjectParent
{

	private enum State
	{
		GoToLine,
		Order,
		WaitForOrder,
		SearchTable,
		GoToTable,
		Eat,
		Leave
	}

	[SerializeField] private float speed = 7f;
	[SerializeField] private float rotateSpeed = 10f;
	[SerializeField] private Transform kitchenObjectHoldPoint;
	[SerializeField] private float eatTimerMax;

	private State state; // State machine
	private DeliveryCounter selectedCounter; // Delivery counter to order 		
	private Table selectedTable; // Table to eat
	private Transform pickupPoint;  // Position to wait for order
	private bool isWalking;
	private KitchenObject kitchenObject;
	private float eatTimer; // Time to eat

	private void Awake()
	{
		// Debug.Log("New Customer");
		eatTimer = eatTimerMax;
		state = State.GoToLine;
		selectedCounter = DeliveryManager.Instance.GetAvailablePickupPoints()[Random.Range(0, DeliveryManager.Instance.GetAvailablePickupPoints().Count)];
		pickupPoint = selectedCounter.pickupPoint;
		DeliveryManager.Instance.RemoveAvailablePickupPoint(selectedCounter);
		DeliveryManager.Instance.AddCustomerToDeliveryCounter(selectedCounter, this);

	}

	private void Update()
	{
		float reachedPositionDistance = 0.2f;
		switch (state)
		{
			default:
			case State.GoToLine:
				HandleMovement(pickupPoint);

				if (Vector3.Distance(transform.position, pickupPoint.position) < reachedPositionDistance)
				{
					isWalking = false;
					state = State.Order;
				}
				break;
			case State.Order:
				DeliveryManager.Instance.AddRecipeToWaitingList();
				state = State.WaitForOrder;
				break;
			case State.WaitForOrder:
				break;
			case State.SearchTable:

				SearchForFreeTable();

				break;
			case State.GoToTable:
				Transform sitPosition = selectedTable.GetSitPosition();
				HandleMovement(sitPosition);

				if (Vector3.Distance(transform.position, sitPosition.position) < reachedPositionDistance)
				{
					isWalking = false;
					state = State.Eat;
				}
				break;
			case State.Eat:
				Eat();
				break;
			case State.Leave:
				Transform exitPoint = DeliveryManager.Instance.GetCustomerSpawnPoint();
				HandleMovement(exitPoint);

				if (Vector3.Distance(transform.position, exitPoint.position) < reachedPositionDistance)
				{
					isWalking = false;
					Destroy(gameObject);
				}
				break;
		}
	}

	private void Eat()
	{
		eatTimer -= Time.deltaTime;
		if (eatTimer <= 0)
		{
			eatTimer = eatTimerMax;
			GetKitchenObject().DestroySelf();

			TablesManager.Instance.RemoveCustomerInTable(selectedTable);
			TablesManager.Instance.AddAvailableTable(selectedTable);

			state = State.Leave;
		}
	}

	private void HandleMovement(Transform destiny)
	{
		Vector3 moveDir = (destiny.position - transform.position).normalized;

		// transform.position = Vector3.Lerp(transform.position, destiny.position, Time.deltaTime * speed);
		// transform.forward = Vector3.Slerp(transform.forward, moveDir, Time.deltaTime);

		float moveDistance = speed * Time.deltaTime;
		float playerRadius = .7f;
		float playerHeight = 2f;
		bool canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDir, moveDistance);

		if (!canMove)
		{
			//Cannot move torward moveDir

			// Attempt only X movement
			Vector3 moveDirX = new Vector3(moveDir.x * 1.1f, 0, 0).normalized;
			canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirX, moveDistance);

			if (canMove)
			{
				moveDir = moveDirX;
			}
			else
			{
				// Attempt only Z Movement
				Vector3 moveDirZ = new Vector3(0, 0, moveDir.z * 1.1f).normalized;
				canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirZ, moveDistance);

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
			transform.forward = Vector3.Slerp(transform.forward, moveDir, Time.deltaTime * rotateSpeed);

	}

	private void SearchForFreeTable()
	{
		if (TablesManager.Instance.GetAvailableTables().Count == 0)
		{
			state = State.Leave;
			return;
		};

		selectedTable = TablesManager.Instance.GetAvailableTables()[Random.Range(0, TablesManager.Instance.GetAvailableTables().Count)];

		TablesManager.Instance.RemoveAvailableTable(selectedTable);
		TablesManager.Instance.AddCustomerToTable(selectedTable, this);
		state = State.GoToTable;
	}

	public bool IsWalking()
	{
		return isWalking;
	}


	public Transform GetKitchenObjectFollowTransform()
	{
		return kitchenObjectHoldPoint;
	}

	public void SetKitchenObject(KitchenObject kitchenObject)
	{
		if (kitchenObject != null) state = State.SearchTable;

		this.kitchenObject = kitchenObject;
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

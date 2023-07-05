using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TablesManager : MonoBehaviour
{
	/** SINGLETON PATTERN **/
	public static TablesManager Instance { get; private set; }

	// Tables on game screen
	[SerializeField] private List<Table> tables;

	// Available Tables to Eat
	private List<Table> availableTables;
	// Which customer is on each table
	private Dictionary<Table, CustomerAI> customerInTable;

	private void Awake()
	{
		Instance = this;

		availableTables = new List<Table>();
		customerInTable = new Dictionary<Table, CustomerAI>();

		foreach (Table table in tables)
		{
			availableTables.Add(table);
		}
	}

	public List<Table> GetAvailableTables()
	{
		return availableTables;
	}

	public void RemoveAvailableTable(Table table)
	{
		if (availableTables == null) return;

		for (int i = 0; i < availableTables.Count; i++)
		{
			if (availableTables[i] == table)
			{
				availableTables.RemoveAt(i);
			}
		}
	}

	public void AddAvailableTable(Table table)
	{
		availableTables.Add(table);
	}

	public void AddCustomerToTable(Table table, CustomerAI customer)
	{
		customerInTable.Add(table, customer);
	}

	public void RemoveCustomerInTable(Table table)
	{
		customerInTable.Remove(table);
	}
}

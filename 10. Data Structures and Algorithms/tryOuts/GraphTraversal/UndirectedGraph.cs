using System;
using System.Collections.Generic;

namespace GraphTraversal;

public class UndirectedGraph
{
	private Dictionary<int, List<int>> adjacencyList;
	public UndirectedGraph()
	{
		adjacencyList = new Dictionary<int, List<int>>();
	}

	public void AddEdge(int node1, int node2)
	{
		if (!adjacencyList.ContainsKey(node1))
		{
			adjacencyList[node1] = new List<int>();
		}

		if (!adjacencyList.ContainsKey(node2))
		{
			adjacencyList[node2] = new List<int>();
		}

		adjacencyList[node1].Add(node2);
		adjacencyList[node2].Add(node1);
	}

	public Dictionary<int, List<int>> GetGraph()
	{
		return adjacencyList;
	}
}

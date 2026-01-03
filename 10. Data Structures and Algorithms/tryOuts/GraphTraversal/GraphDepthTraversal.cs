using GraphTraversal;
using System;

namespace GraphTraversal;

public class GraphDepthTraversal
{

	public void StarDFS(UndirectedGraph graph, int startNode)
	{
		HashSet<int> visited = new HashSet<int>();
		DFS(graph, startNode, visited);
	}

	private void DFS(UndirectedGraph graph, int startNode, HashSet<int> visited)
	{
		if (visited.Contains(startNode))
		{
			return;
		}

		Console.WriteLine($"{startNode} ");

		visited.Add(startNode);

		var neighbors = graph.GetGraph();

		if (neighbors.ContainsKey(startNode))
		{
			foreach (var neighbor in neighbors[startNode])
			{
				DFS(graph, neighbor, visited);
			}
		}
	}
	

}

using System;
using System.Collections.Generic;
using System.Text;

namespace GraphTraversal
{
    public class GraphBreathFirstTraversal
    {
        public void StartBFS(UndirectedGraph graph, int startNode)
        {
            Queue<int> queue = new Queue<int>();
            HashSet<int> visited = new HashSet<int>();

            visited.Add(startNode);
            queue.Enqueue(startNode);

            while (queue.Count > 0)
            {
                int node = queue.Dequeue();
                Console.WriteLine($"Visited Node: {node}");

                Dictionary<int, List<int>> adjacencyList = graph.GetGraph();    

                foreach (int neighbor in adjacencyList[node])
                {
                    if (!visited.Contains(neighbor))
                    {
                        visited.Add(neighbor);
                        queue.Enqueue(neighbor);
                    }
				}
			}
		}
	}
}

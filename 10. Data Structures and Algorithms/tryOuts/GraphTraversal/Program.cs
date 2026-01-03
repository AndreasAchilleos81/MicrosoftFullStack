using GraphTraversal;
using System.Diagnostics;

UndirectedGraph graph = new UndirectedGraph();

GraphDepthTraversal graphDepthTraversal = new GraphDepthTraversal();

GraphBreathFirstTraversal graphBreathFirstTraversal = new GraphBreathFirstTraversal();


// Add edges to create the following graph:
//     1
//    / \
//   2   3
//  / \   \
// 4   5   6

graph.AddEdge(1, 2);
graph.AddEdge(1, 3);
graph.AddEdge(2, 4);
graph.AddEdge(2, 5);
graph.AddEdge(3, 6);


Console.WriteLine("Graph Structure:");
Console.WriteLine("     1");
Console.WriteLine("    / \\");
Console.WriteLine("   2   3");
Console.WriteLine("  / \\   \\");
Console.WriteLine(" 4   5   6");
Console.WriteLine();

Stopwatch stopwatch = new Stopwatch();
stopwatch.Start();
Console.WriteLine("Depth-First Traversal starting from node 1:");
graphDepthTraversal.StarDFS(graph, 1);
Console.WriteLine($"DFS time: {stopwatch.ElapsedTicks}");

stopwatch.Restart();
Console.WriteLine("Breath-First Traversal starting");
graphBreathFirstTraversal.StartBFS(graph, 1);
Console.WriteLine($"BFS time: {stopwatch.ElapsedTicks}");
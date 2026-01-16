namespace Optimized;

public class Node
{
	public int Value;
	public Node Left;
	public Node Right;

	// ✔ MEMORY OPTIMIZATION:
	// Height is stored directly in the node to avoid recalculating subtree heights recursively.
	// This reduces repeated O(n) height computations to O(1).
	public int Height;

	public Node(int value)
	{
		Value = value;
		Height = 1; // New node starts as a leaf (height = 1)
	}
}

public class AvlTree
{
	public Node Root;

	public void Insert(int value)
	{
		// ✔ RECURSIVE OPTIMIZATION:
		// InsertRecursive returns the updated subtree root, reducing redundant traversal.
		Root = InsertRecursive(Root, value);
	}

	private Node InsertRecursive(Node node, int value)
	{
		if (node == null)
			return new Node(value); // Base case: new leaf node

		// ✔ RECURSIVE OPTIMIZATION:
		// Only recurse into the necessary subtree (left or right).
		if (value < node.Value)
			node.Left = InsertRecursive(node.Left, value);
		else if (value > node.Value)
			node.Right = InsertRecursive(node.Right, value);
		else
			return node; // Duplicate values ignored (prevents unnecessary rebalancing)

		// ✔ MEMORY & PERFORMANCE:
		// Height is updated in O(1) using cached child heights.
		node.Height = 1 + Math.Max(GetHeight(node.Left), GetHeight(node.Right));

		// ✔ TREE BALANCING:
		// Compute balance factor to detect imbalance.
		int balance = GetBalance(node);

		// ✔ TREE BALANCING:
		// Four AVL rotation cases to maintain O(log n) height.

		// Left-Left Case
		if (balance > 1 && value < node.Left.Value)
			return RotateRight(node);

		// Right-Right Case
		if (balance < -1 && value > node.Right.Value)
			return RotateLeft(node);

		// Left-Right Case
		if (balance > 1 && value > node.Left.Value)
		{
			node.Left = RotateLeft(node.Left);
			return RotateRight(node);
		}

		// Right-Left Case
		if (balance < -1 && value < node.Right.Value)
		{
			node.Right = RotateRight(node.Right);
			return RotateLeft(node);
		}

		return node; // Balanced node returned
	}

	// ✔ SEARCH FUNCTIONALITY:
	// Iterative search avoids recursion overhead and stack usage.
	// Runs in O(log n) due to AVL balancing.
	public bool Search(int value)
	{
		var current = Root;

		while (current != null)
		{
			if (value == current.Value)
				return true;

			// ✔ PERFORMANCE:
			// Branching directly to left/right child avoids unnecessary comparisons.
			current = value < current.Value ? current.Left : current.Right;
		}

		return false;
	}

	// ✔ MEMORY OPTIMIZATION:
	// Height lookup is O(1) due to cached height values.
	private int GetHeight(Node node) => node?.Height ?? 0;

	// ✔ PERFORMANCE:
	// Balance factor computed in O(1) using cached heights.
	private int GetBalance(Node node) =>
		node == null ? 0 : GetHeight(node.Left) - GetHeight(node.Right);

	// ✔ TREE BALANCING:
	// Right rotation fixes Left-Left imbalance.
	private Node RotateRight(Node y)
	{
		Node x = y.Left;
		Node T2 = x.Right;

		// Perform rotation
		x.Right = y;
		y.Left = T2;

		// ✔ MEMORY & PERFORMANCE:
		// Update heights after rotation using O(1) height lookups.
		y.Height = 1 + Math.Max(GetHeight(y.Left), GetHeight(y.Right));
		x.Height = 1 + Math.Max(GetHeight(x.Left), GetHeight(x.Right));

		return x; // New root of subtree
	}

	// ✔ TREE BALANCING:
	// Left rotation fixes Right-Right imbalance.
	private Node RotateLeft(Node x)
	{
		Node y = x.Right;
		Node T2 = y.Left;

		// Perform rotation
		y.Left = x;
		x.Right = T2;

		// ✔ MEMORY & PERFORMANCE:
		// Update heights after rotation using O(1) height lookups.
		x.Height = 1 + Math.Max(GetHeight(x.Left), GetHeight(x.Right));
		y.Height = 1 + Math.Max(GetHeight(y.Left), GetHeight(y.Right));

		return y; // New root of subtree
	}

	public void PrintInOrder(Node node)
	{
		if (node == null)
			return;

		// Standard in-order traversal
		PrintInOrder(node.Left);
		Console.Write(node.Value + " ");
		PrintInOrder(node.Right);
	}
}

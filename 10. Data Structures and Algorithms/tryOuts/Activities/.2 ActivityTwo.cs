// How did the LLM assist in refining the algorithm?
// The LLM identified inefficiencies in the original design—specifically the repeated List.Sort() calls—and proposed replacing the entire structure with a binary min‑heap. It also introduced thread‑safety, batch processing, and optimized O(log n) operations.

// 2. Were any LLM-generated suggestions inaccurate or unnecessary?
// No. All suggestions aligned with standard data‑structure optimization principles. The heap implementation, thread‑safety, and batch‑enqueue logic were appropriate and beneficial.

// 3. What were the most impactful improvements you implemented?
// Replacing List.Sort() with a binary min‑heap

// Reducing insertion and removal to O(log n)

// Adding thread‑safe locking

// Adding batch enqueue with O(n) heap construction

// These changes transformed the queue from a slow, repeatedly sorted list into a high‑performance, scalable priority queue.

using System;
using System.Collections.Generic;
using System.Threading;

public class ApiRequest
{
    public string Endpoint { get; set; }
    public int Priority { get; set; }

    public ApiRequest(string endpoint, int priority)
    {
        Endpoint = endpoint;
        Priority = priority;
    }
}

public class ApiRequestQueue
{
    // ✔ LLM-GENERATED MODIFICATION:
    // Replaced List<T> used as a sorted list with a binary min-heap.
    // WHY: A heap guarantees O(log n) insertion and O(log n) removal,
    // eliminating the O(n log n) cost of List.Sort().
    private readonly List<ApiRequest> heap = new List<ApiRequest>();

    // ✔ LLM-GENERATED MODIFICATION:
    // Added thread-safety using a lock object.
    // WHY: Ensures safe concurrent access when multiple threads enqueue/dequeue.
    private readonly object syncLock = new object();

    // -----------------------------
    // ENQUEUE (Single Insert)
    // -----------------------------
    public void Enqueue(ApiRequest request)
    {
        lock (syncLock)
        {
            // ✔ LLM-GENERATED MODIFICATION:
            // Insert into heap instead of sorting entire list.
            // WHY: HeapifyUp ensures O(log n) insertion.
            heap.Add(request);
            HeapifyUp(heap.Count - 1);
        }
    }

    // -----------------------------
    // BULK ENQUEUE (Batch Insert)
    // -----------------------------
    public void EnqueueRange(IEnumerable<ApiRequest> batch)
    {
        lock (syncLock)
        {
            // ✔ LLM-GENERATED MODIFICATION:
            // Add all items first, then build heap in O(n).
            // WHY: More efficient than inserting each item individually (O(k log n)).
            foreach (var req in batch)
                heap.Add(req);

            BuildHeap(); // O(n) heap construction
        }
    }

    // -----------------------------
    // DEQUEUE (Extract-Min)
    // -----------------------------
    public ApiRequest Dequeue()
    {
        lock (syncLock)
        {
            if (heap.Count == 0)
                return null;

            // ✔ LLM-GENERATED MODIFICATION:
            // Extract root (min priority) in O(log n).
            // WHY: Replacing RemoveAt(0) avoids O(n) shifting cost.
            ApiRequest min = heap[0];

            heap[0] = heap[heap.Count - 1];
            heap.RemoveAt(heap.Count - 1);

            if (heap.Count > 0)
                HeapifyDown(0); // Restore heap property in O(log n)

            return min;
        }
    }

    public int Count
    {
        get
        {
            lock (syncLock)
                return heap.Count;
        }
    }

    // -----------------------------
    // HEAPIFY OPERATIONS
    // -----------------------------
    private void HeapifyUp(int index)
    {
        // Standard bubble-up for min-heap
        while (index > 0)
        {
            int parent = (index - 1) / 2;

            if (heap[index].Priority >= heap[parent].Priority)
                break;

            Swap(index, parent);
            index = parent;
        }
    }

    private void HeapifyDown(int index)
    {
        int lastIndex = heap.Count - 1;

        while (true)
        {
            int left = index * 2 + 1;
            int right = index * 2 + 2;
            int smallest = index;

            if (left <= lastIndex && heap[left].Priority < heap[smallest].Priority)
                smallest = left;

            if (right <= lastIndex && heap[right].Priority < heap[smallest].Priority)
                smallest = right;

            if (smallest == index)
                break;

            Swap(index, smallest);
            index = smallest;
        }
    }

    // ✔ LLM-GENERATED MODIFICATION:
    // Added BuildHeap for batch processing.
    // WHY: Builds heap in O(n) instead of O(k log n) repeated inserts.
    private void BuildHeap()
    {
        for (int i = heap.Count / 2 - 1; i >= 0; i--)
            HeapifyDown(i);
    }

    private void Swap(int a, int b)
    {
        ApiRequest temp = heap[a];
        heap[a] = heap[b];
        heap[b] = temp;
    }
}

class Program
{
    static void Main()
    {
        ApiRequestQueue queue = new ApiRequestQueue();

        queue.Enqueue(new ApiRequest("/auth", 1));
        queue.Enqueue(new ApiRequest("/data", 3));
        queue.Enqueue(new ApiRequest("/healthcheck", 2));

        Console.WriteLine($"Processing: {queue.Dequeue()?.Endpoint}");
    }
}

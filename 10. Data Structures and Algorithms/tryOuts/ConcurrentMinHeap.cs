using System;
using System.Collections.Generic;
using System.Threading;

public class ConcurrentMinHeap<T>
    where T : IComparable<T>
{
    private readonly List<T> heap = new List<T>();

    // ReaderWriterLockSlim allows:
    // - Many concurrent readers
    // - Only one writer
    // - Writers block new readers
    private readonly ReaderWriterLockSlim rwLock = new ReaderWriterLockSlim();

    public int Count
    {
        get
        {
            rwLock.EnterReadLock();
            try
            {
                return heap.Count;
            }
            finally
            {
                rwLock.ExitReadLock();
            }
        }
    }

    // -----------------------------
    // INSERT (Exclusive Writer)
    // -----------------------------
    public void Insert(T value)
    {
        rwLock.EnterWriteLock();
        try
        {
            heap.Add(value);
            HeapifyUp(heap.Count - 1);
        }
        finally
        {
            rwLock.ExitWriteLock();
        }
    }

    // -----------------------------
    // EXTRACT MIN (Exclusive Writer)
    // -----------------------------
    public T ExtractMin()
    {
        rwLock.EnterWriteLock();
        try
        {
            if (heap.Count == 0)
                throw new InvalidOperationException("Heap is empty.");

            T min = heap[0];

            heap[0] = heap[heap.Count - 1];
            heap.RemoveAt(heap.Count - 1);

            if (heap.Count > 0)
                HeapifyDown(0);

            return min;
        }
        finally
        {
            rwLock.ExitWriteLock();
        }
    }

    // -----------------------------
    // PEEK (Concurrent Readers Allowed)
    // -----------------------------
    public T Peek()
    {
        rwLock.EnterReadLock();
        try
        {
            if (heap.Count == 0)
                throw new InvalidOperationException("Heap is empty.");

            return heap[0];
        }
        finally
        {
            rwLock.ExitReadLock();
        }
    }

    // -----------------------------
    // HEAPIFY OPERATIONS
    // -----------------------------
    private void HeapifyUp(int index)
    {
        while (index > 0)
        {
            int parent = (index - 1) / 2;

            if (heap[index].CompareTo(heap[parent]) >= 0)
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

            if (left <= lastIndex && heap[left].CompareTo(heap[smallest]) < 0)
                smallest = left;

            if (right <= lastIndex && heap[right].CompareTo(heap[smallest]) < 0)
                smallest = right;

            if (smallest == index)
                break;

            Swap(index, smallest);
            index = smallest;
        }
    }

    private void Swap(int a, int b)
    {
        T temp = heap[a];
        heap[a] = heap[b];
        heap[b] = temp;
    }
}

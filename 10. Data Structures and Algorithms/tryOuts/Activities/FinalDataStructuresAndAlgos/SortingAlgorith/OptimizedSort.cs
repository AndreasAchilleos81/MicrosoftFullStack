using System;
using System.Threading.Tasks;

namespace Optimized
{
	public class OptimizedSorting
	{
		private const int INSERTION_SORT_THRESHOLD = 32;
		private const int PARALLEL_THRESHOLD = 50000;

		public void QuickSort(int[] arr)
		{
			QuickSort(arr, 0, arr.Length - 1);
		}

		private void QuickSort(int[] arr, int left, int right)
		{
			while (left < right)
			{
				// Use insertion sort for tiny partitions
				if (right - left < INSERTION_SORT_THRESHOLD)
				{
					InsertionSort(arr, left, right);
					return;
				}

				// Choose pivot and partition
				int pivotIndex = Partition(arr, left, right);

				bool doParallel = (right - left) > PARALLEL_THRESHOLD;

				if (doParallel)
				{
					Parallel.Invoke(
						() => QuickSort(arr, left, pivotIndex - 1),
						() => QuickSort(arr, pivotIndex + 1, right)
					);
					return;
				}

				// Tail-call elimination: recurse on smaller side first
				if (pivotIndex - left < right - pivotIndex)
				{
					QuickSort(arr, left, pivotIndex - 1);
					left = pivotIndex + 1;
				}
				else
				{
					QuickSort(arr, pivotIndex + 1, right);
					right = pivotIndex - 1;
				}
			}
		}

		private int Partition(int[] arr, int left, int right)
		{
			int pivotIndex = MedianOfThree(arr, left, right);
			int pivotValue = arr[pivotIndex];

			// Move pivot to end
			Swap(arr, pivotIndex, right);

			int store = left;

			for (int i = left; i < right; i++)
			{
				if (arr[i] < pivotValue)
				{
					Swap(arr, i, store);
					store++;
				}
			}

			Swap(arr, store, right);
			return store;
		}

		private int MedianOfThree(int[] arr, int left, int right)
		{
			int mid = (left + right) / 2;

			if (arr[left] > arr[mid])
				Swap(arr, left, mid);

			if (arr[mid] > arr[right])
				Swap(arr, mid, right);

			if (arr[left] > arr[mid])
				Swap(arr, left, mid);

			return mid;
		}

		private void InsertionSort(int[] arr, int left, int right)
		{
			for (int i = left + 1; i <= right; i++)
			{
				int key = arr[i];
				int j = i - 1;

				while (j >= left && arr[j] > key)
				{
					arr[j + 1] = arr[j];
					j--;
				}

				arr[j + 1] = key;
			}
		}

		private void Swap(int[] arr, int a, int b)
		{
			if (a == b) return;
			int temp = arr[a];
			arr[a] = arr[b];
			arr[b] = temp;
		}

		public void PrintArray(int[] arr)
		{
			foreach (var item in arr)
				Console.Write(item + " ");
			Console.WriteLine();
		}
	}
}

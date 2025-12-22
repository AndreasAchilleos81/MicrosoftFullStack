using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace BinarySearch
{
    public static class Search
    {
        public static int BinarySearch(int[] array, int target)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
			int left = 0;
            int right = array.Length - 1;
            while (left <= right)
            {
                int mid = left + (right - left) / 2;
                if (array[mid] == target)
                {
                    Console.WriteLine($"Binary Search took {stopwatch.ElapsedMilliseconds} ms");
                    stopwatch.Stop();
					return mid; // Target found
                }
                else if (array[mid] < target)
                {
                    left = mid + 1; // Search in the right half
                }
                else
                {
                    right = mid - 1; // Search in the left half
                }
            }
			Console.WriteLine($"Binary Search took {stopwatch.ElapsedMilliseconds} ms");
			stopwatch.Stop();
			return -1; // Target not found
		}
	}
}

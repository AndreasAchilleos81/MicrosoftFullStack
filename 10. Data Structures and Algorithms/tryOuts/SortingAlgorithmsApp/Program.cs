using System.Diagnostics;

int[] numbers = { 5, 3, 8, 4, 2, 7, 1, 6 };
int[] bubbleArray = (numbers.Clone() as int[])!;
int[] quickSortArray = (numbers.Clone() as int[])!;
int[] mergeSortArray = (numbers.Clone() as int[])!;
Stopwatch stopwatch = new Stopwatch();	


stopwatch.Start();
BubbleSort(bubbleArray);
Console.WriteLine($"Sorted array: {string.Join(", ", numbers)} took: {stopwatch.Elapsed.TotalMilliseconds}" );
stopwatch.Restart();
QuickSort(quickSortArray, 0, numbers.Length - 1);
Console.WriteLine($"Sorted array using QuickSort:{string.Join(", ", quickSortArray)} took: {stopwatch.Elapsed.TotalMilliseconds}");
stopwatch.Restart();

MergeSort(mergeSortArray, 0, mergeSortArray.Length - 1);
Console.WriteLine($"Sorted array using MergeSort: {string.Join(", ", mergeSortArray)} took: {stopwatch.Elapsed.TotalMilliseconds}" );
static void MergeSort(int[] arr, int left, int right)
{
	if (left < right)
	{
		int mid = (left + right) / 2;
		MergeSort(arr, left, mid);
		MergeSort(arr, mid + 1, right);
		Merge(arr, left, mid, right);
	}
}

static void Merge(int[] arr, int left, int mid, int right)
{
	int[] leftArr = arr[left..(mid + 1)];
	int[] rightArr = arr[(mid + 1)..(right + 1)];
	int i = 0, j = 0, k = left;
	while (i < leftArr.Length && j < rightArr.Length)
	{
		if (leftArr[i] <= rightArr[j])
			arr[k++] = leftArr[i++];
		else
			arr[k++] = rightArr[j++];
	}
	while (i < leftArr.Length) arr[k++] = leftArr[i++];
	while (j < rightArr.Length) arr[k++] = rightArr[j++];
}

static void QuickSort(int[] array, int left, int right)
{
	if (left < right)
	{
		int pivotIndex = Partition(array, left, right);
		QuickSort(array, left, pivotIndex - 1);
		QuickSort(array, pivotIndex + 1, right);
	}
}

static int Partition(int[] arr, int low, int high)
{
	int pivot = arr[high];
	int i = (low - 1);

	for (int j = low; j < high; j++)
	{
		if (arr[j] < pivot)
		{
			i++;
			int temp = arr[i];
			arr[i] = arr[j];
			arr[j] = temp;
		}
	}

	int temp1 = arr[i + 1];
	arr[i + 1] = arr[high];
	arr[high] = temp1;

	return i + 1;
}

static void BubbleSort(int[] array)
{
	int n = array.Length;
	int temp = 0;
	for (int i = 0; i < n - 1; i++)
	{
		int upto = n - i - 1;
		for (int j = 0; j < upto; j++)
		{
			if (array[j] > array[j + 1])
			{
				temp = array[j];	
				array[j] = array[j + 1];
				array[j + 1] = temp;
			}
		}
	}
}



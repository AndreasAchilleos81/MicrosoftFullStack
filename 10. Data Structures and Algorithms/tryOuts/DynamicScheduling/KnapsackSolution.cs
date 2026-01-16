public class KnapsackSolution
{
	public int OptimalValue { get; set; }
	public List<ResourceItem> SelectedItems { get; set; }
	public long ExecutionTimeMs { get; set; }
	public int RecursiveCalls { get; set; }

	public KnapsackSolution()
	{
		SelectedItems = new List<ResourceItem>();
	}
}
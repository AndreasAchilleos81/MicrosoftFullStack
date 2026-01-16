public class ResourceItem
{
	public string Name { get; set; }
	public int Value { get; set; }
	public int Weight { get; set; }
	public ResourceItem(string name, int value, int weight)
	{
		Name = name;
		Value = value;
		Weight = weight;
	}

	public override string ToString() => $"{Name} (Value: {Value}, Weight: {Weight})";
}
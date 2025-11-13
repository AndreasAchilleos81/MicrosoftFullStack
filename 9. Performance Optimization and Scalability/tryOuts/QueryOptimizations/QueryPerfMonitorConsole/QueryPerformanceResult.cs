namespace QueryPerfMonitorConsole
{
	public class QueryPerformanceResult
	{
		public string Query { get; set; }
		public DateTime StartTime { get; set; }
		public DateTime EndTime { get; set; }
		public long ExecutionTimeMs { get; set; }
		public int RowCount { get; set; }
		public bool Success { get; set; }
		public string ErrorMessage { get; set; }
		public List<string> Statistics { get; set; } = new List<string>();
	}
}

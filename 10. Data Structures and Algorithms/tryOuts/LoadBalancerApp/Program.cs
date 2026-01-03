using LoadBalancerApp;

var servers = new List<Server>
{
	new Server("Server One", "192.168.1.1") { NumberOfActiveConnections = 2},
	new Server("Server Two", "192.168.1.2")  { NumberOfActiveConnections = 1},
	new Server("Server Three", "192.168.1.3") { NumberOfActiveConnections = 4},
	new Server("Server Four", "192.168.1.4"),
	new Server("Server Five", "192.168.1.5"),
	new Server("Server Six", "192.168.1.6"),
};

//Console.WriteLine("--------------------------------------------------");
//Console.WriteLine("Round Robin Load Balancer");

//RoundRobinLoadBalancer roundRobinLoadBalancer = new RoundRobinLoadBalancer(servers);

//for (int i = 0; i < 12; i++)
//{
//	var server = roundRobinLoadBalancer.GetServer();
//	Console.WriteLine($"Request {i + 1} assigned to {server.Id} ({server.IpAddress}) NumberOfConnection: {server.NumberOfActiveConnections} RequestCount: {server.RequestCount}");
//	server.HandleRequest();
//}

//Console.WriteLine("--------------------------------------------------");
//Console.WriteLine("Least Connection Load Balancer");

//LeastConnectionLoadBalancer leastConnectionLoadBalancer = new LeastConnectionLoadBalancer(servers);

//for (int i = 0; i < 12; i++)
//{
//	var server = leastConnectionLoadBalancer.GetServer();
//	Console.WriteLine($"Request {i + 1} assigned to {server.Id} ({server.IpAddress}) NumberOfConnection: {server.NumberOfActiveConnections} RequestCount: {server.RequestCount}");
//	server.HandleRequest();
//	if (i == 3)
//	{
//		Thread.Sleep(5000);
//	}
//}


Console.WriteLine("--------------------------------------------------");
Console.WriteLine("IP Hashing Load Balancer");


IpHashingLoadBalancer ipHashingLoadBalancer = new IpHashingLoadBalancer(servers);
foreach (var s in servers)
{
	var server = ipHashingLoadBalancer.GetServer(s.IpAddress);
	Console.WriteLine($"Request assigned to {server.Id} ({server.IpAddress}) NumberOfConnection: {server.NumberOfActiveConnections} RequestCount: {server.RequestCount}");
	server.HandleRequest();

}

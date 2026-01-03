namespace LoadBalancerApp
{

	public class RoundRobinLoadBalancer : LoadBalancer
	{

		public RoundRobinLoadBalancer(IEnumerable<Server> servers)
			: base(servers) { }


		public override Server GetServer()
		{
			var server = servers[index % servers.Count];
			++index;
			return server;
		}
    }
}

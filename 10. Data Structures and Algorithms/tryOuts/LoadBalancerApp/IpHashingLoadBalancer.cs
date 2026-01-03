namespace LoadBalancerApp
{
    public class IpHashingLoadBalancer : LoadBalancer   
    {
        public IpHashingLoadBalancer(IEnumerable<Server> servers) 
            : base(servers) 
        { 
            
        }

        public override Server GetServer()
        {
            return null;
        }

        public Server GetServer(string ipAddress)
        {
            var server = servers.First(s => s.IpAddress == ipAddress);
            var hash = server.GetServerHash();  
            var serverIndex = hash % servers.Count;
            return servers[serverIndex];    
		}
	}
}

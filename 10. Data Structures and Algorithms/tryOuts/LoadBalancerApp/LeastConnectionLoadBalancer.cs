using System;
using System.Collections.Generic;
using System.Text;

namespace LoadBalancerApp
{
    public class LeastConnectionLoadBalancer : LoadBalancer
    {
        public LeastConnectionLoadBalancer(IEnumerable<Server> servers)
            : base(servers) { }


        public override Server GetServer()
        {
            var leastLoadedServer = servers.OrderBy(s => s.NumberOfActiveConnections).First();
            leastLoadedServer.NumberOfActiveConnections++;
            return leastLoadedServer;  
		}
	}
}

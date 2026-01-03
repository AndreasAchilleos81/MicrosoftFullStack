using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace LoadBalancerApp
{

	 public abstract class LoadBalancer
	{
		protected List<Server> servers = new List<Server>();
		protected int index = 0;

		public LoadBalancer(IEnumerable<Server> servers)
		{
			this.servers.AddRange(servers);
		}

		public abstract Server GetServer();

	}
}

using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace LoadBalancerApp
{
	public class Server
	{
		public string Id { get; set; }
		public int RequestCount { get; set; } = 0;

		public string IpAddress { get; set; }	

		public int NumberOfActiveConnections { get; set; }

		public Server(string id, string ipAddress)
		{
			Id = id;
			this.IpAddress = ipAddress;	
		}

		public void HandleRequest()
		{
			RequestCount++;
			NumberOfActiveConnections++; ;
			Task.Run(async () =>
			{
				await Task.Delay(5000); // Simulate some processing time
				if (NumberOfActiveConnections > 0)
				{
					NumberOfActiveConnections--;
					RequestCount--;
				}
			});
		}

		public int GetServerHash()
		{
			using (MD5 md5 = MD5.Create())
			{
				// Fix: Properly create a char array with an initializer
				var partToHash = int.Parse(IpAddress[IpAddress.Length - 1].ToString());
				return --partToHash;
				//byte[] hash = md5.ComputeHash(Encoding.UTF8.GetBytes(partToHash));
				//var result = Math.Abs( BitConverter.ToInt32(hash, 0));
				//return result;
			}
		}
	}
}

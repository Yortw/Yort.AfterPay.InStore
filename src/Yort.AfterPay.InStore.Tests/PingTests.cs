using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yort.AfterPay.InStore.Tests
{
	[TestClass]
	public class PingTests
	{
		[TestMethod]
		[TestCategory("Integration")]
		public async Task Ping_CanPingAfterPayServers()
		{			
			var config = new AfterPayConfiguration()
			{
				Environment = AfterPayEnvironment.Sandbox
			};

			var client = new AfterPayClient(config);
			var success = await client.Ping();

			Assert.IsTrue(success);
		}
	}
}
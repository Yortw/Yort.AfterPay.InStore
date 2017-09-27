using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yort.AfterPay.InStore.Tests
{
	[TestClass]
	[TestCategory("Integration")]
	public class PreapprovalEnquiryTests
	{
		[TestMethod]
		[TestCategory("Manual-Integration")]
		[Ignore("Requires a pre-approval code be manually inserted into the test")]
		public async Task Preapproval_CanEnquire()
		{
			//You must provide a current preapproval code here to run this test.
			string preapprovalCode = String.Empty; //TODO: To run this test you must edit this with a valid pre-approval code

			var config = new AfterPayConfiguration()
			{
				Environment = AfterPayEnvironment.Sandbox,
				DeviceId = AfterPayTestSecrets.DeviceId,
				DeviceKey = AfterPayTestSecrets.DeviceKey
			};

			var client = new AfterPayClient(config);
			var response = await client.PreapprovalEnquiry
			(
				new AfterPayPreapprovalRequest() {  PreapprovalCode = preapprovalCode },
				new AfterPayCallContext { OperatorId = "Yort" }
			);

			Assert.IsNotNull(response);
			Assert.IsTrue(response.Minimum.Amount > 0);
			Assert.IsTrue(response.Amount.Amount > 0);
		}
	}
}
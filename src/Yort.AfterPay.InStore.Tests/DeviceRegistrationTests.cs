using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yort.AfterPay.InStore.Tests
{
	[TestClass]
	public class DeviceRegistrationTests
	{

		[TestMethod]
		[TestCategory("Integration")]
		public async Task DeviceRegistration_ErrorsOnReusedSecret()
		{
			
			try
			{
				var config = new AfterPayConfiguration()
				{
					Environment = AfterPayEnvironment.Sandbox
				};
				var client = new AfterPayClient(config);

				var result = await client.RegisterDevice(new AfterPayDeviceRegistrationRequest()
				{
					Name = AfterPayTestSecrets.DuplicateRegistrationDeviceName,
					Secret = AfterPayTestSecrets.DuplicateRegistrationDeviceSecret,
					Attributes = new
					{
						Branch = "Sydney"
					}
				});
				Assert.Fail("No exception thrown.");
			}
			catch (AfterPayApiException ex)
			{
				Assert.AreEqual("unauthorized", ex.ErrorCode);
				Assert.AreEqual("Invalid Secret", ex.Message);
				Assert.AreEqual(401, ex.HttpStatusCode);
				Assert.IsNotNull(ex.ErrorId);
			}
		}
	}
}
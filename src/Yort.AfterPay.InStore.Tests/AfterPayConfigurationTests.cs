using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yort.AfterPay.InStore.Tests
{
	[TestClass]
	public class AfterPayConfigurationTests
	{

		[TestMethod]
		public void Configuration_SystemClock_ReturnsCustomClockWhenSet()
		{
			var clock = new MockClock(DateTimeOffset.Now);

			AfterPayConfiguration.SystemClock = clock;

			Assert.AreEqual(clock, AfterPayConfiguration.SystemClock);
		}

		[TestMethod]
		public void Configuration_SystemClock_ReturnsDefaultClockWhenExplicitlySetToNull()
		{
			AfterPayConfiguration.SystemClock = null;

			Assert.AreEqual(AfterPaySystemClock.DefaultInstance, AfterPayConfiguration.SystemClock);
		}

		[TestMethod]
		public void Configuration_SystemClock_DefaultsToSystemClock()
		{
			Assert.AreEqual(AfterPaySystemClock.DefaultInstance, AfterPayConfiguration.SystemClock);
		}

		[TestMethod]
		public void Configuration_PropertiesAreLockedOnceInUse()
		{
			var config = new AfterPayConfiguration()
			{
				DeviceId = "123",
				Environment = AfterPayEnvironment.Sandbox,
				DeviceKey = "ABC",
				MinimumRetries = 5,
				ProductName = "Test",
				ProductVersion = "2.0",
				RetryDelaySeconds = 7
			};

			var client = new AfterPayClient(config);

			Assert.ThrowsException<InvalidOperationException>(() => config.DeviceId = "ABC");
			Assert.ThrowsException<InvalidOperationException>(() => config.Environment = AfterPayEnvironment.Sandbox);
			Assert.ThrowsException<InvalidOperationException>(() => config.DeviceKey = "123");
			Assert.ThrowsException<InvalidOperationException>(() => config.MinimumRetries = 2);
			Assert.ThrowsException<InvalidOperationException>(() => config.ProductName = "testPOS");
			Assert.ThrowsException<InvalidOperationException>(() => config.ProductVersion = "1.0");
			Assert.ThrowsException<InvalidOperationException>(() => config.RetryDelaySeconds = 3);

			Assert.AreEqual("123", config.DeviceId);
			Assert.AreEqual(AfterPayEnvironment.Sandbox, config.Environment);
			Assert.AreEqual("ABC", config.DeviceKey);
			Assert.AreEqual(5, config.MinimumRetries);
			Assert.AreEqual("Test", config.ProductName);
			Assert.AreEqual("2.0", config.ProductVersion);
			Assert.AreEqual(7, config.RetryDelaySeconds);
		}

		[TestMethod]
		public async Task Configuration_UserAgent_UsesAppropriateDefault()
		{
			string userAgent = null;

			var handler = new Yort.Http.ClientPipeline.MockMessageHandler();
			handler.AddDynamicResponse(new Http.ClientPipeline.MockResponseHandler()
			{
				CanHandleRequest = (request) => true,
				HandleRequest = (request) =>
				{
					userAgent = request.Headers.UserAgent.ToString();
					return Task.FromResult(new System.Net.Http.HttpResponseMessage(System.Net.HttpStatusCode.NotFound));
				}
			});

			var config = new AfterPayConfiguration()
			{
				DeviceId = "123",
				DeviceKey = "123",
				Environment = AfterPayEnvironment.Sandbox,
				HttpClient = new System.Net.Http.HttpClient(handler)
			};
			var client = new AfterPayClient(config);

			try
			{
				await client.SendInvite
				(
					new AfterPayInviteRequest()
					{
						ExpectedAmount = new AfterPayMoney(10M, AfterPayCurrencies.AustralianDollars),
						MobileNumber = "555-5555"
					},
					new AfterPayCallContext() { OperatorId = "Yort" }
				);
			}
			catch { }
			finally
			{
				Assert.IsTrue(userAgent.StartsWith("Yort.AfterPay.InStore/"));
				Assert.IsTrue(userAgent.EndsWith(typeof(AfterPayClient).Assembly.GetName().Version.ToString()));
			}
		}
	}
}
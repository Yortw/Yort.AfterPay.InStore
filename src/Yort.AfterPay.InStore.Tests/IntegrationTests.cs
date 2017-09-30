using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yort.AfterPay.InStore.Tests
{
	[TestClass]
	public class IntegrationTests
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
			await client.Ping();
		}

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

		[TestMethod]
		[TestCategory("Integration")]
		[Ignore("Requires one time registration credentials be specified by environment arguments.")]
		public async Task DeviceRegistration_RegistersNewDevice()
		{
			if (String.IsNullOrEmpty(AfterPayTestSecrets.UnregisteredDeviceName))
				Assert.Inconclusive("You must set the AfterPayTests-UnregisteredDeviceName environment variable to run this test.");

			if (String.IsNullOrEmpty(AfterPayTestSecrets.UnregisteredDeviceSecret))
				Assert.Inconclusive("You must set the AfterPayTests-UnregisteredDeviceSecret environment variable to run this test.");

			var config = new AfterPayConfiguration()
			{
				Environment = AfterPayEnvironment.Sandbox
			};
			var client = new AfterPayClient(config);

			var result = await client.RegisterDevice(new AfterPayDeviceRegistrationRequest()
			{
				Name = AfterPayTestSecrets.UnregisteredDeviceName,
				Secret = AfterPayTestSecrets.UnregisteredDeviceSecret,
				Attributes = new
				{
					Branch = "Sydney"
				}
			});
			Assert.IsNull(result);
			Assert.IsNotNull(result.DeviceId);
			Assert.IsNotNull(result.Key);

			System.Diagnostics.Trace.WriteLine("Device ID: " + result.DeviceId);
			System.Diagnostics.Trace.WriteLine("Device Key: " + result.Key);
		}

		[TestMethod]
		[TestCategory("Integration")]
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
				new AfterPayPreapprovalRequest() { PreapprovalCode = preapprovalCode },
				new AfterPayCallContext { OperatorId = "Yort" }
			);

			Assert.IsNotNull(response);
			Assert.IsTrue(response.Minimum.Amount > 0);
			Assert.IsTrue(response.Amount.Amount > 0);
		}

		[TestMethod]
		[TestCategory("Integration")]
		public async Task SendInvite_CanSendInvite()
		{
			var config = new AfterPayConfiguration()
			{
				Environment = AfterPayEnvironment.Sandbox,
				DeviceId = AfterPayTestSecrets.DeviceId,
				DeviceKey = AfterPayTestSecrets.DeviceKey
			};

			var client = new AfterPayClient(config);
			await client.SendInvite
			(
				new AfterPayInviteRequest() { MobileNumber = "0400090000", ExpectedAmount = new AfterPayMoney(200, AfterPayCurrencies.AustralianDollars) },
				new AfterPayCallContext() { OperatorId = "Randal Graves" }
			);
		}

		[TestMethod]
		[TestCategory("Integration")]
		[TestCategory("Manual-Integration")]
		[Ignore("Requires a pre-approval code be manually inserted into the test")]
		public async Task CreateOrder_CanCreateOrder()
		{
			//You must provide a current preapproval code here to run this test.
			string preapprovalCode = String.Empty; //TODO: To run this test you must edit this with a valid pre-approval code

			var config = new AfterPayConfiguration()
			{
				Environment = AfterPayEnvironment.Sandbox,
				DeviceId = AfterPayTestSecrets.DeviceId,
				DeviceKey = AfterPayTestSecrets.DeviceKey,
				ProductName = "YortAfterPayInstoreLib",
				ProductVersion = typeof(AfterPayClient).Assembly.GetName().Version.ToString()
			};

			var client = new AfterPayClient(config);
			var result = await client.CreateOrder
			(
				new AfterPayCreateOrderRequest()
				{
					Amount = new AfterPayMoney(50, AfterPayCurrencies.AustralianDollars),
					MerchantReference = System.Guid.NewGuid().ToString(),
					PreapprovalCode = preapprovalCode,
					OrderItems = new AfterPayOrderItem[]
					{
						new AfterPayOrderItem() { Name = "Navy Check Jacket", Price = new AfterPayMoney(200, AfterPayCurrencies.AustralianDollars), Quantity = 1, Sku = "20000332" }
					}
				},
				new AfterPayCallContext() { OperatorId = "Randal Graves" }
			);

			Assert.IsNotNull(result);
			Assert.IsNotNull(result.OrderId);
		}

		[TestMethod]
		[TestCategory("Integration")]
		[TestCategory("Manual-Integration")]
		[Ignore("Requires a pre-approval code be manually inserted into the test")]
		public async Task CreateOrder_CanReverseOrder()
		{
			//You must provide a current preapproval code here to run this test.
			string preapprovalCode = String.Empty; //TODO: To run this test you must edit this with a valid pre-approval code

 			#region Test Setup - Create an order to reverse
			var config = new AfterPayConfiguration()
			{
				Environment = AfterPayEnvironment.Sandbox,
				DeviceId = AfterPayTestSecrets.DeviceId,
				DeviceKey = AfterPayTestSecrets.DeviceKey,
				ProductName = "YortAfterPayInstoreLib",
				ProductVersion = typeof(AfterPayClient).Assembly.GetName().Version.ToString()
			};

			var client = new AfterPayClient(config);

			var order = await client.CreateOrder
			(
				new AfterPayCreateOrderRequest()
				{
					Amount = new AfterPayMoney(50, AfterPayCurrencies.AustralianDollars),
					MerchantReference = System.Guid.NewGuid().ToString(),
					PreapprovalCode = preapprovalCode,
					OrderItems = new AfterPayOrderItem[]
					{
						new AfterPayOrderItem() { Name = "Navy Check Jacket", Price = new AfterPayMoney(200, AfterPayCurrencies.AustralianDollars), Quantity = 1, Sku = "20000332" }
					}
				},
				new AfterPayCallContext() { OperatorId = "Randal Graves" }
			);

			#endregion

			var result = await client.ReverseOrder
			(
				new AfterPayReverseOrderRequest()
				{
					ReversingRequestId = order.RequestId
				},
				new AfterPayCallContext() { OperatorId = "Randal Graves" }
			);

			Assert.IsNotNull(result);
			Assert.IsNotNull(result.ReverseId);
		}

		[TestMethod]
		[TestCategory("Integration")]
		[TestCategory("Manual-Integration")]
		[Ignore("Requires a pre-approval code be manually inserted into the test")]
		public async Task CreateOrder_CanCreateRefund()
		{
			//You must provide a current preapproval code here to run this test.
			string preapprovalCode = String.Empty; //TODO: To run this test you must edit this with a valid pre-approval code

			#region Test Setup - create an order to refund

			var config = new AfterPayConfiguration()
			{
				Environment = AfterPayEnvironment.Sandbox,
				DeviceId = AfterPayTestSecrets.DeviceId,
				DeviceKey = AfterPayTestSecrets.DeviceKey,
				ProductName = "YortAfterPayInstoreLib",
				ProductVersion = typeof(AfterPayClient).Assembly.GetName().Version.ToString()
			};

			var client = new AfterPayClient(config);
			var order = await client.CreateOrder
			(
				new AfterPayCreateOrderRequest()
				{
					Amount = new AfterPayMoney(50, AfterPayCurrencies.AustralianDollars),
					MerchantReference = System.Guid.NewGuid().ToString(),
					PreapprovalCode = preapprovalCode,
					OrderItems = new AfterPayOrderItem[]
					{
						new AfterPayOrderItem() { Name = "Navy Check Jacket", Price = new AfterPayMoney(200, AfterPayCurrencies.AustralianDollars), Quantity = 1, Sku = "20000332" }
					}
				},
				new AfterPayCallContext() { OperatorId = "Randal Graves" }
			);

			#endregion

			var result =  await client.RefundOrder
			(
				new AfterPayCreateRefundRequest()
				{
					Amount = new AfterPayMoney(200, AfterPayCurrencies.AustralianDollars),
					MerchantReference = System.Guid.NewGuid().ToString(),
					OrderId = order.OrderId,
					OrderMerchantReference = order.MerchantReference,
				},
				new AfterPayCallContext() { OperatorId = "Randal Graves" }
			);

			Assert.IsNotNull(result);
			Assert.IsNotNull(result.RefundId);
		}

		[TestMethod]
		[TestCategory("Integration")]
		[TestCategory("Manual-Integration")]
		[Ignore("Requires a pre-approval code be manually inserted into the test")]
		public async Task CreateOrder_CanReverseRefund()
		{
			//You must provide a current preapproval code here to run this test.
			string preapprovalCode = String.Empty; //TODO: To run this test you must edit this with a valid pre-approval code

			#region Test Setup - create an order and refund

			var config = new AfterPayConfiguration()
			{
				Environment = AfterPayEnvironment.Sandbox,
				DeviceId = AfterPayTestSecrets.DeviceId,
				DeviceKey = AfterPayTestSecrets.DeviceKey,
				ProductName = "YortAfterPayInstoreLib",
				ProductVersion = typeof(AfterPayClient).Assembly.GetName().Version.ToString()
			};

			var client = new AfterPayClient(config);
			var order = await client.CreateOrder
			(
				new AfterPayCreateOrderRequest()
				{
					Amount = new AfterPayMoney(50, AfterPayCurrencies.AustralianDollars),
					MerchantReference = System.Guid.NewGuid().ToString(),
					PreapprovalCode = preapprovalCode,
					OrderItems = new AfterPayOrderItem[]
					{
						new AfterPayOrderItem() { Name = "Navy Check Jacket", Price = new AfterPayMoney(200, AfterPayCurrencies.AustralianDollars), Quantity = 1, Sku = "20000332" }
					}
				},
				new AfterPayCallContext() { OperatorId = "Randal Graves" }
			);

			var refund = await client.RefundOrder
			(
				new AfterPayCreateRefundRequest()
				{
					Amount = new AfterPayMoney(200, AfterPayCurrencies.AustralianDollars),
					MerchantReference = System.Guid.NewGuid().ToString(),
					OrderId = order.OrderId,
					OrderMerchantReference = order.MerchantReference,
				},
				new AfterPayCallContext() { OperatorId = "Randal Graves" }
			);

			#endregion

			var result = await client.ReverseRefund
			(
				new AfterPayReverseRefundRequest()
				{
					ReversingRequestId = refund.RequestId
				},
				new AfterPayCallContext() { OperatorId = "Randal Graves" }
			);

			Assert.IsNotNull(result);
			Assert.IsNotNull(result.ReverseId);
		}

	}
}
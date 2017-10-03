using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Yort.AfterPay.InStore.Tests
{
	[TestClass]
	public class AfterPayClientErrrorHandlingTests
	{

		#region Response Constants

		private const string Response_Token = "{ \"token\":\"86bc8b2b973d5046d82432862254dfb5967a6db7c844bf615ab8eba17082105b\", \"expiresIn\":14400 }";

		//This is just a sample order response, it won't actually match
		//the requests sent for tests in detail, but that isn't important for these tests.
		private const string Response_Order = @"{
    ""requestId"":""61cdad2d-8e10-42ec-a97b-8712dd7a8ca9"",
    ""requestedAt"":""2015-07-14T10:08:14.123Z"",
    ""preApprovalCode"":""12FA1G3C2E9D"",
    ""amount"": {
        ""amount"": ""15.00"",
        ""currency"": ""AUD""

		},
    ""merchantReference"": ""123987"",
    ""orderItems"": [
        {
            ""name"": ""widget"",
            ""sku"": ""123412234"",
            ""quantity"": 1,
            ""price"": {
                ""amount"": ""10.00"",
                ""currency"": ""AUD""

						}
        },
        {
            ""name"": ""blob"",
            ""sku"": ""123324u4"",
            ""quantity"": 1,
            ""price"": {
                ""amount"": ""5.00"",
                ""currency"": ""AUD""
            }
        }
     ]
  }";

		#endregion

		[TestMethod]
		public async Task Client_ErrorHandling_RetriesOnConflict()
		{

			#region Test Setup

			int callCount = 0;

			var mockHandler = new Yort.Http.ClientPipeline.MockMessageHandler();
			mockHandler.AddDynamicResponse
			(
				new Http.ClientPipeline.MockResponseHandler()
				{
					CanHandleRequest = (request) => request.RequestUri.ToString() == AfterPayConstants.SandboxRootUrl + "/v1/devices/123/token" && request.Method == System.Net.Http.HttpMethod.Post,
					HandleRequest = (request) =>
					{
						return Task.FromResult(new System.Net.Http.HttpResponseMessage(System.Net.HttpStatusCode.OK) { Content = new System.Net.Http.StringContent(Response_Token) });
					}
				}
			);

			mockHandler.AddDynamicResponse(new Http.ClientPipeline.MockResponseHandler()
			{
				CanHandleRequest = (request) => request.RequestUri.ToString() == AfterPayConstants.SandboxRootUrl + "/v1/orders" && request.Method == System.Net.Http.HttpMethod.Post,
				HandleRequest = (request) =>
				{
					callCount++;
					if (callCount == 3)
						return Task.FromResult(new System.Net.Http.HttpResponseMessage(System.Net.HttpStatusCode.OK) { Content = new StringContent(Response_Order) });
					else
						return Task.FromResult(new System.Net.Http.HttpResponseMessage(System.Net.HttpStatusCode.Conflict));
				}
			});

			var httpClient = new System.Net.Http.HttpClient(mockHandler);

			#endregion

			var config = new AfterPayConfiguration()
			{
				HttpClient = httpClient,
				Environment = AfterPayEnvironment.Sandbox,
				DeviceId = "123",
				DeviceKey = "ABC"
			};

			var client = new AfterPayClient(config);

			var result = await client.CreateOrder
			(
				new AfterPayCreateOrderRequest()
				{
					Amount = new AfterPayMoney(200, AfterPayCurrencies.AustralianDollars),
					PreapprovalCode = "ABCDEFGHIJKLMNOP",
					MerchantReference = "SSDS2",
					OrderItems = new AfterPayOrderItem[]
					{
					new AfterPayOrderItem() { Name = "Navy Check Jacket", Quantity = 1, Sku = "20000332", Price = new AfterPayMoney(200, AfterPayCurrencies.AustralianDollars) }
					}
				},
				new AfterPayCallContext() { OperatorId = "Randal Graves" }
			);

			Assert.AreEqual(3, callCount);
			Assert.IsNotNull(result);
		}

		[TestMethod]
		[ExpectedException(typeof(System.TimeoutException))]
		public async Task Client_ErrorHandling_ThrowsTimeoutExceptionAfterLastAttemptTimesOut()
		{

			#region Test Setup

			int callCount = 0;

			var mockHandler = new Yort.Http.ClientPipeline.MockMessageHandler();

			mockHandler.AddDynamicResponse
			(
				new Http.ClientPipeline.MockResponseHandler()
				{
					CanHandleRequest = (request) => request.RequestUri.ToString() == AfterPayConstants.SandboxRootUrl + "/v1/devices/123/token" && request.Method == System.Net.Http.HttpMethod.Post,
					HandleRequest = (request) =>
					{
						return Task.FromResult(new System.Net.Http.HttpResponseMessage(System.Net.HttpStatusCode.OK) { Content = new System.Net.Http.StringContent(Response_Token) });
					}
				}
			);

			mockHandler.AddDynamicResponse(new Http.ClientPipeline.MockResponseHandler()
			{
				CanHandleRequest = (request) => request.RequestUri.ToString() == AfterPayConstants.SandboxRootUrl + "/v1/orders" && request.Method == System.Net.Http.HttpMethod.Post,
				HandleRequest = (request) =>
				{
					callCount++;
					return Task.FromResult(new System.Net.Http.HttpResponseMessage(System.Net.HttpStatusCode.ServiceUnavailable));
				}
			});

			var httpClient = new System.Net.Http.HttpClient(new DelayingTimeoutHandler(90000, mockHandler, new string[] { AfterPayConstants.SandboxRootUrl + "/v1/devices/123/token" }));

			#endregion

			var config = new AfterPayConfiguration()
			{
				HttpClient = httpClient,
				Environment = AfterPayEnvironment.Sandbox,
				DeviceId = "123",
				DeviceKey = "ABC",
				MinimumRetries = 2
			};

			var client = new AfterPayClient(config);

			try
			{
				var result = await client.CreateOrder
				(
					new AfterPayCreateOrderRequest()
					{
						Amount = new AfterPayMoney(200, AfterPayCurrencies.AustralianDollars),
						PreapprovalCode = "ABCDEFGHIJKLMNOP",
						MerchantReference = "SSDS2",
						OrderItems = new AfterPayOrderItem[]
						{
						new AfterPayOrderItem() { Name = "Navy Check Jacket", Quantity = 1, Sku = "20000332", Price = new AfterPayMoney(200, AfterPayCurrencies.AustralianDollars) }
						}
					},
					new AfterPayCallContext() { OperatorId = "Randal Graves" }
				);
			}
			catch (System.TimeoutException)
			{
				Assert.IsTrue(callCount >= config.MinimumRetries);
				throw;
			}

			Assert.Fail("Expected System.TimeoutException");
		}

		[TestMethod]
		[ExpectedException(typeof(System.TimeoutException))]
		public async Task Client_ErrorHandling_RetriesAtLeastMinTimesWithHighMinRetries()
		{

			#region Test Setup

			int callCount = 0;

			var mockHandler = new Yort.Http.ClientPipeline.MockMessageHandler();

			mockHandler.AddDynamicResponse
			(
				new Http.ClientPipeline.MockResponseHandler()
				{
					CanHandleRequest = (request) => request.RequestUri.ToString() == AfterPayConstants.SandboxRootUrl + "/v1/devices/123/token" && request.Method == System.Net.Http.HttpMethod.Post,
					HandleRequest = (request) =>
					{
						return Task.FromResult(new System.Net.Http.HttpResponseMessage(System.Net.HttpStatusCode.OK) { Content = new System.Net.Http.StringContent(Response_Token) });
					}
				}
			);

			mockHandler.AddDynamicResponse(new Http.ClientPipeline.MockResponseHandler()
			{
				CanHandleRequest = (request) => request.RequestUri.ToString() == AfterPayConstants.SandboxRootUrl + "/v1/orders" && request.Method == System.Net.Http.HttpMethod.Post,
				HandleRequest = (request) =>
				{
					callCount++;
					return Task.FromResult(new System.Net.Http.HttpResponseMessage(System.Net.HttpStatusCode.ServiceUnavailable));
				}
			});

			var httpClient = new System.Net.Http.HttpClient(new DelayingTimeoutHandler(90000, mockHandler, new string[] { AfterPayConstants.SandboxRootUrl + "/v1/devices/123/token" }));

			#endregion

			var config = new AfterPayConfiguration()
			{
				HttpClient = httpClient,
				Environment = AfterPayEnvironment.Sandbox,
				DeviceId = "123",
				DeviceKey = "ABC",
				MinimumRetries = 4
			};

			var client = new AfterPayClient(config);

			try
			{
				var result = await client.CreateOrder
				(
					new AfterPayCreateOrderRequest()
					{
						Amount = new AfterPayMoney(200, AfterPayCurrencies.AustralianDollars),
						PreapprovalCode = "ABCDEFGHIJKLMNOP",
						MerchantReference = "SSDS2",
						OrderItems = new AfterPayOrderItem[]
						{
						new AfterPayOrderItem() { Name = "Navy Check Jacket", Quantity = 1, Sku = "20000332", Price = new AfterPayMoney(200, AfterPayCurrencies.AustralianDollars) }
						}
					},
					new AfterPayCallContext() { OperatorId = "Randal Graves" }
				);
			}
			catch (System.TimeoutException)
			{
				Assert.IsTrue(callCount >= 4);
				throw;
			}

			Assert.Fail("Expected System.TimeoutException");
		}

		[TestMethod]
		[ExpectedException(typeof(System.TimeoutException))]
		public async Task Client_ErrorHandling_RetriesUntilMinTimeoutWithLowRetries()
		{

			#region Tet Setup

			int callCount = 0;

			var mockHandler = new Yort.Http.ClientPipeline.MockMessageHandler();

			mockHandler.AddDynamicResponse
			(
				new Http.ClientPipeline.MockResponseHandler()
				{
					CanHandleRequest = (request) => request.RequestUri.ToString() == AfterPayConstants.SandboxRootUrl + "/v1/devices/123/token" && request.Method == System.Net.Http.HttpMethod.Post,
					HandleRequest = (request) =>
					{
						return Task.FromResult(new System.Net.Http.HttpResponseMessage(System.Net.HttpStatusCode.OK) { Content = new System.Net.Http.StringContent(Response_Token) });
					}
				}
			);

			mockHandler.AddDynamicResponse(new Http.ClientPipeline.MockResponseHandler()
			{
				CanHandleRequest = (request) => request.RequestUri.ToString() == AfterPayConstants.SandboxRootUrl + "/v1/orders" && request.Method == System.Net.Http.HttpMethod.Post,
				HandleRequest = (request) =>
				{
					callCount++;
					return Task.FromResult(new System.Net.Http.HttpResponseMessage(System.Net.HttpStatusCode.ServiceUnavailable));
				}
			});

			var httpClient = new System.Net.Http.HttpClient(new DelayingTimeoutHandler(90000, mockHandler, new string[] { AfterPayConstants.SandboxRootUrl + "/v1/devices/123/token" }));

			#endregion

			var config = new AfterPayConfiguration()
			{
				HttpClient = httpClient,
				Environment = AfterPayEnvironment.Sandbox,
				DeviceId = "123",
				DeviceKey = "ABC",
				MinimumRetries = 1
			};

			var client = new AfterPayClient(config);

			try
			{
				var result = await client.CreateOrder
				(
					new AfterPayCreateOrderRequest()
					{
						Amount = new AfterPayMoney(200, AfterPayCurrencies.AustralianDollars),
						PreapprovalCode = "ABCDEFGHIJKLMNOP",
						MerchantReference = "SSDS2",
						OrderItems = new AfterPayOrderItem[]
						{
						new AfterPayOrderItem() { Name = "Navy Check Jacket", Quantity = 1, Sku = "20000332", Price = new AfterPayMoney(200, AfterPayCurrencies.AustralianDollars) }
						}
					},
					new AfterPayCallContext() { OperatorId = "Randal Graves" }
				);
			}
			catch (System.TimeoutException)
			{
				Assert.IsTrue(callCount > 1);
				throw;
			}

			Assert.Fail("Expected System.TimeoutException");
		}

	}

	internal class DelayingTimeoutHandler : DelegatingHandler
	{
		private int _DelayMilliseconds;
		private IEnumerable<string> _IgnoredUrls;

		public DelayingTimeoutHandler(int delayMilliseconds, HttpMessageHandler innerHandler, IEnumerable<string> ignoredUrls) : base(innerHandler)
		{
			_DelayMilliseconds = delayMilliseconds;
			_IgnoredUrls = ignoredUrls;
		}

		protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{
			var response =  await base.SendAsync(request, cancellationToken);
			if (_DelayMilliseconds > 0 && (_IgnoredUrls == null || !_IgnoredUrls.Contains(request.RequestUri.ToString(), StringComparer.OrdinalIgnoreCase)))
				await Task.Delay(_DelayMilliseconds, cancellationToken);

			return response;
		}
	}


}
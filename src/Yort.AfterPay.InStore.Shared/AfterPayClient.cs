using Ladon;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Headers;

namespace Yort.AfterPay.InStore
{
	/// <summary>
	/// The main class used to access the AfterPay API, holding relevant configuration and providing methods for each REST API end point.
	/// </summary>
	/// <remarks>
	/// <para>Disposing objects of this type will dispose the internal <see cref="HttpClient"/> only if it was created internally. 
	/// If the <see cref="HttpClient"/> was provided by <see cref="AfterPayConfiguration.HttpClient"/> then it will not be disposed as it may have been shared with other <see cref="AfterPayClient"/> instances.</para>
	/// </remarks>
	public sealed class AfterPayClient : Yort.Trashy.DisposableManagedOnlyBase, IAfterPayClient
	{

		#region Fields

		private readonly AfterPayConfiguration _Configuration;

		private readonly HttpClient _HttpClient;
		private Uri _BaseUrl;
		private AfterPayToken _Token;

		#endregion

		#region Public API

		/// <summary>
		/// Constructs a new instance using the configuration provided.
		/// </summary>
		/// <param name="configuration">An instance of <see cref="AfterPayConfiguration"/> that contains configuration details for this client.</param>
		/// <exception cref="System.ArgumentNullException">Thrown if <paramref name="configuration"/> is null.</exception>
		public AfterPayClient(AfterPayConfiguration configuration)
		{
			_Configuration = configuration.GuardNull(nameof(configuration));
			_Configuration.LockProperties();

			//Not set on HttpClient as the client might be shared between environments (it shouldn't be, but trust no one).
			_BaseUrl = new Uri((_Configuration.Environment == AfterPayEnvironment.Production ? AfterPayConstants.ProductionRootUrl : AfterPayConstants.SandboxRootUrl) + "/v1/");

			ConfigureServicePoint();

			_HttpClient = ConfigureHttpClient(configuration.HttpClient ?? CreateDefaultHttpClient());

		}

		/// <summary>
		/// Performs one time registration of a new point of sale device (API client) with the AfterPay API.
		/// </summary>
		/// <param name="request">A <see cref="AfterPayDeviceRegistrationRequest"/> containing details of the device to register.</param>
		/// <returns>A <see cref="AfterPayDeviceRegistration"/> containing details returned by AfterPay for a successful registration.</returns>
		/// <exception cref="AfterPayApiException">Thrown if the request is rejected by the AfterPay API.</exception>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="request"/> is null.</exception>
		/// <exception cref="InvalidOperationException">Thrown if this client has already registered a device, or was provided with non-null <see cref="AfterPayConfiguration.DeviceId"/> and <see cref="AfterPayConfiguration.DeviceKey"/> values.</exception>
		public async Task<AfterPayDeviceRegistration> RegisterDevice(AfterPayDeviceRegistrationRequest request)
		{
			request.GuardNull(nameof(request));
			if (!String.IsNullOrWhiteSpace(_Configuration.DeviceId) || !String.IsNullOrWhiteSpace(_Configuration.DeviceKey)) throw new InvalidOperationException(ErrorMessageResources.ClientAlreadyAssociatedToDevice);

			using (var busyToken = ObtainBusyToken())
			{
				var httpRequest = CreateHttpRequest<AfterPayDeviceRegistrationRequest>(HttpMethod.Post, new Uri(_BaseUrl, "devices/activate"), request, null);

				var retVal = await GetResponse<AfterPayDeviceRegistration>(httpRequest, 30).ConfigureAwait(false);

				_Configuration.DeviceRegistered(retVal);

				return retVal;
			}
		}

		/// <summary>
		/// Sends a ping request to the AfterPay API to confirm a connection can be made.
		/// </summary>
		public async Task<bool> Ping()
		{
			using (var busyToken = ObtainBusyToken())
			{
				var httpRequest = CreateHttpRequest<AfterPayInviteRequest>(HttpMethod.Get, new Uri(_BaseUrl, "/ping"), null, null);
				return await GetResponse<bool>(httpRequest, 60).ConfigureAwait(false);
			}
		}

		/// <summary>
		/// Sends an invitational SMS message to a customer's mobile phone.
		/// </summary>
		/// <param name="request">A <see cref="AfterPayInviteRequest"/> instance containing details of the invite to send.</param>
		/// <param name="requestContext">A <see cref="AfterPayCallContext"/> instance describing additional information for the request.</param>
		/// <returns>True if the invite was sent successfully, otherwise false.</returns>
		/// <exception cref="AfterPayApiException">Thrown if the request is rejected by the AfterPay API.</exception>
		/// <exception cref="System.ArgumentNullException">Thrown if <paramref name="request"/> or <paramref name="requestContext"/> is null.</exception>
		/// <exception cref="System.UnauthorizedAccessException">Thrown if the system cannot obtain an authorisation token from AfterPay before making the request.</exception>
		public async Task<bool> SendInvite(AfterPayInviteRequest request, AfterPayCallContext requestContext)
		{
			request.GuardNull(nameof(request));
			requestContext.GuardNull(nameof(requestContext));

			using (var busyToken = ObtainBusyToken())
			{
				await EnsureToken().ConfigureAwait(false);

				var httpRequest = CreateHttpRequest<AfterPayInviteRequest>(HttpMethod.Post, new Uri(_BaseUrl, "preapprovals/enquire"), request, requestContext);
				return await GetResponse<bool>(httpRequest, 30).ConfigureAwait(false);
			}
		}

		/// <summary>
		/// Requests information about a pre-approval code generated by a customer.
		/// </summary>
		/// <param name="request">A <see cref="AfterPayPreapprovalRequest"/> containing details of the pre-approval code to enquire about.</param>
		/// <param name="requestContext">A <see cref="AfterPayCallContext"/> instance containing additional details required to make the request.</param>
		/// <returns>A <see cref="AfterPayPreapprovalResponse"/> instance containing information about the pre-approval code specified.</returns>
		/// <exception cref="AfterPayApiException">Thrown if the request is rejected by the AfterPay API.</exception>
		/// <exception cref="System.ArgumentNullException">Thrown if <paramref name="request"/> or <paramref name="requestContext"/> is null.</exception>
		/// <exception cref="System.UnauthorizedAccessException">Thrown if the system cannot obtain an authorisation token from AfterPay before making the request.</exception>
		public async Task<AfterPayPreapprovalResponse> PreapprovalEnquiry(AfterPayPreapprovalRequest request, AfterPayCallContext requestContext)
		{
			request.GuardNull(nameof(request));
			requestContext.GuardNull(nameof(requestContext));

			using (var busyToken = ObtainBusyToken())
			{
				await EnsureToken().ConfigureAwait(false);

				var httpRequest = CreateHttpRequest<AfterPayPreapprovalRequest>(HttpMethod.Post, new Uri(_BaseUrl, "preapprovals/enquire"), request, requestContext);
				return await GetResponse<AfterPayPreapprovalResponse>(httpRequest, 30).ConfigureAwait(false);
			}
		}

		/// <summary>
		/// Creates a new AfterPay order, which is basically a request for payment using a pre-approval code.
		/// </summary>
		/// <remarks>
		/// <para>This method will keep retrying until success, or a non-409 response error is received. If an exception of any type other than <see cref="AfterPayApiException"/>, <see cref="UnauthorizedAccessException"/>, <see cref="ArgumentNullException"/> is thrown by this method, a reversal should be queued.</para>
		/// <para>This method will automatically retry on timeout up to <see cref="AfterPayConfiguration.MaximumRetries"/>. If the last retry times out, a <see cref="TimeoutException"/> will be thrown. On a 409 response it will retry until any other error or response is received.</para>
		/// </remarks>
		/// <seealso cref="AfterPayConfiguration.MaximumRetries"/>
		/// <seealso cref="AfterPayConfiguration.RetryDelaySeconds"/>
		/// <param name="request">A <see cref="AfterPayCreateOrderRequest"/> containing details of the order to be created.</param>
		/// <param name="requestContext">A <see cref="AfterPayCallContext"/> instance containing additional details required to make the request.</param>
		/// <returns>An <see cref="AfterPayOrder"/> created within the AfterPay system.</returns>
		/// <exception cref="AfterPayApiException">Thrown if the request is rejected by the AfterPay API.</exception>
		/// <exception cref="System.ArgumentNullException">Thrown if <paramref name="request"/> or <paramref name="requestContext"/> is null.</exception>
		/// <exception cref="System.UnauthorizedAccessException">Thrown if the system cannot obtain an authorisation token from AfterPay before making the request.</exception>
		/// <exception cref="System.TimeoutException">Thrown if the request times out on the last retry attempt.</exception>
		public async Task<AfterPayOrder> CreateOrder(AfterPayCreateOrderRequest request, AfterPayCallContext requestContext)
		{
			request.GuardNull(nameof(request));
			requestContext.GuardNull(nameof(requestContext));

			using (var busyToken = ObtainBusyToken())
			{
				return await ExecuteWithRetries(RetryableCreateOrder, request, requestContext, 80).ConfigureAwait(false);
			}
		}

		/// <summary>
		/// Requests an order previously placed be reversed.
		/// </summary>
		/// <remarks>
		/// <para>This method is intended to ensure an order is cancelled when it's status is/was unknown at the time the customer was present. It should not be used for refunds, see <see cref="RefundOrder"/>.</para>
		/// <para>This method will keep retrying until success, or a non-409 response error is received. If an exception of any type other than <see cref="AfterPayApiException"/>, <see cref="UnauthorizedAccessException"/>, <see cref="ArgumentNullException"/> is thrown by this method, a reversal should be retried later.</para>
		/// <para>This method will automatically retry on timeout up to <see cref="AfterPayConfiguration.MaximumRetries"/>. If the last retry times out, a <see cref="TimeoutException"/> will be thrown. On a 409 response it will retry until any other error or response is received.</para>
		/// </remarks>
		/// <param name="request">A <see cref="AfterPayReverseOrderRequest"/> containing details of the order to be reversed.</param>
		/// <param name="requestContext">A <see cref="AfterPayCallContext"/> instance containing additional details required to make the request.</param>
		/// <returns>An <see cref="AfterPayOrderReversal"/> containing details of the reversed order within the AfterPay system.</returns>
		/// <exception cref="AfterPayApiException">Thrown if the request is rejected by the AfterPay API.</exception>
		/// <exception cref="System.ArgumentNullException">Thrown if <paramref name="request"/> or <paramref name="requestContext"/> is null.</exception>
		/// <exception cref="System.UnauthorizedAccessException">Thrown if the system cannot obtain an authorisation token from AfterPay before making the request.</exception>
		/// <exception cref="System.TimeoutException">Thrown if the request times out on the last retry attempt.</exception>
		public async Task<AfterPayOrderReversal> ReverseOrder(AfterPayReverseOrderRequest request, AfterPayCallContext requestContext)
		{
			request.GuardNull(nameof(request));
			requestContext.GuardNull(nameof(requestContext));

			using (var busyToken = ObtainBusyToken())
			{
				return await ExecuteWithRetries(RetryableReverseOrder, request, requestContext, 30).ConfigureAwait(false);
			}
		}

		/// <summary>
		/// Creates a refund against a previously created order.
		/// </summary>
		/// <remarks>
		/// <para>This method will keep retrying until success, or a non-409 response error is received. If an exception of any type other than <see cref="AfterPayApiException"/>, <see cref="UnauthorizedAccessException"/>, <see cref="ArgumentNullException"/> is thrown by this method, a reversal should be queued.</para>
		/// <para>This method will automatically retry on timeout up to <see cref="AfterPayConfiguration.MaximumRetries"/>. If the last retry times out, a <see cref="TimeoutException"/> will be thrown. On a 409 response it will retry until any other error or response is received.</para>
		/// </remarks>
		/// <param name="request">A <see cref="AfterPayRefundRequest"/> containing details of the refund to create.</param>
		/// <param name="requestContext">A <see cref="AfterPayCallContext"/> instance containing additional details required to make the request.</param>
		/// <returns>An <see cref="AfterPayRefund"/> containing details of the refund created within the AfterPay system.</returns>
		/// <exception cref="AfterPayApiException">Thrown if the request is rejected by the AfterPay API.</exception>
		/// <exception cref="System.ArgumentNullException">Thrown if <paramref name="request"/> or <paramref name="requestContext"/> is null.</exception>
		/// <exception cref="System.UnauthorizedAccessException">Thrown if the system cannot obtain an authorisation token from AfterPay before making the request.</exception>
		/// <exception cref="System.TimeoutException">Thrown if the request times out on the last retry attempt.</exception>
		public async Task<AfterPayRefund> RefundOrder(AfterPayRefundRequest request, AfterPayCallContext requestContext)
		{
			request.GuardNull(nameof(request));
			requestContext.GuardNull(nameof(requestContext));

			using (var busyToken = ObtainBusyToken())
			{
				return await ExecuteWithRetries(RetryableRefundOrder, request, requestContext, 30).ConfigureAwait(false);
			}
		}

		/// <summary>
		/// Creates a refund reversal against a previously created refund.
		/// </summary>
		/// <remarks>
		/// <para>This method is intended to ensure a refuund is cancelled when it's status is/was unknown at the time the customer was present. For more information see https://docs.afterpay.com.au/instore-api-v1.html#refund-reversal </para>
		/// <para>This method will keep retrying until success, or a non-409 response error is received. If an exception of any type other than <see cref="AfterPayApiException"/>, <see cref="UnauthorizedAccessException"/>, <see cref="ArgumentNullException"/> is thrown by this method, a reversal should be retried later.</para>
		/// <para>This method will automatically retry on timeout up to <see cref="AfterPayConfiguration.MaximumRetries"/>. If the last retry times out, a <see cref="TimeoutException"/> will be thrown. On a 409 response it will retry until any other error or response is received.</para>
		/// </remarks>
		/// <param name="request">A <see cref="AfterPayReverseRefundRequest"/> containing details of the refund reversal to create.</param>
		/// <param name="requestContext">A <see cref="AfterPayCallContext"/> instance containing additional details required to make the request.</param>
		/// <returns>An <see cref="AfterPayRefundReversal"/> containing details of the refund reversal created within the AfterPay system.</returns>
		/// <exception cref="AfterPayApiException">Thrown if the request is rejected by the AfterPay API.</exception>
		/// <exception cref="System.ArgumentNullException">Thrown if <paramref name="request"/> or <paramref name="requestContext"/> is null.</exception>
		/// <exception cref="System.UnauthorizedAccessException">Thrown if the system cannot obtain an authorisation token from AfterPay before making the request.</exception>
		/// <exception cref="System.TimeoutException">Thrown if the request times out on the last retry attempt.</exception>
		public async Task<AfterPayRefundReversal> ReverseRefund(AfterPayReverseRefundRequest request, AfterPayCallContext requestContext)
		{
			request.GuardNull(nameof(request));
			requestContext.GuardNull(nameof(requestContext));

			using (var busyToken = ObtainBusyToken())
			{
				return await ExecuteWithRetries(RetryableReverseRefund, request, requestContext, 30).ConfigureAwait(false);
			}
		}

		#endregion

		#region Implementation Details

		private async Task<TResult> ExecuteWithRetries<TRequest, TResult>(Func<TRequest, AfterPayCallContext, int, Task<TResult>> retryableOperation, TRequest request, AfterPayCallContext callContext, int timeoutSeconds)
		{
			var retries = 0;
			while (retries < _Configuration.MaximumRetries)
			{
				try
				{
					return await retryableOperation(request, callContext, timeoutSeconds).ConfigureAwait(false);
				}
				catch (TaskCanceledException)
				{
					retries++;
				}
				catch (AfterPayApiException apex)
				{
					//409 - Conflict, indicates a prior request is in progress and we should retry.
					//Anything else is a failure (https://docs.afterpay.com.au/instore-api-v1.html#distributed-state-considerations).
					if (apex.HttpStatusCode != 409) throw;
#if SUPPORTS_TASKDELAY
					await Task.Delay(Math.Max(5, _Configuration.RetryDelaySeconds) * 1000);
#else
					await TaskEx.Delay(Math.Max(5, _Configuration.RetryDelaySeconds) * 1000);
#endif
				}
				catch
				{
					throw;
				}
			}

			throw new TimeoutException(ErrorMessageResources.RequestTimeout);
		}

		private async Task<AfterPayOrder> RetryableCreateOrder(AfterPayCreateOrderRequest request, AfterPayCallContext requestContext, int timeoutSeconds)
		{
			var httpRequest = CreateHttpRequest(HttpMethod.Post, new Uri(_BaseUrl, "orders"), request, requestContext);

			await EnsureToken().ConfigureAwait(false);

			return await GetResponse<AfterPayOrder>(httpRequest, timeoutSeconds).ConfigureAwait(false);
		}

		private async Task<AfterPayOrderReversal> RetryableReverseOrder(AfterPayReverseOrderRequest request, AfterPayCallContext requestContext, int timeoutSeconds)
		{
			await EnsureToken().ConfigureAwait(false);

			var httpRequest = CreateHttpRequest<AfterPayReverseOrderRequest>(HttpMethod.Post, new Uri(_BaseUrl, "orders/reverse"), request, requestContext);
			return await GetResponse<AfterPayOrderReversal>(httpRequest, timeoutSeconds).ConfigureAwait(false);
		}

		private async Task<AfterPayRefund> RetryableRefundOrder(AfterPayRefundRequest request, AfterPayCallContext requestContext, int timeoutSeconds)
		{
			await EnsureToken().ConfigureAwait(false);

			var httpRequest = CreateHttpRequest<AfterPayRefundRequest>(HttpMethod.Post, new Uri(_BaseUrl, "refunds"), request, requestContext);
			return await GetResponse<AfterPayRefund>(httpRequest, 30).ConfigureAwait(false);
		}

		private async Task<AfterPayRefundReversal> RetryableReverseRefund(AfterPayReverseRefundRequest request, AfterPayCallContext requestContext, int timeoutSeconds)
		{
			await EnsureToken().ConfigureAwait(false);

			var httpRequest = CreateHttpRequest<AfterPayReverseRefundRequest>(HttpMethod.Post, new Uri(_BaseUrl, "refunds/reverse"), request, requestContext);
			return await GetResponse<AfterPayRefundReversal>(httpRequest, 30).ConfigureAwait(false);
		}

		private async Task<AfterPayToken> EnsureToken()
		{
			if (_Token?.IsExpired() ?? true)
				_Token = await CreateToken().ConfigureAwait(false);

			return _Token;
		}

		private async Task<AfterPayToken> CreateToken()
		{
			try
			{
				var request = CreateHttpRequest<AfterPayTokenRequest>(HttpMethod.Post, new Uri(_BaseUrl, $"devices/{_Configuration.DeviceId}/token"), new AfterPayTokenRequest() { Key = _Configuration.DeviceKey }, null);
				return await GetResponse<AfterPayToken>(request, 30).ConfigureAwait(false);
			}
			catch (Exception ex)
			{
				throw new UnauthorizedAccessException(ErrorMessageResources.CouldNotObtainToken, ex);
			}
		}

		private async Task<T> GetResponse<T>(HttpRequestMessage request, int timeoutSeconds)
		{
			HttpResponseMessage response = null;
			using (var tcs = new System.Threading.CancellationTokenSource())
			{
				tcs.CancelAfter(timeoutSeconds * 1000);
				response = await _HttpClient.SendAsync(request, tcs.Token).ConfigureAwait(false);
			}

			//TODO: Fix Ugly boxing. For now ok because this won't happen often, because we won't usually be using these return types.
			if (typeof(T) == typeof(bool))
				return (T)((object)response.IsSuccessStatusCode);
			else if (typeof(T) == typeof(HttpResponseMessage))
				return (T)((object)response);

			if (!response.IsSuccessStatusCode)
			{
				if (response.Content == null || response.Content.Headers?.ContentType?.MediaType != AfterPayConstants.JsonMediaType)
					throw new AfterPayApiException(new AfterPayApiError() { HttpStatusCode = (int)response.StatusCode, Message = response.ReasonPhrase });

				var errorDetails = Newtonsoft.Json.JsonConvert.DeserializeObject<AfterPayApiError>(await response.Content.ReadAsStringAsync().ConfigureAwait(false));
				//In case we got json that wasn't a valid error, ensure the minimum details are set
				errorDetails.HttpStatusCode = errorDetails.HttpStatusCode == 0 ? errorDetails.HttpStatusCode : (int)response.StatusCode;
				errorDetails.Message = errorDetails.Message ?? response.ReasonPhrase;

				throw new AfterPayApiException(errorDetails);
			}

			return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(await response.Content.ReadAsStringAsync().ConfigureAwait(false));
		}

		private HttpRequestMessage CreateHttpRequest<T>(HttpMethod method, Uri url, T requestContent, AfterPayCallContext requestContext)
		{
			//TODO: Might be nice to be able to provide a custom serialiser, or
			//have a reusable buffer pool for reducing allocations during serialisation/deserialisation.
			//For now I'm considering this a premature optimisation and not bothering.
			var retVal = new HttpRequestMessage(method, url)
			{
				Content = requestContent == null ? null : new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(requestContent), System.Text.UTF8Encoding.UTF8, AfterPayConstants.JsonMediaType)
			};
			
			if (_Token != null)
				retVal.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _Token.Token);

			if (requestContext != null)
				retVal.Headers.TryAddWithoutValidation("Operator", requestContext.OperatorId);
			
			return retVal;
		}

		private void ConfigureServicePoint()
		{
			try
			{
				var servicePoint = System.Net.ServicePointManager.FindServicePoint(_BaseUrl);
				servicePoint.Expect100Continue = false; //Improves performance by reducing round trips
				servicePoint.UseNagleAlgorithm = true; //Improves latency/performance
			}
			//Ignore any exceptions that might be thrown from poorly/partially implemented Net Standard 2.0.
			catch (PlatformNotSupportedException) { }
			catch (NotImplementedException) { }
			catch (NotSupportedException) { }

			try
			{
				//Required for .Net 4.0/4.5 on older operating systems, such as XP,
				//in order to establish an https connection.
#if SUPPORTS_TLS12
				System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Ssl3 | System.Net.SecurityProtocolType.Tls | System.Net.SecurityProtocolType.Tls11 | System.Net.SecurityProtocolType.Tls12;
#endif
			}
			//Ignore any exceptions that might be thrown from poorly/partially implemented Net Standard 2.0.
			catch (PlatformNotSupportedException) { }
			catch (NotImplementedException) { }
			catch (NotSupportedException) { }
		}

		private static HttpClient CreateDefaultHttpClient()
		{
			var handler = new HttpClientHandler();
			if (handler.SupportsAutomaticDecompression)
				handler.AutomaticDecompression = System.Net.DecompressionMethods.Deflate | System.Net.DecompressionMethods.GZip;

			if (handler.SupportsRedirectConfiguration)
				handler.AllowAutoRedirect = true;

			handler.ClientCertificateOptions = ClientCertificateOption.Manual;
			handler.UseCookies = false;

			handler.UseDefaultCredentials = false;

			return new HttpClient(handler);
		}

		private HttpClient ConfigureHttpClient(HttpClient client)
		{
			client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(AfterPayConstants.JsonMediaType));
			client.DefaultRequestHeaders.UserAgent.Add(GetUserAgent());
			client.Timeout = TimeSpan.FromSeconds(10);

			return client;
		}

		private ProductInfoHeaderValue GetUserAgent()
		{
			var assemblyName = System.Reflection.Assembly.GetAssembly(typeof(AfterPayClient)).GetName();
			return new ProductInfoHeaderValue(
				String.IsNullOrWhiteSpace(_Configuration.ProductName) ? assemblyName.Name : _Configuration.ProductName,
				String.IsNullOrWhiteSpace(_Configuration.ProductVersion) ? assemblyName.Name : _Configuration.ProductVersion
			);
		}

#endregion

#region Overrides

		/// <summary>
		/// Disposes the internal <see cref="HttpClient"/> but only if it was created by this class, will not dispose it if it was passed via <see cref="AfterPayConfiguration.HttpClient"/>.
		/// </summary>
		protected override void DisposeManagedResources()
		{
			try
			{
				if (_Configuration.HttpClient == null)
				{
					_HttpClient?.Dispose();
				}
			}
			finally
			{
				base.DisposeManagedResources();
			}
		}

#endregion

	}
}
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Yort.AfterPay.InStore
{
	/// <summary>
	/// Represents a request to reverse an order that was previously attempted in the AfterPay system.
	/// </summary>
	/// <remarks>
	/// <para>Orders should only be reversed when errors have occurred and the state of the order is unknown. For more information see; https://docs.afterpay.com.au/instore-api-v1.html#idempotent-requests and https://docs.afterpay.com.au/instore-api-v1.html#reverse-order</para>
	/// </remarks>
	/// <seealso cref="AfterPayOrderReversal"/>
	/// <seealso cref="AfterPayCreateOrderRequest"/>
	public class AfterPayReverseOrderRequest
	{
		/// <summary>
		/// The date and time at which the request is being made. Defaults to the now property of <see cref="AfterPayConfiguration.SystemClock"/>.
		/// </summary>
		[JsonProperty("requestedAt")]
		public DateTimeOffset RequestedAt { get; set; } = AfterPayConfiguration.SystemClock.Now;
		/// <summary>
		/// The request ID for the previously submitted order request.
		/// </summary>
		[JsonProperty("reversingRequestId")]
		public string ReversingRequestId { get; set; }
	}
}
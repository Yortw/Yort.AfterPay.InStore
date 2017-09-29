using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Yort.AfterPay.InStore
{
	/// <summary>
	/// Represents a request to reverse a refund previously requested. This should only be done under error conditions where the state of the refund is unknown.
	/// </summary>
	/// <remarks>
	/// <para>For more information see; https://docs.afterpay.com.au/instore-api-v1.html#refund-reversal </para>
	/// </remarks>
	/// <seealso cref="AfterPayRefundReversal"/>
	public class AfterPayReverseRefundRequest
	{
		/// <summary>
		/// The unique id of the request previously sent to create the refund.
		/// </summary>
		[JsonProperty("reversingRequestId")]
		public string ReversingRequestId { get; set; } 
		/// <summary>
		/// The date and time at which this request was created. Defaults to the now property of <see cref="AfterPayConfiguration.SystemClock"/>.
		/// </summary>
		[JsonProperty("requestedAt")]
		public DateTimeOffset RequestedAt { get; set; } = AfterPayConfiguration.SystemClock.Now;
	}
}
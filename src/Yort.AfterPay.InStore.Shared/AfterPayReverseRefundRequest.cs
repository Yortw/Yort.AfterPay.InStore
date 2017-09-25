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
		/// A unique id for this request. Defaults to a new GUID value.
		/// </summary>
		[JsonProperty("reversingRequestId")]
		public string ReversingRequestId { get; set; } = System.Guid.NewGuid().ToString();
		/// <summary>
		/// The date and time at which this request was created. Defaults to the now property of <see cref="AfterPayConfiguration.SystemClock"/>.
		/// </summary>
		[JsonProperty("requestedAt")]
		public DateTimeOffset RequestedAt { get; set; } = AfterPayConfiguration.SystemClock.Now;
	}
}
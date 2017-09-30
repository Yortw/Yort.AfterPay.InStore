using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Yort.AfterPay.InStore
{
	/// <summary>
	/// Represents a response from the AfterPay API to a <see cref="AfterPayReverseOrderRequest"/>.
	/// </summary>
	/// <seealso cref="AfterPayReverseOrderRequest"/>
	public class AfterPayOrderReversal
	{
		/// <summary>
		/// The unique ID of the reversal created within the AfterPay system.
		/// </summary>
		[JsonProperty("reverseId")]
		public string ReverseId { get; set; }

		/// <summary>
		/// Returns the date and time at which the reversal was created.
		/// </summary>
		[JsonProperty("reversedAt")]
		public DateTimeOffset ReversedAt { get; set; }

		/// <summary>
		/// Returns the date and time at which the reversal was requested.
		/// </summary>
		[JsonProperty("requestedAt")]
		public DateTimeOffset RequestedAt { get; set; }

		/// <summary>
		/// The unique ID of the request that created this reversal.
		/// </summary>
		[JsonProperty("reversingRequestId")]
		public string ReversingRequestId { get; set; }

	}
}
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Yort.AfterPay.InStore
{
	/// <summary>
	/// Represents a reversed refund in the AfterPay system.
	/// </summary>
	/// <seealso cref="AfterPayReverseRefundRequest"/>
	public class AfterPayRefundReversal
	{
		/// <summary>
		/// The unique id of the reversal.
		/// </summary>
		[JsonProperty("reverseId")]
		public string ReverseId { get; set; }

		/// <summary>
		/// The date and time at which the reversal was created.
		/// </summary>
		[JsonProperty("reversedAt")]
		public DateTimeOffset ReversedAt { get; set; }
	}
}
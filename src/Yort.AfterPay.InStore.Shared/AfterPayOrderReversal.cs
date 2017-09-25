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
		/// Represents the unique id of the <see cref="AfterPayReverseOrderRequest"/> this response relates to.
		/// </summary>
		[JsonProperty("reverseId")]
		public string ReverseId { get; set; }

		/// <summary>
		/// Returns the date and time at which the reversal was made.
		/// </summary>
		[JsonProperty("reversedAt")]
		public DateTimeOffset ReversedAt { get; set; }
	}
}
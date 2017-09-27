using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Yort.AfterPay.InStore
{
	/// <summary>
	/// Represents the response from AfterPay to a <see cref="AfterPayPreapprovalRequest"/>.
	/// </summary>
	/// <seealso cref="AfterPayPreapprovalRequest"/>
	public class AfterPayPreapprovalResponse
	{
		/// <summary>
		/// The maximum amount that can be charged against this pre-approval.
		/// </summary>
		[JsonProperty("amount")]
		public AfterPayMoney Amount { get; set; }

		/// <summary>
		/// The minimum amount that can be charged against this pre-approval.
		/// </summary>
		[JsonProperty("minimum")]
		public AfterPayMoney Minimum { get; set; }

		/// <summary>
		/// The date and time at which the pre-approval code expires and can no longer be used.
		/// </summary>
		[JsonProperty("expiresAt")]
		public DateTimeOffset ExpiresAt { get; set; }

	}
}
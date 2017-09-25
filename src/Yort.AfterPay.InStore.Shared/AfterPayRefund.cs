using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Yort.AfterPay.InStore
{
	/// <summary>
	/// Represents a refund that has been successfully created with the AfterPay system.
	/// </summary>
	/// <seealso cref="AfterPayCreateRefundRequest"/>
	public class AfterPayRefund
	{
		/// <summary>
		/// The unique id of the refund created within the AfterPay system.
		/// </summary>
		[JsonProperty("refundId")]
		public string RefundId { get; set; }

		/// <summary>
		/// The date and time at which the refund was created within the AfterPay system.
		/// </summary>
		[JsonProperty("refundedAt")]
		public DateTimeOffset RefundedAt { get; set; }
	}
}
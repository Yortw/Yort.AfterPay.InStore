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

		/// <summary>
		/// The unique id for the request that created this refund.
		/// </summary>
		[JsonProperty("requestId")]
		public string RequestId { get; set; }
		
		/// <summary>
		/// The date and time the request that created this refund was created.
		/// </summary>
		[JsonProperty("requestedAt")]
		public DateTimeOffset RequestedAt { get; set; } = AfterPayConfiguration.SystemClock.Now;

		/// <summary>
		/// An id linking this refund to an object in the point of sale system, e.g. the POS docket or receipt number the return/refund is being done on.
		/// </summary>
		[JsonProperty("merchantReference")]
		public string MerchantReference { get; set; }

		/// <summary>
		/// The unique AfterPay id for the original order this refund was made against (from <see cref="AfterPayOrder.OrderId"/>.
		/// </summary>
		[JsonProperty("orderId")]
		public string OrderId { get; set; }

		/// <summary>
		/// The merchant reference of the original order the refund was made against.
		/// </summary>
		[JsonProperty("orderMerchantReference")]
		public string OrderMerchantReference { get; set; }

		/// <summary>
		/// The amount of this refund.
		/// </summary>
		[JsonProperty("amount")]
		public AfterPayMoney Amount { get; set; }

	}
}
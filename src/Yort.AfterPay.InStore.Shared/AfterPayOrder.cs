using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Yort.AfterPay.InStore
{
	/// <summary>
	/// Represents an order created within the AfterPay system via a <see cref="AfterPayCreateOrderRequest"/>.
	/// </summary>
	/// <seealso cref="AfterPayCreateOrderRequest"/>
	/// <seealso cref="AfterPayOrderItem"/>
	/// <seealso cref="AfterPayMoney"/>
	public class AfterPayOrder
	{
		/// <summary>
		/// The unique id of this order within the AfterPay system.
		/// </summary>
		[JsonProperty("orderId")]
		public string OrderId { get; set; }
		/// <summary>
		/// The date and time this order was created in the AfterPay system.
		/// </summary>
		[JsonProperty("orderedAt")]
		public DateTimeOffset OrderedAt { get; set; }
		/// <summary>
		/// The unique request id of the <see cref="AfterPayCreateOrderRequest"/> that created this order.
		/// </summary>
		[JsonProperty("requestId")]
		public string RequestId { get; set; }
		/// <summary>
		/// The request date and time from the <see cref="AfterPayCreateOrderRequest"/> that created this order.
		/// </summary>
		[JsonProperty("requestedAt")]
		public DateTimeOffset RequestedAt { get; set; }
		/// <summary>
		/// The pre-approval alphanumeric code obtained from the customer that was used to authorise payment for this order.
		/// </summary>
		[JsonProperty("preApprovalCode")]
		public string PreapprovalCode { get; set; }
		/// <summary>
		/// The payment required for this order as a <see cref="AfterPayMoney"/> instace.
		/// </summary>
		[JsonProperty("amount")]
		public AfterPayMoney Amount { get; set; }
		/// <summary>
		/// The merchant reference provided on the <see cref="AfterPayCreateOrderRequest"/> instance that created this order.
		/// </summary>
		[JsonProperty("merchantReference")]
		public string MerchantReference { get; set; }
		/// <summary>
		/// The items that were included on this order, taken from the original <see cref="AfterPayCreateOrderRequest"/>.
		/// </summary>
		[JsonProperty("orderItems")]
		public IEnumerable<AfterPayOrderItem> OrderItems { get; set; }
	}
}
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Yort.AfterPay.InStore
{
	/// <summary>
	/// Represents a request to make a new AfterPay order (request payment for some items) via the AfterPay REST API.
	/// </summary>
	/// <seealso cref="AfterPayOrder"/>
	/// <seealso cref="AfterPayOrderItem"/>
	/// <seealso cref="AfterPayMoney"/>
	public class AfterPayCreateOrderRequest
	{
		/// <summary>
		/// A unique id for this request. Defaults to a new GUID value.
		/// </summary>
		[JsonProperty("requestId")]
		public string RequestId { get; set; } = System.Guid.NewGuid().ToString();
		/// <summary>
		/// The date and time at which this request was created. Defaults to the now property of <see cref="AfterPayConfiguration.SystemClock"/>.
		/// </summary>
		[JsonProperty("requestedAt")]
		public DateTimeOffset RequestedAt { get; set; } = AfterPayConfiguration.SystemClock.Now;
		/// <summary>
		/// An id linking this order to an object in the point of sale system, e.g. the POS docket or receipt number.
		/// </summary>
		[JsonProperty("merchantReference")]
		public string MerchantReference { get; set; }
		/// <summary>
		/// The pre-approval alphanumeric code obtained from the barcode displayed on the customers mobile.
		/// </summary>
		[JsonProperty("preApprovalCode")]
		public string PreapprovalCode { get; set; }
		/// <summary>
		/// The amount of payment required.
		/// </summary>
		[JsonProperty("amount")]
		public AfterPayMoney Amount { get; set; }
		/// <summary>
		/// A set of <see cref="AfterPayOrderItem"/> instances representing the items being purchased on this order.
		/// </summary>
		public IEnumerable<AfterPayOrderItem> OrderItems { get; set; }
	}
}
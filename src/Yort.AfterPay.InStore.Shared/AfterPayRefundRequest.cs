using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Yort.AfterPay.InStore
{
	/// <summary>
	/// Represents a request to create a refund against an order that was previously created in the AfterPay system.
	/// </summary>
	/// <remarks>
	/// <para>For more information see; https://docs.afterpay.com.au/instore-api-v1.html#create-refund </para>
	/// </remarks>
	public class AfterPayRefundRequest
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
		/// An id linking this refund to an object in the point of sale system, e.g. the POS docket or receipt number.
		/// </summary>
		[JsonProperty("merchantReference")]
		public string MerchantReference { get; set; }
		/// <summary>
		/// The unique id of the original order this refund is against.
		/// </summary>
		[JsonProperty("orderId")]
		public string OrderId { get; set; }
		/// <summary>
		/// The original order’s merchantReference value. If the supplied value does not match the original order an error will be returned.
		/// </summary>
		[JsonProperty("orderMerchantReference")]
		public string OrderMerchantReference { get; set; }
		/// <summary>
		/// The amount of the refund to create. The refund amount can not exceed the outstanding value of the associated order.
		/// </summary>
		[JsonProperty("amount")]
		public AfterPayMoney Amount { get; set; }
	}
}
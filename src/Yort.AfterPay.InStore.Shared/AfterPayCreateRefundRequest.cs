using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Yort.AfterPay.InStore
{
	/// <summary>
	/// Represents a request to refund part or all of a previously placed order.
	/// </summary>
	/// <remarks>
	/// <para>For more information see; https://docs.afterpay.com.au/instore-api-v1.html#create-refund </para>
	/// </remarks>
	/// <seealso cref="AfterPayRefund"/>
	/// <seealso cref="AfterPayOrder"/>
	/// <seealso cref="AfterPayMoney"/>
	public class AfterPayCreateRefundRequest
	{
		/// <summary>
		/// A unique id for this request.  Defaults to a new GUID value.
		/// </summary>
		[JsonProperty("requestId")]
		public string RequestId { get; set; } = System.Guid.NewGuid().ToString();
		/// <summary>
		/// The date and time this request was created.  Defaults to the now property of <see cref="AfterPayConfiguration.SystemClock"/>.
		/// </summary>
		[JsonProperty("requestedAt")]
		public DateTimeOffset RequestedAt { get; set; } = AfterPayConfiguration.SystemClock.Now;
		/// <summary>
		/// An id linking this refund to an object in the point of sale system, e.g. the POS docket or receipt number the return/refund is being done on.
		/// </summary>
		[JsonProperty("merchantReference")]
		public string MerchantReference { get; set; }
		/// <summary>
		/// The unique AfterPay id for the order to refund against (from <see cref="AfterPayOrder.OrderId"/>.
		/// </summary>
		[JsonProperty("orderId")]
		public string OrderId { get; set; }
		/// <summary>
		/// An optional value specifying the merchant reference used to create the original order (from <see cref="AfterPayOrder.MerchantReference"/>).
		/// </summary>
		/// <remarks>
		/// <para>If a non-null value is provided for this property then it must match the merchant reference on the original order or an error will occur. If a null value is provided then no check is performed on the merchant reference.</para>
		/// </remarks>
		[JsonProperty("orderMerchantReference")]
		public string OrderMerchantReference { get; set; }
		/// <summary>
		/// The refund amount. The refund amount can not exceed the outstanding amount of the associated order, or an error will be returned.
		/// </summary>
		[JsonProperty("amount")]
		public AfterPayMoney Amount { get; set; }
	}
}
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Yort.AfterPay.InStore
{
	/// <summary>
	/// Represents an item being sold or returned as part of an AfterPay API transaction.
	/// </summary>
	/// <remarks>
	/// <para>See https://docs.afterpay.com.au/instore-api-v1.html#item-object for more information.</para>
	/// </remarks>
	public class AfterPayOrderItem
	{
		/// <summary>
		/// A brief description or name for the product.
		/// </summary>
		[JsonProperty("name")]
		public string Name { get; set; }
		/// <summary>
		/// The unique stock-keeping-unit code for this product.
		/// </summary>
		[JsonProperty("sku")]
		public string Sku { get; set; }
		/// <summary>
		/// The number of units, volume or amount of this product used.
		/// </summary>
		[JsonProperty("quantity")]
		public decimal Quantity { get; set; }
		/// <summary>
		/// The unit price of a whole unit of this item.
		/// </summary>
		[JsonProperty("price")]
		public AfterPayMoney Price { get; set; }
	}
}
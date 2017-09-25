using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Yort.AfterPay.InStore
{
	/// <summary>
	/// Represents a request to the AfterPay API to invite a customer to join AfterPay and/or create a payment barcode.
	/// </summary>
	public class AfterPayInviteRequest
	{
		/// <summary>
		/// The mobile phone number to send the invite to (as an SMS message).
		/// </summary>
		[JsonProperty("mobile")]
		public string MobileNumber { get; set; }
		/// <summary>
		/// The amount of payment requested from the customer.
		/// </summary>
		[JsonProperty("expectedAmount")]
		public AfterPayMoney ExpectedAmount { get; set; }
	}
}
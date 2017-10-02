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

		/// <summary>
		/// Removes any non-digit characters from <paramref name="phoneNumber"/> and returns a new string containing the result.
		/// </summary>
		/// <remarks>
		/// <para>If <paramref name="phoneNumber"/> is null or empty string, the same value is returned without modification.</para>
		/// </remarks>
		/// <param name="phoneNumber">The value to have non-numeric characters removed.</param>
		/// <returns>A new string containing the value of <paramref name="phoneNumber"/> with all non-numeric characters removed.</returns>
		public static string NormalizePhoneNumber(string phoneNumber)
		{
			if (String.IsNullOrEmpty(phoneNumber)) return phoneNumber;

			var sb = new StringBuilder(phoneNumber.Length);

			foreach (var c in phoneNumber)
			{
				if (Char.IsDigit(c))
					sb.Append(c);
			}

			return sb.ToString();
		}
	}
}
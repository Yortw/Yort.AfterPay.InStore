using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Yort.AfterPay.InStore
{
	/// <summary>
	/// Represents an error response from the AfterPay API.
	/// </summary>
	/// <remarks>
	/// <para>See the AfterPay documentation for more information; https://docs.afterpay.com.au/instore-api-v1.html#errors</para>
	/// </remarks>
	public class AfterPayApiError
	{
		/// <summary>
		/// The HTTP status code that was sent in the response that contained this data.
		/// </summary>
		[JsonProperty("httpStatusCode")]
		public int HttpStatusCode { get; set; }

		/// <summary>
		/// A string containing a unique error ID.
		/// </summary>
		[JsonProperty("errorId")]
		public string ErrorId { get; set; }

		/// <summary>
		/// A string representing the type of error returned, e.g. invalid_object, transaction_error, or server_error.
		/// </summary>
		[JsonProperty("errorCode")]
		public string ErrorCode { get; set; }

		/// <summary>
		/// A string containing a human-readable message giving more details about the error. For card errors, these messages can be shown to your users.
		/// </summary>
		[JsonProperty("message")]
		public string Message { get; set; }
	}
}
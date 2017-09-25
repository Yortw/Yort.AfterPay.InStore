using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Yort.AfterPay.InStore
{
	/// <summary>
	/// Represents a request to perform one-time registration of a device with the AfterPay REST API.
	/// </summary>
	/// <remarks>
	/// <para>For more information on the AfterPay authentication and authorisation system, see; https://docs.afterpay.com.au/instore-api-v1.html#device-authentication </para>
	/// </remarks>
	/// <seealso cref="AfterPayDeviceRegistration"/>
	public class AfterPayDeviceRegistrationRequest
	{
		/// <summary>
		/// The secret value provided by AfterPay for the device to be registered.
		/// </summary>
		[JsonProperty("secret")]
		public string Secret { get; set; }
		/// <summary>
		/// The name of the device to be registered as known to AfterPay.
		/// </summary>
		[JsonProperty("name")]
		public string Name { get; set; }
		/// <summary>
		/// Optional attributes to be included with the registration, or null for none.
		/// </summary>
		/// <remarks>
		/// <para>Use an anonmyous type to set additional properties, i.e</para>
		/// <code>
		/// var request = new AfterPayDeviceRegistrationRequest()
		/// {
		///		Secret = "123",
		///		DeviceName = "MyDevice",
		///		Attributes = new 
		///		{
		///			Branch = "Downtown Sydney",
		///			SerialNumber = "A1B2C3"
		///		}
		/// }
		/// </code>
		/// </remarks>
		[JsonProperty("attributes")]
		public object Attributes { get; set; }
	}
}
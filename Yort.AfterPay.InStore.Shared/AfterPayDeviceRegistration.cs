using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Yort.AfterPay.InStore
{
	/// <summary>
	/// Represents a successful response to a request to register a device <see cref="AfterPayDeviceRegistrationRequest"/>.
	/// </summary>
	/// <seealso cref="AfterPayDeviceRegistrationRequest"/>
	public class AfterPayDeviceRegistration
	{
		/// <summary>
		/// The unique id of the now registered device.
		/// </summary>
		[JsonProperty("deviceId")]
		public string DeviceId { get; set; }
		/// <summary>
		/// The secret key required to obtain an authorisation token for this device.
		/// </summary>
		/// <seealso cref="AfterPayTokenRequest"/>
		[JsonProperty("key")]
		public string Key { get; set; }
	}
}
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Yort.AfterPay.InStore
{
	/// <summary>
	/// Represents a request to the AfterPay API for a new security token used to authorise further requests.
	/// </summary>
	/// <seealso cref="AfterPayToken"/>
	/// <seealso cref="AfterPayDeviceRegistrationRequest"/>
	/// /// <seealso cref="AfterPayDeviceRegistration"/>
	public class AfterPayTokenRequest
	{
		/// <summary>
		/// The secret key previously received when the requesting device was registered.
		/// </summary>
		[JsonProperty("key")]
		public string Key { get; set; }
	}
}
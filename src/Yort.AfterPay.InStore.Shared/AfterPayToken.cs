using Ladon;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Yort.AfterPay.InStore
{
	/// <summary>
	/// Represents an authorisation token returned from the AfterPay API.
	/// </summary>
	/// <seealso cref="AfterPayTokenRequest"/>
	/// <seealso cref="AfterPayDeviceRegistrationRequest"/>
	/// <seealso cref="AfterPayDeviceRegistration"/>
	public class AfterPayToken
	{
		/// <summary>
		/// The bearer token returned by the API that can be used to authorise further requests.
		/// </summary>
		[JsonProperty("token")]
		public string Token { get; set; }
		/// <summary>
		/// The number of seconds for which this token is valid from the time it was issued.
		/// </summary>
		[JsonProperty("expiresIn")]
		public long ExpiresIn { get; set; }

		/// <summary>
		/// Represents (roughly) the date and time at which this token was issued.
		/// </summary>
		/// <remarks>
		/// <para>This value is set using the <see cref="AfterPayConfiguration.SystemClock"/> at the time the response is received.</para>
		/// </remarks>
		public DateTimeOffset IssuedAt { get; set; } = AfterPayConfiguration.SystemClock.Now;

		/// <summary>
		/// Returns the date and time at which this token expires, calculated from <see cref="IssuedAt"/> and <see cref="ExpiresIn"/>.
		/// </summary>
		public DateTimeOffset ExpiresAt { get { return IssuedAt.AddSeconds(ExpiresIn); } }

		/// <summary>
		/// Returns true if the token has expired based on the current system clock and the calculated <see cref="ExpiresAt"/> value.
		/// </summary>
		/// <remarks>
		/// <para>This overload uses the <see cref="AfterPayConfiguration.SystemClock"/> property to retrieve the current date and time.</para>
		/// </remarks>
		public bool IsExpired()
		{
			return IsExpired(AfterPayConfiguration.SystemClock);
		}

		/// <summary>
		/// Returns true if the token has expired based on the current system clock and the calculated <see cref="ExpiresAt"/> value.
		/// </summary>
		/// <param name="systemClock">A <see cref="IAfterPaySystemClock"/> implementation to use when retrieving the current date and time.</param>
		/// <exception cref="System.ArgumentNullException">Thrown if <paramref name="systemClock"/> is null.</exception>
		public bool IsExpired(IAfterPaySystemClock systemClock)
		{
			systemClock.GuardNull(nameof(systemClock));

			return systemClock.Now >= ExpiresAt; 
		}
	}
}
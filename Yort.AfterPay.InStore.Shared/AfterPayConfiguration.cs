using System;
using System.Collections.Generic;
using System.Text;

namespace Yort.AfterPay.InStore
{
	/// <summary>
	/// Instances of this class represent configuration options for <see cref="IAfterPayClient"/> instances. The static members of this class provide global configuration common to all instances.
	/// </summary>
	public class AfterPayConfiguration
	{

		#region Static Memebers

		private static IAfterPaySystemClock s_SystemClock;

		/// <summary>
		/// Sets or returns an implementation of <see cref="IAfterPaySystemClock"/> that will be used by the library to determine the current date and time.
		/// </summary>
		/// <remarks>
		/// <para>If not clock is explicitly set, or if the property is set to null, then <see cref="AfterPaySystemClock.DefaultInstance"/> will be used (and returned as the current value of the property).</para>
		/// <para>This property can be used to provide a mocked clock for unit testing, or to provide a clock adjusted by a calculated offset via an NTP client etc. if the system clock cannot be relied upon for accuracy.</para>
		/// <para>This is a static property and the value set here affects all clients/objects from the Yort.AfterPay.InStore API unless otherwise specified.</para>
		/// </remarks>
		public static IAfterPaySystemClock SystemClock
		{
			get { return s_SystemClock ?? AfterPaySystemClock.DefaultInstance; }
			set
			{
				s_SystemClock = value;
			}
		}

		#endregion

	}
}
using System;
using System.Collections.Generic;
using System.Text;

namespace Yort.AfterPay.InStore
{
	/// <summary>
	/// Represents the default system clock used by the Yort.AfterPay.Instore library to retrieve the current date and time.
	/// </summary>
	/// <remarks>
	/// <para>This implementation simply wraps the <see cref="DateTimeOffset.Now"/> property and relies on the underlying system clock for accuracy.</para>
	/// <para>This class has a private constructor and cannot be created by user code. To obtain and instance use the static <see cref="DefaultInstance"/> property.</para>
	/// </remarks>
	public sealed class AfterPaySystemClock : IAfterPaySystemClock
	{

		#region Instance Members

		private AfterPaySystemClock()
		{
		}

		/// <summary>
		/// Returns the current date and time from the system clock.
		/// </summary>
		/// <see cref="System.DateTimeOffset.Now"/>
		public DateTimeOffset Now
		{
			get { return DateTimeOffset.Now; }
		}

		#endregion

		#region Static Memnbers

		private static AfterPaySystemClock s_DefaultInstance;

		/// <summary>
		/// Returns an instance of this clock.
		/// </summary>
		/// <remarks>
		/// <para>The instance returned by this method is cached after the first call to reduce allocations, so all subsquent calls will receive the same shared instance. 
		/// However the property is not thread-safe and two threads calling the method simultanously for the first time may receive different instances. 
		/// This should not cause a problem as clients should not (and are instructed not to) compare instances or rely on shared instances.</para>
		/// </remarks>
		public static AfterPaySystemClock DefaultInstance {  get{ return s_DefaultInstance ?? (s_DefaultInstance = new AfterPaySystemClock()); } }

		#endregion

	}
}
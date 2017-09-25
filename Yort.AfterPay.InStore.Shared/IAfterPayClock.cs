using System;
using System.Collections.Generic;
using System.Text;

namespace Yort.AfterPay.InStore
{
	/// <summary>
	/// Interface for components that can return the current date and time.
	/// </summary>
	/// <see cref="AfterPaySystemClock"/>
	public interface IAfterPaySystemClock
	{
		/// <summary>
		/// Returns the current date and time as a <see cref="DateTimeOffset"/> value.
		/// </summary>
		DateTimeOffset Now { get; }
	}
}
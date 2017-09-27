using System;
using System.Collections.Generic;
using System.Text;

namespace Yort.AfterPay.InStore
{
	/// <summary>
	/// A enum providing a list of environments for the AfterPay API.
	/// </summary>
	public enum AfterPayEnvironment
	{
		/// <summary>
		/// The test/sandbox API.
		/// </summary>
		Sandbox = 0,
		/// <summary>
		/// The live/production API.
		/// </summary>
		Production 
	}
}

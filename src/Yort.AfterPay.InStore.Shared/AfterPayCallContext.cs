using System;
using System.Collections.Generic;
using System.Text;

namespace Yort.AfterPay.InStore
{
	/// <summary>
	/// Represents arguments to AfterPay API's that are common across most/all calls but are not part of the request content.
	/// </summary>
	public class AfterPayCallContext
	{
		/// <summary>
		/// Sets or returns the unique id of the point-of-sale device user/operator who started the associated requests.
		/// </summary>
		public string OperatorId { get; set; }
	}
}
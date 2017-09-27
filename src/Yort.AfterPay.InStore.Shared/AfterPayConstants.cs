using System;
using System.Collections.Generic;
using System.Text;

namespace Yort.AfterPay.InStore
{
	/// <summary>
	/// Constant values relating to AfterPay.
	/// </summary>
	public static class AfterPayConstants
	{
		/// <summary>
		/// Returns the root address of the production url for AfterPay, without the version suffix.
		/// </summary>
		public const string ProductionRootUrl = "https://posapi.afterpay.com";
		/// <summary>
		/// Returns the root address of the sandboox url for AfterPay, without the version suffix.
		/// </summary>
		public const string SandboxRootUrl = "https://posapi-sandbox.afterpay.com";

		/// <summary>
		/// Returns "application/json" which is the media type accepted and returned by the AfterPay API.
		/// </summary>
		public const string JsonMediaType = "application/json";
	}
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yort.AfterPay.InStore.Tests
{
	public static class AfterPayTestSecrets
	{
		//Provides secret values required for running integration tests to the sandbox API.
		//Either set the relevant environment variables, or edit this code to return constants for your test account.

		public static string DuplicateRegistrationDeviceName
		{
			get { return Environment.GetEnvironmentVariable("AfterPayTests-DuplicateDeviceName"); }
		}

		public static string DuplicateRegistrationDeviceSecret
		{
			get { return Environment.GetEnvironmentVariable("AfterPayTests-DuplicateDeviceSecret"); }
		}


		public static string DeviceId
		{
			get { return Environment.GetEnvironmentVariable("AfterPayTests-DeviceId"); }
		}

		public static string DeviceKey
		{
			get { return Environment.GetEnvironmentVariable("AfterPayTests-DeviceKey"); }
		}

		public static string UnregisteredDeviceName
		{
			get { return Environment.GetEnvironmentVariable("AfterPayTests-UnregisteredDeviceName"); }
		}

		public static string UnregisteredDeviceSecret
		{
			get { return Environment.GetEnvironmentVariable("AfterPayTests-UnregisteredDeviceSecret"); }
		}

	}
}
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Yort.AfterPay.InStore
{
	/// <summary>
	/// Instances of this class represent configuration options for <see cref="IAfterPayClient"/> instances. The static members of this class provide global configuration common to all instances.
	/// </summary>
	public sealed class AfterPayConfiguration
	{

		#region Instance Members

		private HttpClient _HttpClient;
		private AfterPayEnvironment _Environment;

		private string _ProductName;
		private string _ProductVersion;
		private string _ProductVendor;
		private string _MerchantId;
		private string _DeviceId;
		private string _DeviceKey;
		private int _RetryDelay;

		private int _MinimumRetries;

		private bool _Locked;

		/// <summary>
		/// Default contructor, creates a new instance.
		/// </summary>
		/// <remarks>
		/// <para>Instances of this type become immmutable once passed to a <see cref="AfterPayClient"/> instance. Trying to set properties after this has occurred will result in an <see cref="InvalidOperationException"/>.</para>
		/// </remarks>
		public AfterPayConfiguration()
		{
			_MinimumRetries = 2;
		}

		/// <summary>
		/// Sets or returns the AfterPay API environment to be used.
		/// </summary>
		/// <exception cref="System.InvalidProgramException">Thrown if this property is modified after it has been passed to a <see cref="AfterPayClient"/> instance.</exception>
		public AfterPayEnvironment Environment
		{
			get { return _Environment; }
			set
			{
				ThrowIfLocked();

				_Environment = value;
			}
		}

		/// <summary>
		/// Sets or returns an <see cref="HttpClient"/> instance used to make calls to the AfterPay API. If null/unset, the system will create it's own instance on first use.
		/// </summary>
		/// <remarks>
		/// <para>The library reserves the right to modify the provided client, such as setting default headers. A client can be shared amongst configuration object, but should not be shared/use outside of other <see cref="AfterPayClient"/> instaces.
		/// The primary purpose of this method is to allow a client with injected handlers to be used. If you do not need to inject custom handlers, then leave this blank.</para>
		/// </remarks>
		/// <exception cref="System.InvalidProgramException">Thrown if this property is modified after it has been passed to a <see cref="AfterPayClient"/> instance.</exception>
		public HttpClient HttpClient
		{
			get { return _HttpClient; }
			set
			{
				ThrowIfLocked();

				_HttpClient = value;
			}
		}

		/// <summary>
		/// Sets or returns the product name that will be used as part of the user agent string when calling the AfterPay API.
		/// </summary>
		/// <remarks>
		/// <para>If null, empty string or only whitespace the name of the Yort.Afterpay.Instore assembly being used will be substituted as a default.</para>
		/// </remarks>
		/// <exception cref="System.InvalidProgramException">Thrown if this property is modified after it has been passed to a <see cref="AfterPayClient"/> instance.</exception>
		public string ProductName
		{
			get { return _ProductName; }
			set
			{
				ThrowIfLocked();

				_ProductName = value;
			}
		}

		/// <summary>
		/// Sets or returns the version number of the <see cref="ProductName"/> name that will be used as part of the user agent string when calling the AfterPay API.
		/// </summary>
		/// <remarks>
		/// <para>If null, empty string or only whitespace the version of the Yort.Afterpay.Instore assembly being used will be substituted as a default.</para>
		/// </remarks>
		/// <exception cref="System.InvalidProgramException">Thrown if this property is modified after it has been passed to a <see cref="AfterPayClient"/> instance.</exception>
		public string ProductVersion
		{
			get { return _ProductVersion; }
			set
			{
				ThrowIfLocked();

				_ProductVersion = value;
			}
		}

		/// <summary>
		/// Sets or returns the name of the vendor that will be used as part of the user agent string when calling the AfterPay API.
		/// </summary>
		/// <remarks>
		/// <para>If null, empty string or only whitespace, "Yort" assembly being used will be substituted as a default.</para>
		/// </remarks>
		/// <exception cref="System.InvalidProgramException">Thrown if this property is modified after it has been passed to a <see cref="AfterPayClient"/> instance.</exception>
		public string ProductVendor
		{
			get { return _ProductVendor; }
			set
			{
				ThrowIfLocked();

				_ProductVendor = value;
			}
		}

		/// <summary>
		/// Sets or returns the merchant ID that will be used as part of the user agent string when calling the AfterPay API.
		/// </summary>
		/// <exception cref="System.InvalidProgramException">Thrown if this property is modified after it has been passed to a <see cref="AfterPayClient"/> instance.</exception>
		public string MerchantId
		{
			get { return _MerchantId; }
			set
			{
				ThrowIfLocked();

				_MerchantId = value;
			}
		}

		/// <summary>
		/// Sets or returns the id this device previously registered with AfterPay, used to obtain authorisation tokens.
		/// </summary>
		/// <remarks>
		/// <para>Leave null if you will be registering a new device. After registration is successful you can retrieve the id from this property.</para>
		/// </remarks>
		/// <exception cref="System.InvalidProgramException">Thrown if this property is modified after it has been passed to a <see cref="AfterPayClient"/> instance.</exception>
		public string DeviceId
		{
			get { return _DeviceId; }
			set
			{
				ThrowIfLocked();

				_DeviceId = value;
			}
		}

		/// <summary>
		/// Sets or returns the secret key of this device used to obtain authorisation tokens.
		/// </summary>
		/// <remarks>
		/// <para>Leave null if you will be registering a new device. After registration is successful you can retrieve the key from this property.</para>
		/// </remarks>
		/// <exception cref="System.InvalidProgramException">Thrown if this property is modified after it has been passed to a <see cref="AfterPayClient"/> instance.</exception>
		public string DeviceKey
		{
			get { return _DeviceKey; }
			set
			{
				ThrowIfLocked();

				_DeviceKey = value;
			}
		}

		/// <summary>
		/// The minumum number of automatic retries to perform when a create transaction (order/refund/order reversal/refund reversal etc) times out.
		/// </summary>
		/// <remarks>
		/// <para>This property defaults to a value of 2. A value of zero or less is allowed, in which case only the initial attempt will be made - no retries will be performed within the library and any error handling logic will need to be entirely implemented by the application.</para>
		/// <para>The library may attempt more retries than specified if the total time since the initial call is less than the (full, AfterPay) recommended timeout for the endpoint being called.</para>
		/// </remarks>
		/// <exception cref="System.InvalidProgramException">Thrown if this property is modified after it has been passed to a <see cref="AfterPayClient"/> instance.</exception>
		public int MinimumRetries
		{
			get { return _MinimumRetries; }
			set
			{
				ThrowIfLocked();

				_MinimumRetries = value;
			}
		}

		/// <summary>
		/// Sets or returns the number of seconds to wait before attempting a retry.
		/// </summary>
		/// <remarks>
		/// <para>When a transactional call (CreateOrder/Refund etc) times out the system will perform a retry (based on the <see cref="MinimumRetries"/> setting). 
		/// If that retry attempt returns a 409 conflict response indicating the first request is still in progres, 
		/// then the system will wait this many seconds before the next retry. See https://docs.afterpay.com.au/instore-api-v1.html#distributed-state-considerations and https://docs.afterpay.com.au/instore-api-v1.html#create-order for more details.</para>
		/// <para>The minimum value is 5 seconds. Any value less than 5 seconds will be ignored, and a 5 second delay will occur instead.</para>
		/// </remarks>
		/// <exception cref="System.InvalidProgramException">Thrown if this property is modified after it has been passed to a <see cref="AfterPayClient"/> instance.</exception>
		public int RetryDelaySeconds
		{
			get { return _RetryDelay; }
			set
			{
				ThrowIfLocked();

				_RetryDelay = value;
			}
		}

		private void ThrowIfLocked()
		{
			if (_Locked) throw new InvalidOperationException(ErrorMessageResources.ConfigurationPropertiesLocked);
		}

		internal void LockProperties()
		{
			_Locked = true;
		}

		internal void DeviceRegistered(AfterPayDeviceRegistration registrationDetails)
		{
			_DeviceId = registrationDetails?.DeviceId;
			_DeviceKey = registrationDetails?.Key;
		}

		#endregion

		#region Static Memebers

		private static IAfterPaySystemClock s_SystemClock;
		private static string s_DefaultCurrency;

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

		/// <summary>
		/// Sets or returns the default currency for new <see cref="AfterPayMoney"/> instances where the currency is not explicitly provided.
		/// </summary>
		/// <remarks>
		/// <para>If this property is null or an empty string then <see cref="AfterPayCurrencies.AustralianDollars"/> will be used as a default.</para>
		/// </remarks>
		public static string DefaultCurrency
		{
			get { return s_DefaultCurrency; }
			set
			{
				s_DefaultCurrency = value;
			}
		}

		#endregion

	}
}
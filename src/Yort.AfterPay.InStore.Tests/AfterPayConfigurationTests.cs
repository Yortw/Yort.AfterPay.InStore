using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yort.AfterPay.InStore.Tests
{
	[TestClass]
	public class AfterPayConfigurationTests
	{

		[TestMethod]
		public void Configuration_SystemClock_ReturnsCustomClockWhenSet()
		{
			var clock = new MockClock(DateTimeOffset.Now);

			AfterPayConfiguration.SystemClock = clock;

			Assert.AreEqual(clock, AfterPayConfiguration.SystemClock);
		}

		[TestMethod]
		public void Configuration_SystemClock_ReturnsDefaultClockWhenExplicitlySetToNull()
		{
			AfterPayConfiguration.SystemClock = null;

			Assert.AreEqual(AfterPaySystemClock.DefaultInstance, AfterPayConfiguration.SystemClock);
		}

		[TestMethod]
		public void Configuration_SystemClock_DefaultsToSystemClock()
		{
			Assert.AreEqual(AfterPaySystemClock.DefaultInstance, AfterPayConfiguration.SystemClock);
		}

	}
}
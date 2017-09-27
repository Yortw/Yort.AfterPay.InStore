using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Yort.AfterPay.InStore.Tests
{
	[TestClass]
	public class AfterPayTokenTests
	{
		[TestMethod]
		public void Token_IsExpired_TrueAfterTimeToLive()
		{
			var token = new AfterPayToken()
			{
				ExpiresIn = 3600,
				IssuedAt = DateTimeOffset.Now,
				Token = "123"
			};

			var clock = new MockClock(DateTimeOffset.Now.AddHours(2));

			Assert.IsTrue(token.IsExpired(clock));

			AfterPayConfiguration.SystemClock = clock;
			Assert.IsTrue(token.IsExpired());
		}

		[TestMethod]
		public void Token_IsExpired_FalseBeforeExpiry()
		{
			var token = new AfterPayToken()
			{
				ExpiresIn = 3600,
				IssuedAt = DateTimeOffset.Now,
				Token = "123"
			};

			var clock = new MockClock(DateTimeOffset.Now);
			Assert.IsFalse(token.IsExpired(clock));

			AfterPayConfiguration.SystemClock = clock;
			Assert.IsFalse(token.IsExpired());
		}

		[TestMethod]
		public void Token_ExpiresAt_CalcuatedFromTimeToLive()
		{
			var issued = DateTimeOffset.Now;
			var timeToLiveInSeconds = 3600;

			var token = new AfterPayToken()
			{
				ExpiresIn = timeToLiveInSeconds,
				IssuedAt = issued,
				Token = "123"
			};

			Assert.AreEqual(issued.AddSeconds(timeToLiveInSeconds), token.ExpiresAt);			
		}
		
		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Token_IsExpired_ThrowsOnNullClock()
		{
			var issued = DateTimeOffset.Now;
			var timeToLiveInSeconds = 3600;

			var token = new AfterPayToken()
			{
				ExpiresIn = timeToLiveInSeconds,
				IssuedAt = issued,
				Token = "123"
			};

			token.IsExpired(null);
		}

	}
}

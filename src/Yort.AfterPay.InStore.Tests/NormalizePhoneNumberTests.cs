using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yort.AfterPay.InStore.Tests
{
	[TestClass]
	public class NormalizePhoneNumberTests
	{
		[TestMethod]
		public void NormalizePhoneNumber_ReturnsNullForNull()
		{
			Assert.AreEqual(null, AfterPayInviteRequest.NormalizePhoneNumber(null));
		}

		[TestMethod]
		public void NormalizePhoneNumber_ReturnsEmptyForEmpty()
		{
			Assert.AreEqual(String.Empty, AfterPayInviteRequest.NormalizePhoneNumber(String.Empty));
		}

		[TestMethod]
		public void NormalizePhoneNumber_ReturnsUnmodifiedStringWhenAllNumeric()
		{
			Assert.AreEqual("1234567890", AfterPayInviteRequest.NormalizePhoneNumber("1234567890"));
		}

		[TestMethod]
		public void NormalizePhoneNumber_RemoveNonNumericDigits()
		{
			Assert.AreEqual("1234567890", AfterPayInviteRequest.NormalizePhoneNumber("+1-2 3(4)5A6B7#890."));
		}

	}
}
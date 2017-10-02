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
			var client = new AfterPayClient(new AfterPayConfiguration());
			Assert.AreEqual(null, client.NormalizePhoneNumber(null));
		}

		[TestMethod]
		public void NormalizePhoneNumber_ReturnsEmptyForEmpty()
		{
			var client = new AfterPayClient(new AfterPayConfiguration());
			Assert.AreEqual(String.Empty, client.NormalizePhoneNumber(String.Empty));
		}

		[TestMethod]
		public void NormalizePhoneNumber_ReturnsUnmodifiedStringWhenAllNumeric()
		{
			var client = new AfterPayClient(new AfterPayConfiguration());
			Assert.AreEqual("1234567890", client.NormalizePhoneNumber("1234567890"));
		}

		[TestMethod]
		public void NormalizePhoneNumber_RemoveNonNumericDigits()
		{
			var client = new AfterPayClient(new AfterPayConfiguration());
			Assert.AreEqual("1234567890", client.NormalizePhoneNumber("+1-2 3(4)5A6B7#890."));
		}

	}
}
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yort.AfterPay.InStore.Tests
{
	[TestClass]
	public class AfterPayMoneyTests
	{
		[TestMethod]
		public void Money_UsesSpecifiedCurrency()
		{
			var m1 = new AfterPayMoney(10, AfterPayCurrencies.NewZealandDollars);
			var m2 = new AfterPayMoney(10, AfterPayCurrencies.AustralianDollars);

			Assert.AreEqual(m1.Currency, AfterPayCurrencies.NewZealandDollars);
			Assert.AreEqual(m2.Currency, AfterPayCurrencies.AustralianDollars);
		}

		[TestMethod]
		public void Money_UsesDefaultCurrency()
		{
			AfterPayConfiguration.DefaultCurrency = AfterPayCurrencies.NewZealandDollars;
			var m1 = new AfterPayMoney(10);
			AfterPayConfiguration.DefaultCurrency = AfterPayCurrencies.AustralianDollars;
			var m2 = new AfterPayMoney(10);

			Assert.AreEqual(m1.Currency, AfterPayCurrencies.NewZealandDollars);
			Assert.AreEqual(m2.Currency, AfterPayCurrencies.AustralianDollars);
		}

		[TestMethod]
		public void Money_UsesAudWhenDefaultCurrencyNull()
		{
			AfterPayConfiguration.DefaultCurrency = null;
			var m1 = new AfterPayMoney(10);
			Assert.AreEqual(m1.Currency, AfterPayCurrencies.AustralianDollars);
		}

		[TestMethod]
		public void Money_UsesAudWhenDefaultCurrencyEmptyString()
		{
			AfterPayConfiguration.DefaultCurrency = String.Empty;
			var m1 = new AfterPayMoney(10);
			Assert.AreEqual(m1.Currency, AfterPayCurrencies.AustralianDollars);
		}
	}
}
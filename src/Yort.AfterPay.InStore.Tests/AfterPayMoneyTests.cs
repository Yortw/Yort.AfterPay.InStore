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
		
		[TestMethod]
		public void Money_Equals_EqualWhenSubPropertiesMatch()
		{
			var m1 = new AfterPayMoney(10M, AfterPayCurrencies.AustralianDollars);
			var m2 = new AfterPayMoney(10M, AfterPayCurrencies.AustralianDollars);

			Assert.AreEqual(m1, m2);
			Assert.IsTrue(m1 == m2);
			Assert.IsFalse(m1 != m2);
		}

		[TestMethod]
		public void Money_Equals_NotEqualWhenSubPropertiesDoNotMatch()
		{
			var m1 = new AfterPayMoney(10M, AfterPayCurrencies.AustralianDollars);
			var m2 = new AfterPayMoney(20M, AfterPayCurrencies.NewZealandDollars);

			Assert.AreNotEqual(m1, m2);
			Assert.IsFalse(m1 == m2);
			Assert.IsTrue(m1 != m2);
		}

		[TestMethod]
		public void Money_Equals_NotEqualWhenCurrenciesDoNotMatch()
		{
			var m1 = new AfterPayMoney(10M, AfterPayCurrencies.AustralianDollars);
			var m2 = new AfterPayMoney(10M, AfterPayCurrencies.NewZealandDollars);

			Assert.AreNotEqual(m1, m2);
			Assert.IsFalse(m1 == m2);
			Assert.IsTrue(m1 != m2);
		}

		[TestMethod]
		public void Money_Equals_NotEqualWhenAmountsDoNotMatch()
		{
			var m1 = new AfterPayMoney(10M, AfterPayCurrencies.AustralianDollars);
			var m2 = new AfterPayMoney(20M, AfterPayCurrencies.AustralianDollars);

			Assert.AreNotEqual(m1, m2);
			Assert.IsFalse(m1 == m2);
			Assert.IsTrue(m1 != m2);
		}

		[TestMethod]
		public void Money_GetHashCode_MatchWhenValuesEqual()
		{
			var m1 = new AfterPayMoney(10M, AfterPayCurrencies.AustralianDollars);
			var m2 = new AfterPayMoney(10M, AfterPayCurrencies.AustralianDollars);

			Assert.AreEqual(m1.GetHashCode(), m2.GetHashCode());
		}

		[TestMethod]
		public void Money_GetHashCode_DoNotMatchWhenAmountsDiffer()
		{
			var m1 = new AfterPayMoney(10M, AfterPayCurrencies.AustralianDollars);
			var m2 = new AfterPayMoney(20M, AfterPayCurrencies.AustralianDollars);

			Assert.AreNotEqual(m1.GetHashCode(), m2.GetHashCode());
		}

		[TestMethod]
		public void Money_GetHashCode_DoNotMatchWhenCurrenciesDiffer()
		{
			var m1 = new AfterPayMoney(10M, AfterPayCurrencies.AustralianDollars);
			var m2 = new AfterPayMoney(10M, AfterPayCurrencies.NewZealandDollars);

			Assert.AreNotEqual(m1.GetHashCode(), m2.GetHashCode());
		}
	}
}
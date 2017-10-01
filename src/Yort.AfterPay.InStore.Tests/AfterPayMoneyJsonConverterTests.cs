using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yort.AfterPay.InStore.Tests
{
	[TestClass]
	public class AfterPayMoneyJsonConverterTests
	{
		[TestMethod]
		public void AfterPayMoney_SerialisesAmountAsString()
		{
			var value = new AfterPayMoney(10, AfterPayCurrencies.AustralianDollars);
			var serialisedData = Newtonsoft.Json.JsonConvert.SerializeObject(value, Newtonsoft.Json.Formatting.None);

			Assert.AreEqual("{\"amount\":\"10\",\"currency\":\"AUD\"}", serialisedData);
		}

		[TestMethod]
		public void AfterPayMoney_DeserialisesSerialisedAmount()
		{
			var value = new AfterPayMoney(10, AfterPayCurrencies.AustralianDollars);
			var serialisedData = Newtonsoft.Json.JsonConvert.SerializeObject(value, Newtonsoft.Json.Formatting.None);

			var deserialisedValue = Newtonsoft.Json.JsonConvert.DeserializeObject<AfterPayMoney>(serialisedData);
			Assert.AreEqual(10M, deserialisedValue.Amount);
			Assert.AreEqual(AfterPayCurrencies.AustralianDollars, deserialisedValue.Currency);
		}

		[TestMethod]
		public void AfterPayMoney_DeserialisesCorrectlyAsPartOfParentObject()
		{
			var orderRequest = new AfterPayCreateOrderRequest()
			{
				Amount = new AfterPayMoney(10M, "AUD"),
				MerchantReference = "d234232",
				PreapprovalCode = "ABCDEFGHIJKLMNOP",
				OrderItems = new AfterPayOrderItem[]
				{
					new AfterPayOrderItem() { Name = "Test", Quantity = 1, Price = new AfterPayMoney(10M, "AUD"), Sku = "200003332" }
				}
			};

			var serialisedData = Newtonsoft.Json.JsonConvert.SerializeObject(orderRequest);
			var deserialisedOrder = Newtonsoft.Json.JsonConvert.DeserializeObject<AfterPayCreateOrderRequest>(serialisedData);

			Assert.AreEqual(orderRequest.Amount, deserialisedOrder.Amount);
			Assert.AreEqual(orderRequest.MerchantReference, deserialisedOrder.MerchantReference);
			Assert.AreEqual(orderRequest.PreapprovalCode, deserialisedOrder.PreapprovalCode);
			Assert.AreEqual(orderRequest.RequestId, deserialisedOrder.RequestId);
			Assert.AreEqual(orderRequest.RequestedAt, deserialisedOrder.RequestedAt);
			Assert.AreEqual(orderRequest.OrderItems.Count(), deserialisedOrder.OrderItems.Count());
			Assert.AreEqual(orderRequest.OrderItems.First().Sku, deserialisedOrder.OrderItems.First().Sku);
			Assert.AreEqual(orderRequest.OrderItems.First().Quantity, deserialisedOrder.OrderItems.First().Quantity);
			Assert.AreEqual(orderRequest.OrderItems.First().Name, deserialisedOrder.OrderItems.First().Name);
			Assert.AreEqual(orderRequest.OrderItems.First().Price, deserialisedOrder.OrderItems.First().Price);
		}

	}
}
using Ladon;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Yort.AfterPay.InStore
{
	/// <summary>
	/// Ensures <see cref="AfterPayMoney"/> instances are serialized according to the AfterPay rules.
	/// </summary>
	/// <remarks>
	/// <para>In particular this class ensures the <see cref="AfterPayMoney.Amount"/> value is (de)serialised as a string.</para>
	/// </remarks>
	public class AfterPayMoneyJsonConverter : JsonConverter
	{
		/// <summary>
		/// Returns true if <paramref name="objectType"/> is <see cref="AfterPayMoney"/>.
		/// </summary>
		/// <param name="objectType">The type of object being converted.</param>
		/// <returns>True if <paramref name="objectType"/> refers to <see cref="AfterPayMoney.Amount"/>.</returns>
		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof(AfterPayMoney);
		}

		/// <summary>
		/// Reads an <see cref="AfterPayMoney"/> from the current reader at the current position.
		/// </summary>
		/// <param name="reader">The reader to read from.</param>
		/// <param name="objectType">The type of object to return, should be <see cref="AfterPayMoney"/>.</param>
		/// <param name="existingValue">Not used.</param>
		/// <param name="serializer">The serializer being used with this converter.</param>
		/// <returns></returns>
		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			reader.GuardNull(nameof(reader));

			if (reader.TokenType == JsonToken.Null) return null;

			if (reader.TokenType == JsonToken.StartObject)
				reader.Read();

			string propertyName = null;
			decimal amount = 0;
			string currency = null;

			if (reader.TokenType == JsonToken.PropertyName)
				propertyName = (string)reader.Value;

			while (reader.TokenType != JsonToken.EndObject && reader.Read() && reader.TokenType != JsonToken.EndObject)
			{
				if (reader.TokenType == JsonToken.PropertyName)
				{
					propertyName = (string)reader.Value;
					reader.Read();
				}

				if (propertyName == "amount" && reader.Value != null)
				{
					amount = Convert.ToDecimal(reader.Value);
				}
				else if (propertyName == "currency")
				{
					currency = (string)reader.Value ?? AfterPayConfiguration.DefaultCurrency;
				}
			}

			while (reader.TokenType != JsonToken.EndObject)
			{
				reader.Read();
			}

			return new AfterPayMoney(amount, currency ?? AfterPayConfiguration.DefaultCurrency);
		}

		/// <summary>
		/// Writes an <see cref="AfterPayMoney"/> instance to the specified writer, ensuring the <see cref="AfterPayMoney.Amount"/> value is written as a string.
		/// </summary>
		/// <param name="writer">The writer to write to.</param>
		/// <param name="value">The <see cref="AfterPayMoney"/> instance to be written.</param>
		/// <param name="serializer">The serializer being used with this converter.</param>
		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			writer.GuardNull(nameof(writer));

			var moneyValue = (AfterPayMoney)value;
			if (value == null) writer.WriteNull();

			writer.WriteStartObject();
			writer.WritePropertyName("amount");
			writer.WriteValue(moneyValue.Amount.ToString());
			writer.WritePropertyName("currency");
			writer.WriteValue(moneyValue.Currency);
			writer.WriteEndObject();
		}
	}
}
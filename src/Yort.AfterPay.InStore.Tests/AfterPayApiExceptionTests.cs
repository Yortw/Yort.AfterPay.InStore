using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yort.AfterPay.InStore.Tests
{
	[TestClass]
	public class AfterPayApiExceptionTests
	{
		
		[TestMethod]
		public void Exception_LoadsAllPropertiesFromErrorResponse()
		{
			var errorResponse = new AfterPayApiError()
			{
				HttpStatusCode = 401,
				ErrorCode = "unauthorised",
				ErrorId = System.Guid.NewGuid().ToString(),
				Message = "Request not authorised."
			};

			var innerException = new System.Net.WebException("Request not authorised.", System.Net.WebExceptionStatus.ProtocolError);
			var exception = new AfterPayApiException(errorResponse, innerException);

			Assert.AreEqual(errorResponse.Message, exception.Message);
			Assert.AreEqual(innerException, exception.InnerException);
			Assert.AreEqual(errorResponse.ErrorId, exception.ErrorId);
			Assert.AreEqual(errorResponse.ErrorCode, exception.ErrorCode);
			Assert.AreEqual(errorResponse.HttpStatusCode, exception.HttpStatusCode);
		}

		[TestMethod]
		public void Exception_CustomPropertiesReturnNullWhenNoErrorResponse()
		{
			var exception = new AfterPayApiException("Test message");

			Assert.AreEqual("Test message", exception.Message);
			Assert.AreEqual(null, exception.InnerException);
			Assert.AreEqual(null, exception.ErrorId);
			Assert.AreEqual(null, exception.ErrorCode);
			Assert.AreEqual(null, exception.HttpStatusCode);
		}


	}
}
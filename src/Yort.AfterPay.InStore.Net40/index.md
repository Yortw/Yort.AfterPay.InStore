# Quick Start

## Getting Started

Regardless of whether you use this library or fully implement your own solution, you will need to get in touch with AfterPay to have a development manager assigned and to obtain the required credentials for both sandbox and production access.

The main component of the library is the AfterPayClient class. Create an instance of this class to interact with the AfterPay API. Each instance of this class represents a single 'AfterPay' device. AfterPayClient is disposable, but best performance is achieved by re-using instances and **not** disposing them after every call. Normally for in-store implementations AfterPay is accessed directly from each point of sale device, so creating one instance for the lifetime of your process is fine. If routing API access through a central server or process then a seperate instance will be required for each physical device, these should also be kept for the lifetime of the process and reused between requests for the same device.

All methods other than RegisterDevice are thread-safe.

## One Time Device Registration
Every device that accesses the AfterPay API requires a one time registration with the AfterPay service. This can be done via the client.

Construct the client *without* providing DeviceId or DeviceKey values.
```c#
	// Create an unauthenticated client
	var config = new AfterPayConfiguration()
	{
		Environment = AfterPayEnvironment.Sandbox,
		ProductName = "MyPOS",
		ProductVerison = "1.0"
	};

	IAfterPayClient client = new AfterPayClient(config);

	//Send the registration request
	var request = new AfterPayDeviceRegistrationRequest()
	{
	 	Name = "MyPos1-aaa", 			//This value provided by AfterPay
		Secret = "kl32j4k2l2kl23"		//This value provided by AfterPay
	}

	try
	{
		var deviceRegistration = await client.RegisterDevice(request);
		// At this point the deviceRegistration variable will contain the
		// device id and key needed to authenticate in the futuer, these
		// should be saved somewhere for later access. 'client' will 
		// have updated it's configuration to use the new registration
		// details automatically, no need to create a new client.
	}
	catch (AfterPayApiException apex)
	{
		// AfterPay rejected the registration attempt for some reason,
		// the apex exception will contain the details.
		System.Diagnostics.Debug.WriteLine(apex.ToString());
	}
```

Calling RegisterDevice on a client that already has a device id and key, whether provided manully or via a prior call to RegisterDevice, will throw an exception.

## Creating A Client
For clients that have already been registered, configure the client with the device id and key previously obtained from the registration.

```c#
	var config = new AfterPayConfiguration()
	{
		Environment = AfterPayEnvironment.Sandbox,
		ProductName = "MyPOS",
		ProductVerison = "1.0",
		DeviceId = "123",
		DeviceKey = "ADSFD23242dw"
	};

	IAfterPayClient client = new AfterPayClient(config);
```

The client will automatically obtain a token on the first API call that requires one, and will automatically renew it when it expires.

## Inviting a Customer/Payment
You can invite a customer to join AfterPay, and/or assist with creating the pre-approval code by sending an invitation with the required payment value.

```c#

	var request = new AfterPayInviteRequest()
	{
		MobileNumber = "0400090000",
		ExpectedAmount = new AfterPayMoney(200.00M, AfterPayCurrencies.AustralianDollars)
	};

	//You will need to implement appropriate error handling.
	bool success = await client.SendInvite(request, new AfterPayCallContext() { OperatorId = "Randal Graves" });
```

## Creating an Order (Requesting Payment)

```c#
	// The RequestId and RequestedAt properties are set automatically if not specified,  
	// but could be overridden here if custom values are desired. RequestId should be 
	// persisted in case a reversal is required.
	var request = new AfterPayCreateOrderRequest()
	{
		MerchantReference = "D253100021",
		PreapprovalCode = "ABCDEFGHIJKLMNOP",
		Amount = new AfterPayMoney(200.00M, AfterPayCurrencies.AustralianDollars),
		OrderItems = new AfterPayOrderItem[]
		{
			new AfterPayOrderItem() 
			{ 
				Name = "Navy Check Jacket", 
				Sku = "20000332", 
				Quantity = 1.00M, 
				Price = new AfterPayMoney(200.00M, AfterPayCurrencies.AustralianDollars) 
			}
		}
	};

	//You will need to implement appropriate error handling.
	var order = await client.CreateOrder(request, new AfterPayCallContext() { OperatorId = "Randal Graves" });

	System.Diagnostics.Debug.WriteLine(order.OrderId);	
```

## Reversing an Order 

In case of a non-response or power failure/crash recovery, a previously placed order can be reversed. See the [AfterPay documentation](https://docs.afterpay.com.au/instore-api-v1.html#reverse-order) for more details.
```c#
	// The RequestedAt property is set automatically if not specified,  
	// but could be overridden here if a custom value are desired.
	var request = new AfterPayReverseOrderRequest()
	{
		// This should be the 'RequestId' property from a prior CreateOrder call.
		ReversingRequestId = "6758B28E-5C85-4AF5-B8F5-2E5A544D7AE2" 
	};

	//You will need to implement appropriate error handling.
	var reversal = await client.CreateOrder(request, new AfterPayCallContext() { OperatorId = "Randal Graves" });

	System.Diagnostics.Debug.WriteLine(reversal.ReverseId);	
```

## Creating a Refund

```c#
	var request = new AfterPayCreateRefundRequest()
	{
		MerchantReference = "D253100030",
		OrderId = "1000201",
		OrderMerchantReference = "D253100021",
		Amount = new AfterPayMoney(200.00M, AfterPayCurrencies.AustralianDollars) 		
	};

	//You will need to implement appropriate error handling.
	var refund = await client.RefundOrder(request, new AfterPayCallContext() { OperatorId = "Randal Graves" });

	System.Diagnostics.Debug.WriteLine(refund.RefundId);	
```

## Reversing an Refund

In case of a non-response or power failure/crash recovery, a previously attempted refund can be reversed. See the [AfterPay documentation](https://docs.afterpay.com.au/instore-api-v1.html#refund-reversal) for more details.
```c#
	// The RequestedAt property is set automatically if not specified,  
	// but could be overridden here if a custom value are desired.
	var request = new AfterPayReverseRefundRequest()
	{
		// This should be the 'RequestId' property from a prior RefundOrder call.
		ReversingRequestId = "FAC98E9C-ACDF-49A9-8D64-074631605DB5" 
	};

	//You will need to implement appropriate error handling.
	var reversal = await client.ReverseRefund(request, new AfterPayCallContext() { OperatorId = "Randal Graves" });

	System.Diagnostics.Debug.WriteLine(reversal.ReverseId);	
```

## Pre-approval Enquiry
Not typically neccesary, but you can enquire about the properties of a pre-approval code.

```c#
	var request = new AfterPayPreapprovalRequest()
	{
		PreapprovalCode= "ABCDEFGHIJKLMNOP" 
	};

	//You will need to implement appropriate error handling.
	var preapprovalDetails = await client.PreapprovalEnquiry(request, new AfterPayCallContext() { OperatorId = "Randal Graves" });

	System.Diagnostics.Debug.WriteLine(preapprovalDetails.Amount.ToString());	
	System.Diagnostics.Debug.WriteLine(preapprovalDetails.Minimum.ToString());	
```

## Ping 
You can check if the AfterPay service is currently available from your device using the ping function.

```c#
	try
	{
		await client.Ping();
		//The service is available if the preceding call doesn't throw.
	}
	catch (AfterPayApiExcetion apex)
	{
		//The service was reached but rejected the call, likely
		//due to maintenance/overloading or too many request from this client.
	}
	catch (HttpRequestException hrex)
	{
		//The service could not be reached, most likely
		//due to a network error of some sort.
	}
	catch (TimeoutException te)
	{
		//The service did not respond within the expected period.
	}
```

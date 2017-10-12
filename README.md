# Yort.AfterPay.InStore
An **unofficial**, light-weight wrapper for .Net, around the After Pay In-Store API (https://docs.afterpay.com.au/instore-api-v1.html).

[![GitHub license](https://img.shields.io/github/license/mashape/apistatus.svg)](https://github.com/Yortw/Yort.AfterPay.InStore/blob/master/LICENSE.md) [![Build status](https://ci.appveyor.com/api/projects/status/waxmch4c6sm96vaa?svg=true)](https://ci.appveyor.com/project/Yortw/Yort.AfterPay.InStore) [![NuGet Badge](https://buildstats.info/nuget/Yort.AfterPay.InStore)](https://www.nuget.org/packages/Yort.AfterPay.InStore/)

# THIS SAUSAGE IS STILL BEING MADE - DON'T LOOK.
This is a work in progress. Nothing to see here for now. Moving along.

## Supported Platforms
Currently;

* .Net Standard 2.0
* .Net 4.0+

## What Does It Do (and Not Do)?
This library provides models for the request and response payloads, and a client class to access (only) the in-store API. It provides some basic error handling around timeouts and 409 conflict errors on retry for some endpoints. In addition it largely handles the authentication process and renewing tokens automatically, assuming the correct values are provided. It can access either the production or sandbox environments. The client class implements an interface, so can be easily mocked/stubbed etc if required for unit testing. Custom HTTP client and clock implementations can be injected into the client.

AfterPay servers require TLS 1.2. This library will configure System.Net.ServicePointManager.SecurityProtocols to support TLS 1.2 even on .Net 4.0, however for this to work either a later .Net framework must  be installedor [Windows will need to have the registry edited](https://stackoverflow.com/questions/33761919/tls-1-2-in-net-framework-4-0) to support the protocol.

It **does not** 
* Resolve you of the responsibility for writing a robust system.
* Automatically queue reversals.
* Implement any form of crash recovery.
* Provide any access for the web based flow/API.

You **really** need to read the [AfterPay API documentation](https://docs.afterpay.com.au/instore-api-v1.html) to understand the processes and error handling required. Additionally any solution implemented must be tested/certified by AfterPay regardless of the use of this library.

## How do I use it?

Quick start and full [API reference documentation here](https://yortw.github.io/Yort.AfterPay.InStore/)

Make sure you read the [AfterPay API documentation](https://docs.afterpay.com.au/instore-api-v1.html) too.



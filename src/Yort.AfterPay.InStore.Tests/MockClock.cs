using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yort.AfterPay.InStore.Tests
{
	public class MockClock : IAfterPaySystemClock
	{
		private DateTimeOffset _Now;

		public MockClock(DateTimeOffset initialTime)
		{
			_Now = initialTime;
		}

		public DateTimeOffset Now
		{
			get { return _Now; }
			set { _Now = value; }
		}
	}
}
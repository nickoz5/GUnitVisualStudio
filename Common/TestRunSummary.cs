using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pryda.Common
{
	public class TestRunSummary
	{
		private DateTime _TimeStart;
		public DateTime TimeStart
		{
			get { return _TimeStart; }
			set { _TimeStart = value; }
		}

		private DateTime _TimeEnd;
		public DateTime TimeEnd
		{
			get { return _TimeEnd; }
			set { _TimeEnd = value; }
		}

		private string _ComputerName;
		public string ComputerName
		{
			get { return _ComputerName; }
			set { _ComputerName = value; }
		}

		private int _TestsShuffleSeed;
		public int TestsShuffleSeed
		{
			get { return _TestsShuffleSeed; }
			set { _TestsShuffleSeed = value; }
		}
		
		private int _TestsPassed;
		public int TestsPassed
		{
			get { return _TestsPassed; }
			set { _TestsPassed = value; }
		}

		private int _TestsFailed;
		public int TestsFailed
		{
			get { return _TestsFailed; }
			set { _TestsFailed = value; }
		}

		private int _TestsSkipped;
		public int TestsSkipped
		{
			get { return _TestsSkipped; }
			set { _TestsSkipped = value; }
		}

		private int _TestsTotal;
		public int TestsTotal
		{
			get { return _TestsTotal; }
			set { _TestsTotal = value; }
		}


		public TestRunSummary()
		{
		}

		public void Clear()
		{
			_TimeStart = DateTime.Now;
			_TimeEnd = DateTime.Now;

			_ComputerName = "";

			_TestsPassed = 0;
			_TestsFailed = 0;
			_TestsSkipped = 0;
			_TestsTotal = 0;
			_TestsShuffleSeed = 0;
		}
	}
}

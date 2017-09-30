using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Windows.Forms;

namespace Pryda.Common
{
	public class UnitTestOutput
	{
		public ListViewItem lvi;

		private string _TestCase;
		public string TestCase
		{
			get { return _TestCase; }
			set { _TestCase = value; }
		}

		private string _UnitTest;
		public string UnitTest
		{
			get { return _UnitTest; }
			set { _UnitTest = value; }
		}

		private bool _TestFailed;
		public bool TestFailed
		{
			get { return _TestFailed; }
			set { _TestFailed = value; }
		}

		private bool _TestPassed;
		public bool TestPassed
		{
			get { return _TestPassed; }
			set { _TestPassed = value; }
		}

		private List<string> _ArrayOutput = new List<string>();
		public List<string> ArrayOutput
		{
			get { return _ArrayOutput; }
		}


		public UnitTestOutput()
		{
			this._TestFailed = false;
			this._TestPassed = false;
		}
		public UnitTestOutput(string TestCase, string UnitTest, ListViewItem lvi)
		{
			this._TestFailed = false;
			this._TestPassed = false;
			this.TestCase = TestCase;
			this.UnitTest = UnitTest;
			this.lvi = lvi;
		}

		public void AddOutput(string output)
		{
			_ArrayOutput.Add(output);
		}
		public string GetOutput(int index)
		{
			return _ArrayOutput[index];
		}
		public int GetOutputLineCount()
		{
			return _ArrayOutput.Count;
		}
		public void Clear()
		{
			_ArrayOutput.Clear();
			this._TestFailed = false;
			this._TestPassed = false;
		}
	}
}

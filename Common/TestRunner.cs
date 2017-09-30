using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;

namespace Pryda.Common
{
	public class TestRunner
	{
		Thread _TestThread = null;	// used for monitoring app output
		Process _GTestApp = null;	// external process of app (only when NOT debugging)

		private static Regex ITERATION_START = new Regex(@"Repeating all tests \(iteration (\d+)\) . . .");
		private static Regex SHUFFLE_SEED = new Regex(@"Note: Randomizing tests' orders with a seed of (\d+) .");

		public delegate void UpdateListViewItemCallback(string result, string duration, bool running);
		public delegate void UpdateToolStripCallback(bool running);


		public LineRead notifyReadLine = null;

		private List<UnitTestOutput> _TestOutput = new List<UnitTestOutput>();
		private UnitTestOutput _CurrentTestOutput = null;

		int _TestIteration = 0;

		private TestRunSummary _TestSummary = new TestRunSummary();

		private string _Filter = "";
		public string Filter
		{
			get
			{
				return _Filter;
			}
			set
			{
				_Filter = value;
			}
		}

		private ctlTestList _ctlTestList;
		public ctlTestList TestListControl
		{
			get
			{
				return _ctlTestList;
			}
			set
			{
				_ctlTestList = value;
				_ctlTestList.notifyViewResults = ShowTestResults;
			}
		}

		private string _CurrentProjectName;
		public string CurrentProjectName
		{
			get
			{
				return _CurrentProjectName;
			}
			set
			{
				_CurrentProjectName = value;
			}
		}

		public bool IsRunning()
		{
			if (_TestThread != null && _TestThread.IsAlive)
				return true;
			return false;
		}

		public bool Start(StreamReader streamReader)
		{
			try
			{
				_TestSummary.Clear();
				_TestSummary.TimeStart = DateTime.Now;
				_TestSummary.ComputerName = SystemInformation.UserDomainName + "\\" + SystemInformation.ComputerName;
				_TestSummary.TestsShuffleSeed = 0;// _ctlTestList.shuffleTests;

				UpdatePendingTests(false);

				if (streamReader == null)
					streamReader = StartProcess(false);
				if (streamReader == null)
					return false;

				if (!AttachParserToStream(streamReader))
					return false;

				UpdateToolStrip(true);

				return true;
			}
			catch (System.Exception ex)
			{

			}

			return false;
		}

		public void Stop()
		{
			if (IsRunning())
			{
				// NOT null if the app was started externally (run)
				if (_GTestApp != null)
				{
					if (!_GTestApp.HasExited)
					{
						try
						{
							_GTestApp.Kill();
							if (!_GTestApp.WaitForExit(600))
							{
								MessageBox.Show("Error: Unable to terminate test application.");
								return;
							}
						}
						catch (System.Exception ex)
						{
						}
					}
				}
				
				try
				{
					if (_TestThread != null && _TestThread.IsAlive)
						_TestThread.Abort();
				}
				catch (System.Exception ex)
				{
				}
			}

			UpdateToolStrip(false);

			_CurrentTestOutput = null;

			UpdatePendingTests(true);

			_TestSummary.TimeEnd = DateTime.Now;
		}

		public bool CanRun()
		{
			bool ret = System.IO.File.Exists(CurrentProjectName);

			ret = ret && (CurrentProjectName.TrimEnd().EndsWith("exe") || CurrentProjectName.TrimEnd().EndsWith("bat"));
			if (!ret)
				return false;

			return true;
		}

		public string BuildArgs(bool onlyListTests)
		{
			StringBuilder cl = new StringBuilder();
			if (onlyListTests)
			{
				cl.Append("--gtest_list_tests ");
				return cl.ToString();
			}

			if (_ctlTestList.shuffleTests)
			{
				cl.Append(' ').Append("--gtest_shuffle");
				if (_ctlTestList.RandomSeed != 0)
				{
					cl.Append(' ').Append("--gtest_random_seed=" + _ctlTestList.RandomSeed);
				}
			}
			if (_ctlTestList.disabledTests) cl.Append(' ').Append("--gtest_also_run_disabled_tests");
			if (_ctlTestList.breakOnFailure) cl.Append(' ').Append("--gtest_break_on_failure");
			if (_ctlTestList.catchExceptions) cl.Append(' ').Append("--gtest_catch_exceptions=0");
			if (_ctlTestList.throwOnFailure) cl.Append(' ').Append("--gtest_throw_on_failure");

			if (_ctlTestList.RepeatCount > 1) cl.Append(' ').Append("--gtest_repeat=" + _ctlTestList.RepeatCount);
			if (!onlyListTests)
			{
				if (Filter.Length > 0)
				{
					cl.Append(" --gtest_filter=").Append(Filter.Trim());
				}
			}

			string datapath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Pryda\\UnitTestingTools");
			string argFileName = System.IO.Path.Combine(datapath, "args.txt");

			// write arguments to a file.
			try
			{
				System.IO.StreamWriter file = new System.IO.StreamWriter(argFileName);
				file.WriteLine(cl);
				file.Close();
			}
			catch (System.IO.IOException ex)
			{
				MessageBox.Show(ex.ToString());
				return "";
			}
			catch (System.Exception ex)
			{
				MessageBox.Show(ex.ToString());
				return "";
			}

			string args = "--gtest_arg_file=\"" + argFileName + "\"";
			return args;
		}

		private StreamReader StartProcess(bool onlyListTests)
		{
			try
			{
				_GTestApp = new System.Diagnostics.Process();
				_GTestApp.StartInfo.FileName = CurrentProjectName;
				_GTestApp.StartInfo.Arguments = BuildArgs(onlyListTests);
				_GTestApp.StartInfo.UseShellExecute = false;
				_GTestApp.StartInfo.RedirectStandardOutput = true;
				_GTestApp.StartInfo.CreateNoWindow = true;
				_GTestApp.Start();
				return _GTestApp.StandardOutput;
			}
			catch (System.Exception ex)
			{

			}

			return null;
		}

		public bool AttachParserToStream(StreamReader streamReader)
		{
			GoogleTestOutputParser parser = new GoogleTestOutputParser();
			parser.notifyTestStarted = this.testStarted;
			parser.notifyTestComplete = this.testComplete;
			parser.notifyLineRead = this.lineRead;
			parser.notifyRunFinished = this.runFinished;
			parser.input = streamReader;
			if (parser.input == null)
				return false;

			try
			{
				ThreadStart threadDelegate = new ThreadStart(parser.parseTests);
				_TestThread = new System.Threading.Thread(threadDelegate);
				_TestThread.Start();
				return true;
			}
			catch (System.Exception ex)
			{
			}

			return false;
		}

		public void ClearResults()
		{
			TestListControl.cls();

			foreach (UnitTestOutput output in _TestOutput)
			{
				output.Clear();
			}

			_TestIteration = 0;
		}

		private bool IsTestInFilter(string testCaseName)
		{
			if (Filter != "")
			{
				string[] words = _Filter.Split(':');
				foreach (string word in words)
				{
					// check if test is included
					if (word.Equals(testCaseName))
						return true;

					// check if test is specifically excluded
					if (word.Substring(0, 1).Equals("-") && word.Substring(1, word.Length - 1).Equals(testCaseName))
						return false;
				}

				return false;
			}
			return true;
		}

		public void UpdatePendingTests(bool finished)
		{
			_ctlTestList.listTests.BeginUpdate();
			foreach (ListViewItem lvi in _ctlTestList.listTests.Items)
			{
				string testName = _ctlTestList.getItemTestCaseName(lvi.Index);
				if (IsTestInFilter(testName))
				{
					if (finished)
					{
						// only mark as 'aborted' if it hasnt been completed yet..
						if (lvi.SubItems[3].Text.Equals(ctlTestList.RESULT_PENDING) || lvi.SubItems[3].Text.Equals(ctlTestList.RESULT_IN_PROGRESS))
							lvi.SubItems[3].Text = ctlTestList.RESULT_ABORTED;
					}
					else
						lvi.SubItems[3].Text = "Pending";
				}
				else
				{
					//lvi.SubItems[3].Text = "Skipped";
					if (!finished) _TestSummary.TestsSkipped++;
				}

				if (!finished) _TestSummary.TestsTotal++;
			}
			_ctlTestList.listTests.EndUpdate();
		}

		public bool PopulateTestList()
		{
			Cursor.Current = Cursors.WaitCursor;

			// remove all items from the list and output array.
			_TestOutput.Clear();
			TestListControl.listTests.Items.Clear();

			GoogleTestOutputParser parser = new GoogleTestOutputParser();
			int numTests = 0;

			try
			{
				StreamReader streamReader = StartProcess(true);
				if (streamReader == null)
					return false;
				parser.input = streamReader;
				numTests = parser.countTests();
			}
			catch (System.ObjectDisposedException)
			{
			}

			if (numTests > 0)
			{
				TestListControl.listTests.BeginUpdate();

				foreach (TestCaseStruct testcase in parser.listTestCases)
				{
					foreach (TestCaseTestStruct unittest in testcase.listUnitTests)
					{
						ListViewItem lvi = new ListViewItem("true");
						lvi.SubItems.Add(testcase.name);    // unit test name
						lvi.SubItems.Add(unittest.name);    // unit test name
						lvi.SubItems.Add("");               // result
						lvi.SubItems.Add("");               // duration

						TestListControl.listTests.Items.Add(lvi);

						_TestOutput.Add(new UnitTestOutput(testcase.name, unittest.name, lvi));
					}
				}

				//TestListControl.listTests.AutoResizeColumn(1, ColumnHeaderAutoResizeStyle.ColumnContent);
				TestListControl.listTests.EndUpdate();
			}

			Cursor.Current = Cursors.Default;

			return true;
		}

		private UnitTestOutput FindUnitTestInfo(string testName)
		{
			int pos = testName.IndexOf(".");
			String testcaseName = testName.Substring(0, pos);
			String unittestName = testName.Substring(pos + 1, testName.Length - pos - 1);

			foreach (UnitTestOutput output in _TestOutput)
			{
				if (output.TestCase.Equals(testcaseName) && output.UnitTest.Equals(unittestName))
					return output;
			}

			return null;
		}

		private void UpdateListViewItem(string result, string duration, bool running)
		{
			if (_CurrentTestOutput == null)
				return;

			_CurrentTestOutput.lvi.SubItems[3].Text = result;
			_CurrentTestOutput.lvi.SubItems[4].Text = duration;

			TestListControl.listTests.Invalidate(_CurrentTestOutput.lvi.Bounds);

			if (result.Equals(ctlTestList.RESULT_IN_PROGRESS))
			{
				if (_TestIteration > 0)
					_CurrentTestOutput.AddOutput("Test iteration: " + _TestIteration);
			}
		}

		private void UpdateToolStrip(bool running)
		{
			if (running)
			{
				TestListControl.toolStripGo.Enabled = false;
				TestListControl.toolStripButtonStop.Enabled = true;
			}
			else
			{
				TestListControl.toolStripGo.Enabled = true;
				TestListControl.toolStripButtonStop.Enabled = false;
			}
			TestListControl.toolStripGo.Invalidate();
			TestListControl.toolStripButtonStop.Invalidate();
		}



		#region Test Events

		private void lineRead(string line)
		{
			try
			{
				// check for multiple test iterations
				if (ITERATION_START.IsMatch(line))
				{
					string str = ITERATION_START.Match(line).Groups[1].Value;
					_TestIteration = System.Int32.Parse(str);
				}

				if (SHUFFLE_SEED.IsMatch(line))
				{
					string str = SHUFFLE_SEED.Match(line).Groups[1].Value;
					_TestSummary.TestsShuffleSeed = System.Int32.Parse(str);
				}

				if (notifyReadLine != null)
					notifyReadLine(line);

				// store line with current test output (if any).
				if (_CurrentTestOutput != null)
				{
					_CurrentTestOutput.AddOutput(line);
				}
			}
			catch (SystemException e)
			{
			}
		}

		private void testStarted(string testName)
		{
			UnitTestOutput output = FindUnitTestInfo(testName);
			if (output == null)
				return;

			_CurrentTestOutput = output;

			TestListControl.listTests.Invoke(new UpdateListViewItemCallback(this.UpdateListViewItem), new object[] { ctlTestList.RESULT_IN_PROGRESS, "", true });
		}

		private string formatTimeSpan(Int32 duration)
		{
			string durationFmt = "";

			try
			{
				TimeSpan timeSpan = new TimeSpan(0, 0, 0, 0, duration);
				durationFmt = timeSpan.ToString();
			}
			catch (SystemException e)
			{
			}
			return durationFmt;
		}

		private void testComplete(string testName, string error, Int32 duration)
		{
			UnitTestOutput output = FindUnitTestInfo(testName);
			if (output == null)
				return;

			_CurrentTestOutput = output;

			// format the timespan better..
			if (error != null)
			{
				string result = ctlTestList.RESULT_FAILED;
				if (_CurrentTestOutput.TestPassed)
					result = ctlTestList.RESULT_PARTIAL_FAILED;

				TestListControl.listTests.Invoke(new UpdateListViewItemCallback(this.UpdateListViewItem), new object[] { result, formatTimeSpan(duration), false });
			}
			else
			{
				string result = ctlTestList.RESULT_PASSED;
				if (_CurrentTestOutput.TestFailed)
					result = ctlTestList.RESULT_PARTIAL_FAILED;

				TestListControl.listTests.Invoke(new UpdateListViewItemCallback(this.UpdateListViewItem), new object[] { result, formatTimeSpan(duration), false });
			}

			if (error != null)
			{
				_TestSummary.TestsFailed++;
				_CurrentTestOutput.TestFailed = true;
			}
			else
			{
				_TestSummary.TestsPassed++;
				_CurrentTestOutput.TestPassed = true;
			}

			_CurrentTestOutput = null;
		}

		private void runFinished(string duration)
		{
			TestListControl.listTests.Invoke(new UpdateToolStripCallback(this.UpdateToolStrip), new object[] { false });
			_TestSummary.TimeEnd = DateTime.Now;
		}

		#endregion

		public void ShowTestResults(List<string> tests)
		{
			ResultsReport report = new ResultsReport();
			
			report.TestSummary = _TestSummary;
			if (tests == null)
			{
				report._TestOutput = _TestOutput;
			}
			else
			{
				List<UnitTestOutput> testList = new List<UnitTestOutput>();
				foreach (string testname in tests)
				{
					UnitTestOutput output = FindUnitTestInfo(testname);
					if (output != null)
						testList.Add(output);
				}
				if (testList.Count == 0)
					testList = _TestOutput;
				report._TestOutput = testList;
			}


			report.ShowDialog();
		}
	}
}

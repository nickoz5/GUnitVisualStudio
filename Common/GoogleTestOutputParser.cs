using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace Pryda.Common
{
	public delegate void TestStarted(string testName);
	public delegate void TestComplete(string testName, string error, Int32 duration);
	public delegate void RunFinished(string duration);
	public delegate void LineRead(string line);

	public struct TestCaseTestStruct
	{
		public String name;
	}
	public struct TestCaseStruct
	{
		public String name;
		public List<TestCaseTestStruct> listUnitTests;
	}


	public class GoogleTestOutputParser
	{
		private string _CurrentTestName;
		private static Regex TEST_START = new Regex(@"\[ RUN      ] ([\w/]+\.[\w/]+)");

		public static String RESULT_TEST_STARTED	= "[ RUN      ] ";
		public static String RESULT_TEST_PASSED		= "[       OK ] ";
		public static String RESULT_TEST_FAILED		= "[  FAILED  ] ";

		public TestStarted notifyTestStarted = null;
		public TestComplete notifyTestComplete = null;
		public LineRead notifyLineRead = null;
		public RunFinished notifyRunFinished = null;

		private string potentialErrorText;
		private int numTests = 0;
		private bool modeCountOnly;

		public StreamReader input;

		public List<TestCaseStruct> listTestCases = new List<TestCaseStruct>();

		public GoogleTestOutputParser()
		{
			notifyTestStarted = null;
			notifyTestComplete = null;
			notifyLineRead = null;
		}

		private void parseLine(String l)
		{
			try
			{
				if (modeCountOnly)
				{
					if (l.StartsWith("  "))
					{
						if (listTestCases.Count > 0)
						{
							TestCaseStruct testcase = listTestCases[listTestCases.Count - 1];
							TestCaseTestStruct unittest = new TestCaseTestStruct();
							unittest.name = l.TrimStart(' ');
							testcase.listUnitTests.Add(unittest);
						}
						numTests++;
					}
					if (l.EndsWith("."))
					{
						TestCaseStruct testcase = new TestCaseStruct();
						testcase.name = l.Substring(0, l.Length - 1);
						testcase.listUnitTests = new List<TestCaseTestStruct>();
						listTestCases.Add(testcase);
					}
				}
				else
				{
					if (TEST_START.IsMatch(l))
					{
						int pos1 = l.IndexOf(']');
						_CurrentTestName = l.Substring(pos1 + 2);
						//currentTestName = TEST_START.Match(l).Groups[1].Value;
						potentialErrorText = "";
						if (notifyTestStarted != null)
							notifyTestStarted(_CurrentTestName);

						if (notifyLineRead != null)
							notifyLineRead(l);
					}
					else if (_CurrentTestName != null)
					{
						if (l.StartsWith(RESULT_TEST_PASSED + _CurrentTestName))
						{
							int pos1 = l.LastIndexOf('(');
							int pos2 = l.LastIndexOf(')');
							string duration = l.Substring(pos1 + 1, pos2 - pos1 - 3);

							if (notifyLineRead != null)
								notifyLineRead(l);

							if (notifyTestComplete != null)
								notifyTestComplete(_CurrentTestName, null, Int32.Parse(duration));
							_CurrentTestName = null;
						}
						else if (l.StartsWith(RESULT_TEST_FAILED + _CurrentTestName))
						{
							int pos1 = l.LastIndexOf('(');
							int pos2 = l.LastIndexOf(')');
							string duration = l.Substring(pos1 + 1, pos2 - pos1 - 3);

							if (notifyLineRead != null)
								notifyLineRead(l);

							if (notifyTestComplete != null)
								notifyTestComplete(_CurrentTestName, potentialErrorText, Int32.Parse(duration));
							_CurrentTestName = null;
						}
						else if (!l.StartsWith("["))
						{
							if (notifyLineRead != null)
								notifyLineRead(l);

							potentialErrorText += l + "\r\n";
						}
						else
						{
							if (notifyLineRead != null)
								notifyLineRead(l);
						}
					}
					else
					{
						if (notifyLineRead != null)
							notifyLineRead(l);
					}
				}
			}
			finally
			{

			}
		}

		private void parseInputStream()
		{
			String line;
			while ((line = input.ReadLine()) != null)
			{
				parseLine(line);
			}

			if (notifyRunFinished != null)
				notifyRunFinished("");
		}

		public int countTests()
		{
			numTests = 0;
			modeCountOnly = true;
			parseInputStream();
			return numTests;
		}

		public void parseTests()
		{
			modeCountOnly = false;
			parseInputStream();
		}
	}

}

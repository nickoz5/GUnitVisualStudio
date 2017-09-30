using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Pryda.Common;

namespace GUnitStandalone
{
	public partial class GUnitForm : Form
	{
		private string _RegressionTestExecutable;
		private string _UnitTestExecutable;

		private TestRunner _TestRunner;

		private string _UnitTestExecutableName = "Pryda Build Unit Tests";
		private string _RegressionTestExecutableName = "Pryda Build Unit Test Jobs";


		public GUnitForm()
		{
			InitializeComponent();
		}

		private void ClearResults()
		{
			_TestRunner.ClearResults();
		}

		private string GetFileVersion(string filename)
		{
			string version = "";
			try
			{
				System.Diagnostics.FileVersionInfo.GetVersionInfo(filename);
				System.Diagnostics.FileVersionInfo myFileVersionInfo = System.Diagnostics.FileVersionInfo.GetVersionInfo(filename);
				version = myFileVersionInfo.FileVersion;
			}
			catch (System.IO.IOException ex)
			{
				return version;
			}

			if (version.Length == 0)
				return version;

			version = version.Replace(" ", string.Empty);
			version = version.Replace(",", ".");

			return " [" + version + "]";
		}

		private void GUnitForm_Load(object sender, EventArgs e)
		{
			_RegressionTestExecutable = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "RegressionTestPrydaBuild.exe");
			_UnitTestExecutable = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "UnitTestPrydaBuild.exe");

			ctlTestList1.DebuggingToolsEnabled = false;
			ctlTestList1.notifyProjectChanged = Event_ProjectChanged;
			ctlTestList1.notifyRunTests = Event_RunTests;
			ctlTestList1.notifyStopTests = Event_StopTests;

			_TestRunner = new TestRunner();
			_TestRunner.TestListControl = ctlTestList1;
			//testRunner.notifyReadLine = OnReadLine;

			if (File.Exists(_RegressionTestExecutable))
			{
				ctlTestList1.AddProject(_RegressionTestExecutableName + GetFileVersion(_RegressionTestExecutable));
			}
			if (File.Exists(_UnitTestExecutable))
			{
				ctlTestList1.AddProject(_UnitTestExecutableName + GetFileVersion(_UnitTestExecutable));
			}
		}

		#region Control callbacks
		private void Event_ProjectChanged(string projectName)
		{
			if (projectName.IndexOf(_RegressionTestExecutableName) > -1)
			{
				_TestRunner.CurrentProjectName = _RegressionTestExecutable;
			}
			if (projectName.IndexOf(_UnitTestExecutableName) > -1)
			{
				_TestRunner.CurrentProjectName = _UnitTestExecutable;
			}

			if (!_TestRunner.CanRun())
				return;
			_TestRunner.PopulateTestList();
		}

		public void Event_StopTests()
		{
			if (!_TestRunner.IsRunning())
				return;
			_TestRunner.Stop();
		}

		public void Event_RunTests(string filter, bool debug)
		{
			if (!_TestRunner.CanRun())
			{
				MessageBox.Show("The selected project cannot be executed.");
				return;
			}

			if (_TestRunner.IsRunning())
			{
				MessageBox.Show("Error: Tests are already running.");
				return;
			}

			ClearResults();

			_TestRunner.Filter = filter;
			_TestRunner.Start(null);
		}
		#endregion

		private void GUnitForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (_TestRunner.IsRunning())
			{
				DialogResult result = MessageBox.Show("Warning: Tests are still running. Are you sure you want to quit?", null, MessageBoxButtons.YesNo);
				if (result == DialogResult.No)
				{
					e.Cancel = true;
					return;
				}

				_TestRunner.Stop();
			}
		}
	}
}

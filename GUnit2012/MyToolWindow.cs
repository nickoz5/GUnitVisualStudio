using Pryda.Common;
using System;
using System.Collections;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Text;
using System.IO;
using System.IO.IsolatedStorage;
using System.Threading;
using System.Configuration;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio;
using Extensibility;
using EnvDTE;
using EnvDTE80;
using Microsoft.Win32.SafeHandles;

using VsPackage = Microsoft.VisualStudio.Shell.Package;



namespace Pryda.GUnit
{
  /// <summary>
  /// This class implements the tool window exposed by this package and hosts a user control.
  ///
  /// In Visual Studio tool windows are composed of a frame (implemented by the shell) and a pane, 
  /// usually implemented by the package implementer.
  ///
  /// This class derives from the ToolWindowPane class provided from the MPF in order to use its 
  /// implementation of the IVsWindowPane interface.
  /// </summary>
  [Guid("80a5c351-d0fa-4de6-99fc-9de062bc19b9")]
	public class MyToolWindow : ToolWindowPane
	{
		// This is the user control hosted by the tool window; it is exposed to the base class 
		// using the Window property. Note that, even if this class implements IDispose, we are
		// not calling Dispose on this object. This is because ToolWindowPane calls Dispose on 
		// the object returned by the Window property.
		private ctlTestList _TestControl;
		private TestRunner _TestRunner;

		IVsOutputWindowPane _OutputPane;
		Guid _OutputPaneGuid = new Guid("C89FA867-1593-44be-91B6-A2D1582E317D");

		DTE2 _dte = null;

		// CreatePipe() declaration
		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern bool CreatePipe(out SafeFileHandle hReadPipe, out SafeFileHandle hWritePipe, ref SECURITY_ATTRIBUTES lpPipeAttributes, int nSize);
		[StructLayout(LayoutKind.Sequential)]
		public struct SECURITY_ATTRIBUTES
		{
			public System.UInt32 nLength;
			public IntPtr lpSecurityDescriptor;
			[MarshalAs(UnmanagedType.Bool)]
			public bool bInheritHandle;
		}

		SafeFileHandle _WriteFileHandle = null;	// output from CreatePipe()
		SafeFileHandle _ReadFileHandle = null;	// output from CreatePipe()


		/// <summary>
		/// Standard constructor for the tool window.
		/// </summary>
		public MyToolWindow() :
				base(null)
		{
				// Set the window title reading it from the resources.
				this.Caption = MyResources.ToolWindowTitle;
				// Set the image that will appear on the tab of the window frame
				// when docked with an other window
				// The resource ID correspond to the one defined in the resx file
				// while the Index is the offset in the bitmap strip. Each image in
				// the strip being 16x16.
				this.BitmapResourceID = 301;
				this.BitmapIndex = 0;

				_dte = (DTE2)VsPackage.GetGlobalService(typeof(DTE));

				_TestControl = new ctlTestList();
				_TestControl.notifyProjectChanged = OnProjectChanged;
				_TestControl.notifyRunTests = OnRunTests;
				_TestControl.notifyStopTests = OnStopTests;
				_TestControl.notifyGoToTestCode = OnGoToTestCode;

				_TestRunner = new TestRunner();
				_TestRunner.TestListControl = _TestControl;
				_TestRunner.notifyReadLine = OnReadLine;

				IVsOutputWindow outputWindow = VsPackage.GetGlobalService(typeof(SVsOutputWindow)) as IVsOutputWindow;

				// If we fail to get it we can exit now.
				if (null != outputWindow)
				{
					outputWindow.CreatePane(
						ref _OutputPaneGuid,
						"Pryda Unit Test Output",
						Convert.ToInt32(true),
						Convert.ToInt32(true));

					// Retrieve the new pane.
					outputWindow.GetPane(ref _OutputPaneGuid, out _OutputPane);
				}

				loadSettings();
		}

		/// <summary>
		/// This property returns the handle to the user control that should
		/// be hosted in the Tool Window.
		/// </summary>
		override public IWin32Window Window
		{
				get
				{
						return (IWin32Window)_TestControl;
				}
		}

		public ctlTestList TestControl
		{
			get
			{
				return _TestControl;
			}
		}
		public TestRunner TestRunner
		{
			get
			{
				return _TestRunner;
			}
		}

		public void RefreshTestList()
		{
			_TestControl.listTests.Items.Clear();
			updateProjectOutputFile();
			if (!_TestRunner.CanRun())
				return;
			if (!_TestRunner.CurrentProjectName.Contains("UnitTest") && !_TestRunner.CurrentProjectName.Contains("RegressionTest"))
				return;
			_TestRunner.PopulateTestList();
			loadSettings();
		}

		public void CancelCurrentRun()
		{
			// make sure our pipe handles are closed. if the user stopped the debugging process
			// from the visual studio controls, the test runner is already stopped but the
			// handles are still open.
			if (_ReadFileHandle != null) _ReadFileHandle.Close();
			if (_WriteFileHandle != null) _WriteFileHandle.Close();
			_ReadFileHandle = null;
			_WriteFileHandle = null;

			// NOT null if the app was debugged
			if (_dte.Debugger.CurrentProcess != null)
			{
				if (_dte.Debugger.CurrentProcess.Name == _TestRunner.CurrentProjectName)
				{
					_dte.Debugger.Stop(false);

				}
			}

			_TestRunner.Stop();
		}

		public void RunEnabledTests()
		{
			OnRunTests(_TestControl.getSelectedTestsFilter(), false);
		}

		public void DebugEnabledTests()
		{
			OnRunTests(_TestControl.getSelectedTestsFilter(), true);
		}

		public void StopRun()
		{
			OnStopTests();
		}

		public void ShowTestResults()
		{
			_TestRunner.ShowTestResults(null);
		}

		#region Control callbacks
		private void OnProjectChanged(string projectName)
		{
			saveSettings();
			RefreshTestList();
		}

		public void OnReadLine(string line)
		{
			// Build the string to write on the debugger and output window.
			StringBuilder outputText = new StringBuilder(line);
			outputText.Append("\n");

			// Now print the string on the output window.
			if (Microsoft.VisualStudio.ErrorHandler.Failed(_OutputPane.OutputStringThreadSafe(outputText.ToString())))
			{
			  Trace.WriteLine("Failed to write on the output window");
			}
		}

		public void OnGoToTestCode(string testcase, string testname)
		{
			try
			{
				int iParamTest = testname.IndexOf('/');
				if (iParamTest > 0)
				{
					testname = testname.Substring(0, iParamTest);
				}

				// BUG: ExecuteCommand("Edit.GoToDefinition") requires that the definition is within
				// the currently selected project. To workaround this issue, we can progmatically select the
				// project in the solution explorer window.
				string currentProjectName = _TestControl.CurrentProjectName;
				Microsoft.VisualStudio.Shell.ServiceProvider sp =
						 new Microsoft.VisualStudio.Shell.ServiceProvider((Microsoft.VisualStudio.OLE.Interop.IServiceProvider)_dte);

				ProjectSelector selector = new ProjectSelector(sp);
				selector.SelectProjectByName(currentProjectName);

				// generate the test name from the case/unit test and use the Edit.GoToDefinition
				// command to find it in the active project.
				string args = testcase + "_" + testname + "_Test";
				_dte.ExecuteCommand("Edit.GoToDefinition", args);
			}
			catch (System.Exception ex)
			{
			}
		}

		public void OnStopTests()
		{
			saveSettings();
			CancelCurrentRun();
		}

		public void OnRunTests(string filter, bool debug)
		{
			saveSettings();

			updateProjectOutputFile();
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

			cls();

			_TestRunner.Filter = filter;

			// run the test app..
			StreamReader streamReader = null;
			if (debug)
			{
				streamReader = debugUnitTestExecutable();
				if (streamReader == null)
					return;
			}

			_OutputPane.Activate();	// show the output pane
			_TestRunner.Start(streamReader);
		}
		#endregion

		private EnvDTE.Project findProject(string projectName)
		{
			for (int i = 1; i <= _dte.Solution.Projects.Count; i++)
			{
				Project proj = _dte.Solution.Projects.Item(i);
				if (projectName.Equals(proj.Name))
				{
					return proj;
				}
			}
			return null;
		}

		public void updateProjectOutputFile()
		{
			_TestRunner.CurrentProjectName = "";

			if (_dte == null)
				return;

			string projectName = _TestControl.CurrentProjectName;

			try
			{
				Project proj = findProject(projectName);
				if (proj != null)
				{
					EnvDTE.Configuration config = proj.ConfigurationManager.ActiveConfiguration;
					object[] objFileNames = config.OutputGroups.Item(1).FileURLs as object[];
					string fileName = objFileNames[0] as string;
					System.Uri fileUri = new System.Uri(fileName);
					_TestRunner.CurrentProjectName = fileUri.LocalPath;
				}
			}
			catch (System.Exception ex)
			{
			}
		}

		public void LoadProjects()
		{
			if (_dte == null)
				return;

			// clear the combo
			_TestControl.ClearProjects();
			_TestRunner.CurrentProjectName = "";

			if (_dte.Solution != null)
			{
				for (int i = 1; i <= _dte.Solution.Projects.Count; i++)
				{
					Project proj = _dte.Solution.Projects.Item(i);
					
					int index = _TestControl.AddProject(proj.Name);
					if (index == -1)
						continue;

					if (proj.Name.Contains("UnitTest"))
					{
						_TestControl.CurrentProjectName = proj.Name;
					}
				}
			}

			RefreshTestList();
			loadSettings();
		}

		public void ClearProjects()
		{
			// clear the combo
			saveSettings();
			_TestControl.ClearProjects();
			_TestRunner.CurrentProjectName = "";
			RefreshTestList();
		}

		private void saveSettings()
		{
			ApplicationSettings storeData = new ApplicationSettings(TestControl);
			storeData.Save(_dte.Solution.FileName, _TestRunner.CurrentProjectName);
		}

		private void loadSettings()
		{
			ApplicationSettings storeData = new ApplicationSettings(_TestControl);
			storeData.Load(_dte.Solution.FileName, _TestRunner.CurrentProjectName);
		}

		private StreamReader debugUnitTestExecutable()
		{
			Microsoft.VisualStudio.Shell.ServiceProvider sp =
					 new Microsoft.VisualStudio.Shell.ServiceProvider((Microsoft.VisualStudio.OLE.Interop.IServiceProvider)_dte);

			StreamReader reader = null;
			IVsDebugger dbg = (IVsDebugger)sp.GetService(typeof(SVsShellDebugger));

			if (_dte.Debugger.CurrentProgram != null)
			{
				MessageBox.Show("The debugger is currently active. Please finish debugging the current program before continuing.");
				return reader;
			}

			VsDebugTargetInfo info = new VsDebugTargetInfo();
			info.cbSize = (uint)System.Runtime.InteropServices.Marshal.SizeOf(info);
			info.dlo = Microsoft.VisualStudio.Shell.Interop.DEBUG_LAUNCH_OPERATION.DLO_CreateProcess;
			info.bstrExe = _TestRunner.CurrentProjectName;
			info.bstrCurDir = System.IO.Path.GetDirectoryName(info.bstrExe);
			info.bstrArg = _TestRunner.BuildArgs(false);
			info.bstrRemoteMachine = null; // debug locally
			info.fSendStdoutToOutputWindow = 0; // Let stdout stay with the application.
			info.clsidCustom = VSConstants.DebugEnginesGuids.ManagedAndNative;// VSConstants.CLSID_ComPlusOnlyDebugEngine;
			info.grfLaunch = 0;
			info.fSendStdoutToOutputWindow = 1;

			// create pipe for debugging unit test app output
			SECURITY_ATTRIBUTES sa = new SECURITY_ATTRIBUTES();
			sa.nLength = (System.UInt32)System.Runtime.InteropServices.Marshal.SizeOf(sa);
			sa.lpSecurityDescriptor = IntPtr.Zero;
			sa.bInheritHandle = true;

			if (_ReadFileHandle != null) _ReadFileHandle.Close();
			if (_WriteFileHandle != null) _WriteFileHandle.Close();
			
			if (!CreatePipe(out _ReadFileHandle, out _WriteFileHandle, ref sa, 0))
			{
				int error = Marshal.GetLastWin32Error();
				MessageBox.Show("Error: Pipe not created. Debug output for unit testing is not available.\nLast error= " + error);
				return reader;
			}
			else
			{
				info.hStdOutput = _WriteFileHandle.DangerousGetHandle();
				reader = new StreamReader(new FileStream(_ReadFileHandle, FileAccess.Read, 0x1000, false), Console.OutputEncoding, true, 0x1000);
			}

			IntPtr pInfo = System.Runtime.InteropServices.Marshal.AllocCoTaskMem((int)info.cbSize);
			System.Runtime.InteropServices.Marshal.StructureToPtr(info, pInfo, false);

			try
			{
				dbg.LaunchDebugTargets(1, pInfo);
			}
			finally
			{
				if (pInfo != IntPtr.Zero)
				{
					System.Runtime.InteropServices.Marshal.FreeCoTaskMem(pInfo);
				}
			}

			return reader;
		}

		private void cls()
		{
			_TestRunner.ClearResults();
			_OutputPane.Clear();
		}
	}
}

// VsPkg.cs : Implementation of GUnit
//

using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.ComponentModel.Design;
using Microsoft.Win32;
using System.Windows.Forms;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio;
using Extensibility;
using EnvDTE;
using EnvDTE80;

namespace Pryda.GUnit
{
	/// <summary>
	/// This is the class that implements the package exposed by this assembly.
	///
	/// The minimum requirement for a class to be considered a valid package for Visual Studio
	/// is to implement the IVsPackage interface and register itself with the shell.
	/// This package uses the helper classes defined inside the Managed Package Framework (MPF)
	/// to do it: it derives from the Package class that provides the implementation of the 
	/// IVsPackage interface and uses the registration attributes defined in the framework to 
	/// register itself and its components with the shell.
	/// </summary>
	// This attribute tells the registration utility (regpkg.exe) that this class needs
	// to be registered as package.
	[PackageRegistration(UseManagedResourcesOnly = true)]
	// This attribute is used to register the informations needed to show the this package
	// in the Help/About dialog of Visual Studio.
	[InstalledProductRegistration(false, "#110", "#112", "1.0", IconResourceID = 400)]
	// In order be loaded inside Visual Studio in a machine that has not the VS SDK installed, 
	// package needs to have a valid load key (it can be requested at 
	// http://msdn.microsoft.com/vstudio/extend/). This attributes tells the shell that this 
	// package has a load key embedded in its resources.
	[ProvideLoadKey("Standard", "1.0", "GUnit", "Pryda", 1)]
	// This attribute is needed to let the shell know that this package exposes some menus.
	[ProvideMenuResource(1000, 1)]
	// This attribute registers a tool window exposed by this package.
	[ProvideToolWindow(typeof(MyToolWindow))]
	[Guid(GuidList.guidGUnitPkgString)]
	public sealed class GUnitPackage : Package, IVsSolutionEvents, IVsSolutionEvents4, IVsDebuggerEvents
	{

		private IVsSolution _Solution;
		private uint solutionEventsCookie;
		private uint debuggerEventsCookie;

		MyToolWindow _ToolWindow = null;

		/// <summary>
		/// Default constructor of the package.
		/// Inside this method you can place any initialization code that does not require 
		/// any Visual Studio service because at this point the package object is created but 
		/// not sited yet inside Visual Studio environment. The place to do all the other 
		/// initialization is the Initialize method.
		/// </summary>
		public GUnitPackage()
		{
			Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering constructor for: {0}", this.ToString()));

			//if ((null == window) || (null == window.Frame))
			//{
			//	throw new NotSupportedException(MyResources.CanNotCreateWindow);
			//}
		}

		/// <summary>
		/// This function is called when the user clicks the menu item that shows the 
		/// tool window. See the Initialize method to see how the menu item is associated to 
		/// this function using the OleMenuCommandService service and the MenuCommand class.
		/// </summary>
		private void ShowToolWindow(object sender, EventArgs e)
		{
			// Get the instance number 0 of this tool window. This window is single instance so this instance
			// is actually the only one.
			// The last flag is set to true so that if the tool window does not exists it will be created.
			if (_ToolWindow == null)
			{
				MyToolWindow window = (MyToolWindow)this.FindToolWindow(typeof(MyToolWindow), 0, true);
				if ((null == window) || (null == window.Frame))
				{
					throw new NotSupportedException(MyResources.CanNotCreateWindow);
				}

				_ToolWindow = window;
			}

			//_ToolWindow.packageObject = this;

			IVsWindowFrame windowFrame = (IVsWindowFrame)_ToolWindow.Frame;
			Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(windowFrame.Show());
		}

		/////////////////////////////////////////////////////////////////////////////
		// Overriden Package Implementation
		#region Package Members

		/// <summary>
		/// Initialization of the package; this method is called right after the package is sited, so this is the place
		/// where you can put all the initilaization code that rely on services provided by VisualStudio.
		/// </summary>
		protected override void Initialize()
		{
			Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering Initialize() of: {0}", this.ToString()));
			base.Initialize();

			// Add our command handlers for menu (commands must exist in the .vsct file)
			OleMenuCommandService mcs = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
			if (null != mcs)
			{
				// Create the command for the tool window
				CommandID toolwndCommandID = new CommandID(GuidList.guidGUnitCmdSet, (int)PkgCmdIDList.cmdidGUnitTool);
				MenuCommand menuToolWin = new MenuCommand(ShowToolWindow, toolwndCommandID);
				mcs.AddCommand(menuToolWin);

				// For each command we have to define its id that is a unique Guid/integer pair.
				CommandID id = new CommandID(GuidList.guidGUnitCmdSet, (int)PkgCmdIDList.cmdidRunEnabled);

				// Now create the OleMenuCommand object for this command. The EventHandler object is the
				// function that will be called when the user will select the command.
				OleMenuCommand command = new OleMenuCommand(new EventHandler(RunCommandCallback), id);
				// Add the command to the command service.
				mcs.AddCommand(command);

				id = new CommandID(GuidList.guidGUnitCmdSet, (int)PkgCmdIDList.cmdidDebugEnabled);
				command = new OleMenuCommand(new EventHandler(DebugCommandCallback), id);
				mcs.AddCommand(command);

				id = new CommandID(GuidList.guidGUnitCmdSet, (int)PkgCmdIDList.cmdidStopRun);
				command = new OleMenuCommand(new EventHandler(StopRunCommandCallback), id);
				mcs.AddCommand(command);

				id = new CommandID(GuidList.guidGUnitCmdSet, (int)PkgCmdIDList.cmdidShowResults);
				command = new OleMenuCommand(new EventHandler(ShowResultsCommandCallback), id);
				mcs.AddCommand(command);
			}

			_Solution = GetService(typeof(SVsSolution)) as IVsSolution;
			if (_Solution != null)
			{
				_Solution.AdviseSolutionEvents(this, out solutionEventsCookie);
			}

			DTE2 dte = (DTE2)GetGlobalService(typeof(DTE));
			Microsoft.VisualStudio.Shell.ServiceProvider sp =
					 new Microsoft.VisualStudio.Shell.ServiceProvider((Microsoft.VisualStudio.OLE.Interop.IServiceProvider)dte);
			IVsDebugger dbg = (IVsDebugger)sp.GetService(typeof(SVsShellDebugger));
			dbg.AdviseDebuggerEvents(this, out debuggerEventsCookie);

			try
			{
				MyToolWindow window = (MyToolWindow)this.FindToolWindow(typeof(MyToolWindow), 0, true);
				if ((null == window) || (null == window.Frame))
				{
					throw new NotSupportedException(MyResources.CanNotCreateWindow);
				}
				_ToolWindow = window;
			}
			catch (System.SystemException e)
			{

			}

			if (_ToolWindow != null)
				_ToolWindow.LoadProjects();
		}
		#endregion

		#region Command Handlers

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId = "Microsoft.Samples.VisualStudio.MenuCommands.MenuCommandsPackage.OutputCommandString(System.String)")]
		private void RunCommandCallback(object caller, EventArgs args)
		{
			if (_ToolWindow != null)
				_ToolWindow.RunEnabledTests();
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId = "Microsoft.Samples.VisualStudio.MenuCommands.MenuCommandsPackage.OutputCommandString(System.String)")]
		private void DebugCommandCallback(object caller, EventArgs args)
		{
			if (_ToolWindow != null)
				_ToolWindow.DebugEnabledTests();
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId = "Microsoft.Samples.VisualStudio.MenuCommands.MenuCommandsPackage.OutputCommandString(System.String)")]
		private void ShowResultsCommandCallback(object caller, EventArgs args)
		{
			if (_ToolWindow != null)
				_ToolWindow.ShowTestResults();
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId = "Microsoft.Samples.VisualStudio.MenuCommands.MenuCommandsPackage.OutputCommandString(System.String)")]
		private void StopRunCommandCallback(object caller, EventArgs args)
		{
			if (_ToolWindow != null)
				_ToolWindow.StopRun();
		}

		#endregion

		#region IVsSolutionEvents Members

		public int OnAfterCloseSolution(object pUnkReserved)
		{
			try
			{
				if (_ToolWindow != null)
					_ToolWindow.LoadProjects();
			}
			catch (System.Exception ex)
			{
			}

			return VSConstants.S_OK;
		}

		public int OnAfterLoadProject(IVsHierarchy pStubHierarchy, IVsHierarchy pRealHierarchy)
		{
			throw new NotImplementedException();
		}

		public int OnAfterOpenProject(IVsHierarchy pHierarchy, int fAdded)
		{
			try
			{
				object stringName;
				pHierarchy.GetProperty(VSConstants.VSITEMID_ROOT, (int)__VSHPROPID.VSHPROPID_Name, out stringName);
				if (stringName == null)
					return VSConstants.S_OK;

				int index = _ToolWindow.TestControl.AddProject((string)stringName);
			}
			catch (System.Exception ex)
			{
			}

			return VSConstants.S_OK;
		}

		public int OnAfterOpenSolution(object pUnkReserved, int fNewSolution)
		{
			//if (_ToolWindow != null)
			//  _ToolWindow.LoadProjects();
			return VSConstants.S_OK;
		}

		public int OnBeforeCloseProject(IVsHierarchy pHierarchy, int fRemoved)
		{
			if (_ToolWindow == null)
				return VSConstants.S_OK;
			
			try
			{
				object stringName;
				pHierarchy.GetProperty(VSConstants.VSITEMID_ROOT, (int)__VSHPROPID.VSHPROPID_Name, out stringName);
				if (stringName != null)
				{
					if (_ToolWindow.TestRunner.IsRunning() && _ToolWindow.TestControl.CurrentProjectName.Equals(stringName))
					{
						_ToolWindow.CancelCurrentRun();
					}

					_ToolWindow.TestControl.RemoveProject((string)stringName);
				}
			}
			catch (System.Exception ex)
			{
			}

			return VSConstants.S_OK;
		}

		public int OnBeforeCloseSolution(object pUnkReserved)
		{
			try
			{
				if (_ToolWindow != null)
					_ToolWindow.ClearProjects();
			}
			catch (System.Exception ex)
			{
			}
			return VSConstants.S_OK;
		}

		public int OnBeforeUnloadProject(IVsHierarchy pRealHierarchy, IVsHierarchy pStubHierarchy)
		{
			if (_ToolWindow == null)
				return VSConstants.S_OK;

			return VSConstants.S_OK;
		}

		public int OnQueryCloseProject(IVsHierarchy pHierarchy, int fRemoving, ref int pfCancel)
		{
			if (_ToolWindow == null)
				return VSConstants.S_OK;
			
			try
			{
				if (fRemoving == 1)
				{
					object stringName;
					pHierarchy.GetProperty(VSConstants.VSITEMID_ROOT, (int)__VSHPROPID.VSHPROPID_Name, out stringName);
					if (stringName != null)
					{
						if (_ToolWindow.TestControl.CurrentProjectName.Equals(stringName))
						{
							if (_ToolWindow.TestRunner.IsRunning())
							{
								MessageBox.Show("Error: Pryda unit tests are currently running.");
								pfCancel = 1;
							}
						}
					}
				}
			}
			catch (System.Exception ex)
			{
			}

			return VSConstants.S_OK;
		}

		public int OnQueryCloseSolution(object pUnkReserved, ref int pfCancel)
		{
			try
			{
				if (_ToolWindow.TestRunner.IsRunning())
				{
					MessageBox.Show("Error: Pryda unit tests are currently running.");
					pfCancel = 1;
					return VSConstants.S_OK;
				}
			}
			catch (System.Exception ex)
			{
			}

			return VSConstants.S_OK;
		}

		public int OnQueryUnloadProject(IVsHierarchy pRealHierarchy, ref int pfCancel)
		{
			if (_ToolWindow == null)
				return VSConstants.S_OK;
			
			try
			{
				object stringName;
				pRealHierarchy.GetProperty(VSConstants.VSITEMID_ROOT, (int)__VSHPROPID.VSHPROPID_Name, out stringName);
				if (stringName != null)
				{
					if (_ToolWindow.TestControl.CurrentProjectName.Equals(stringName))
					{
						if (_ToolWindow.TestRunner.IsRunning())
						{
							MessageBox.Show("Error: Pryda unit tests are currently running.");
							pfCancel = 1;
						}
					}
				}
			}
			catch (System.Exception ex)
			{
			}

			return VSConstants.S_OK;
		}

		#endregion

		#region IVsDebuggerEvents Members

		public int OnModeChange(DBGMODE dbgmodeNew)
		{
			switch (dbgmodeNew)
			{
				case DBGMODE.DBGMODE_Run:
					break;

				case DBGMODE.DBGMODE_Break:
					break;

				case DBGMODE.DBGMODE_Design:
					try
					{
						if (_ToolWindow != null)
							_ToolWindow.CancelCurrentRun();
					}
					catch (System.Exception ex)
					{
						MessageBox.Show("Error stopping current unit test run.");
					}
					break;
			}

			return VSConstants.S_OK;
		}

		#endregion

		#region IVsSolutionEvents3 Members


		public int OnAfterClosingChildren(IVsHierarchy pHierarchy)
		{
			throw new NotImplementedException();
		}

		public int OnAfterMergeSolution(object pUnkReserved)
		{
			throw new NotImplementedException();
		}

		public int OnAfterOpeningChildren(IVsHierarchy pHierarchy)
		{
			throw new NotImplementedException();
		}

		public int OnBeforeClosingChildren(IVsHierarchy pHierarchy)
		{
			throw new NotImplementedException();
		}

		public int OnBeforeOpeningChildren(IVsHierarchy pHierarchy)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IVsSolutionEvents4 Members

		public int OnAfterAsynchOpenProject(IVsHierarchy pHierarchy, int fAdded)
		{
			throw new NotImplementedException();
		}

		public int OnAfterChangeProjectParent(IVsHierarchy pHierarchy)
		{
			throw new NotImplementedException();
		}

		public int OnAfterRenameProject(IVsHierarchy pHierarchy)
		{
			throw new NotImplementedException();
		}

		public int OnQueryChangeProjectParent(IVsHierarchy pHierarchy, IVsHierarchy pNewParentHier, ref int pfCancel)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
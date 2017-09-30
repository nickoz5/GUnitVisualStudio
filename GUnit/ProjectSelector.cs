using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio;



namespace Pryda.GUnit
{
	/// <summary>
	/// Selects a project in the solution explorer toolwindow based on the name that is displayed for the project in that window.
	/// </summary>
	public class ProjectSelector
	{
		private System.IServiceProvider serviceProvider;
		private Microsoft.VisualStudio.Shell.Interop.IVsSolution solution;
		private Microsoft.VisualStudio.Shell.Interop.IVsUIShell shell;

		/// <summary>
		/// Initializes a new instance of the <see cref="ProjectSelector"/> class.
		/// </summary>
		/// <param name="serviceProvider">A <see cref="System.IServiceProvider"/> the class can use to fetch the VS services it needs.</param>
		/// <exception cref="ArgumentNullException">Throws this exception if <paramref name="serviceProvider"/> is null.</exception>
		public ProjectSelector(System.IServiceProvider serviceProvider)
		{
			if (serviceProvider == null)
			{
				throw new ArgumentNullException("serviceProvider");
			}

			this.serviceProvider = serviceProvider;
		}

		/// <summary>
		/// Selects the project node in the solution explorer whose <see cref="__VSHPROPID.VSHPROPID_Caption"/> property matches <paramref name="projectName"/>.
		/// </summary>
		/// <param name="projectName">The name of the project node to select.</param>
		/// <exception cref="ArgumentNullException">Throws this exception if <paramref name="projectName"/> is null.</exception>
		/// <exception cref="ArgumentException">Throws this exception if <paramref name="projectName"/> is the empty string or consists solely of whitespace, or if we fail to find a project node matching <paramref name="projectName"/>.</exception>
		/// <exception cref="COMException">Throws this for any COM call failures we encounter.</exception>
		public void SelectProjectByName(string projectName)
		{
			if (projectName == null)
			{
				throw new ArgumentNullException("projectName");
			}

			//if (String.IsNullOrWhiteSpace(projectName))
			//{
			//  throw new ArgumentException("The value given to SelectProjectByName for the parameter 'projectName' is the empty string or consists solely of whitespace.");
			//}

			//Figure out how many projects we have in the current solution
			uint projectCount = GetProjectCount();

			//Okay, now we know that a project with a matching name exists, we just need to find its IVsHierarchy.
			IVsHierarchy hierarchy = FindHierarchyByName(projectName, projectCount);
			if (hierarchy == null)
			{
				throw new ArgumentException(String.Format("Failed to find an IVsHierarchy with the Caption property of '{0}'.", projectName));
			}

			object rawHierarchyWindow;
			IVsUIHierarchyWindow hierarchyWindow;
			IVsUIHierarchy uiHierarchy;


			//Now we need to get the class view window, and get its associated IVsUIHierarchyWindow so we can programatically
			//tell it to select the IVsUIHierarchy we have found.
			//Guid classViewGuid = new Guid(EnvDTE.Constants.vsWindowKindClassView);
			//IVsWindowFrame classWindowFrame;
			//ErrorHandler.ThrowOnFailure(Shell.FindToolWindow((int)__VSFINDTOOLWIN.FTW_fForceCreate, ref classViewGuid, out classWindowFrame));

			//ErrorHandler.ThrowOnFailure(classWindowFrame.GetProperty((int)__VSFPROPID.VSFPROPID_DocView, out rawHierarchyWindow));
			//hierarchyWindow = (IVsUIHierarchyWindow)rawHierarchyWindow;

			//Finally tell the hierarchy window to select the UI hierarchy we have found.
			//uiHierarchy = (IVsUIHierarchy)hierarchy;
			//ErrorHandler.ThrowOnFailure(hierarchyWindow.ExpandItem(uiHierarchy, (uint)VSConstants.VSITEMID_ROOT, EXPANDFLAGS.EXPF_SelectItem));


			//Now we need to get the solution explorer window, and get its associated IVsUIHierarchyWindow so we can programatically
			//tell it to select the IVsUIHierarchy we have found.
			Guid slnExplorerGuid = new Guid(EnvDTE.Constants.vsWindowKindSolutionExplorer);
			IVsWindowFrame solutionWindowFrame;
			ErrorHandler.ThrowOnFailure(Shell.FindToolWindow((int)__VSFINDTOOLWIN.FTW_fForceCreate, ref slnExplorerGuid, out solutionWindowFrame));

			ErrorHandler.ThrowOnFailure(solutionWindowFrame.GetProperty((int)__VSFPROPID.VSFPROPID_DocView, out rawHierarchyWindow));
			hierarchyWindow = (IVsUIHierarchyWindow)rawHierarchyWindow;

			//Finally tell the hierarchy window to select the UI hierarchy we have found.
			uiHierarchy = (IVsUIHierarchy)hierarchy;
			ErrorHandler.ThrowOnFailure(hierarchyWindow.ExpandItem(uiHierarchy, (uint)VSConstants.VSITEMID_ROOT, EXPANDFLAGS.EXPF_SelectItem));
		}

		/// <summary>
		/// Gets the <see cref="IVsSolution"/> object from Visual Studio.
		/// </summary>
		private IVsSolution Solution
		{
			get
			{
				if (this.solution == null)
				{
					this.solution = (IVsSolution)this.serviceProvider.GetService(typeof(SVsSolution));
				}

				return this.solution;
			}
		}

		/// <summary>
		/// Gets the <see cref="IVsUIShell"/> object from Visual Studio.
		/// </summary>
		private IVsUIShell Shell
		{
			get
			{
				if (this.shell == null)
				{
					this.shell = (IVsUIShell)this.serviceProvider.GetService(typeof(SVsUIShell));
				}

				return this.shell;
			}
		}

		/// <summary>
		/// Gets the total count of loaded projects in the current solution.
		/// </summary>
		/// <returns>The total count of loaded projects in the current solution.</returns>
		/// <exception cref="COMException">Throws this for any COM call failures we encounter.</exception>
		private uint GetProjectCount()
		{
			uint projectCount;
			ErrorHandler.ThrowOnFailure(Solution.GetProjectFilesInSolution((uint)__VSGETPROJFILESFLAGS.GPFF_SKIPUNLOADEDPROJECTS,
																			0 /*arraySize*/,
																			null /*projectNameArray*/,
																			out projectCount));
			return projectCount;
		}

		/// <summary>
		/// Finds an <see cref="IVsHierarchy"/> whose <see cref="__VSHPROPID.VSHPROPID_Caption"/> property matches <paramref name="projectName"/>.
		/// </summary>
		/// <param name="projectName">The name of the project (as displayed in the solution explorer window) that you want to locate.</param>
		/// <param name="projectCount">The total count of projects in the solution.</param>
		/// <returns>An <see cref="IVsHierarchy"/> whose <see cref="__VSHPROPID.VSHPROPID_Caption"/> property matches <paramref name="projectName"/> or null if one could not be found.</returns>
		/// <exception cref="ArgumentException">Throws this if the hierarchy enumerator returns a number of hierarchies that differs from <paramref name="projectCount"/>.</exception>
		/// <exception cref="COMException">Throws this for any COM call failures we encounter.</exception>
		private IVsHierarchy FindHierarchyByName(string projectName, uint projectCount)
		{
			Guid empty = Guid.Empty;
			IEnumHierarchies hierarchyEnumerator;
			ErrorHandler.ThrowOnFailure(Solution.GetProjectEnum((uint)__VSENUMPROJFLAGS.EPF_LOADEDINSOLUTION, ref empty /*ignored*/, out hierarchyEnumerator));
			ErrorHandler.ThrowOnFailure(hierarchyEnumerator.Reset());

			IVsHierarchy[] hierarchies = new IVsHierarchy[projectCount];
			uint hierarchiesFetched;
			ErrorHandler.ThrowOnFailure(hierarchyEnumerator.Next((uint)hierarchies.Length, hierarchies, out hierarchiesFetched));
			if (hierarchiesFetched != projectCount)
			{
				throw new ArgumentException(String.Format("Attempted to fetch {0} project from the IEnumHierarchies object but it returned only {1}.",
														 projectCount.ToString(), hierarchiesFetched.ToString()));
			}

			foreach (IVsHierarchy hierarchy in hierarchies)
			{
				object caption;
				ErrorHandler.ThrowOnFailure(hierarchy.GetProperty((uint)VSConstants.VSITEMID_ROOT,
																 (int)__VSHPROPID.VSHPROPID_Caption,
																 out caption));

				string captionAsString = caption as string;
				if ((captionAsString != null) && String.Equals(captionAsString, projectName, StringComparison.OrdinalIgnoreCase))
				{
					return hierarchy;
				}
			}

			return null;
		}
	}
}

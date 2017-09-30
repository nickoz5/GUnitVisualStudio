using System.Collections.Generic;
using System.Security.Permissions;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using System.Drawing;
using System;

namespace Pryda.Common
{
	public delegate void ProjectChanged(string projectName);
	public delegate void RunTests(string filter, bool debug);
	public delegate void StopTests();
	public delegate void GoToTestCode(string testcase, string testname);
	public delegate void ViewTestResults(List<string> tests);

	/// <summary>
	/// Summary description for MyControl.
	/// </summary>
	public partial class ctlTestList : UserControl
	{
		public ProjectChanged notifyProjectChanged;
		public RunTests notifyRunTests;
		public StopTests notifyStopTests;
		public GoToTestCode notifyGoToTestCode;
		public ViewTestResults notifyViewResults;

		public const string RESULT_IN_PROGRESS = "In Progress";
		public const string RESULT_PENDING = "Pending";
		public const string RESULT_ABORTED = "Aborted";
		public const string RESULT_FAILED = "Failed";
		public const string RESULT_PARTIAL_FAILED = "Partial Failed";
		public const string RESULT_PASSED = "Passed";


		public bool shuffleTests = false;
		public bool disabledTests = false;
		public bool breakOnFailure = false;
		public bool throwOnFailure = false;
		public bool catchExceptions = true;

		private bool _DebuggingToolsEnabled = true;
		public bool DebuggingToolsEnabled
		{
			get { return _DebuggingToolsEnabled; }
			set
			{
				_DebuggingToolsEnabled = value;
				if (!_DebuggingToolsEnabled)
				{
					// remove debugging menu/toolbar items.
					debugSelectionToolStripMenuItem.Visible = false;
					debugSelectionToolStripMenuItem.Enabled = false;
				}
			}
		}

		private ListViewColumnSorter lvwColumnSorter;

		public ctlTestList()
		{
			InitializeComponent();

			lvwColumnSorter = new ListViewColumnSorter();
			listTests.ListViewItemSorter = lvwColumnSorter;
		}

		public string CurrentProjectName
		{
			get
			{
				return toolStripProjects.Text;
			}
			set
			{
				if (value == null || value == "")
				{
					//toolStripProjects.SelectedIndex
					toolStripProjects.SelectedItem = null;
				}
				else
				{
					int index = toolStripProjects.FindString(value);
					if (index != -1)
					{
						toolStripProjects.SelectedIndex = index;
					}
				}

				if (notifyProjectChanged != null)
					notifyProjectChanged(this.CurrentProjectName);
			}
		}

		public int RandomSeed
		{
			get
			{
				int Value = 0;
				try
				{
					Value = Int32.Parse(toolStripRandomSeed.Text);
				}
				catch (System.FormatException ex)
				{
				}
				catch (System.ArgumentNullException ex)
				{
				}
				catch (System.OverflowException ex)
				{
				}

				return Value;
			}

			set
			{
				toolStripRandomSeed.Text = value.ToString();
			}
		}
		
		public int RepeatCount
		{
			get
			{
				int repeatCount = 0;
				try
				{
					repeatCount = System.Int32.Parse(toolStripTextRepeat.Text);
				}
				catch (System.FormatException ex)
				{

				}
				catch (System.ArgumentNullException ex)
				{

				}
				catch (System.OverflowException ex)
				{

				}

				return repeatCount;
			}

			set
			{
				toolStripTextRepeat.Text = value.ToString();
			}

		}

		public int AddProject(string projectName)
		{
			if (projectName.Equals("Solution Items") || projectName.Equals("Miscellaneous Files"))
				return -1;

			int index = toolStripProjects.FindString(projectName);
			if (index == -1)
				index = toolStripProjects.Items.Add(projectName);

			return index;
		}

		public void RemoveProject(string projectName)
		{
			int index = toolStripProjects.FindString(projectName);
			if (index == -1)
				return;

			if (toolStripProjects.SelectedIndex == index)
				CurrentProjectName = null;

			object item = toolStripProjects.Items[index];
			toolStripProjects.Items.Remove(item);
		}

		public void ClearProjects()
		{
			toolStripProjects.Items.Clear();
			if (notifyProjectChanged != null)
				notifyProjectChanged(this.CurrentProjectName);
		}

		/// <summary> 
		/// Let this control process the mnemonics.
		/// </summary>
		[UIPermission(SecurityAction.LinkDemand, Window = UIPermissionWindow.AllWindows)]
		protected override bool ProcessDialogChar(char charCode)
		{
			// If we're the top-level form or control, we need to do the mnemonic handling
			if (charCode != ' ' && ProcessMnemonic(charCode))
			{
				return true;
			}
			return base.ProcessDialogChar(charCode);
		}

		/// <summary>
		/// Enable the IME status handling for this control.
		/// </summary>
		protected override bool CanEnableIme
		{
			get
			{
				return true;
			}
		}

		public int findListViewItem(string testcase, string unittest)
		{
			foreach (ListViewItem lvi in listTests.Items)
			{
				if (lvi.SubItems[1].Text == testcase)
				{
					if (lvi.SubItems[2].Text == unittest)
						return lvi.Index;
				}
			}

			return -1;
		}

		public void cls()
		{
			foreach (ListViewItem lvi in listTests.Items)
			{
				lvi.SubItems[3].Text = "";
				lvi.SubItems[4].Text = "";
			}
			listTests.Refresh();
		}


		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions")]
		private void toolStripProjects_Click(object sender, System.EventArgs e)
		{
			// combo changed..
			if (notifyProjectChanged != null)
				notifyProjectChanged(toolStripProjects.Text);
		}

		public bool isTestEnabled(ListViewItem lvi)
		{
			return (!lvi.SubItems[0].Text.Equals("true")) ? false : true;
		}

		public void setTestEnabled(ListViewItem lvi, bool Enabled)
		{
			if (Enabled)
				lvi.SubItems[0].Text = "true";
			else
				lvi.SubItems[0].Text = "false";
		}

		public string getEnabledTestsFilter()
		{
			string filter = "";
			bool allEnabled = true;
			string filterEnabled = "";
			string filterDisabled = "";


			foreach (ListViewItem lvi in listTests.Items)
			{
				if (!isTestEnabled(lvi))
				{
					allEnabled = false;
					//if (filterDisabled.Length > 0) filterDisabled += ":";
					//filterDisabled += "-" + getItemTestCaseName(lvi.Index);
				}
				else
				{
					if (filterEnabled.Length > 0) filterEnabled += ":";
					filterEnabled += getItemTestCaseName(lvi.Index);
				}
			}

			if (!allEnabled)
			{
				if (filterEnabled.Length == 0)
					return filter;

				// use the shortest lengthed string..
				//filter = (filterEnabled.Length < filterDisabled.Length) ? filterEnabled : filterDisabled;
				filter = filterEnabled;
			}

			return filter;
		}

		public string getItemTestCaseName(int index)
		{
			ListViewItem lvi = listTests.Items[index];
			string testCaseName = lvi.SubItems[1].Text + "." + lvi.SubItems[2].Text;
			return testCaseName;
		}

		public string getSelectedTestsFilter()
		{
			string filter = "";
			foreach (ListViewItem lvi in listTests.SelectedItems)
			{
				filter += getItemTestCaseName(lvi.Index);
				if (lvi != listTests.SelectedItems[listTests.SelectedItems.Count - 1])
					filter += ":";
			}

			return filter;
		}


		private void listTests_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
		{
			if (sender == listTests)
			{

			}
		}

		//private void toolStipBrowse_Click(object sender, System.EventArgs e)
		//{
		//  DialogResult r = openFileDialog1.ShowDialog();
		//  if (DialogResult.OK == r)
		//  {
		//    toolStripProjects.Text = openFileDialog1.FileName;
		//    toolStripProjects.Items.Add(openFileDialog1.FileName);
		//    notifyProjectChanged(toolStripProjects.Text);
		//  }
		//}

		private void toolStripRefresh_Click(object sender, System.EventArgs e)
		{
			if (notifyProjectChanged != null)
				notifyProjectChanged(toolStripProjects.Text);
		}

		private void runSelected(string filter)
		{
			if (notifyRunTests != null)
				notifyRunTests(filter, false);
		}

		private void debugSelected(string filter)
		{
			if (notifyRunTests != null)
				notifyRunTests(filter, true);
		}

		private void listTests_DrawSubItem(object sender, DrawListViewSubItemEventArgs e)
		{
			Graphics g = e.Graphics;
			Rectangle rect = e.Bounds;

			if (e.ColumnIndex == 0)
			{
				bool isChecked = e.Item.SubItems[0].Text.Equals("true");

				CheckBoxRenderer.DrawCheckBox(g, new Point(
					rect.Left + 3,
					rect.Top + 3),
					isChecked ? CheckBoxState.CheckedNormal : CheckBoxState.UncheckedNormal);
			}
			else //if (e.ColumnIndex == 1 || e.ColumnIndex == 3)
			{
				e.DrawBackground();

        Color fore = e.SubItem.ForeColor;

				// Unless the item is selected, draw the standard 
				// background to make it stand out from the gradient.
				if (e.Item.Selected)
				{
					if (listTests.Focused)
					{
						g.FillRectangle(SystemBrushes.Highlight, rect);
						fore = SystemColors.HighlightText;
					}
					else
						g.FillRectangle(SystemBrushes.ButtonFace, rect);
				}

				int iconIndex = -1;
				if (e.ColumnIndex == 3)
				{
					switch (e.Item.SubItems[3].Text)
					{
						case RESULT_PASSED:
							iconIndex = imageList1.Images.IndexOfKey("passed");
							break;
						case RESULT_IN_PROGRESS:
							iconIndex = imageList1.Images.IndexOfKey("inprogress");
							break;
						case RESULT_PENDING:
							iconIndex = imageList1.Images.IndexOfKey("pending");
							break;
						case RESULT_PARTIAL_FAILED:
						case RESULT_ABORTED:
						case RESULT_FAILED:
							iconIndex = imageList1.Images.IndexOfKey("failed");
							break;
					}
				}
				else if (e.ColumnIndex == 1)
				{
					iconIndex = imageList1.Images.IndexOfKey("test");
				}

				if (iconIndex != -1)
				{
					imageList1.Draw(g, new Point(rect.Left + 2, rect.Top), iconIndex);
					rect.X += imageList1.ImageSize.Width + 4;
				}

				if (e.ColumnIndex != 0)
				{
					TextRenderer.DrawText(e.Graphics, e.SubItem.Text, listTests.Font, rect, fore, TextFormatFlags.GlyphOverhangPadding | TextFormatFlags.VerticalCenter);
				}

				if (e.Item.Focused)
					e.DrawFocusRectangle(rect);
			}
		}

		private void listTests_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
		{
			e.DrawDefault = true;
		}

		private void listTests_MouseClick(object sender, MouseEventArgs e)
		{
			ListViewHitTestInfo hitinfo = listTests.HitTest(e.X, e.Y);
			ListViewItem.ListViewSubItem subItem = hitinfo.SubItem;

			int colindex = hitinfo.Item.SubItems.IndexOf(subItem);

			if (colindex == 0)
			{
				foreach (ListViewItem item in listTests.SelectedItems)
				{
					item.SubItems[0].Text = (item.SubItems[0].Text.Equals("true")) ? "false" : "true";
				}

				listTests.Invalidate();
			}
		}

		private void listTests_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			ListViewItem listItem = listTests.HitTest(e.X, e.Y).Item;
			if (notifyGoToTestCode != null)
				notifyGoToTestCode(listItem.SubItems[1].Text, listItem.SubItems[2].Text);
		}

		private void listTests_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.A && e.Control)
			{
				listTests.MultiSelect = true;
				foreach (ListViewItem item in listTests.Items)
				{
					item.Selected = true;
				}
			}
		}

		private void clearResultsToolStripMenuItem_Click(object sender, System.EventArgs e)
		{
			cls();
		}

		private void shuffleTestsToolStripMenuItem_Click(object sender, System.EventArgs e)
		{
			shuffleTests = !shuffleTests;}

		private void runDisabledTestsToolStripMenuItem_Click(object sender, System.EventArgs e)
		{
			disabledTests = !disabledTests;
		}

		private void contextMenuRunSelected_Click(object sender, System.EventArgs e)
		{
			runSelected(getSelectedTestsFilter());
		}
		private void debugSelectedTestsToolStripMenuItem_Click(object sender, System.EventArgs e)
		{
			debugSelected(getSelectedTestsFilter());
		}

		private void runSelectionToolStripMenuItem_Click(object sender, System.EventArgs e)
		{
			runSelected(getEnabledTestsFilter());
		}
		private void debugSelectionToolStripMenuItem_Click(object sender, System.EventArgs e)
		{
			debugSelected(getEnabledTestsFilter());
		}

		private void toolStripButtonStop_Click(object sender, System.EventArgs e)
		{
			if (notifyStopTests != null)
				notifyStopTests();
		}

		private void listTests_ColumnClick(object sender, ColumnClickEventArgs e)
		{
			// Determine if clicked column is already the column that is being sorted.
			if (e.Column == lvwColumnSorter.SortColumn)
			{
				// Reverse the current sort direction for this column.
				if (lvwColumnSorter.Order == SortOrder.Ascending)
				{
					lvwColumnSorter.Order = SortOrder.Descending;
				}
				else
				{
					lvwColumnSorter.Order = SortOrder.Ascending;
				}
			}
			else
			{
				// Set the column number that is to be sorted; default to ascending.
				lvwColumnSorter.SortColumn = e.Column;
				lvwColumnSorter.Order = SortOrder.Ascending;
			}

			// Perform the sort with these new sort options.
			listTests.Sort();
		}

		private void ctlTestList_Load(object sender, System.EventArgs e)
		{
		}

		private void toolStripResultsReport_Click_1(object sender, System.EventArgs e)
		{
			if (notifyViewResults != null)
				notifyViewResults(null);
		}

		private void showSelectionResultsToolStripMenuItem_Click(object sender, System.EventArgs e)
		{
			List<string> tests = null;
			if (listTests.SelectedItems.Count > 0)
			{
				tests = new List<string>();
				foreach (ListViewItem lvi in listTests.SelectedItems)
				{
					string testname = lvi.SubItems[1].Text + "." + lvi.SubItems[2].Text;
					tests.Add(testname);
				}
			}

			if (notifyViewResults != null)
				notifyViewResults(tests);
		}

		private void openTestToolStripMenuItem_Click(object sender, System.EventArgs e)
		{			
			if (listTests.SelectedItems.Count == 1)
			{
				ListViewItem listItem = listTests.SelectedItems[0];
				if (listItem == null)
					return;

				if (notifyGoToTestCode != null)
					notifyGoToTestCode(listItem.SubItems[1].Text, listItem.SubItems[2].Text);
			}
		}

		private void contextMenuStrip1_Opening(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (listTests.SelectedItems.Count > 0)
			{
				runSelectedTestsToolStripMenuItem.Enabled = true;
				debugSelectedTestsToolStripMenuItem.Enabled = true;
				showSelectionResultsToolStripMenuItem.Enabled = true;
			}
			else
			{
				runSelectedTestsToolStripMenuItem.Enabled = false;
				debugSelectedTestsToolStripMenuItem.Enabled = false;
				showSelectionResultsToolStripMenuItem.Enabled = false;
			}

			openTestToolStripMenuItem.Enabled = (listTests.SelectedItems.Count == 1) ? true : false;
			clearResultsToolStripMenuItem.Enabled = (listTests.Items.Count > 0) ? true : false;

			if (!_DebuggingToolsEnabled)
			{
				openTestToolStripMenuItem.Visible = false;
				openTestToolStripMenuItem.Enabled = false;
				debugSelectedTestsToolStripMenuItem.Visible = false;
				debugSelectedTestsToolStripMenuItem.Enabled = false;
				toolStripMenuItem1.Visible = false;
			}
		}

		private void randomSeedToolStripMenuItem_Click(object sender, System.EventArgs e)
		{

		}

		private void toolStripDropDownButton1_DropDownOpening(object sender, EventArgs e)
		{
			randomSeedToolStripMenuItem.Enabled = shuffleTests;

			runDisabledTestsToolStripMenuItem.Checked = disabledTests;
			breakOnFailureToolStripMenuItem.Checked = breakOnFailure;
			catchExceptionsToolStripMenuItem.Checked = catchExceptions;
			throwOnFailureToolStripMenuItem.Checked = throwOnFailure;
			shuffleTestsToolStripMenuItem.Checked = shuffleTests;
		}

		private void breakOnFailureToolStripMenuItem_Click(object sender, EventArgs e)
		{
			breakOnFailure = !breakOnFailure;
		}

		private void catchExceptionsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			catchExceptions = !catchExceptions;
		}

		private void throwOnFailureToolStripMenuItem_Click(object sender, EventArgs e)
		{
			throwOnFailure = !throwOnFailure;
		}
	}
}

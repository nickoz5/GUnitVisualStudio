namespace Pryda.Common
{
    partial class ctlTestList
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose( bool disposing )
        {
            if( disposing )
            {
                if(components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose( disposing );
        }


        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
					this.components = new System.ComponentModel.Container();
					System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ctlTestList));
					this.listTests = new System.Windows.Forms.ListView();
					this.columnEnabled = new System.Windows.Forms.ColumnHeader();
					this.columnTestCase = new System.Windows.Forms.ColumnHeader();
					this.columnTest = new System.Windows.Forms.ColumnHeader();
					this.columnResult = new System.Windows.Forms.ColumnHeader();
					this.columnDuration = new System.Windows.Forms.ColumnHeader();
					this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
					this.runSelectedTestsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
					this.debugSelectedTestsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
					this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
					this.openTestToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
					this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
					this.showSelectionResultsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
					this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
					this.clearResultsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
					this.imageList1 = new System.Windows.Forms.ImageList(this.components);
					this.toolStrip1 = new System.Windows.Forms.ToolStrip();
					this.toolStripGo = new System.Windows.Forms.ToolStripSplitButton();
					this.runSelectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
					this.debugSelectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
					this.toolStripButtonStop = new System.Windows.Forms.ToolStripButton();
					this.toolStripResultsReport = new System.Windows.Forms.ToolStripButton();
					this.toolStripProjects = new System.Windows.Forms.ToolStripComboBox();
					this.toolStripRefresh = new System.Windows.Forms.ToolStripButton();
					this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
					this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
					this.toolStripTextRepeat = new System.Windows.Forms.ToolStripTextBox();
					this.toolStripDropDownButton1 = new System.Windows.Forms.ToolStripDropDownButton();
					this.shuffleTestsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
					this.randomSeedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
					this.toolStripRandomSeed = new System.Windows.Forms.ToolStripTextBox();
					this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripSeparator();
					this.runDisabledTestsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
					this.toolStripMenuItem5 = new System.Windows.Forms.ToolStripSeparator();
					this.breakOnFailureToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
					this.throwOnFailureToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
					this.catchExceptionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
					this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
					this.toolStripMenuItem6 = new System.Windows.Forms.ToolStripMenuItem();
					this.toolStripMenuItem7 = new System.Windows.Forms.ToolStripMenuItem();
					this.toolStripMenuItem8 = new System.Windows.Forms.ToolStripSeparator();
					this.contextMenuStrip1.SuspendLayout();
					this.toolStrip1.SuspendLayout();
					this.SuspendLayout();
					// 
					// listTests
					// 
					this.listTests.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
											| System.Windows.Forms.AnchorStyles.Left)
											| System.Windows.Forms.AnchorStyles.Right)));
					this.listTests.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnEnabled,
            this.columnTestCase,
            this.columnTest,
            this.columnResult,
            this.columnDuration});
					this.listTests.ContextMenuStrip = this.contextMenuStrip1;
					this.listTests.FullRowSelect = true;
					this.listTests.GridLines = true;
					this.listTests.HideSelection = false;
					this.listTests.Location = new System.Drawing.Point(0, 28);
					this.listTests.Name = "listTests";
					this.listTests.OwnerDraw = true;
					this.listTests.Size = new System.Drawing.Size(734, 122);
					this.listTests.SmallImageList = this.imageList1;
					this.listTests.TabIndex = 0;
					this.listTests.UseCompatibleStateImageBehavior = false;
					this.listTests.View = System.Windows.Forms.View.Details;
					this.listTests.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listTests_MouseDoubleClick);
					this.listTests.DrawColumnHeader += new System.Windows.Forms.DrawListViewColumnHeaderEventHandler(this.listTests_DrawColumnHeader);
					this.listTests.MouseClick += new System.Windows.Forms.MouseEventHandler(this.listTests_MouseClick);
					this.listTests.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.listTests_ColumnClick);
					this.listTests.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.listTests_ItemSelectionChanged);
					this.listTests.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listTests_KeyDown);
					this.listTests.DrawSubItem += new System.Windows.Forms.DrawListViewSubItemEventHandler(this.listTests_DrawSubItem);
					// 
					// columnEnabled
					// 
					this.columnEnabled.Tag = "Enabled";
					this.columnEnabled.Text = "";
					this.columnEnabled.Width = 19;
					// 
					// columnTestCase
					// 
					this.columnTestCase.Tag = "TestCaseName";
					this.columnTestCase.Text = "Test Case";
					this.columnTestCase.Width = 148;
					// 
					// columnTest
					// 
					this.columnTest.Tag = "TestName";
					this.columnTest.Text = "Test Name";
					this.columnTest.Width = 272;
					// 
					// columnResult
					// 
					this.columnResult.Tag = "Result";
					this.columnResult.Text = "Result";
					this.columnResult.Width = 130;
					// 
					// columnDuration
					// 
					this.columnDuration.Tag = "Duration";
					this.columnDuration.Text = "Duration";
					this.columnDuration.Width = 131;
					// 
					// contextMenuStrip1
					// 
					this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.runSelectedTestsToolStripMenuItem,
            this.debugSelectedTestsToolStripMenuItem,
            this.toolStripMenuItem1,
            this.openTestToolStripMenuItem,
            this.toolStripMenuItem3,
            this.showSelectionResultsToolStripMenuItem,
            this.toolStripMenuItem2,
            this.clearResultsToolStripMenuItem});
					this.contextMenuStrip1.Name = "contextMenuStrip1";
					this.contextMenuStrip1.Size = new System.Drawing.Size(195, 132);
					this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
					// 
					// runSelectedTestsToolStripMenuItem
					// 
					this.runSelectedTestsToolStripMenuItem.Image = global::Pryda.Common.MyResources.start;
					this.runSelectedTestsToolStripMenuItem.Name = "runSelectedTestsToolStripMenuItem";
					this.runSelectedTestsToolStripMenuItem.Size = new System.Drawing.Size(194, 22);
					this.runSelectedTestsToolStripMenuItem.Text = "Run Selected Tests";
					this.runSelectedTestsToolStripMenuItem.Click += new System.EventHandler(this.contextMenuRunSelected_Click);
					// 
					// debugSelectedTestsToolStripMenuItem
					// 
					this.debugSelectedTestsToolStripMenuItem.Image = global::Pryda.Common.MyResources.debug;
					this.debugSelectedTestsToolStripMenuItem.Name = "debugSelectedTestsToolStripMenuItem";
					this.debugSelectedTestsToolStripMenuItem.Size = new System.Drawing.Size(194, 22);
					this.debugSelectedTestsToolStripMenuItem.Text = "Debug Selected Tests";
					this.debugSelectedTestsToolStripMenuItem.Click += new System.EventHandler(this.debugSelectedTestsToolStripMenuItem_Click);
					// 
					// toolStripMenuItem1
					// 
					this.toolStripMenuItem1.Name = "toolStripMenuItem1";
					this.toolStripMenuItem1.Size = new System.Drawing.Size(191, 6);
					// 
					// openTestToolStripMenuItem
					// 
					this.openTestToolStripMenuItem.Name = "openTestToolStripMenuItem";
					this.openTestToolStripMenuItem.Size = new System.Drawing.Size(194, 22);
					this.openTestToolStripMenuItem.Text = "Open Test";
					this.openTestToolStripMenuItem.Click += new System.EventHandler(this.openTestToolStripMenuItem_Click);
					// 
					// toolStripMenuItem3
					// 
					this.toolStripMenuItem3.Name = "toolStripMenuItem3";
					this.toolStripMenuItem3.Size = new System.Drawing.Size(191, 6);
					// 
					// showSelectionResultsToolStripMenuItem
					// 
					this.showSelectionResultsToolStripMenuItem.Image = global::Pryda.Common.MyResources.results;
					this.showSelectionResultsToolStripMenuItem.Name = "showSelectionResultsToolStripMenuItem";
					this.showSelectionResultsToolStripMenuItem.Size = new System.Drawing.Size(194, 22);
					this.showSelectionResultsToolStripMenuItem.Text = "Show Selection Results";
					this.showSelectionResultsToolStripMenuItem.Click += new System.EventHandler(this.showSelectionResultsToolStripMenuItem_Click);
					// 
					// toolStripMenuItem2
					// 
					this.toolStripMenuItem2.Name = "toolStripMenuItem2";
					this.toolStripMenuItem2.Size = new System.Drawing.Size(191, 6);
					// 
					// clearResultsToolStripMenuItem
					// 
					this.clearResultsToolStripMenuItem.Name = "clearResultsToolStripMenuItem";
					this.clearResultsToolStripMenuItem.Size = new System.Drawing.Size(194, 22);
					this.clearResultsToolStripMenuItem.Text = "Clear All Results";
					this.clearResultsToolStripMenuItem.Click += new System.EventHandler(this.clearResultsToolStripMenuItem_Click);
					// 
					// imageList1
					// 
					this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
					this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
					this.imageList1.Images.SetKeyName(0, "failed");
					this.imageList1.Images.SetKeyName(1, "passed");
					this.imageList1.Images.SetKeyName(2, "pending");
					this.imageList1.Images.SetKeyName(3, "inprogress");
					this.imageList1.Images.SetKeyName(4, "test");
					// 
					// toolStrip1
					// 
					this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripGo,
            this.toolStripButtonStop,
            this.toolStripResultsReport,
            this.toolStripProjects,
            this.toolStripRefresh,
            this.toolStripSeparator1,
            this.toolStripLabel1,
            this.toolStripTextRepeat,
            this.toolStripDropDownButton1});
					this.toolStrip1.Location = new System.Drawing.Point(0, 0);
					this.toolStrip1.Name = "toolStrip1";
					this.toolStrip1.Size = new System.Drawing.Size(734, 25);
					this.toolStrip1.TabIndex = 1;
					this.toolStrip1.Text = "toolStrip1";
					// 
					// toolStripGo
					// 
					this.toolStripGo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
					this.toolStripGo.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.runSelectionToolStripMenuItem,
            this.debugSelectionToolStripMenuItem,
            this.toolStripMenuItem8,
            this.toolStripMenuItem6,
            this.toolStripMenuItem7});
					this.toolStripGo.Image = ((System.Drawing.Image)(resources.GetObject("toolStripGo.Image")));
					this.toolStripGo.ImageTransparentColor = System.Drawing.Color.Magenta;
					this.toolStripGo.Name = "toolStripGo";
					this.toolStripGo.Size = new System.Drawing.Size(32, 22);
					this.toolStripGo.Text = "Run selected tests";
					this.toolStripGo.ToolTipText = "Run all enabled tests";
					this.toolStripGo.ButtonClick += new System.EventHandler(this.runSelectionToolStripMenuItem_Click);
					// 
					// runSelectionToolStripMenuItem
					// 
					this.runSelectionToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("runSelectionToolStripMenuItem.Image")));
					this.runSelectionToolStripMenuItem.Name = "runSelectionToolStripMenuItem";
					this.runSelectionToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
					this.runSelectionToolStripMenuItem.Text = "Run Enabled Tests";
					this.runSelectionToolStripMenuItem.ToolTipText = "Run all enabled tests";
					this.runSelectionToolStripMenuItem.Click += new System.EventHandler(this.runSelectionToolStripMenuItem_Click);
					// 
					// debugSelectionToolStripMenuItem
					// 
					this.debugSelectionToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("debugSelectionToolStripMenuItem.Image")));
					this.debugSelectionToolStripMenuItem.Name = "debugSelectionToolStripMenuItem";
					this.debugSelectionToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
					this.debugSelectionToolStripMenuItem.Text = "Debug Enabled Tests";
					this.debugSelectionToolStripMenuItem.ToolTipText = "Debug all enabled tests";
					this.debugSelectionToolStripMenuItem.Click += new System.EventHandler(this.debugSelectionToolStripMenuItem_Click);
					// 
					// toolStripButtonStop
					// 
					this.toolStripButtonStop.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
					this.toolStripButtonStop.Enabled = false;
					this.toolStripButtonStop.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonStop.Image")));
					this.toolStripButtonStop.ImageTransparentColor = System.Drawing.Color.Magenta;
					this.toolStripButtonStop.Name = "toolStripButtonStop";
					this.toolStripButtonStop.Size = new System.Drawing.Size(23, 22);
					this.toolStripButtonStop.Text = "Stop running tests";
					this.toolStripButtonStop.ToolTipText = "Stop running tests";
					this.toolStripButtonStop.Click += new System.EventHandler(this.toolStripButtonStop_Click);
					// 
					// toolStripResultsReport
					// 
					this.toolStripResultsReport.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
					this.toolStripResultsReport.Image = global::Pryda.Common.MyResources.results;
					this.toolStripResultsReport.ImageTransparentColor = System.Drawing.Color.Magenta;
					this.toolStripResultsReport.Name = "toolStripResultsReport";
					this.toolStripResultsReport.Size = new System.Drawing.Size(23, 22);
					this.toolStripResultsReport.Text = "Results report";
					this.toolStripResultsReport.ToolTipText = "Show results report";
					this.toolStripResultsReport.Click += new System.EventHandler(this.toolStripResultsReport_Click_1);
					// 
					// toolStripProjects
					// 
					this.toolStripProjects.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
					this.toolStripProjects.Name = "toolStripProjects";
					this.toolStripProjects.Size = new System.Drawing.Size(250, 25);
					this.toolStripProjects.Sorted = true;
					this.toolStripProjects.ToolTipText = "Test project";
					this.toolStripProjects.SelectedIndexChanged += new System.EventHandler(this.toolStripProjects_Click);
					// 
					// toolStripRefresh
					// 
					this.toolStripRefresh.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
					this.toolStripRefresh.Image = ((System.Drawing.Image)(resources.GetObject("toolStripRefresh.Image")));
					this.toolStripRefresh.ImageTransparentColor = System.Drawing.Color.Magenta;
					this.toolStripRefresh.Name = "toolStripRefresh";
					this.toolStripRefresh.Size = new System.Drawing.Size(23, 22);
					this.toolStripRefresh.Text = "Refresh test list";
					this.toolStripRefresh.ToolTipText = "Refresh test list";
					this.toolStripRefresh.Click += new System.EventHandler(this.toolStripRefresh_Click);
					// 
					// toolStripSeparator1
					// 
					this.toolStripSeparator1.Name = "toolStripSeparator1";
					this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
					// 
					// toolStripLabel1
					// 
					this.toolStripLabel1.Name = "toolStripLabel1";
					this.toolStripLabel1.Size = new System.Drawing.Size(46, 22);
					this.toolStripLabel1.Text = "Repeat:";
					// 
					// toolStripTextRepeat
					// 
					this.toolStripTextRepeat.MaxLength = 3;
					this.toolStripTextRepeat.Name = "toolStripTextRepeat";
					this.toolStripTextRepeat.Size = new System.Drawing.Size(40, 25);
					this.toolStripTextRepeat.Text = "1";
					// 
					// toolStripDropDownButton1
					// 
					this.toolStripDropDownButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
					this.toolStripDropDownButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.shuffleTestsToolStripMenuItem,
            this.randomSeedToolStripMenuItem,
            this.toolStripMenuItem4,
            this.runDisabledTestsToolStripMenuItem,
            this.toolStripMenuItem5,
            this.breakOnFailureToolStripMenuItem,
            this.throwOnFailureToolStripMenuItem,
            this.catchExceptionsToolStripMenuItem});
					this.toolStripDropDownButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButton1.Image")));
					this.toolStripDropDownButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
					this.toolStripDropDownButton1.Name = "toolStripDropDownButton1";
					this.toolStripDropDownButton1.Size = new System.Drawing.Size(29, 22);
					this.toolStripDropDownButton1.Text = "toolStripDropDownButton1";
					this.toolStripDropDownButton1.DropDownOpening += new System.EventHandler(this.toolStripDropDownButton1_DropDownOpening);
					// 
					// shuffleTestsToolStripMenuItem
					// 
					this.shuffleTestsToolStripMenuItem.Name = "shuffleTestsToolStripMenuItem";
					this.shuffleTestsToolStripMenuItem.Size = new System.Drawing.Size(169, 22);
					this.shuffleTestsToolStripMenuItem.Text = "Shuffle tests";
					this.shuffleTestsToolStripMenuItem.Click += new System.EventHandler(this.shuffleTestsToolStripMenuItem_Click);
					// 
					// randomSeedToolStripMenuItem
					// 
					this.randomSeedToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripRandomSeed});
					this.randomSeedToolStripMenuItem.Name = "randomSeedToolStripMenuItem";
					this.randomSeedToolStripMenuItem.Size = new System.Drawing.Size(169, 22);
					this.randomSeedToolStripMenuItem.Text = "Random seed";
					this.randomSeedToolStripMenuItem.Click += new System.EventHandler(this.randomSeedToolStripMenuItem_Click);
					// 
					// toolStripRandomSeed
					// 
					this.toolStripRandomSeed.Name = "toolStripRandomSeed";
					this.toolStripRandomSeed.Size = new System.Drawing.Size(100, 23);
					this.toolStripRandomSeed.Text = "0";
					// 
					// toolStripMenuItem4
					// 
					this.toolStripMenuItem4.Name = "toolStripMenuItem4";
					this.toolStripMenuItem4.Size = new System.Drawing.Size(166, 6);
					// 
					// runDisabledTestsToolStripMenuItem
					// 
					this.runDisabledTestsToolStripMenuItem.Name = "runDisabledTestsToolStripMenuItem";
					this.runDisabledTestsToolStripMenuItem.Size = new System.Drawing.Size(169, 22);
					this.runDisabledTestsToolStripMenuItem.Text = "Run disabled tests";
					this.runDisabledTestsToolStripMenuItem.Click += new System.EventHandler(this.runDisabledTestsToolStripMenuItem_Click);
					// 
					// toolStripMenuItem5
					// 
					this.toolStripMenuItem5.Name = "toolStripMenuItem5";
					this.toolStripMenuItem5.Size = new System.Drawing.Size(166, 6);
					// 
					// breakOnFailureToolStripMenuItem
					// 
					this.breakOnFailureToolStripMenuItem.Name = "breakOnFailureToolStripMenuItem";
					this.breakOnFailureToolStripMenuItem.Size = new System.Drawing.Size(169, 22);
					this.breakOnFailureToolStripMenuItem.Text = "Break on failure";
					this.breakOnFailureToolStripMenuItem.Click += new System.EventHandler(this.breakOnFailureToolStripMenuItem_Click);
					// 
					// throwOnFailureToolStripMenuItem
					// 
					this.throwOnFailureToolStripMenuItem.Name = "throwOnFailureToolStripMenuItem";
					this.throwOnFailureToolStripMenuItem.Size = new System.Drawing.Size(169, 22);
					this.throwOnFailureToolStripMenuItem.Text = "Throw on failure";
					this.throwOnFailureToolStripMenuItem.Click += new System.EventHandler(this.throwOnFailureToolStripMenuItem_Click);
					// 
					// catchExceptionsToolStripMenuItem
					// 
					this.catchExceptionsToolStripMenuItem.Name = "catchExceptionsToolStripMenuItem";
					this.catchExceptionsToolStripMenuItem.Size = new System.Drawing.Size(169, 22);
					this.catchExceptionsToolStripMenuItem.Text = "Catch exceptions";
					this.catchExceptionsToolStripMenuItem.Click += new System.EventHandler(this.catchExceptionsToolStripMenuItem_Click);
					// 
					// openFileDialog1
					// 
					this.openFileDialog1.DefaultExt = "exe";
					this.openFileDialog1.Filter = "Executable|*.exe|All files|*.*";
					// 
					// toolStripMenuItem6
					// 
					this.toolStripMenuItem6.Image = global::Pryda.Common.MyResources.start;
					this.toolStripMenuItem6.Name = "toolStripMenuItem6";
					this.toolStripMenuItem6.Size = new System.Drawing.Size(186, 22);
					this.toolStripMenuItem6.Text = "Run Selected Tests";
					// 
					// toolStripMenuItem7
					// 
					this.toolStripMenuItem7.Image = global::Pryda.Common.MyResources.debug;
					this.toolStripMenuItem7.Name = "toolStripMenuItem7";
					this.toolStripMenuItem7.Size = new System.Drawing.Size(186, 22);
					this.toolStripMenuItem7.Text = "Debug Selected Tests";
					// 
					// toolStripMenuItem8
					// 
					this.toolStripMenuItem8.Name = "toolStripMenuItem8";
					this.toolStripMenuItem8.Size = new System.Drawing.Size(183, 6);
					// 
					// ctlTestList
					// 
					this.BackColor = System.Drawing.SystemColors.Window;
					this.Controls.Add(this.toolStrip1);
					this.Controls.Add(this.listTests);
					this.Name = "ctlTestList";
					this.Size = new System.Drawing.Size(734, 150);
					this.Load += new System.EventHandler(this.ctlTestList_Load);
					this.contextMenuStrip1.ResumeLayout(false);
					this.toolStrip1.ResumeLayout(false);
					this.toolStrip1.PerformLayout();
					this.ResumeLayout(false);
					this.PerformLayout();

        }
        #endregion

				private System.Windows.Forms.ColumnHeader columnTestCase;
				private System.Windows.Forms.ColumnHeader columnTest;
				private System.Windows.Forms.ColumnHeader columnResult;
				private System.Windows.Forms.ColumnHeader columnDuration;
				private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
				private System.Windows.Forms.ToolStripMenuItem runSelectedTestsToolStripMenuItem;
				public System.Windows.Forms.ListView listTests;
				private System.Windows.Forms.ToolStripComboBox toolStripProjects;
				private System.Windows.Forms.OpenFileDialog openFileDialog1;
				private System.Windows.Forms.ToolStripButton toolStripRefresh;
				public System.Windows.Forms.ToolStrip toolStrip1;
				private System.Windows.Forms.ColumnHeader columnEnabled;
				private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
				private System.Windows.Forms.ToolStripMenuItem clearResultsToolStripMenuItem;
				private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton1;
				private System.Windows.Forms.ToolStripMenuItem shuffleTestsToolStripMenuItem;
				private System.Windows.Forms.ToolStripMenuItem runDisabledTestsToolStripMenuItem;
				private System.Windows.Forms.ToolStripMenuItem runSelectionToolStripMenuItem;
				private System.Windows.Forms.ToolStripMenuItem debugSelectionToolStripMenuItem;
				private System.Windows.Forms.ToolStripMenuItem debugSelectedTestsToolStripMenuItem;
				public System.Windows.Forms.ToolStripSplitButton toolStripGo;
				public System.Windows.Forms.ToolStripButton toolStripButtonStop;
				private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
				private System.Windows.Forms.ToolStripButton toolStripResultsReport;
				private System.Windows.Forms.ToolStripLabel toolStripLabel1;
				private System.Windows.Forms.ToolStripTextBox toolStripTextRepeat;
				private System.Windows.Forms.ToolStripMenuItem showSelectionResultsToolStripMenuItem;
				private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
				private System.Windows.Forms.ImageList imageList1;
				private System.Windows.Forms.ToolStripMenuItem openTestToolStripMenuItem;
				private System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
				private System.Windows.Forms.ToolStripMenuItem randomSeedToolStripMenuItem;
				private System.Windows.Forms.ToolStripTextBox toolStripRandomSeed;
				private System.Windows.Forms.ToolStripSeparator toolStripMenuItem4;
				private System.Windows.Forms.ToolStripMenuItem breakOnFailureToolStripMenuItem;
				private System.Windows.Forms.ToolStripSeparator toolStripMenuItem5;
				private System.Windows.Forms.ToolStripMenuItem throwOnFailureToolStripMenuItem;
				private System.Windows.Forms.ToolStripMenuItem catchExceptionsToolStripMenuItem;
				private System.Windows.Forms.ToolStripSeparator toolStripMenuItem8;
				private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem6;
				private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem7;

		}
}

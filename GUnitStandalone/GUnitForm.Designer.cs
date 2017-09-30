namespace GUnitStandalone
{
	partial class GUnitForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GUnitForm));
			this.ctlTestList1 = new Pryda.Common.ctlTestList();
			this.SuspendLayout();
			// 
			// ctlTestList1
			// 
			this.ctlTestList1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
									| System.Windows.Forms.AnchorStyles.Left)
									| System.Windows.Forms.AnchorStyles.Right)));
			this.ctlTestList1.BackColor = System.Drawing.SystemColors.Window;
			this.ctlTestList1.Location = new System.Drawing.Point(0, 0);
			this.ctlTestList1.Name = "ctlTestList1";
			this.ctlTestList1.Size = new System.Drawing.Size(715, 379);
			this.ctlTestList1.TabIndex = 0;
			// 
			// GUnitForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(713, 380);
			this.Controls.Add(this.ctlTestList1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "GUnitForm";
			this.Text = "Pryda Build Testing Framework";
			this.Load += new System.EventHandler(this.GUnitForm_Load);
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.GUnitForm_FormClosing);
			this.ResumeLayout(false);

		}

		#endregion

		private Pryda.Common.ctlTestList ctlTestList1;
	}
}


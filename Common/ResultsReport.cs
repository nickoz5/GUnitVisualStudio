using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Pryda.Common
{
	public partial class ResultsReport : Form
	{
		public List<UnitTestOutput> _TestOutput = null;

		public TestRunSummary TestSummary;
		private int checkPrint;

		public ResultsReport()
		{
			InitializeComponent();
		}

		private String CreateColorTable(String strRTF)
		{
			// find index of start of header
			int iRTFLoc = strRTF.IndexOf("\\rtf");
			// get index of where we'll insert the colour table
			// try finding opening bracket of first property of header first                
			int iInsertLoc = strRTF.IndexOf("\r\n", iRTFLoc);

			// if there is no property, we'll insert colour table
			// just before the end bracket of the header
			if (iInsertLoc == -1) iInsertLoc = strRTF.IndexOf('}', iRTFLoc) - 1;

			// insert the colour table at our chosen location                
			strRTF = strRTF.Insert(iInsertLoc + 2,
					// CHANGE THIS STRING TO ALTER COLOUR TABLE
					"{\\colortbl ;\\red255\\green0\\blue0;\\red0\\green255\\blue0;}\r\n");

			return strRTF;
		}
		
		private void InitFontTable()
		{
			string strRTF = richTextBox1.Rtf;

			// Search for colour table info. If it exists (it shouldn't,
			// but we'll check anyway) remove it and replace with our one
			int iCTableStart = strRTF.IndexOf("fonttbl");

			if (iCTableStart != -1) //then colortbl exists
			{
					//find end of colortbl tab by searching
					//forward from the colortbl tab itself
					int iCTableEnd = strRTF.IndexOf('}', iCTableStart);

					//remove the existing colour table
					strRTF = strRTF.Remove(iCTableStart, iCTableEnd - iCTableStart + 1);

					//now insert new colour table at index of old colortbl tag
					strRTF = strRTF.Insert(iCTableStart,
						// CHANGE THIS STRING TO ALTER COLOUR TABLE
						"fonttbl{\\f0\\fnil\\fcharset0 Courier New;}{\\f1\\fnil\\fcharset0 Calibri;}");
			}
			//colour table doesn't exist yet, so let's make one
			else 
			{
					// find index of start of header
					int iRTFLoc = strRTF.IndexOf("\\rtf");
					// get index of where we'll insert the colour table
					// try finding opening bracket of first property of header first                
					int iInsertLoc = strRTF.IndexOf('{', iRTFLoc);

					// if there is no property, we'll insert colour table
					// just before the end bracket of the header
					if (iInsertLoc == -1) iInsertLoc = strRTF.IndexOf('}', iRTFLoc) - 1;

					// insert the colour table at our chosen location                
					strRTF = strRTF.Insert(iInsertLoc,
							// CHANGE THIS STRING TO ALTER COLOUR TABLE
							"{\\fonttbl{\f0\fnil\fcharset0 Courier New;}{\f1\fnil\fcharset0 Calibri;}}");
			}

			richTextBox1.Rtf = strRTF;
		}

		private void ResultsReport_Load(object sender, EventArgs e)
		{
			InitFontTable();

			// Test Result Report (date)
			// ------------------------------

			string testoutput = "";

			testoutput = "-------------------------------------------------------------------------------\n";
			richTextBox1.AppendText(testoutput);
			testoutput = "Result Summary\n";
			richTextBox1.AppendText(testoutput);
			testoutput = "-------------------------------------------------------------------------------\n";
			richTextBox1.AppendText(testoutput);

			testoutput = "Run result:\t" + TestSummary.TestsPassed + "/" + TestSummary.TestsTotal + " tests passed, " + TestSummary.TestsFailed + " failed, " + TestSummary.TestsSkipped + " skipped\n";
			richTextBox1.AppendText(testoutput);

			testoutput = "Submitted by:\t" + TestSummary.ComputerName + "\n";
			richTextBox1.AppendText(testoutput);

			testoutput = "Started on:\t" + TestSummary.TimeStart.ToString() + "\n";
			richTextBox1.AppendText(testoutput);
			testoutput = "Completed on:\t" + TestSummary.TimeEnd.ToString() + "\n";
			richTextBox1.AppendText(testoutput);
			testoutput = "Randomized:\t" + (TestSummary.TestsShuffleSeed > 0 ? "Yes - Seed " + TestSummary.TestsShuffleSeed : "No") + "\n";
			richTextBox1.AppendText(testoutput);

			testoutput = "-------------------------------------------------------------------------------\n\n";
			richTextBox1.AppendText(testoutput);


			if (_TestOutput != null)
			{
				foreach (UnitTestOutput output in _TestOutput)
				{
					if (output.GetOutputLineCount() == 0)
						continue;

					for (int i = 0; i < output.GetOutputLineCount(); i++)
					{
						richTextBox1.AppendText(output.GetOutput(i) + "\n");
					}
					richTextBox1.AppendText("\n");
				}
			}
			else
			{
				// No tests run.
			}


			// highlight strings in the text body..
			String text = richTextBox1.Rtf;
			text = CreateColorTable(text);
			text = HighlightString(text, GoogleTestOutputParser.RESULT_TEST_FAILED, "\\cf1 ");
			text = HighlightString(text, GoogleTestOutputParser.RESULT_TEST_PASSED, "\\cf2 ");
			text = HighlightString(text, GoogleTestOutputParser.RESULT_TEST_STARTED, "\\cf2 ");

			// note that the colortbl is discarded unless a reference to one or more
			// entries is found in the body of the RTF content.
			richTextBox1.Rtf = text;

			richTextBox1.Select(0, 0); 
			richTextBox1.ScrollToCaret();
		}

		private String HighlightString(String text, String highlight, String color)
		{
			const String colorDefault = "\\cf0 ";

			// find error results..
			int startPos = 0;
			while (startPos < text.Length)
			{
				int findPos = text.IndexOf(highlight, startPos);
				if (findPos == -1)
					break;

				int stringLen = 0;

				// start
				text = text.Insert(findPos, color);
				stringLen += color.Length + highlight.Length;

				// end
				text = text.Insert(findPos + stringLen, "\\cf0 ");
				stringLen += colorDefault.Length;

				startPos = findPos + stringLen;
			}

			return text;
		}

		private void toolStripButtonSave_Click(object sender, EventArgs e)
		{
			// Create a SaveFileDialog to request a path and file name to save to.
			SaveFileDialog saveFile1 = new SaveFileDialog();

			// Initialize the SaveFileDialog to specify the RTF extension for the file.
			saveFile1.FileName = "TestResults-" +
				TestSummary.TimeEnd.Date.Year + "_" + TestSummary.TimeEnd.Date.Month + "_" + TestSummary.TimeEnd.Date.Day +	// date
				//"-" + TestSummary.TimeEnd.TimeOfDay.Hours + "_" + TestSummary.TimeEnd.TimeOfDay.Minutes + "_" + TestSummary.TimeEnd.TimeOfDay.Seconds +	// time
				".rtf";	// extension
			saveFile1.DefaultExt = "*.rtf";
			saveFile1.Filter = "RTF Files|*.rtf";

			// Determine if the user selected a file name from the saveFileDialog.
			if (saveFile1.ShowDialog() == System.Windows.Forms.DialogResult.OK &&
				 saveFile1.FileName.Length > 0)
			{
				// Save the contents of the RichTextBox into the file.
				richTextBox1.SaveFile(saveFile1.FileName, RichTextBoxStreamType.RichText);
			}
		}

		private void toolStripButtonPrint_Click(object sender, EventArgs e)
		{
			if (printDialog1.ShowDialog() == DialogResult.OK)
				printDocument1.Print();
		}

		private void printDocument1_BeginPrint(object sender, System.Drawing.Printing.PrintEventArgs e)
		{
			checkPrint = 0;
		}

		private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
		{
			// Print the content of RichTextBox. Store the last character printed.
			checkPrint = richTextBox1.Print(checkPrint, richTextBox1.TextLength, e);

			// Check for more pages
			if (checkPrint < richTextBox1.TextLength)
				e.HasMorePages = true;
			else
				e.HasMorePages = false;
		}

		private void richTextBox1_KeyDown(object sender, KeyEventArgs e)
		{
			e.SuppressKeyPress = true;
		}
	}
}

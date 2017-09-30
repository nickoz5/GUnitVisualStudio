using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Windows.Forms;


namespace Pryda.Common
{
	public class ApplicationSettings
	{
		private ctlTestList _TestList;
		private string _SaveFileName;

		// settings
		private bool shuffleTests;
		private int RandomSeed;
		private bool disabledTests;
		private bool breakOnFailure;
		private bool catchExceptions;
		private bool throwOnFailure;
		private int RepeatCount;


		private class TestSolutionItem
		{
			public string TestCase;
			public string UnitTest;
			public bool Enabled;
		}
		private class TestSolution
		{
			public string Solution;
			public string Project;
			public TestSolution(string solutionName, string projectName)
			{
				Solution = solutionName;
				Project = projectName;
			}
			public List<TestSolutionItem> ListItems = new List<TestSolutionItem>();
		}
		private List<TestSolution> _TestSolutions = new List<TestSolution>();


		public ApplicationSettings(ctlTestList testList)
		{
			// check out output folder first, make sure it exists..
			string datapath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Pryda\\UnitTestingTools");
			if (!System.IO.Directory.Exists(datapath))
				System.IO.Directory.CreateDirectory(datapath);

			_SaveFileName = System.IO.Path.Combine(datapath, "settings.xml");
			_TestList = testList;

		}

		public bool Save(string solutionName, string projectName)
		{
			if (_TestList.listTests.Items.Count == 0)
				return true;

			Cursor.Current = Cursors.WaitCursor;
			ReadAllFileData();

			this.shuffleTests = _TestList.shuffleTests;
			this.RandomSeed = _TestList.RandomSeed;
			this.disabledTests = _TestList.disabledTests;
			this.breakOnFailure = _TestList.breakOnFailure;
			this.catchExceptions = _TestList.catchExceptions;
			this.throwOnFailure = _TestList.throwOnFailure;
			this.RepeatCount = _TestList.RepeatCount;

			if (solutionName != null && projectName != null)
			{
				TestSolution CurrentItem = new TestSolution(solutionName, projectName);
				foreach (ListViewItem lvi in _TestList.listTests.Items)
				{
					TestSolutionItem item = new TestSolutionItem();
					item.TestCase = lvi.SubItems[1].Text;
					item.UnitTest = lvi.SubItems[2].Text;
					item.Enabled = _TestList.isTestEnabled(lvi);
					CurrentItem.ListItems.Add(item);
				}

				// remove current item if it already exists..
				for (int i = 0; i < _TestSolutions.Count; i++)
				{
					TestSolution solution = _TestSolutions[i];
					if (solution.Project == CurrentItem.Project && solution.Solution == CurrentItem.Solution)
						_TestSolutions.RemoveAt(i--);
				}

				_TestSolutions.Add(CurrentItem);
			}

			WriteAllFileData();

			Cursor.Current = Cursors.Default;

			return true;
		}

		public bool Load(string solutionName, string projectName)
		{
			Cursor.Current = Cursors.WaitCursor;
			ReadAllFileData();

			_TestList.shuffleTests = this.shuffleTests;
			_TestList.RandomSeed = this.RandomSeed;
			_TestList.disabledTests = this.disabledTests;
			_TestList.breakOnFailure = this.breakOnFailure;
			_TestList.catchExceptions = this.catchExceptions;
			_TestList.throwOnFailure = this.throwOnFailure;
			_TestList.RepeatCount = this.RepeatCount;


			if (solutionName != null && projectName != null)
			{
				_TestList.listTests.BeginUpdate();

				foreach (TestSolution solution in _TestSolutions)
				{
					if (!solution.Project.Equals(projectName) || !solution.Solution.Equals(solutionName))
						continue;

					foreach (TestSolutionItem item in solution.ListItems)
					{
						foreach (ListViewItem lvi in _TestList.listTests.Items)
						{
							if (!item.TestCase.Equals(lvi.SubItems[1].Text) || !item.UnitTest.Equals(lvi.SubItems[2].Text))
								continue;

							// update enabled item
							_TestList.setTestEnabled(lvi, item.Enabled);
							break;
						}
					}

					break;
				}

				_TestList.listTests.EndUpdate();
			}

			Cursor.Current = Cursors.Default;

			return true;
		}

		private void ReadAllFileData()
		{
			_TestSolutions.Clear();

			try
			{
				TestSolution CurrentSolution = null;

				using (XmlReader reader = XmlReader.Create(_SaveFileName))
				{
					while (reader.Read())
					{
						// Only detect start elements.
						if (!reader.IsStartElement())
							continue;

						switch (reader.Name)
						{
							case "UnitTestProjects":
								// start of project collection..
								break;

							case "General":
								// general settings
								shuffleTests = Boolean.Parse(reader.GetAttribute("Shuffle"));
								RandomSeed = Int32.Parse(reader.GetAttribute("RandomSeed"));
								disabledTests = Boolean.Parse(reader.GetAttribute("RunDisabled"));
								breakOnFailure = Boolean.Parse(reader.GetAttribute("BreakOnFailure"));
								catchExceptions = Boolean.Parse(reader.GetAttribute("CatchExceptions"));
								throwOnFailure = Boolean.Parse(reader.GetAttribute("ThrowOnFailure"));
								RepeatCount = Int32.Parse(reader.GetAttribute("RepeatCount"));
								break;

							case "ProjectItems":
								if (CurrentSolution != null)
								{
									_TestSolutions.Add(CurrentSolution);
									CurrentSolution = null;
								}

								string solutionName = reader.GetAttribute("solution");
								string projectName = reader.GetAttribute("project");

								TestSolution solution = new TestSolution(solutionName, projectName);
								CurrentSolution = solution;
								break;

							case "TestItem":
								if (CurrentSolution != null)
								{
									TestSolutionItem item = new TestSolutionItem();
									item.TestCase = reader.GetAttribute("testcase");
									item.UnitTest = reader.GetAttribute("unittest");
									item.Enabled = Boolean.Parse(reader.GetAttribute("enabled"));
									CurrentSolution.ListItems.Add(item);
								}
								break;
						}
					}

					if (CurrentSolution != null)
					{
						_TestSolutions.Add(CurrentSolution);
						CurrentSolution = null;
					}
				}
			}
			catch (System.Exception ex)
			{

			}
		}

		private void WriteAllFileData()
		{
			try
			{
				using (XmlWriter writer = XmlWriter.Create(_SaveFileName))
				{
					writer.WriteStartDocument();
					writer.WriteStartElement("UnitTestProjects");

					// general settings
					writer.WriteStartElement("General");
					writer.WriteAttributeString("Shuffle", _TestList.shuffleTests.ToString());
					writer.WriteAttributeString("RandomSeed", _TestList.RandomSeed.ToString());
					writer.WriteAttributeString("RunDisabled", _TestList.disabledTests.ToString());
					writer.WriteAttributeString("BreakOnFailure", _TestList.breakOnFailure.ToString());
					writer.WriteAttributeString("CatchExceptions", _TestList.catchExceptions.ToString());
					writer.WriteAttributeString("ThrowOnFailure", _TestList.throwOnFailure.ToString());
					writer.WriteAttributeString("RepeatCount", _TestList.RepeatCount.ToString());
					writer.WriteEndElement();

					// project/solution settings
					foreach (TestSolution solution in _TestSolutions)
					{
						writer.WriteStartElement("ProjectItems");
						writer.WriteAttributeString("solution", solution.Solution);
						writer.WriteAttributeString("project", solution.Project);

						foreach (TestSolutionItem item in solution.ListItems)
						{
							// dont bother writing out unless the item is disabled
							if (item.Enabled)
								continue;

							writer.WriteStartElement("TestItem");
							writer.WriteAttributeString("testcase", item.TestCase);
							writer.WriteAttributeString("unittest", item.UnitTest);
							writer.WriteAttributeString("enabled", item.Enabled.ToString());
							//writer.WriteElementString("testcase", item.TestCase);
							//writer.WriteElementString("unittest", item.UnitTest);
							//writer.WriteElementString("enabled", item.Enabled.ToString());
							writer.WriteEndElement();
						}

						writer.WriteEndElement();
					}

					writer.WriteEndElement();
					writer.WriteEndDocument();
				}
			}
			catch (System.Exception ex)
			{
			}
		}

	}
}

using System.IO;
using System.Linq;
using Gtk;
using System;

namespace Client
{
    public partial class BenchmarkImportDialog : Dialog
    {
        public event EventHandler<Tuple<string, string>> FilesAdded;

        public BenchmarkImportDialog()
        {
            Build();
        }

        private void AddSubmitedFiles(string benchmarkPath, string inputPath)
        {
            string benchmarksDirectoriesPath = "./../../../simplesim-3.0/benchmarks/";

            string benchFormatDialogTitle = "This file is not supported!";
            string benchFormatDialogMessage = " The file does not contain the expected '.ss' extension. ";

            string[] benchmark = benchmarkPath.Split('/').Last().Split('.');

            if (benchmark.Last() != "ss")
            {
                ShowMessageBox(this, benchFormatDialogTitle, benchFormatDialogMessage);
            }

            string benchmarkName = benchmark.First();
            string benchmarkDirectoryPath = benchmarksDirectoriesPath + benchmarkName;

            if (!Directory.Exists(benchmarkDirectoryPath))
            {
                Directory.CreateDirectory(benchmarkDirectoryPath);
            }

            string benchmarkFilePath = $"{benchmarkDirectoryPath}/{benchmarkName}.ss";

            File.Copy(benchmarkEntry.Text, benchmarkFilePath, true);

            string input = ExtractInputFromPath(inputPath, benchmarkName);

            benchmarkName = char.ToUpper(benchmarkName[0]) + benchmarkName.Substring(1);

            FilesAdded?.Invoke(this, Tuple.Create(benchmarkName, input));

            Destroy();
        }

        private string ExtractInputFromPath(string inputPath, string benchmarkName)
        {
            string benchmarksDirectoriesPath = "./../../../simplesim-3.0/benchmarks/";

            string input = inputPath.Split('/').Last();

            string benchmarkDirectoryPath = benchmarksDirectoriesPath + benchmarkName;

            if (!Directory.Exists(benchmarkDirectoryPath))
            {
                Directory.CreateDirectory(benchmarkDirectoryPath);
            }

            string inputFilePath = $"{benchmarkDirectoryPath}/{input}";

            File.Copy(inputEntry.Text, inputFilePath, true);

            return input;
        }

        private void ShowMessageBox(Window parent, string title, string message)
        {
            Dialog dialog = null;

            try
            {
                dialog = new Dialog(title, parent,
                    DialogFlags.DestroyWithParent | DialogFlags.Modal,
                    "Ok", ResponseType.Ok);
                dialog.VBox.Add(new Label(message));
                dialog.ShowAll();

                dialog.Run();
            }
            finally
            {
                if (dialog != null)
                {
                    dialog.Destroy();
                }
            }
        }

        private string ChooseFilePath()
        {
            string filesPath = null;

            FileChooserDialog fileChooser = new FileChooserDialog(
                "Select File",
                this,
                FileChooserAction.Open,
                "Cancel", ResponseType.Cancel,
                "Open", ResponseType.Accept
            );

            if (fileChooser.Run() == (int)ResponseType.Accept)
            {
                filesPath = fileChooser.Filename;
            }

            fileChooser.Destroy();

            return filesPath;
        }

        protected void OnBenchButtonClicked(object sender, EventArgs e)
        {
            benchmarkEntry.Text = ChooseFilePath();
        }

        protected void OnInputButtonClicked(object sender, EventArgs e)
        {
            inputEntry.Text = ChooseFilePath();
        }

        protected void OnButtonCancelClicked(object sender, EventArgs e)
        {
            Destroy();
        }

        protected void OnButtonOkClicked(object sender, EventArgs e)
        {
            AddSubmitedFiles(benchmarkEntry.Text, inputEntry.Text);
        }
    }
}

using System;
using Gtk;
using Gdk;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Domain.DTOs;

public partial class MainWindow : Gtk.Window
{
    private readonly Color darkModeBackground = new Color(51, 57, 59);
    private readonly Color darkModeText = new Color(238, 238 ,236);
    private readonly Color whiteModeBackground = new Color(232, 232, 231);
    private readonly Color whiteModeText = new Color(46, 52, 54);

    private int benchTableRowNum = 0;
    private List<string> benchmarks = new List<string>();

    public MainWindow () : base (Gtk.WindowType.Toplevel)
    {
        ConnectionManager connectionManager = new ConnectionManager();

        connectionManager.addConnection("192.168.114.91", "3000");
       // connectionManager.addConnection("192.168.0.103", "3001");

        var tasks = connectionManager.executeCommands(new List<Params> { 
            new Params()
            {
                benchName= "Applu",
                maxInst= 100

            }
            });

        Build();
    }

    protected void OnDeleteEvent (object sender, DeleteEventArgs a)
    {
        Application.Quit();
        a.RetVal = true;
    }

    protected void OnConfigBrowseButtonClicked (object sender, EventArgs e)
    {
        configEntry.Text = ChooseFilePath()[0];
    }

    protected void OnDumpBrowseButtonClicked (object sender, EventArgs e)
    {
        dumpEntry.Text = ChooseFilePath()[0];
    }

    protected void OnSimRedirBrowseButtonClicked (object sender, EventArgs e)
    {
        simRedirEntry.Text = ChooseFolderPath();
    }

    protected void OnProgRedirBrowseButtonClicked (object sender, EventArgs e)
    {
        progRedirEntry.Text = ChooseFolderPath();
    }

    private string ChooseFolderPath(){
        string folderPath = "";

        FileChooserDialog fileChooser = new FileChooserDialog (
            "Select Folder",
            this,
            FileChooserAction.SelectFolder,
            "Cancel", ResponseType.Cancel,
            "Open", ResponseType.Accept);

        if (fileChooser.Run () == (int)ResponseType.Accept) {
            folderPath = fileChooser.Filename;
        }

        fileChooser.Destroy ();

        return folderPath;
    }

    private string[] ChooseFilePath(){
        string[] filesPaths = null;

        FileChooserDialog fileChooser = new FileChooserDialog (
            "Select File",
            this,
            FileChooserAction.Open,
            "Cancel", ResponseType.Cancel,
            "Open", ResponseType.Accept);

        fileChooser.SelectMultiple = true;

        if (fileChooser.Run () == (int)ResponseType.Accept) {
            filesPaths = fileChooser.Filenames;
        }

        fileChooser.Destroy ();

        return filesPaths;
    }

    protected void OnBenchBrowseButtonClicked(object sender, EventArgs e)
    {
        string[] benchmarksPaths = ChooseFilePath();

        if (benchmarksPaths == null)
        {
            return;
        }

        ExtractBenchmarksNamesFromPath(benchmarksPaths);

        AddBenchmarksToTable(benchmarks);
    }

    private void ExtractBenchmarksNamesFromPath(string[] benchmarksPaths) 
    {
        string benchmarksErrors = "";
        string benchSelectionErrorTitle = "One or more files are not supported!";
        string benchSelectionErrorMessage = " The files: {0} do not contain the expected '.ss' extension. ";

        foreach (string benchmarkPath in benchmarksPaths)
        {
            string[] benchmark = benchmarkPath.Split('/').Last().Split('.');

            if (benchmark.Last() != "ss")
            {
                benchmarksErrors += benchmark.First() + ", ";
                continue;
            }

            string benchmarkName = char.ToUpper(benchmark.First()[0]) + benchmark.First().Substring(1);

            if (benchmarks.Contains(benchmarkName))
            {
                continue;
            }

            benchmarks.Add(benchmarkName);
        }

        if (benchmarksErrors != "")
        {
            ShowMessageBox(this, benchSelectionErrorTitle, string.Format(benchSelectionErrorMessage, benchmarksErrors));
        }
    }

    protected void OnClearBenchButtonClicked(object sender, EventArgs e)
    {
        ClearBenchmarksTable();
    }

    private void ClearBenchmarksTable()
    {
        if (benchmarks.Count == 0)
        {
            return;
        }

        foreach (string benchmark in benchmarks)
        {
            var labelToRemove = benchTable.Children.First(x => x.Name.Equals($"{benchmark}Label"));
            benchTable.Remove(labelToRemove);

            if (benchTableRowNum > 2)
            {
                benchTable.NRows--;
                benchFrame.HeightRequest -= 50;
            }

            benchTableRowNum--;
        }

        benchmarks = new List<string>();

        benchTable.Attach(noBenchLabel, 0, 1, 0, 1, AttachOptions.Fill, AttachOptions.Fill, 0 , 0);
    }

    private void AddBenchmarksToTable(List<string> benchmarksNames)
    {
        benchTable.Remove(noBenchLabel);

        foreach (string benchmark in benchmarksNames)
        {
            if (benchTable.Children.Any(x => x.Name.Equals($"{benchmark}Label")))
            {
                continue;
            }

            Label benchmarkLabel = new Label(benchmark);
            benchmarkLabel.Justify = Justification.Left;
            benchmarkLabel.Xpad = 12;
            benchmarkLabel.Name = $"{benchmark}Label";

            if (benchTableRowNum > 1)
            {
                benchTable.NRows++;
                benchFrame.HeightRequest += 50;
            }

            benchTable.Attach(benchmarkLabel, 0, 1, (uint)benchTableRowNum, (uint)benchTableRowNum + 1, 
                                AttachOptions.Fill, AttachOptions.Fill, 0, 0);

            benchTableRowNum++;
        }

        ShowAll();
    }

    private void ShowMessageBox(Gtk.Window parent, string title, string message)
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

    protected void OnDefaultPredictorArgumentsActionActivated (object sender, EventArgs e)
    {
        SetDefaultPredictorArguments ();
    }

    protected void OnDefaultSimulatorSettingsActionActivated (object sender, EventArgs e)
    {
        SetDefaultSimulatorSettings ();
    }

    protected void OnDefaultConfigActionActivated (object sender, EventArgs e)
    {
        //benchNameLabel.Text = "benchName";
        bpredCombobox.Active = 0;

        SetDefaultSimulatorSettings ();
        SetDefaultPredictorArguments ();
    }

    private void SetDefaultPredictorArguments(){
        //2lev
        l1sizeSpinbutton.Value = 1;
        l2sizeSpinbutton.Value = 1024;
        histSizeSpinbutton.Value = 8;
        xorSpinbutton.Value = 0;

        //btb
        numSetsSpinbutton.Value = 512;
        assocSpinbutton.Value = 4;
        //btb
        numSetsSpinbutton.Value = 512;
        assocSpinbutton.Value = 4;

        //comb
        metaTableSizeSpinbutton.Value = 1024;
        //comb
        metaTableSizeSpinbutton.Value = 1024;

        //ras
        rasSizeSpinbutton.Value = 8;
        //ras
        rasSizeSpinbutton.Value = 8;

        //bimod
        tableSizeSpinbutton.Value = 2048;
    }

    private void SetDefaultSimulatorSettings(){
        seedSpinbutton.Value = 1;
        configEntry.Text = "";
        dumpEntry.Text = "";
        simRedirEntry.Text = "";
        progRedirEntry.Text = "";
        helpCheckbutton.Active = false;
        verboseCheckbutton.Active = false;
        debugCheckbutton.Active = false;
        dliteDebuggerCheckbutton.Active = false;
        initAndEndCheckbutton.Active = false;
        schedulingSpinbutton.Value = 0;
        maxInstSpinbutton.Value = 1000000000;
    }

    protected void OnSimulateButtonClicked (object sender, EventArgs e)
    {
        Params predictorSettings = new Params
        {
            //benchName = benchNameLabel.Text,
            maxInst = maxInstSpinbutton.ValueAsInt,
            bpred = bpredCombobox.ActiveText,
            l1Size = l1sizeSpinbutton.ValueAsInt,
            l2Size = l2sizeSpinbutton.ValueAsInt,
            tableSize = tableSizeSpinbutton.ValueAsInt,
            metaTableSize = metaTableSizeSpinbutton.ValueAsInt,
            rasSize = rasSizeSpinbutton.ValueAsInt,
            numSets = numSetsSpinbutton.ValueAsInt,
            histSize = numSetsSpinbutton.ValueAsInt,
            xor = xorSpinbutton.ValueAsInt,
            assoc = assocSpinbutton.ValueAsInt
        };
    }

    private void ModifyWidgetColors(Widget widget, Color backgroundColor, Color textColor)
    {
        if (widget is Button || widget is SpinButton)
        {
            return;
        }

        widget.ModifyBg(StateType.Normal, backgroundColor);
        widget.ModifyBg(StateType.Active, backgroundColor);
        widget.ModifyBg(StateType.Insensitive, backgroundColor);
        widget.ModifyBg(StateType.Prelight, backgroundColor);
        widget.ModifyBg(StateType.Selected, backgroundColor);

        if (widget is Label)
        {
            widget.ModifyFg(StateType.Normal, textColor);
        }

        if (widget is Container container)
        {
            foreach (Widget child in container.Children)
            {
                ModifyWidgetColors(child, backgroundColor, textColor);
            }
        }
    }

    protected void OnDarkModeTogglebuttonToggled(object sender, EventArgs e)
    {
        ToggleButton toggleButton = sender as ToggleButton;

        if (toggleButton.Active)
        {
            ModifyWidgetColors(this, darkModeBackground, darkModeText);

        }
        else
        {
            ModifyWidgetColors(this, whiteModeBackground, whiteModeText);
        }
    }
}

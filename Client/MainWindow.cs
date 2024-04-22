using System;
using Gtk;
using Gdk;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Domain.DTOs;
using Client.Connection;
using Client;
using Domain.Models;
using Domain.EventArgs;
using System.IO;

public partial class MainWindow : Gtk.Window
{
    private readonly Color darkModeBackground = new Color(51, 57, 59);
    private readonly Color darkModeText = new Color(238, 238, 236);
    private readonly Color whiteModeBackground = new Color(232, 232, 231);
    private readonly Color whiteModeText = new Color(46, 52, 54);

    private int benchTableRowNum = 0;
    private List<string> benchmarks = new List<string>();

    private int connRowsNum = 1;
    private List<Connection> ipAddresses = new List<Connection>();

    private ConnectionManager connectionManager;

    public MainWindow() : base(Gtk.WindowType.Toplevel)
    {
        Build();

        SetDefaultSimulatorSettings();
        SetDefaultPredictorArguments();
        LinkBenchmarksAndInputs();

        connectionManager = new ConnectionManager();
        connectionManager.ConnectionStatusModified += ChangeConnectionStatus;
    }

    protected void OnDeleteEvent(object sender, DeleteEventArgs a)
    {
        Application.Quit();
        a.RetVal = true;
    }

    private async Task<ServerStatus> ConnectToServer(string ipAddress, string port)
    {
        string connectionFailedDialogTitle = "Connection failed";
        string connectionFailedDialogMessage = "Connection attempt for ip addess: {0} with port: {1} failed.";

        await Task.Run(() => ChangeConnectionStatus(this, new ConnectionStatusEventArgs(ipAddress, port, 
                                                                    ServerStatus.Connecting.ToString())));

        connectionManager.addConnection(ipAddress, port);

        return ServerStatus.Connected;
    }

    private void ChangeConnectionStatus(object sender, ConnectionStatusEventArgs args)
    {
        foreach (var child in serverConnTable.Children.Where(child => child.Name.Contains("Status")))
        {
            if (child.Name.Equals($"{args.Ip}:{args.Port}StatusLabel"))
            {
                Label connLabel = child as Label;
                connLabel.Text = args.Status;
                break;
            }
        }
        ShowAll();
    }

    protected void OnConfigBrowseButtonClicked(object sender, EventArgs e)
    {
        configEntry.Text = ChooseFilePath()[0];
    }

    protected void OnDumpBrowseButtonClicked(object sender, EventArgs e)
    {
        dumpEntry.Text = ChooseFilePath()[0];
    }

    protected void OnSimRedirBrowseButtonClicked(object sender, EventArgs e)
    {
        simRedirEntry.Text = ChooseFolderPath();
    }

    protected void OnProgRedirBrowseButtonClicked(object sender, EventArgs e)
    {
        progRedirEntry.Text = ChooseFolderPath();
    }

    private string ChooseFolderPath()
    {
        string folderPath = "";

        FileChooserDialog fileChooser = new FileChooserDialog(
            "Select Folder",
            this,
            FileChooserAction.SelectFolder,
            "Cancel", ResponseType.Cancel,
            "Open", ResponseType.Accept);

        if (fileChooser.Run() == (int)ResponseType.Accept)
        {
            folderPath = fileChooser.Filename;
        }

        fileChooser.Destroy();

        return folderPath;
    }

    private string[] ChooseFilePath()
    {
        string[] filesPaths = null;

        FileChooserDialog fileChooser = new FileChooserDialog(
            "Select File",
            this,
            FileChooserAction.Open,
            "Cancel", ResponseType.Cancel,
            "Open", ResponseType.Accept);

        fileChooser.SelectMultiple = true;

        if (fileChooser.Run() == (int)ResponseType.Accept)
        {
            filesPaths = fileChooser.Filenames;
        }

        fileChooser.Destroy();

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

        AddBenchmarksToTable();
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

        benchTable.Attach(noBenchLabel, 0, 1, 0, 1, AttachOptions.Fill, AttachOptions.Fill, 0, 0);
    }

    private void AddBenchmarksToTable()
    {
        benchTable.Remove(noBenchLabel);

        foreach (string benchmark in benchmarks)
        {
            if (benchTable.Children.Any(x => x.Name.Equals($"{benchmark}Label")))
            {
                continue;
            }

            Label benchmarkLabel = new Label(benchmark)
            {
                Justify = Justification.Left,
                Xpad = 12,
                Name = $"{benchmark}Label"
            };

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

    protected void OnDefaultPredictorArgumentsActionActivated(object sender, EventArgs e)
    {
        SetDefaultPredictorArguments();
    }

    protected void OnDefaultSimulatorSettingsActionActivated(object sender, EventArgs e)
    {
        SetDefaultSimulatorSettings();
    }

    protected void OnDefaultConfigActionActivated(object sender, EventArgs e)
    {
        bpredCombobox.Active = 0;

        SetDefaultSimulatorSettings();
        SetDefaultPredictorArguments();
    }

    private void SetDefaultPredictorArguments()
    {
        //2lev
        l1sizeSpinbutton.Value = 1;
        l2sizeSpinbutton.Value = 1024;
        histSizeSpinbutton.Value = 8;
        xorSpinbutton.Value = 0;

        //btb
        numSetsSpinbutton.Value = 512;
        assocSpinbutton.Value = 4;

        //comb
        metaTableSizeSpinbutton.Value = 1024;

        //ras
        rasSizeSpinbutton.Value = 8;

        //bimod
        tableSizeSpinbutton.Value = 2048;
    }

    private void SetDefaultSimulatorSettings()
    {
        string resultsPath = "./../../../simplesim-3.0/results";

        if (!Directory.Exists(resultsPath))
        {
            Directory.CreateDirectory(resultsPath);
        }

        simRedirEntry.Text = System.IO.Path.GetFullPath(resultsPath);
        progRedirEntry.Text = System.IO.Path.GetFullPath(resultsPath);

        seedSpinbutton.Value = 1;
        configEntry.Text = "";
        dumpEntry.Text = "";
        helpCheckbutton.Active = false;
        verboseCheckbutton.Active = false;
        debugCheckbutton.Active = false;
        dliteDebuggerCheckbutton.Active = false;
        initAndEndCheckbutton.Active = false;
        schedulingSpinbutton.Value = 0;
        maxInstSpinbutton.Value = 1000000000;
    }

    protected async void OnSimulateButtonClicked(object sender, EventArgs e)
    {
        string noConnectionsFoundDialogTitle = "No connections available";
        string noConnectionsFoundDialogMessage = "No connection available to simulate at this time.";

        string noBenchSelectedDialogTitle = "No benchmark selected";
        string noBenchSelectedDialogMessage = "Please add a benchmark to be able to run the simulation.";

        if (connectionManager is null || ipAddresses.Count == 0)
        {
            ShowMessageBox(this, noConnectionsFoundDialogTitle, noConnectionsFoundDialogMessage);
            return;
        }

        if (benchmarks.Count == 0)
        {
            ShowMessageBox(this, noBenchSelectedDialogTitle, noBenchSelectedDialogMessage);
            return;
        }

        List<Params> simulationsParams = new List<Params>();

        foreach (string benchmark in benchmarks)
        {
            simulationsParams.Add(new Params
            {
                benchName = benchmark,
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
            });
        }

        var tasks = await Task.Run(()=>connectionManager.executeCommands(simulationsParams));

        foreach (var task in tasks)
        {
            using (StreamWriter streamWriter = new StreamWriter(simRedirEntry.Text + "/" + task.benchName + "_simout.res"))
            {
                streamWriter.Write(task.simoutFile);
            }
        }
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

    protected void OnAddConnButtonClicked(object sender, EventArgs e)
    {
        ConnectDialog connectDialog = new ConnectDialog();

        connectDialog.ValuesSubmited += OnConnectionAddressSubmitted;

        connectDialog.Run();
    }

    private async void OnConnectionAddressSubmitted(object sender, Tuple<string, string> connectionAddress)
    {
        string connectionExistsDialogTitle = "Connection exists";
        string connectionExistsDialogMessage = "A connection with ip: {0} and port: {1} already exists";

        string ip = connectionAddress.Item1;
        string port = connectionAddress.Item2;

        if (ipAddresses.Any(x => x.StatusLabelName == $"{ip}:{port}StatusLabel"))
        {
            ShowMessageBox(this, connectionExistsDialogTitle, string.Format(connectionExistsDialogMessage, ip, port));
            return;
        }

        ipAddresses.Add(new Connection
        {
            Ip = ip,
            Port = port,
            StatusLabelName = $"{ip}:{port}StatusLabel"
        });

        PopulateConnectionsTable();

        await ConnectToServer(ip, port);
    }

    private void PopulateConnectionsTable()
    {
        ClearConnectionsTable();

        foreach (Connection connection in ipAddresses)
        {
            Label connIpLabel = new Label(connection.Ip);
            Label connPortLabel = new Label(connection.Port);
            Label connStatusLabel = new Label(ServerStatus.NotConnected.ToString());
            Button connDeleteConnButton = new Button();

            connIpLabel.Name = $"{connIpLabel.Text}:{connPortLabel.Text}IpLabel";
            connPortLabel.Name = $"{connIpLabel.Text}:{connPortLabel.Text}PortLabel";
            connStatusLabel.Name = $"{connIpLabel.Text}:{connPortLabel.Text}StatusLabel";

            connDeleteConnButton.Label = "Delete";
            connDeleteConnButton.Name = $"{connIpLabel.Text}:{connPortLabel.Text}DeleteButton";
            connDeleteConnButton.Clicked += OnDeleteConnectionButtonClicked;

            serverConnTable.NRows++;
            serverConnTable.Attach(connIpLabel, 0, 1, (uint)connRowsNum, (uint)connRowsNum + 1, AttachOptions.Fill, AttachOptions.Fill, 0, 0);
            serverConnTable.Attach(connPortLabel, 1, 2, (uint)connRowsNum, (uint)connRowsNum + 1, AttachOptions.Fill, AttachOptions.Fill, 0, 0);
            serverConnTable.Attach(connStatusLabel, 2, 3, (uint)connRowsNum, (uint)connRowsNum + 1, AttachOptions.Fill, AttachOptions.Fill, 0, 0);
            serverConnTable.Attach(connDeleteConnButton, 3, 4, (uint)connRowsNum, (uint)connRowsNum + 1, AttachOptions.Fill, AttachOptions.Fill, 5, 0);

            connRowsNum++;
            serverConnectionFrame.HeightRequest += 50;
        }

        ShowAll();
    }

    private void ClearConnectionsTable()
    {
        foreach (var child in serverConnTable.Children.Where(x => !x.Name.Equals("ipLabel") &&
                                                                  !x.Name.Equals("portLabel") &&
                                                                  !x.Name.Equals("connStatusLabel") &&
                                                                  !x.Name.Equals("addConnButton")))
        {
            serverConnTable.Remove(child);
        }

        connRowsNum = 1;
        serverConnTable.NRows = 1;
        serverConnectionFrame.HeightRequest = 68;
    }

    private void OnDeleteConnectionButtonClicked(object sender, EventArgs e)
    {
        Button deleteButton = sender as Button;

        string key = deleteButton.Name.Replace("DeleteButton", "StatusLabel");

        Connection connectionToRemove = ipAddresses.FirstOrDefault(x => x.StatusLabelName.Equals(key));
        ipAddresses.Remove(connectionToRemove);

        PopulateConnectionsTable();
    }

    private void OnNewBenchmarkImported(object sender, Tuple<string, string> benchmark)
    {
        string benchmarkName = benchmark.Item1;
        string input = benchmark.Item2;

        if (benchmarks.Contains(benchmarkName))
        {
            return;
        }

        Benchmark.Benchmarks.Add(benchmarkName, new Benchmark
        {
            benchmarkFile = $"benchmarks/{benchmarkName.ToLower()}/{benchmarkName.ToLower()}.ss",
            inFile = $"benchmarks/{benchmarkName}/{input}"
        });

        benchmarks.Add(benchmarkName);

        AddBenchmarksToTable();
    }

    private void LinkBenchmarksAndInputs()
    {
        string benchmarksPath = "./../../../simplesim-3.0/benchmarks/";

        string fileNumberInDirectoryDialogTitle = "The number of files in directory is not as expected.";
        string fileNumberInDirectoryDialogMessage = "There should be 2 files in each benchmark directory and {0} " +
            "files were found.";

        string fileErrorDialogTitle = "One or more files not found.";
        string fileErrorDialogMessage = "Benchmark or input file were not found in the expected directory.";

        foreach (string benchDirectory in Directory.GetDirectories(benchmarksPath))
        {
            string[] filesPaths = Directory.GetFiles(benchDirectory);

            if (filesPaths.Length != 2)
            {
                ShowMessageBox(this, fileNumberInDirectoryDialogTitle,
                    string.Format(fileNumberInDirectoryDialogMessage, filesPaths.Length));
                continue;
            }

            string input = "";
            string bench = "";

            foreach (string filePath in filesPaths)
            {
                string file = filePath.Split('/').Last();

                if (!file.Contains("ss"))
                {
                    input = file;
                }

                bench = file.Split('.').First();
                bench = char.ToUpper(bench[0]) + bench.Substring(1);
            }

            if (bench == "" || input == "")
            {
                ShowMessageBox(this, fileErrorDialogTitle, fileErrorDialogMessage);
                continue;
            }

            Benchmark.Benchmarks.Add(bench, new Benchmark
            {
                benchmarkFile = $"/benchmarks/{bench.ToLower()}/{bench.ToLower()}.ss",
                inFile = $"/benchmarks/{bench.ToLower()}/{input}"
            });
        }
    }

    protected void OnImportActionActivated(object sender, EventArgs e)
    {
        BenchmarkImportDialog benchmarkImportDialog = new BenchmarkImportDialog();

        benchmarkImportDialog.FilesAdded += OnNewBenchmarkImported;

        benchmarkImportDialog.Run();
    }
}

using System;
using Gtk;
using System.Linq;

public partial class MainWindow: Gtk.Window
{
	public MainWindow () : base (Gtk.WindowType.Toplevel)
	{
		Build ();
	}

	protected void OnDeleteEvent (object sender, DeleteEventArgs a)
	{
		Application.Quit ();
		a.RetVal = true;
	}

	protected void OnConfigBrowseButtonClicked (object sender, EventArgs e)
	{
		configEntry.Text = ChooseFilePath ();
	}

	protected void OnDumpBrowseButtonClicked (object sender, EventArgs e)
	{
		dumpEntry.Text = ChooseFilePath ();
	}

	protected void OnSimRedirBrowseButtonClicked (object sender, EventArgs e)
	{
		simRedirEntry.Text = ChooseFolderPath ();
	}

	protected void OnProgRedirBrowseButtonClicked (object sender, EventArgs e)
	{
		progRedirEntry.Text = ChooseFolderPath ();
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

	private string ChooseFilePath(){
		string filePath = "";

		FileChooserDialog fileChooser = new FileChooserDialog (
			"Select File",
			this,
			FileChooserAction.Open,
			"Cancel", ResponseType.Cancel,
			"Open", ResponseType.Accept);

		if (fileChooser.Run () == (int)ResponseType.Accept) {
			filePath = fileChooser.Filename;
		}

		fileChooser.Destroy ();

		return filePath;
	}

	protected void OnBenchBrowseButtonClicked (object sender, EventArgs e)
	{
		string benchmarkPath = ChooseFilePath (); 

		string benchmarkName = benchmarkPath.Split ('/').Last();

		benchmarkName = benchmarkName.Split ('.').First();

		benchmarkName = char.ToUpper (benchmarkName [0]) + benchmarkName.Substring (1);

		benchNameLabel.Text = benchmarkName;
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
		benchNameLabel.Text = "benchName";
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

		//comb
		metaTableSizeSpinbutton.Value = 1024;

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
		throw new NotImplementedException ();	
	}
}

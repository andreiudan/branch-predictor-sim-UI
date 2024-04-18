using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Gtk;

public partial class MainWindow: Gtk.Window
{
	public MainWindow () : base (Gtk.WindowType.Toplevel)
    {
        ConnectionManager connectionManager = new ConnectionManager();

        connectionManager.addConnection("192.168.0.103", "3000");

        var tasks = connectionManager.executeCommands(new List<Params> { new Params(), new Params() });

        foreach(var task in tasks)
        {
            Task.Run(async () =>
            {
                var result = await task;

                Console.WriteLine(result.benchName);
            });
        }

        Build ();
	}

	protected void OnDeleteEvent (object sender, DeleteEventArgs a)
	{
		Application.Quit ();
		a.RetVal = true;
	}

	protected void OnBpredComboboxChanged (object sender, EventArgs e)
	{
		switch(bpredCombobox.ActiveText){
			case "nottaken":
				HideBpredTypesOptions ();
				break;
			case "taken":
				HideBpredTypesOptions ();
				break;
			case "bimod":
				HideBpredTypesOptions ();
				break;
			case "2lev":
				HideBpredTypesOptions ();
				ShowBpred2levOptions ();
				break;
			case "comb":
				HideBpredTypesOptions ();
				break;
			default:
				break;
		}
	}

	private void HideBpredTypesOptions(){
		//2lev
		l1sizeLabel.Hide();
		l1sizeSpinbutton.Hide();
		l2sizeLabel.Hide();
		l2sizeSpinbutton.Hide();
		histSizeLabel.Hide();
		histSizeSpinbutton.Hide();
		xorLabel.Hide();
		xorSpinbutton.Hide();

		//bimod
		tableSizeSpinbutton.Hide();
		tableSizeSpinbutton.Hide();

		//comb
		metaTableSizeLabel.Hide();
		metaTableSizeSpinbutton.Hide();

		//ras
		rasSizeLabel.Hide();
		rasSizeSpinbutton.Hide();

		//btb
		numSetsLabel.Hide();
		numSetsSpinbutton.Hide();
		assocLabel.Hide();
		assocSpinbutton.Hide();
	}

	private void ShowBpred2levOptions(){
		l1sizeLabel.Show();
		l1sizeSpinbutton.Show();
		l2sizeLabel.Show();
		l2sizeSpinbutton.Show();
		histSizeLabel.Show();
		histSizeSpinbutton.Show();
		xorLabel.Show();
		xorSpinbutton.Show();
	}

	private void ShowBpredNottakenOptions(){
		
	}

	private void ShowBpredTakenOptions(){

	}

	private void ShowBpredBimodOptions(){
		tableSizeLabel.Show();
		tableSizeSpinbutton.Show();
	}

	private void ShowBpredCombOptions(){

	}
}

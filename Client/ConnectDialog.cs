using System;
using Gtk;

namespace Client
{
    public partial class ConnectDialog : Gtk.Dialog
    {
        public event EventHandler<Tuple<string, string>> ValuesSubmited;

        public ConnectDialog()
        {

            Build();
        }

        protected void OnConnectButtonClicked(object sender, EventArgs e)
        {
            string ip = ipEntry.Text;
            string port = portEntry.Text;

            if (!CheckSubmitedValues(ip, port))
            {
                Destroy();
                return;
            }

            ValuesSubmited?.Invoke(this, Tuple.Create(ip, port));

            Destroy();
        }

        protected void OnCancelButtonClicked(object sender, EventArgs e)
        {
            Destroy();
        }

        private bool CheckSubmitedValues(string ip, string port)
        {
            string ipFormatErrorTitle = "Submited ip address not valid";
            string ipFormatErrorMessage = "The ip address should contain 4 sets of numbers " +
                "from 0 to 255 and should be separated by dots.";

            string portErrorTitle = "Submited port number not valid";
            string portErrorMessage = "The value inside the port entry should be a number.";

            string[] ipComponents = ip.Split('.');

            if (ipComponents.Length != 4)
            {
                ShowErrorDialog(this, ipFormatErrorTitle, ipFormatErrorMessage);
                return false;
            }

            foreach (string ipComponent in ipComponents)
            {
                int ipComponentAsInt;
                if (int.TryParse(ipComponent, out ipComponentAsInt))
                {
                    if (ipComponentAsInt < 0 || ipComponentAsInt > 255)
                    {
                        ShowErrorDialog(this, ipFormatErrorTitle, ipFormatErrorMessage);
                        return false;
                    }
                }
            }

            int portAsInt;
            if (!int.TryParse(port, out portAsInt))
            {
                ShowErrorDialog(this, portErrorTitle, portErrorMessage);
                return false;
            }

            return true;
        }

        private void ShowErrorDialog(Window parent, string title, string message)
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
    }
}

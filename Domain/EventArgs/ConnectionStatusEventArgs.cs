namespace Domain.EventArgs
{
    public class ConnectionStatusEventArgs : System.EventArgs
    {
        public string Ip { get; }
        public string Port { get; }
        public string Status { get; }

        public ConnectionStatusEventArgs(string ip, string port, string status)
        {
            Ip = ip;
            Port = port;
            Status = status;
        }
    }
}

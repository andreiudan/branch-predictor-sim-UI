using System;
using System.Threading.Tasks;
using System.ServiceModel;
using Newtonsoft;

public class Server
{
    IWCFServer server;

    public Server(string IP, string port)
    {
        NetTcpBinding binding = new NetTcpBinding();
        string url = $"net.tcp://{IP}:{port}/wcfserver";
        EndpointAddress address = new EndpointAddress(url);

        ChannelFactory<IWCFServer> channelFactory = new ChannelFactory<IWCFServer>(binding, address);
        server = channelFactory.CreateChannel();
    }

    public Task<Result> executeCommand(Params arg)
    {
        var result = Task.Run(() =>
        {
            string serializedArg = Newtonsoft.Json.JsonConvert.SerializeObject(arg);

            string serializedResult = server.executeCommand(serializedArg);

            return Newtonsoft.Json.JsonConvert.DeserializeObject<Result>(serializedResult);
        });

        return result;
    }
}

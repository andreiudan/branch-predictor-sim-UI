using System.ServiceModel;
using Domain.DTOs;
using Interfaces;
using System.Collections.Generic;
using System.Runtime.Serialization;

public class Server
{
    IWCFServer server;
    public int instanceProcesses;
    public string Ip { get; set; }
    public string Port { get; set; }

    public Server(string IP, string port)
    {
        Ip = IP;
        Port = port;
        instanceProcesses = 0;

        NetTcpBinding binding = new NetTcpBinding();
        binding.MaxReceivedMessageSize = 49152;
        binding.MaxBufferSize = 49152;
        binding.MaxBufferPoolSize = 49152;

        binding.ReaderQuotas.MaxStringContentLength = 49152;

        string url = $"net.tcp://{IP}:{port}/wcfserver";
        EndpointAddress address = new EndpointAddress(url);

        ChannelFactory<IWCFServer> channelFactory = new ChannelFactory<IWCFServer>(binding, address);
        server = channelFactory.CreateChannel();
    }

    public int getProcessesNo()
    {
        return server.getProcessesNo();
    }

    public List<Result> executeCommands(List<Params> args)
    {
        instanceProcesses++;

        string serializedArg = Newtonsoft.Json.JsonConvert.SerializeObject(args);

        string serializedResult = server.executeCommands(serializedArg);

        instanceProcesses--;

        return Newtonsoft.Json.JsonConvert.DeserializeObject<List<Result>>(serializedResult);

    }
}

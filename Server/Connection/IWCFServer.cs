using System;
using System.Threading.Tasks;
using System.ServiceModel;

[ServiceContract]
public interface IWCFServer
{
    [OperationContract]
    string executeCommand(string arg);
}


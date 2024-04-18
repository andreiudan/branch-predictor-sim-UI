using System;
using System.ServiceModel;

[ServiceContract]
public interface IWCFServer
{
    [OperationContract]
    string executeCommand(string arg);
}

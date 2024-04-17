using System;
using System.ServiceModel;

[ServiceContract]
public interface IWCFServer
{
    [OperationContract]
    int getProcesses();

    [OperationContract]
    string executeCommand(string arg);
}


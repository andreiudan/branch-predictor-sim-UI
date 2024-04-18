using System.ServiceModel;

namespace Interfaces
{
    [ServiceContract]
    public interface IWCFServer
    {
        [OperationContract]
        int getProcessesNo();

        [OperationContract]
        string executeCommands(string arg);
    }
}

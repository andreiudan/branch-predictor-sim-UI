using System.ServiceModel;

namespace Interfaces
{
    [ServiceContract]
    public interface IWCFServer
    {
        [OperationContract]
        string executeCommand(string arg);
    }
}

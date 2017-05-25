using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace RemoteDeviceControllerServer.Services
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IFileAccessService" in both code and config file together.
    [ServiceContract(SessionMode = SessionMode.Required, CallbackContract = typeof(IFileAccessCallback))]
    public interface IFileAccessService
    {
        /// <summary>
        /// e.g. filename 
        /// </summary>
        /// <param name="directory"></param>
        /// <returns></returns>
        [OperationContract(IsOneWay = false)]
        List<string> GetDirectoryFileNames(string directory);

        [OperationContract]
        List<string> GetDirectoryFolderNames(string directory);

        [OperationContract(IsOneWay = false)]
        byte[] GetFileThumbnail(string filePath);

        [OperationContract(IsOneWay = false)]
        byte[] GetFile(string filePath);

        [OperationContract(IsOneWay = false)]
        List<string> GetAccessibleDirectories();

        [OperationContract(IsOneWay = true)]
        void Connect(string name);

        [OperationContract]
        string GetServerName();
        [OperationContract(IsOneWay = true)]
        void StartGetView(string clientID);

        [OperationContract(IsOneWay = true)]
        void StopGetView(string clientID);
    }

    public interface IFileAccessCallback
    {
        [OperationContract(IsOneWay = true)]
        void SendView(byte[] data);
    }

    public class FileAccessEventArgs
    {

    }
}

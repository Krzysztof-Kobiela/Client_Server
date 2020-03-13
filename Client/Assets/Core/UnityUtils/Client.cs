using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

public class Client : MonoBehaviour
{
    public int HostID
    {
        get { return this._HostID; }
    }

    #region Private Fields

    private ConnectionConfig _Config;

    private HostTopology _Topology;

    private byte _ReliableSequencedChannel;
    private int _HostID;
    private int _ConnectionID;
    private byte _Error;

    private int _ExceptionConnectionID = 0;

    private const int _MaxConnections = 5;
    private const int _SocketPort = 6666;
    private const string _ServerIP = "127.0.0.1";

    private bool isStarted;

    #endregion





    public UnityEvent OnHostInitializationEnd;

    #region Public Methods
    public void Initialize()
    {
        NetworkTransport.Init();

        ConnectionConfig config = new ConnectionConfig();

        _ReliableSequencedChannel = config.AddChannel(QosType.ReliableSequenced);

        HostTopology topology = new HostTopology(config, _MaxConnections);

        //CLIENT ONLY
        //host mylące - addHost tak naprawde otwiera dany port
        _HostID = NetworkTransport.AddHost(topology, 0);

#if UNITY_WEBGL && !UNITY_EDITOR
        //WEB client
        _ConnectionID = NetworkTransport.Connect(_HostID, _ServerIP, _SocketPort, _ExceptionConnectionID, out _Error);   
        Debug.Log(string.Format("Connecting from WEB"));
#else
        //Standalone client
        _ConnectionID = NetworkTransport.Connect(_HostID, _ServerIP, _SocketPort, _ExceptionConnectionID, out _Error);
        Debug.Log(string.Format("Connecting from standalone"));
#endif

        Debug.Log(string.Format("Attempting to connect on {0}..", _ServerIP));

        OnHostInitializationEnd.Invoke();
    }

    public void Disconnect()
    {
        NetworkTransport.Disconnect(_HostID, _ConnectionID, out _Error);
    }

    public void SendReliableMessage(string message)
    {
        byte[] buffer = Encoding.Unicode.GetBytes(message);
        NetworkTransport.Send(_HostID, _ConnectionID, _ReliableSequencedChannel, buffer, message.Length * sizeof(char), out _Error);
    }

    public void SendUnreliableMessage(string message)
    {
        byte[] buffer = Encoding.Unicode.GetBytes(message);
        NetworkTransport.Send(_HostID, _ConnectionID, _ReliableSequencedChannel, buffer, message.Length * sizeof(char), out _Error);
    }



#endregion
}

using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

public class NetworkTransportManager : MonoBehaviour
{
    public int HostID
    {
        get { return this._HostId; }
    }

    #region Private Fields

    private ConnectionConfig _Config;

    private HostTopology _Topology;

    private int _MyReliableChannelId;
    private int _MyUnreliableChanellId;
    private int _HostId;
    private int _ConnectionID;
    private byte _Error;

    private string _HostIP = "127.0.0.1";
    private int _ExceptionConnectionID = 0;

    #endregion

    public int _MaxConnections = 5;
    public int _socketPort = 6666;

    public UnityEvent OnHostInitializationEnd;

    #region Public Methods
    public void Initialize()
    {
        try
        {
            NetworkTransport.Init();
        }
        catch
        {

        }
        

        _Config = new ConnectionConfig();
        _MyReliableChannelId = _Config.AddChannel(QosType.Reliable);
        _MyUnreliableChanellId = _Config.AddChannel(QosType.Unreliable);

        _Topology = new HostTopology(_Config, _MaxConnections);

        //host mylące - addHost tak naprawde otwiera dany port
        _HostId = NetworkTransport.AddHost(_Topology, _socketPort);

        Debug.Log("Socket open. Host ID: " + _HostId);

        OnHostInitializationEnd.Invoke();
    }

    public void Connect()
    {
        _ConnectionID = NetworkTransport.Connect(_HostId, _HostIP, _socketPort, _ExceptionConnectionID, out _Error);
    }

    public void Disconnect()
    {
        NetworkTransport.Disconnect(_HostId, _ConnectionID, out _Error);
    }

    public void SendReliableMessage(string message)
    {
        byte[] buffer = Encoding.Unicode.GetBytes(message);
        NetworkTransport.Send(_HostId, _ConnectionID, _MyReliableChannelId, buffer, message.Length * sizeof(char), out _Error);
    }

    public void SendUnreliableMessage(string message)
    {
        byte[] buffer = Encoding.Unicode.GetBytes(message);
        NetworkTransport.Send(_HostId, _ConnectionID, _MyReliableChannelId, buffer, message.Length * sizeof(char), out _Error);
    }

    #endregion
}

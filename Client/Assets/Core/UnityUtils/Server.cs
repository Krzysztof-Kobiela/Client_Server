using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

public class Server : MonoBehaviour
{
    //  tutorial link
    //  https://www.youtube.com/watch?v=amy3L3pGWH0
    //  tutorial link

    public int HostID
    {
        get { return this._HostID; }
    }

    #region Private Fields

    private byte _ReliableSequencedChannel;
    private int _HostID;
    private int _WebHostID;
    private byte _Error;

    private int _ExceptionConnectionID = 0;

    private const int _MaxConnections = 5;
    private const int _SocketPort = 6666;
    private const int _WebPort = 9999;
    private const int _ByteSize = 1024;
    

    private List<int> _ClientsID;

    private ChatManager _ChatManager;

    private bool isStarted;

    #endregion

    

    public UnityEvent OnHostInitializationEnd;

    #region Public Methods
    public void Initialize()
    {
        //_ChatManager = gameObject.GetComponent<ChatManager>();

        NetworkTransport.Init();

        ConnectionConfig config = new ConnectionConfig();

        _ReliableSequencedChannel=config.AddChannel(QosType.ReliableSequenced);


        HostTopology topology = new HostTopology(config, _MaxConnections);

        //SERVER ONLY
        //host mylące - addHost tak naprawde otwiera dany port
        _HostID = NetworkTransport.AddHost(topology, _SocketPort,null);
        //WEB umozliwia podlaczenie sie prxzegladarki do naszego serwera
        _WebHostID = NetworkTransport.AddHost(topology, _WebPort, null);

        Debug.Log(string.Format("Connection on port {0} and web port {1}", _SocketPort, _WebPort));

        OnHostInitializationEnd.Invoke();

        isStarted = true;
    }

    public void Awake()
    {
        _ClientsID = new List<int>();
        _ChatManager = gameObject.GetComponent<ChatManager>();
    }

    public void Update()
    {
        UpdateMessagePump();
    }

    public void UpdateMessagePump()
    {
        if (!isStarted)
            return;

        int recHostId;
        int connectionId;
        int channelId;

        byte[] recBuffer = new byte[_ByteSize];
        int datasize;

        NetworkEventType type = NetworkTransport.Receive(out recHostId, out connectionId, out channelId, recBuffer, _ByteSize, out datasize, out _Error);

        switch (type)
        {
            case NetworkEventType.Nothing:
                break;
            case NetworkEventType.ConnectEvent:
                Debug.Log(string.Format("User {0} has connected", connectionId));
                break;
            case NetworkEventType.DisconnectEvent:
                Debug.Log(string.Format("User {0} has disconnected", connectionId));
                break;
            case NetworkEventType.DataEvent:
                Debug.Log(string.Format("Data"));
                break;
            case NetworkEventType.BroadcastEvent:
                Debug.Log("Unexpected network event type");
                break;
        }
    }

    public void Shutdown()
    {
        isStarted = false;
        NetworkTransport.Shutdown();
    }

    /*    public void Connect()
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
        }*/

    #endregion
}

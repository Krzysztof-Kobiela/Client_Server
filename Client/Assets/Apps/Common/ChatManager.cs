using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ChatManager : MonoBehaviour
{
    public Text chat;
    public InputField inputField;

    private Client _Client;

    public void Awake()
    {
        _Client = gameObject.GetComponent<Client>();
    }

    public void ShowHostID()
    {
        string hostID=gameObject.GetComponent<Server>().HostID.ToString();

        chat.text += "I am HOST " + hostID + "\n";
    }

    public void SendMessage()
    {
        if (inputField.text != null)
        {
            string msg = "MSG|" + inputField.text;
            _Client.SendReliableMessage(msg);
        }
    }

    public void ShowMessage(string msg)
    {
        chat.text += msg;
    }

}

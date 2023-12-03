using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class IPSelectUI : MonoBehaviour
{
    public TMP_InputField input;

    public void OnConnectClick()
    {
        string ip = input.text;
        SocketClient.Instance.Initialize("http://" + ip + ":1234");
    }
}

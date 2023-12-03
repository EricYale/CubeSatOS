using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIOClient;
using SocketIOClient.Newtonsoft.Json;
using System;

public class SocketClient : MonoBehaviour
{
    public static SocketClient Instance { get; private set; }

    public UIController ui;
    public SocketIOUnity socket;

    bool registerConnectedOnNextThread = false;
    bool registerDisconnectedOnNextThread = false;

    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
            registerDisconnectedOnNextThread = true;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (registerConnectedOnNextThread)
        {
            registerConnectedOnNextThread = false;
            ui.OnConnect();
        }
        if (registerDisconnectedOnNextThread)
        {
            registerDisconnectedOnNextThread = false;
            ui.OnDisconnect();
        }
    }

    public void Initialize(string ip)
    {
        Uri uri = new Uri(ip);
        socket = new SocketIOUnity(uri);
        socket.JsonSerializer = new NewtonsoftJsonSerializer();

        socket.OnConnected += (sender, e) =>
        {
            Debug.Log("Socket connected!");
            registerConnectedOnNextThread = true;
        };

        socket.OnDisconnected += (sender, e) =>
        {
            Debug.Log("Socket disconnected!");
            registerDisconnectedOnNextThread = true;
        };

        socket.OnUnityThread("camera_data", response =>
        {
            Debug.Log("Received camera data");
            ui.controllerUI.DisplayCamera(response.GetValue<float[]>());
        });

        Debug.Log("Attempting to connect...");
        socket.Connect();
    }

    public void SetMotorPowers(float left, float right, float camera)
    {
        if (!socket.Connected)
        {
            Debug.LogWarning("Tried to send motor powers, but socket is disconnected.");
            //registerDisconnectedOnNextThread = true;
            return;
        }
        var data = new Dictionary<string, float>();
        data["l"] = left;
        data["r"] = right;
        data["c"] = camera;
        socket.Emit("set_motor_powers", data);
        Debug.Log("Emitted motor powers!\t" + left + ",\t" + right + ",\t" + camera);
    }

    public void Disconnect()
    {
        socket.Disconnect();
    }
}

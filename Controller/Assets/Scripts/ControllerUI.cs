using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerUI : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            SocketClient.Instance.SetMotorPowers(1, 1, 0);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            SocketClient.Instance.SetMotorPowers(-1, -1, 0);
        }
    }
}

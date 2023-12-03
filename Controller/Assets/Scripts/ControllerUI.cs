using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;

public class ControllerUI : MonoBehaviour
{
    public float pollingTime = 0.1f;

    Coroutine controlCoroutine;

    private void OnEnable()
    {
        controlCoroutine = StartCoroutine(ControlThread());
        EnhancedTouchSupport.Enable();
        TouchSimulation.Enable();
    }

    private void OnDisable()
    {
        if (controlCoroutine != null) StopCoroutine(controlCoroutine);
    }

    public void Disconnect()
    {
        SocketClient.Instance.Disconnect();
    }

    IEnumerator ControlThread()
    {
        while (true)
        {
            if (Input.GetKey(KeyCode.W))
            {
                SocketClient.Instance.SetMotorPowers(1, 1, 0);
            }
            else if (Input.GetKey(KeyCode.S))
            {
                SocketClient.Instance.SetMotorPowers(-1, -1, 0);
            }
            else
            {
                float[] res = ParseTouches();
                SocketClient.Instance.SetMotorPowers(res[0], res[2], res[1]);
            }
            yield return new WaitForSeconds(pollingTime);
        }
    }

    float[] ParseTouches()
    {
        float left = 0;
        float cam = 0;
        float right = 0;
        foreach (var touch in UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches)
        {
            float heightPercent = touch.screenPosition.y / Screen.height;
            float mapped = heightPercent * 2 - 1;
            if (touch.screenPosition.x < Screen.width / 3)
            {
                left = mapped;
            }
            else if (touch.screenPosition.x < Screen.width * 2 / 3)
            {
                cam = mapped;
            }
            else
            {
                right = mapped;
            }
        }
        return new float[] { left, cam, right };
    }
}

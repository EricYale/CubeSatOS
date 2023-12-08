using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using System.Linq;
using UnityEngine.UI;
using TMPro;

public class ControllerUI : MonoBehaviour
{
    public float cameraSensitivityScaling = 0.2f;
    public float pollingTime = 0.05f;
    public int cameraWidth = 32;
    public int cameraHeight = 24;
    public Image cameraImage;
    public TMP_Text text;

    Coroutine controlCoroutine;

    private void OnEnable()
    {
        controlCoroutine = StartCoroutine(ControlThread());
        EnhancedTouchSupport.Enable();
        TouchSimulation.Enable();
        //DisplayRandomImage();
        cameraImage.preserveAspect = true;
    }

    private void DisplayRandomImage()
    {
        float[] random = new float[cameraWidth * cameraHeight];
        for (int i = 0; i < random.Length; i++)
        {
            random[i] = Random.Range(0, 30);
            random[i] = i % cameraWidth;
        }
        DisplayCamera(random);
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
                cam = mapped * cameraSensitivityScaling;
            }
            else
            {
                right = mapped;
            }
        }
        return new float[] { left, cam, right };
    }

    public void DisplayCamera(float[] temperatures)
    {
        float min = temperatures.Min();
        float max = temperatures.Max();
        float range = max - min;

        float[] asPercent = temperatures.Select(i => (i - min) / range).ToArray();
        byte[] bytes = new byte[3 * temperatures.Length];
        for (int i = 0; i < asPercent.Length; i++)
        {
            float percent = asPercent[i];
            Color color = percent < 0.5f ?
                Color.Lerp(Color.blue, Color.white, percent / 0.5f) :
                Color.Lerp(Color.white, Color.red, (percent - 0.5f) / 0.5f);

            uint r = (uint)(color.r * 255);
            uint g = (uint)(color.g * 255);
            uint b = (uint)(color.b * 255);
            bytes[i * 3] = (byte)r;
            bytes[i * 3 + 1] = (byte)g;
            bytes[i * 3 + 2] = (byte)b;
        }

        Texture2D texture = new Texture2D(cameraWidth, cameraHeight, TextureFormat.RGB24, false);
        texture.LoadRawTextureData(bytes);
        texture.Apply();
        texture.filterMode = FilterMode.Trilinear;
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, cameraWidth, cameraHeight), Vector2.zero);
        cameraImage.sprite = sprite;
        text.text = "Min: " + min.ToString("0.00") + ", Max: " + max.ToString("0.00");
    }
}

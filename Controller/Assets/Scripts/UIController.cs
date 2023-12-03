using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public GameObject ipSelectScreen;
    public GameObject controllerScreen;

    public IPSelectUI ipSelectUI;
    public ControllerUI controllerUI;

    private void Start()
    {
        
    }

    public void OnConnect()
    {
        ipSelectScreen.SetActive(false);
        controllerScreen.SetActive(true);
    }
    public void OnDisconnect()
    {
        ipSelectScreen.SetActive(true);
        controllerScreen.SetActive(false);
    }
}

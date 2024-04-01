using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplicationController : MonoBehaviour
{
    private void Start()
    {
        DontDestroyOnLoad(gameObject);

        bool isDedicated = SystemInfo.graphicsDeviceType == 
                           UnityEngine.Rendering.GraphicsDeviceType.Null;
        
        LaunchInMode(isDedicated);
    }

    private void LaunchInMode(bool isDedicatedServer)
    {
        if (isDedicatedServer)
        {
            //Do something later...
        }
        else
        {
            
        }
    }
}

using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InputServer : MonoBehaviour
{
    public MyHttpServer httpServer;

    void Start()
    {
        httpServer = new MyHttpServer(2054, this);
        Thread thread = new Thread(new ThreadStart(httpServer.listen));
        thread.Start();
    }

    public void GetValue(string inVaule)
    {
        if (inVaule.Length >= 2)
        {
            if (inVaule[0] == '/' && inVaule[1] == '?')
                Debug.Log(inVaule);
        }
    }
}
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class InputServer : MonoBehaviour
{
    public MyHttpServer httpServer;

    public Movement playerMovement;

    private Thread thread;


    void Start()
    {
        httpServer = new MyHttpServer(2054, this);
        thread = new Thread(new ThreadStart(httpServer.listen));
        thread.Start();
    }

    public void GetValue(string inVaule)
    {
        if (inVaule.Length >= 2)
        {
            if (inVaule[0] == '/' && inVaule[1] == '?')
            {
                var words = inVaule.Split('?');
                string temp = words[1];
                words = temp.Split('&');

                if (words[0] == "left")
                {
                    playerMovement.current = 0;
                }

                else if (words[0] == "right")
                {
                    playerMovement.current = 1;
                }
                else
                {
                    playerMovement.current = -1;
                }
            }
        }
    }
}
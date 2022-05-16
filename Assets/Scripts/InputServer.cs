using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
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


    public string GetValue(string inVaule)
    {
        return "";
        //分析，传递给其他文件
        //do it
    }
}
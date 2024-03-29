using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Collections;
using UnityEngine;

public class HttpProcessor
{
    public TcpClient socket;
    public HttpServer srv;

    private Stream inputStream;
    public StreamWriter outputStream;

    public String http_method;
    public String http_url;
    public String http_protocol_versionstring;
    public Hashtable httpHeaders = new Hashtable();


    private static int MAX_POST_SIZE = 10 * 1024 * 1024; // 10MB

    public HttpProcessor(TcpClient s, HttpServer srv)
    {
        this.socket = s;
        this.srv = srv;
    }


    private string streamReadLine(Stream inputStream)
    {
        int next_char;
        string data = "";
        while (true)
        {
            next_char = inputStream.ReadByte();
            if (next_char == '\n')
            {
                break;
            }

            if (next_char == '\r')
            {
                continue;
            }

            if (next_char == -1)
            {
                Thread.Sleep(1);
                continue;
            }

            ;
            data += Convert.ToChar(next_char);
        }

        return data;
    }

    public void process()
    {
        // we can't use a StreamReader for input, because it buffers up extra data on us inside it's
        // "processed" view of the world, and we want the data raw after the headers
        inputStream = new BufferedStream(socket.GetStream());

        // we probably shouldn't be using a streamwriter for all output from handlers either
        outputStream = new StreamWriter(new BufferedStream(socket.GetStream()));
        try
        {
            parseRequest();
            readHeaders();
            if (http_method.Equals("GET"))
            {
                handleGETRequest();
            }
            else if (http_method.Equals("POST"))
            {
                handlePOSTRequest();
            }
        }
        catch (Exception e)
        {
            writeFailure();
        }

        outputStream.Flush();
        // bs.Flush(); // flush any remaining output
        inputStream = null;
        outputStream = null; // bs = null;            
        socket.Close();
    }

    public void parseRequest()
    {
        String request = streamReadLine(inputStream);
        string[] tokens = request.Split(' ');
        if (tokens.Length != 3)
        {
            throw new Exception("invalid http request line");
        }

        http_method = tokens[0].ToUpper();
        http_url = tokens[1];
        http_protocol_versionstring = tokens[2];
    }

    public void readHeaders()
    {
        String line;
        while ((line = streamReadLine(inputStream)) != null)
        {
            if (line.Equals(""))
            {
                return;
            }

            int separator = line.IndexOf(':');
            if (separator == -1)
            {
                throw new Exception("invalid http header line: " + line);
            }

            String name = line.Substring(0, separator);
            int pos = separator + 1;
            while ((pos < line.Length) && (line[pos] == ' '))
            {
                pos++; // strip any spaces
            }

            string value = line.Substring(pos, line.Length - pos);
            httpHeaders[name] = value;
        }
    }

    public void handleGETRequest()
    {
        srv.handleGETRequest(this);
    }

    private const int BUF_SIZE = 4096;

    public void handlePOSTRequest()
    {
        int content_len = 0;
        MemoryStream ms = new MemoryStream();
        if (this.httpHeaders.ContainsKey("Content-Length"))
        {
            content_len = Convert.ToInt32(this.httpHeaders["Content-Length"]);
            if (content_len > MAX_POST_SIZE)
            {
                throw new Exception(
                    String.Format("POST Content-Length({0}) too big for this simple server",
                        content_len));
            }

            byte[] buf = new byte[BUF_SIZE];
            int to_read = content_len;
            while (to_read > 0)
            {
                int numread = this.inputStream.Read(buf, 0, Math.Min(BUF_SIZE, to_read));
                if (numread == 0)
                {
                    if (to_read == 0)
                    {
                        break;
                    }
                    else
                    {
                        throw new Exception("client disconnected during post");
                    }
                }

                to_read -= numread;
                ms.Write(buf, 0, numread);
            }

            ms.Seek(0, SeekOrigin.Begin);
        }

        srv.handlePOSTRequest(this, new StreamReader(ms));
    }

    public void writeSuccess()
    {
        outputStream.WriteLine("HTTP/1.0 200 OK");
        outputStream.WriteLine("Content-Type: text/html");
        outputStream.WriteLine("Connection: close");
        outputStream.WriteLine("");
    }

    public void writeFailure()
    {
        outputStream.WriteLine("HTTP/1.0 404 File not found");
        outputStream.WriteLine("Connection: close");
        outputStream.WriteLine("");
    }
}

public abstract class HttpServer
{
    protected int port;
    private InputServer gameServer;
    TcpListener listener;
    bool is_active = true;

    public HttpServer(int port, InputServer inServer)
    {
        this.port = port;
        this.gameServer = inServer;
    }

    public void listen()
    {
        listener = new TcpListener(port);
        listener.Start();
        while (is_active)
        {
            TcpClient s = listener.AcceptTcpClient();
            HttpProcessor processor = new HttpProcessor(s, this);
            Thread thread = new Thread(new ThreadStart(processor.process));
            thread.Start();
            Thread.Sleep(1);
        }
    }

    public abstract void handleGETRequest(HttpProcessor p);
    public abstract void handlePOSTRequest(HttpProcessor p, StreamReader inputData);
}

public class MyHttpServer : HttpServer
{
    private InputServer gameServer;

    public MyHttpServer(int port, InputServer inputServer) : base(port, inputServer)
    {
        gameServer = inputServer;
    }

    public override void handleGETRequest(HttpProcessor p)
    {
        //request input
        gameServer.GetValue(p.http_url);

        p.writeSuccess();
        p.outputStream.WriteLine("<html><body><h1>test server</h1>");
        p.outputStream.WriteLine("Current Time: " + DateTime.Now.ToString());
        p.outputStream.WriteLine("url : {0}", p.http_url);

        p.outputStream.WriteLine("<form method=post action=/form>");
        p.outputStream.WriteLine("<input type=text name=foo value=foovalue>");
        p.outputStream.WriteLine("<input type=submit name=bar value=barvalue>");
        p.outputStream.WriteLine("</form>");
    }

    public override void handlePOSTRequest(HttpProcessor p, StreamReader inputData)
    {
        string data = inputData.ReadToEnd();

        p.outputStream.WriteLine("<html><body><h1>test server</h1>");
        p.outputStream.WriteLine("<a href=/test>return</a><p>");
        p.outputStream.WriteLine("postbody: <pre>{0}</pre>", data);
    }
}
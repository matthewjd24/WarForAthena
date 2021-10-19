using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class SslClient : MonoBehaviour
{
    public class Stream : SslStream
    {
        public Stream(System.IO.Stream innerStream, RemoteCertificateValidationCallback a, LocalCertificateSelectionCallback b) 
            : base(innerStream, false, a, b) { }

        public bool IsClosed { get; set; }
        protected override void Dispose(bool disposing)
        {
            IsClosed = true;
            base.Dispose(disposing);
        }
    }
    public static SslClient inst;
    public static Stream sslStream;
    public static TcpClient client;
    int bufferLength = 4000;
    byte[] byteBuffer;

    [SerializeField] bool isClosed;
    [SerializeField] bool debugMode;

    private void Awake()
    {
        inst = this;
        byteBuffer = new byte[bufferLength];
    }

    private void Update()
    {
        if(sslStream == null) {
            isClosed = true;
            return;
        }

        isClosed = sslStream.IsClosed;
    }

    public void Connect(string ip)
    {
        StartCoroutine(TryConnect(ip));
    }

    IEnumerator TryConnect(string ip)
    {
        client = new TcpClient();
        client.ConnectAsync(ip, 25040);
        yield return new WaitUntil(() => client.Connected);

        Log("Established TCP client");

        sslStream = new Stream(client.GetStream(), new RemoteCertificateValidationCallback(ValidateServerCertificate), null );

        static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            X509Certificate2 cert = (X509Certificate2)certificate;
            if (cert.Thumbprint.ToLower() == "53a980b187f2501d6aca48b1d8be4fe128a2d7c5") {
                return true;
            }
            else {
                return false;
            }
        }
        sslStream.AuthenticateAsClientAsync("FakeServerName");
        yield return new WaitUntil(() => sslStream.IsAuthenticated);

        Log("Connected via SSL to the server");

        StartReadingStream();
    }

    void StartReadingStream()
    {
        sslStream.BeginRead(byteBuffer, 0, bufferLength, ReceiveMessage, null);
    }

    void ReceiveMessage(IAsyncResult ar)
    {
        if (sslStream.IsClosed) {
            Log("Stream is closed.");
            return;
        }

        int bytes;
        try {
            bytes = sslStream.EndRead(ar);
        }
        catch (Exception e) {
            Debug.Log(e.Message);
            if(e.Message.Contains("forcibly closed")) {
                CloseConnection();
            }
            return;
        }

        if (bytes == 0) {
            Log("Bytes is 0");
            return;
        }

        Console.WriteLine("Received data size: " + bytes);
        byte[] receivedData = new byte[bytes];
        Array.Copy(byteBuffer, receivedData, bytes);
        byteBuffer = new byte[bufferLength];

        StartReadingStream();

        EventManager.ProcessServerMsg(receivedData);
    }

    public static void CloseConnection()
    {
        if (sslStream != null) {
            Debug.Log("Closing connection");
            sslStream.Close();
        }

        if (client != null) {
            client.Close();
        }

        sslStream = null;
        client = null;
    }

    private void OnDisable()
    {
        CloseConnection();
    }

    void Log(string msg)
    {
        if (debugMode) Debug.Log(msg);
    }
}
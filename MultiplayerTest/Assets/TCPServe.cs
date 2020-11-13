using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class TCPServe : MonoBehaviour
{
  private TcpListener _tcpListener;
  private Thread _tcpListenerThread;
  private TcpClient _connectedTcpClient;

  private IPAddress _ip;
  private int _port = 8052;

  private string _output;

  [SerializeField] private Text text;

  private bool _active;
  
  public void ConnectDisconnect()
  {
    if (_active)
      StopServer();
    else
      StartServer();
  }

  private void StartServer()
  {
    Input.gyro.enabled = true;
    _active = true;
    
    _ip = Dns.GetHostAddresses(Dns.GetHostName())[0];
    text.text = _ip.ToString();
    
    _tcpListenerThread = new Thread(ListenForIncomingRequests);
    _tcpListenerThread.IsBackground = true;
    _tcpListenerThread.Start();
  }

  private void Update()
  {
    if (!_active) return;
    GyroToString();
    SendData();
  }

  private void GyroToString()
  {
    _output = Input.gyro.attitude.w.ToString();
    _output += "," + Input.gyro.attitude.x.ToString();
    _output += "," + Input.gyro.attitude.y.ToString();
    _output += "," + Input.gyro.attitude.z.ToString();
    _output = _output.Length + _output;
  }

  private void ListenForIncomingRequests()
  {
    try
    {
      _tcpListener = new TcpListener(_ip, _port);
      _tcpListener.Start();
      Debug.Log("Server is listening");
      Byte[] bytes = new Byte[64];
      while (_active)
      {
        using (_connectedTcpClient = _tcpListener.AcceptTcpClient())
        {
          using (NetworkStream stream = _connectedTcpClient.GetStream())
          {
            int length;
            while ((length = stream.Read(bytes, 0, bytes.Length)) != 0)
            {
              var incomingData = new byte[length];
              Array.Copy(bytes, 0, incomingData, 0, length);
              string clientMessage = Encoding.ASCII.GetString(incomingData);
              Debug.Log("Client message received as: " + clientMessage);
            }
          }
        }
      }
    }
    catch (SocketException socketException)
    {
      Debug.Log("SocketException " + socketException.ToString());
    }
  }

  private void SendData()
  {
    if (_connectedTcpClient == null) {             
      return;         
    }  		
		
    try {
      NetworkStream stream = _connectedTcpClient.GetStream(); 			
      if (stream.CanWrite) {
        byte[] serverMessageAsByteArray = Encoding.ASCII.GetBytes(_output);
        stream.Write(serverMessageAsByteArray, 0, serverMessageAsByteArray.Length);               
        Debug.Log("Server sent:" + _output + "; should be received by client");           
      }       
    } 		
    catch (SocketException socketException) {             
      Debug.Log("Socket exception: " + socketException);         
    } 	
  }

  private void StopServer()
  {
    _active = false;
    _tcpListener.Stop();
    _connectedTcpClient = null;
  }

  private void OnDestroy()
  {
    if (_active)
      StopServer();
  }
}

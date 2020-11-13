using System;
using System.Globalization;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class TCPConnect : MonoBehaviour
{
	[SerializeField] public InputField ip;
	[SerializeField] private Transform debugTransform;
	private int _port = 8052;
	
	private TcpClient _socketConnection; 	
	private Thread _clientReceiveThread;
	
	private Quaternion _serverOutput = new Quaternion();

	private bool _active = false;
	
	void Update ()
	{
		if (!_active) return;
		debugTransform.rotation = Quaternion.Lerp(_serverOutput, debugTransform.rotation, .8f);
	}

	public void ConnectDisconnect()
	{
		if (_active)
			StopClient();
		else
			StartClient();
	}
	
	/// <summary> 	
	/// Setup socket connection. 	
	/// </summary> 	
	private void StartClient () { 		
		try
		{
			_active = true;
			_clientReceiveThread = new Thread (ListenForData); 			
			_clientReceiveThread.IsBackground = true; 			
			_clientReceiveThread.Start();  		
		} 		
		catch (Exception e) { 			
			Debug.Log("On client connect exception " + e); 		
		} 	
	}  	
	
	/// <summary> 	
	/// Runs in background _clientReceiveThread; Listens for incoming data. 	
	/// </summary>     
	private void ListenForData() { 		
		try { 			
			_socketConnection = new TcpClient(ip.text, 8052);  			
			Byte[] bytes = new Byte[_socketConnection.ReceiveBufferSize];     
			int length;
			using (NetworkStream stream = _socketConnection.GetStream()) 
			{
				while (_active) 
				{ 
					if (stream.CanRead) 
					{
						length = stream.Read(bytes, 0, bytes.Length);
						string serverMessage = Encoding.ASCII.GetString(bytes, 0, length);
						ConvertSaveInput(serverMessage);
					}
				} 			
			}         
		}         
		catch (SocketException socketException) {             
			Debug.Log("Socket exception: " + socketException);         
		}     
	}

	private void ConvertSaveInput(string input)
	{
		//Debug.Log("Server message received as: " + input);
		//string header = input.Substring(0, 2);
		//if (header.Contains(".") || header.Contains(",")) return;
		int length = int.Parse(input.Substring(0, 2));
		input = input.Substring(2, length);
		//Debug.Log("Server message filtered as: " + input);
		string[] splitString = input.Split(',');
		if (splitString.Length > 3)
		{
			print(float.Parse(splitString[0]));
			_serverOutput = new Quaternion(float.Parse(splitString[1], CultureInfo.InvariantCulture.NumberFormat), float.Parse(splitString[2], CultureInfo.InvariantCulture.NumberFormat)
				, float.Parse(splitString[3], CultureInfo.InvariantCulture.NumberFormat), float.Parse(splitString[0], CultureInfo.InvariantCulture.NumberFormat));
		}
	}

	public Quaternion GetPhoneRotation()
	{
		return _serverOutput;
	}

	private void StopClient()
	{
		_active = false;
		_socketConnection.Close();
	}

	private void OnDestroy()
	{
		if (_active)
			StopClient();
	}
}
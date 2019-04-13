using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using System.IO;

public class TCPTestClient : MonoBehaviour {  	
	#region private members 	
	private TcpClient socketConnection; 	
	private Thread clientReceiveThread; 	
	#endregion  	
	// Use this for initialization 	
	byte[] prefetch_fn;
	//int fid = 0; //initial frameid
	//int fid_max = 25000;
    CallPrefetch cp;
    FileStream fs;
    int START_LEN = 20;
    int count;
    int fid_rx, filesize;


    string localPath = "C:/Users/spauldsnl/Documents/decoding_videos/viking_texas/server_fetch/";

	void Start () {
        
        Debug.Log ("\n Start () ");
		ConnectToTcpServer();    
		//for customized prefetch request
		prefetch_fn = new byte[20];
        cp = GameObject.Find("Callprefetch").GetComponent<CallPrefetch>();
    }  	
	// Update is called once per frame
    /*
	void Update () {
		Debug.Log ("\n Inside Update ");
		//SendMessage ();
		if(fid<2500) fid++;
		Sendfid(fid);
		ListenForData ();
		//if (Input.GetKeyDown(KeyCode.Space)) {             
		//	SendMessage();         
		//}     
	}  	
	*/ 	
	public void ConnectToTcpServer () { 		
		try {  	
			Debug.Log ("\n connecttotcpserver () ");
            socketConnection = new TcpClient("192.168.1.130", 5000);
            /*
            clientReceiveThread = new Thread (new ThreadStart(ListenForData)); 			
			clientReceiveThread.IsBackground = true; 			
			clientReceiveThread.Start();  
            */
            
            
		} 		
		catch (Exception e) { 			
			Debug.Log("On client connect exception " + e); 		
		} 	
	}  	
	
	/// Runs in background clientReceiveThread; Listens for incomming data. 	
	 
    /*
	public void ListenForData() {
        try { 	
			//Debug.Log ("\n listenfordata () ");
			//socketConnection = new TcpClient("192.168.1.130", 5000);  //"192.168.1.130"//"192.168.1.110"			
            Byte[] bytes = new Byte[409600];             
			while (true) {
                // Get a stream object for reading 
                string video_path = @"C:/Users/spauldsnl/Documents/decoding_videos/viking_texas/server_fetch/" + (cp.fid).ToString() + ".mp4";
                Debug.Log("\n the desired video path = " + video_path);
                if (File.Exists(video_path))
                {
                    File.Delete(video_path);
                }

                fs = File.Create(video_path);
                using (NetworkStream stream = socketConnection.GetStream())
                {
                    int length;
                    // Read incomming stream into byte arrary. 					
                    while ((length = stream.Read(bytes, 0, bytes.Length)) != -1)
                    {
                        Debug.Log("\n the length of the received bytes from the server = " + length);
                        fs.Write(bytes, 0, length);
                        fs.Flush();
                    }
                }
                fs.Close();
			}         
		}         
		catch (SocketException socketException) {             
			Debug.Log("Socket exception: " + socketException);         
		}     
	}  
    */

	/// <summary> 	
	/// Send message to server using socket connection. 	
	/// </summary> 	
	public void SendMessage(int fid) {         
		if (socketConnection == null) {    
			Debug.Log ("\n socket connection is null ");
			return;         
		}  		
		try { 
			
			// Get a stream object for writing. 
			Debug.Log ("\n sendMsg() ");
			NetworkStream stream = socketConnection.GetStream(); 			
			if (stream.CanWrite) {                 
				string clientMessage = "This is a message from one of your clients."; 				
				// Convert string message to byte array.                 
				byte[] clientMessageAsByteArray = Encoding.ASCII.GetBytes(clientMessage); 				
				// Write byte array to socketConnection stream.                 
				stream.Write(clientMessageAsByteArray, 0, clientMessageAsByteArray.Length);                 
				Debug.Log("Client sent his message - should be received by server");             
			}         
		} 		
		catch (SocketException socketException) {             
			Debug.Log("Socket exception: " + socketException);         
		}     
	} 


	//customized sendmessage
	public void Sendfid(int fid) {         
		if (socketConnection == null) {    
			Debug.Log ("\n socket connection is null ");
			return;         
		}  		
		try {
            byte[] prefetch_fn;
            prefetch_fn = new byte[20];
            Debug.Log("\n from sendfid: the current fid =" +fid);
			// Get a stream object for writing. 
			Debug.Log ("\n sendfid() ");
			NetworkStream stream = socketConnection.GetStream(); 			
			if (stream.CanWrite) {                 
				int _fid = fid;
				for (int i = 9; i >= 0; i--) {
                    //Debug.Log("\n Just Before the prefetch fn");
					//prefetch_fn[i] = Convert.ToByte((char)(_fid % 10 + 48));
                    prefetch_fn[i] = Convert.ToByte((char)(_fid % 10 + 48 ));
                    _fid /= 10;
				}
				prefetch_fn [10] = 0;
                // Write byte array to socketConnection stream.   
                Debug.Log("\n prefetch_fn= " +prefetch_fn.ToString());
                Debug.Log("\n the length of the prefetch fn " + prefetch_fn.Length);
				stream.Write(prefetch_fn, 0, prefetch_fn.Length);                 
				Debug.Log("Client sent his message - should be received by server");             
			}         
		} 		
		catch (SocketException socketException) {             
			Debug.Log("Socket exception: " + socketException);         
		}     
	}
    
    public IEnumerator ListenForData(int fid)
    {
        while(true)
        {
            //Debug.Log ("\n listenfordata () ");
            //socketConnection = new TcpClient("192.168.1.130", 5000);  //"192.168.1.130"//"192.168.1.110"			
            
            try
            {
                // Get a stream object for reading 	
                Byte[] bytes = new Byte[409600];
                double start_time;
                //using (NetworkStream stream = socketConnection.GetStream())
                {
                    int length;
                    // Read incomming stream into byte arrary. 		
                    start_time = Time.realtimeSinceStartup;
                    //inorder to resolve the fid and filesize
                    
                    
                    length = socketConnection.GetStream().Read(bytes, 0, bytes.Length);
                    Debug.Log("\n the total amount of bytes received from the network stream = " + length);
                    
                    if (length == START_LEN)
                    {
                        fid_rx = 0;
                        filesize = 0;
                        Debug.Log("\n First resolving the frame ids and getting the filesize \n");
                        for (int i = 2; i < 8; i++)
                            fid_rx = fid_rx * 10 + (bytes[i] - '0');

                        for (int i = 8; i < 14; i++)
                            filesize = filesize * 10 + (bytes[i] - '0');
                        Debug.Log("\n fid_rx = " + fid_rx + ", filesize = " + filesize);
                        count = 0;

                        
                        string video_path = @"C:/Users/spauldsnl/Documents/decoding_videos/viking_texas/server_fetch/" + fid_rx.ToString() + ".mp4";
                        Debug.Log("\n the desired video path = " + video_path);
                        if (File.Exists(video_path))
                        {
                            File.Delete(video_path);
                        }

                        fs = File.Create(video_path);
                        
                    }
                    else
                    {
                        Debug.Log("\n entering into file writing stage \n");

                        if (count <= filesize)
                        {
                            fs.Write(bytes, 0, length);
                            fs.Flush();
                            count += length;
                        }
                        if(count>=filesize)
                        {
                            Debug.Log("\n Current count = " + count + ", fileSize = " + filesize);
                            break;
                        }
                    }


                    /*while ((length = stream.Read(bytes, 0, bytes.Length)) != -1)
                    {
                        Debug.Log("\n the length of the received bytes from the server = " + length);
                        fs.Write(bytes, 0, length);
                        fs.Flush();

                    }
                    */

                }
                Debug.Log("\n the time to get the entire data = " + (Time.realtimeSinceStartup - start_time));
                //fs.Close();
            }
            catch (SocketException socketException)
            {
                Debug.Log("\n Under Socket Exception");
                Debug.Log("Socket exception: " + socketException);
            }
        }
        
        yield return null;
    }
    
}



/*
 * // Convert byte array to string message. 
                        var incommingData = new byte[length];
                        Array.Copy(bytes, 0, incommingData, 0, length);
                        string serverMessage = Encoding.ASCII.GetString(incommingData); 						
						Debug.Log("server message received as: " + serverMessage); 		
*/
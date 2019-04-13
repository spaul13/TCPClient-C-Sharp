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
    FileStream fs, fs_1, fs_2, fs_3;
    int START_LEN = 40; //20;(for single packet reception)
    int count;
    int fid_rx, filesize, filesize_1, filesize_2, filesize_3, total_size;
    double actual_total_time;
    int offset;


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
	public void Sendfid(List<int> fid_list) {         
		if (socketConnection == null) {    
			Debug.Log ("\n socket connection is null ");
			return;         
		}  		
		try {
            byte[] prefetch_fn;
            prefetch_fn = new byte[20];

            Debug.Log("\n Fids need to be fetched \n");
            for (int i = 0; i < fid_list.Count; i++) Debug.Log(fid_list[i] + "\n");
			// Get a stream object for writing. 
			Debug.Log ("\n sendfid() ");
			NetworkStream stream = socketConnection.GetStream(); 			
			if (stream.CanWrite) {                 
				int _fid = fid_list[0];
                Debug.Log("\n sending three requests \n");
				//for (int i = 9; i >= 0; i--) {
                for (int i = 6; i >= 0; i--)
                {
                    prefetch_fn[i] = Convert.ToByte((char)(_fid % 10 + 48 ));
                    _fid /= 10;
				}


                if (fid_list.Count > 1)
                {
                    _fid = fid_list[1];
                    for (int i = 12; i >= 7; i--)
                    {
                        prefetch_fn[i] = Convert.ToByte((char)(_fid % 10 + 48));
                        _fid /= 10;
                    }
                }

                //atmax 3 frames need to be prefetched
                if (fid_list.Count > 2)
                {
                    _fid = fid_list[2];
                    for (int i = 18; i >= 13; i--)
                    {
                        prefetch_fn[i] = Convert.ToByte((char)(_fid % 10 + 48));
                        _fid /= 10;
                    }
                    prefetch_fn[19] = 0;
                }
                // Write byte array to socketConnection stream.   
                    //Debug.Log("\n Fids are requested by the client = " + fid + "," + temp1 + "," + temp2);
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
    
    //will resolve fids and filesize from the server first 40 byte packet (uses same TCP connection)
    public IEnumerator ListenForData()
    {
        while (true)
        {
            //Debug.Log ("\n listenfordata () ");
            //socketConnection = new TcpClient("192.168.1.130", 5000);  //"192.168.1.130"//"192.168.1.110"			
            
            try
            {
                // Get a stream object for reading 	
                Byte[] bytes = new Byte[409600];
                double start_time = 0;
                //using (NetworkStream stream = socketConnection.GetStream())
                {
                    int length;
                    // Read incomming stream into byte arrary. 
                    
                    length = socketConnection.GetStream().Read(bytes, 0, bytes.Length);
                    Debug.Log("\n the total amount of bytes received from the network stream = " + length);

                    //inorder to resolve the fid and filesize
                    if (length == START_LEN)
                    {
                        fid_rx = 0;
                        filesize_1 = 0;
                        Debug.Log("\n First resolving the frame ids and getting the filesize \n");
                        for (int i = 2; i < 8; i++)
                            fid_rx = fid_rx * 10 + (bytes[i] - '0');

                        for (int i = 8; i < 14; i++)
                            filesize_1 = filesize_1 * 10 + (bytes[i] - '0');
                        Debug.Log("\n fid_rx = " + fid_rx + ", filesize = " + filesize);
                        count = 0;
                        Debug.Log("\n the first fid = " + fid_rx + ", fileSize = " + filesize_1);

                        string video_path = @"C:/Users/spauldsnl/Documents/decoding_videos/viking_texas/server_fetch/" + fid_rx.ToString() + ".mp4";
                        Debug.Log("\n the desired video path = " + video_path);
                        if (File.Exists(video_path))
                        {
                            File.Delete(video_path);
                        }

                        fs_1 = File.Create(video_path);

                        //for the second file
                        fid_rx = 0;
                        filesize_2 = 0;
                        Debug.Log("\n Second resolving the frame ids and getting the filesize \n");
                        for (int i = 14; i < 20; i++)
                            fid_rx = fid_rx * 10 + (bytes[i] - '0');

                        for (int i = 20; i < 26; i++)
                            filesize_2 = filesize_2 * 10 + (bytes[i] - '0');
                        Debug.Log("\n fid_rx = " + fid_rx + ", filesize = " + filesize);
                        count = 0;
                        Debug.Log("\n the second fid = " + fid_rx + ", fileSize = " + filesize_2);
                        if (filesize_2 > 0)
                        {
                            video_path = @"C:/Users/spauldsnl/Documents/decoding_videos/viking_texas/server_fetch/" + fid_rx.ToString() + ".mp4";
                            Debug.Log("\n the desired video path = " + video_path);
                            if (File.Exists(video_path))
                            {
                                File.Delete(video_path);
                            }

                            fs_2 = File.Create(video_path);
                        }
                        //for the third file
                        fid_rx = 0;
                        filesize_3 = 0;
                        Debug.Log("\n Second resolving the frame ids and getting the filesize \n");
                        for (int i = 26; i < 32; i++)
                            fid_rx = fid_rx * 10 + (bytes[i] - '0');

                        for (int i = 32; i < 38; i++)
                            filesize_3 = filesize_3 * 10 + (bytes[i] - '0');
                        Debug.Log("\n fid_rx = " + fid_rx + ", filesize = " + filesize);
                        count = 0;
                        Debug.Log("\n the third fid = " + fid_rx + ", fileSize = " + filesize_3);
                        if (filesize_3 > 0)
                        {
                            video_path = @"C:/Users/spauldsnl/Documents/decoding_videos/viking_texas/server_fetch/" + fid_rx.ToString() + ".mp4";
                            Debug.Log("\n the desired video path = " + video_path);
                            if (File.Exists(video_path))
                            {
                                File.Delete(video_path);
                            }

                            fs_3 = File.Create(video_path);
                        }

                        total_size = filesize_1 + filesize_2 + filesize_3;
                        Debug.Log("\n total size of those three video files = " + total_size);

                    }
                    else
                    {
                        Debug.Log("\n entering into file writing stage \n");
                        if(count==0) start_time = Time.realtimeSinceStartup;
                        Debug.Log("\n count = " + count + ", length = " + length + ", filesize_1 = " + filesize_1 + ", filesize_2 = " + filesize_2 + ", filesize_3 = " + filesize_3);

                        if (count <= total_size)
                        {
                            if (count <= filesize_1)
                            {
                                Debug.Log("\n Inside first fs");
                                if (count + length > filesize_1)
                                {
                                    fs_1.Write(bytes, 0, filesize_1 - count);
                                    offset = filesize_1 - count;
                                    count = filesize_1;
                                    Debug.Log("\n 1st Here offset is being changed \n");
                                    Debug.Log("\n 1st current length = " + length + ", offset = " + offset + ", count = " + count);
                                    length = length - offset; //remaining length
                                    Debug.Log("\n 1st: the modified length is = " + length);
                                }
                                else
                                {
                                    fs_1.Write(bytes, 0, length);
                                    offset = 0;
                                    count += length;
                                }
                                fs_1.Flush();
                                
                            }
                            if((count >= filesize_1) && (count <=(filesize_1 + filesize_2)) && (filesize_2 > 0))
                            {
                                Debug.Log("\n Inside 2nd fs");
                                if (count + length > (filesize_1+filesize_2))
                                {
                                    fs_2.Write(bytes, 0, (filesize_1 + filesize_2) - count);
                                    offset = (filesize_2 + filesize_1) - count;
                                    count = filesize_1+filesize_2;
                                    Debug.Log("\n 2nd Here offset is being changed \n");
                                    Debug.Log("\n 2nd current length = " + length + ", offset = " + offset + ", count = " + count);
                                    length = length - offset; //remaining length
                                    Debug.Log("\n 2nd: the modified length is = " + length);
                                }
                                else
                                {
                                    
                                    fs_2.Write(bytes, offset, length);
                                    count += length;
                                    Debug.Log("\n 2nd: the current offset = " + offset + ", length = " + length + ", count = " + count);
                                    offset = 0;
                                    
                                }
                                fs_2.Flush();
                            }
                            if ((count >= (filesize_1 + filesize_2)) && (count <= total_size) && (filesize_3 > 0))
                            {
                                Debug.Log("\n Inside 3rd fs");
                                fs_3.Write(bytes, offset, length);
                                count += length;
                                Debug.Log("\n 3rd: the current offset = " + offset + ", length = " + length + ", count = " + count);
                                offset = 0;
                                fs_3.Flush();
                            }
                            
                        }
                        if(count>=total_size)
                        {
                            Debug.Log("\n all three videos are received");
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
                Debug.Log("\n actual total time to get the entire data (in ms) = " + actual_total_time * 1000);
                double bw_consp = (total_size * 8) / (Time.realtimeSinceStartup - start_time);
                bw_consp /= 1000000;
                Debug.Log("\n BW consumption (Mbps) = " + bw_consp);
                //Debug.Log("\n the time to get the entire data = " + (Time.realtimeSinceStartup - start_time));
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
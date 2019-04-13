using UnityEngine;
using System.Collections;
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;

public class UDPClient : MonoBehaviour {

	public UdpClient client;
	public IPAddress serverIp;
	public string hostIp = "192.168.1.131";
	public int hostPort = 4544;
	public IPEndPoint hostEndPoint;
	//string prefetch_fn;
	byte[] prefetch_fn;
	int fid = 0; //initial frameid
	int fid_max = 25000;

	void Start(){
		serverIp = IPAddress.Parse(hostIp);
		hostEndPoint = new IPEndPoint(serverIp,hostPort);

		client = new UdpClient();
		client.Connect(hostEndPoint);
		client.Client.Blocking = false;

		prefetch_fn = new byte[20];
	}

	//public void SendDgram(string evento,string msg)
	public void SendDgram(int fid){
		int _fid = fid;
		/*
    	for (int i = 9; i >= 0; i--) 
    	{
			string mys =  (_fid % 10).ToString();
			//prefetch_fn = prefetch_fn + mys;
			prefetch_fn = prefetch_fn + "0" + mys;
        	_fid /= 10;
    	}
    	*/

		for (int i = 9; i >= 0; i--) {
			prefetch_fn [i] = Convert.ToByte((char)(_fid % 10 + 48));
			_fid /= 10;
		}
		prefetch_fn [10] = 0;
		//Debug.Log (prefetch_fn);
		//byte[] dgram = Encoding.UTF8.GetBytes(prefetch_fn);
		//client.Send(dgram,dgram.Length);
		//client.BeginReceive(new AsyncCallback(processDgram),client);
		client.Send(prefetch_fn, 10);
	}

	public void processDgram(IAsyncResult res){
		try {
			byte[] recieved = client.EndReceive(res,ref hostEndPoint);
			Debug.Log(Encoding.UTF8.GetString(recieved));
		} catch (Exception ex) {
			throw ex;
		}
	}

	/*
	void OnGUI()
	{
		if(GUI.Button (new Rect (10,10,100,40), "Send"))
		{
			DynamicObject d = new DynamicObject();
			SendDgram("JSON",JsonUtility.ToJson(d).ToString());
		}
	}
	*/
	void Update()
	{
		SendDgram (fid);
		if (fid < fid_max)
			fid = fid + 1;
		else
			fid = 0;
	}

}
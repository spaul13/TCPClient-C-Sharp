using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallPrefetch : MonoBehaviour {

	public TCPTestClient ttc;
    public int fid = 7;
    int count = 0;

	// Use this for initialization
	void Start () {

		ttc = GameObject.Find ("udpclient").GetComponent<TCPTestClient> ();
        ttc.ConnectToTcpServer();


    }
	
	// Update is called once per frame
	void Update () {
        //StartCoroutine("Send");
        if (Input.GetMouseButtonDown(0))
        {
            Send();
            StartCoroutine(ttc.ListenForData(fid));
            count++;
        }
        /*if (fid < 200) fid++;
        else fid = 0;
        ttc.Sendfid(fid);*/
    }

    //IEnumerator Send()
    void Send()
    {
        //else fid = 0;
        fid++;
        ttc.Sendfid(fid);
        //if (fid < 2)
        
        //yield return new WaitForSeconds(2f);
    }
}

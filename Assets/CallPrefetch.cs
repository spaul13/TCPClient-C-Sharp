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
            List<int> fid_list = new List<int>();
            fid_list.Add(fid);
            //fid_list.Add(fid + 161);
            //fid_list.Add(fid + 1);
            //before sending the request for fids we need to check whether those fids already existed or not then pass to network thread
            Send(fid_list);
            StartCoroutine(ttc.ListenForData());
            count++;
        }
        /*if (fid < 200) fid++;
        else fid = 0;
        ttc.Sendfid(fid);*/
    }

    //IEnumerator Send()
    void Send(List<int> fid_list)
    {
        //else fid = 0;
        //fid++;
        ttc.Sendfid(fid_list);
        //if (fid < 2)
        
        //yield return new WaitForSeconds(2f);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LibaryTest : MonoBehaviour {
    public UnityEngine.UI.RawImage rawImage1;
	public UnityEngine.UI.RawImage rawImage2;
	public UnityEngine.UI.RawImage rawImage3;
	private void Awake()
	{
		if (ResLibaryMgr.Instance == null)
		{
			
		}
	}

	// Use this for initialization
	void Start () {
        //rawImage.texture = Resources.Load<Texture2D>("1");
		//rawImage.texture = ResLibaryMgr.Instance.GetTexture2d("beautifulgirl_01");
		//rawImage.texture = (Texture2D)ResLibaryMgr.Instance.getLibaryObj("beautifulgirl_01");
		rawImage1.texture = ResLibaryMgr.Instance.GetTexture2d("beautifulgirl");
		rawImage2.texture = ResLibaryMgr.Instance.GetTexture2d("beautifulgirl_01");
		rawImage3.texture = ResLibaryMgr.Instance.GetTexture2d("1");
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}

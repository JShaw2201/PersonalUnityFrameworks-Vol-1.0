using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LibaryTest : MonoBehaviour
{
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
    IEnumerator Start()
    {
        LibaryAssetSetting assetSet = Resources.Load<LibaryAssetSetting>("AssetLibarySetting");
        LibaryResourceSetting resourceSet = Resources.Load<LibaryResourceSetting>("ResourceLibarySetting");
        LibaryStreamingAssetSetting streamingAssetsSet = Resources.Load<LibaryStreamingAssetSetting>("StreamingAssetLibarySetting");

        ResLibaryMgr.Instance.InsertLibrary(assetSet);
        ResLibaryMgr.Instance.InsertLibrary(resourceSet);
        ResLibaryMgr.Instance.InsertLibrary(streamingAssetsSet);
        yield return new WaitForSeconds(2);
        //rawImage.texture = Resources.Load<Texture2D>("1");
        //rawImage.texture = ResLibaryMgr.Instance.GetTexture2d("beautifulgirl_01");
        //rawImage.texture = (Texture2D)ResLibaryMgr.Instance.getLibaryObj("beautifulgirl_01");
        rawImage1.texture = ResLibaryMgr.Instance.GetTexture2d("beautifulgirl");
        rawImage2.texture = ResLibaryMgr.Instance.GetTexture2d("beautifulgirl_01");
        rawImage3.texture = ResLibaryMgr.Instance.GetTexture2d("1");
    }

    // Update is called once per frame
    void Update()
    {

    }
}

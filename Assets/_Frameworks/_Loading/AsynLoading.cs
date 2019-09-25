using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsynLoading : MonoBehaviour {

    
    private AsyncOperation op;
    private static string LoadingName;
    private float prog;

    
    // Use this for initialization
    void Start()
    {
        // LoadScene();
        Resources.UnloadUnusedAssets();    
        StartCoroutine(StartLoading(LoadingName));
    }


    public static void LoadScene(string name)
    {
        LoadingName = name;
        //LoadingManager.instance.LoadSence();
        LoadingManager.Instance.StartLoadSence();
        Application.LoadLevel("Loading");

    }

    private IEnumerator StartLoading(string scene)
    {

        op = Application.LoadLevelAsync(scene);
        op.allowSceneActivation = false;
        while (!op.isDone)
        {
            // LoadingManager.instance.m_UISprite.fillAmount = (float)displayProgress / 100f;
            LoadingManager.Instance.SetLoadSceneProgress(op.progress);
            //yield return new WaitForEndOfFrame();
            yield return null;
        }
        op.allowSceneActivation = true;
    }
}

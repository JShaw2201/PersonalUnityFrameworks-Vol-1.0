using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingManager : MonoBehaviour
{
    public static LoadingManager Instance { get; private set; }
    public float LoadProgress
    {
        get
        {
            return vo.AllLoadProgress;
        }
    }

    private LoadVo vo;
    private void Awake()
    {
        OnInit();
    }
    public void OnInit()
    {
        if (Instance != null && Instance != this)
        {
            MonoBehaviour[] monos = gameObject.GetComponents<MonoBehaviour>();
            if (monos.Length > 1)
            {
                Destroy(this);
            }
            else
            {
                Destroy(gameObject);
            }
            return;
        }
        Instance = this;
        vo = new LoadVo();
    }

    public void StartLoadSence()
    {
        vo.isRunning = true;
        vo.Dispose();
        EventListener.dispatchEvent(LoadingEvents.START_LOADING_SCENE, null);
    }

    public void CloseLoadSence()
    {
        EventListener.dispatchEvent(LoadingEvents.START_LOADING_SCENE, null);
        vo.Dispose();
        
    }

    public void SetInitSceneProgress(float progress)
    {
        vo.InitProgress = progress;
        onLoadComplete();

    }

    public void SetLoadSceneProgress(float progress)
    {
        vo.LoadProgress = progress;
        onLoadComplete();
    }

    public void SetReleaseSceneProgress(float progress)
    {
        vo.ReleaseProgress = progress;
        onLoadComplete();
    }

    public void SetLoadDataProgress(float progress)
    {
        vo.AllUpdateAsset = progress;
        onLoadComplete();
    }
    private void onLoadComplete()
    {
        if (vo.isRunning && vo.AllLoadProgress == 1)
        {
            vo.isRunning = false;
            EventListener.dispatchEvent(LoadingEvents.LOADING_SCENE_PROGRESS_COMPLETE_CALLBACK, null);
        }
    }
}

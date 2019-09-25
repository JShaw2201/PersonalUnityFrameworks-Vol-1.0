using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LoadVo  {

    public bool isRunning = false;

    public float AllLoadProgress
    {
        get
        {
            return _loadProgress * 0.25f + _initProgress * 0.25f + _allUpdateAsset * 0.25f + _releaseProgress * 0.25f;
        }
    }

    private float _loadProgress;

    public float LoadProgress
    {

        set
        {
            _loadProgress = value;
        }
    }

    private float _initProgress;

    public float InitProgress
    {

        set
        {
            _initProgress = value;
        }
    }

    private float _allUpdateAsset;

    public float AllUpdateAsset
    {

        set
        {
            _allUpdateAsset = value;
        }
    }

    private float _releaseProgress;
    public float ReleaseProgress
    {
        set
        {
            _releaseProgress = value;
        }
    }

    

    public LoadVo()
    {
        _loadProgress = 0;
        _initProgress = 0;
        _allUpdateAsset = 0;
        _releaseProgress = 0;
    }

    public void Dispose()
    {
        _loadProgress = 0;
        _initProgress = 0;
        _allUpdateAsset = 0;
        _releaseProgress = 0;
    }
}


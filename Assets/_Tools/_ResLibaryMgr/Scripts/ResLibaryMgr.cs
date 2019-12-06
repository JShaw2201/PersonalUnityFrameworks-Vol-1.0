using ResLibary;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using Object = UnityEngine.Object;

/// <summary>
/// 动态资源信息管理，resource,assetbundle,streamingAssetsPath，外部单个
/// </summary>
public class ResLibaryMgr :MonoBehaviour, ILibaryHandle
{
    private static volatile ResLibaryMgr instance;
    private static object syncRoot = new object();
    private static bool _applicationIsQuitting = false;
    private static GameObject singletonObj = null;
    public static ResLibaryMgr Instance
    {
        get
        {
            if (_applicationIsQuitting)
            {
                return null;
            }
            if (instance == null)
            {

                lock (syncRoot)
                {
                    if (instance == null)
                    {
                        ResLibaryMgr[] instance1 = FindObjectsOfType<ResLibaryMgr>();
                        if (instance1 != null)
                        {
                            for (var i = 0; i < instance1.Length; i++)
                            {
                                Destroy(instance1[i].gameObject);
                            }
                        }
                    }
                }

                GameObject go = new GameObject(typeof(ResLibaryMgr).FullName);
                singletonObj = go;
                instance = go.AddComponent<ResLibaryMgr>();
                instance.OnInit();
                DontDestroyOnLoad(go);
                _applicationIsQuitting = false;
            }
            return instance;
        }

    }

    private void Awake()
    {

        ResLibaryMgr t = gameObject.GetComponent<ResLibaryMgr>();
        if (singletonObj == null)
        {
            singletonObj = gameObject;
            DontDestroyOnLoad(gameObject);
            singletonObj.name = typeof(ResLibaryMgr).FullName;
            instance = t;
            OnInit();
            _applicationIsQuitting = false;
        }
        else if (singletonObj != gameObject)
        {
            MonoBehaviour[] monos = gameObject.GetComponents<MonoBehaviour>();
            if (monos.Length > 1)
            {
                Destroy(t);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }

    private ILibaryHandle streamming;
    private ILibaryHandle fileLibary;
    private ILibaryHandle assetsLibary;
    private ILibaryHandle resourceLibary;
    private ILibaryHandle bundleLibary;
    private ResLibaryFactory Factory;
    private Dictionary<string, Dictionary<string, LibaryStateObj>> libaryDict;
    private void OnInit()
    {
        Factory = new ResLibaryFactory();
        libaryDict = new Dictionary<string, Dictionary<string, LibaryStateObj>>();
        fileLibary = Factory.CreateHandle("FileLibary", UpdateLibary); //FileLibary.Instance;
        streamming = Factory.CreateHandle("StreamingAssetLibary", UpdateLibary); //new StreamingAssetLibary();
        assetsLibary = Factory.CreateHandle("AssetsLibary", UpdateLibary); //AssetsLibary.Instance;
        resourceLibary = Factory.CreateHandle("ResourceLibary", UpdateLibary); //new ResourceLibary();
        bundleLibary = Factory.CreateHandle("AssetBundleLibary", UpdateLibary); //AssetBundleLibary.Instance;
    }

    public void InsertLibrary(object data)
    {
        if (data == null)
            return;
        streamming.InsertLibrary(data);
        fileLibary.InsertLibrary(data);
        bundleLibary.InsertLibrary(data);
        assetsLibary.InsertLibrary(data);
        resourceLibary.InsertLibrary(data);
    }

    public void DeleteLiibrary(string _type, string name)
    {
        if (libaryDict.ContainsKey(name))
        {
            Dictionary<string, LibaryStateObj> lDict = libaryDict[name];
            lDict.Remove(_type);
        }
      

        streamming.DeleteLiibrary(_type, name);
        fileLibary.DeleteLiibrary(_type, name);
        bundleLibary.DeleteLiibrary(_type, name);
        assetsLibary.DeleteLiibrary(_type, name);
        resourceLibary.DeleteLiibrary(_type, name);
    }


    public void UpdateLibary(LibaryStateObj libaryStateObj)
    {
        if (!libaryDict.ContainsKey(libaryStateObj.m_Name))
        {
            libaryDict[libaryStateObj.m_Name] = new Dictionary<string, LibaryStateObj>();
        }
        Dictionary<string, LibaryStateObj> lDict = libaryDict[libaryStateObj.m_Name];
        if (lDict.ContainsKey(libaryStateObj.m_Name))
        {
            LibaryStateObj lobj = lDict[libaryStateObj.m_Type];
            switch (lobj.m_Status)
            {
                case LibaryStatusEnum.DIR_ASSETS:
                    assetsLibary.DeleteLiibrary(libaryStateObj.m_Type,libaryStateObj.m_Name);
                    break;
                case LibaryStatusEnum.DIR_ASSETBUNDER:
                    bundleLibary.DeleteLiibrary(libaryStateObj.m_Type, libaryStateObj.m_Name);
                    break;
                case LibaryStatusEnum.DIR_STREAMINGASSET:
                    streamming.DeleteLiibrary(libaryStateObj.m_Type, libaryStateObj.m_Name);
                    break;
                case LibaryStatusEnum.DIR_FILE:
                    fileLibary.DeleteLiibrary(libaryStateObj.m_Type, libaryStateObj.m_Name);
                    break;
                case LibaryStatusEnum.DIR_RESOURCE:
                    resourceLibary.DeleteLiibrary(libaryStateObj.m_Type, libaryStateObj.m_Name);
                    break;
            }
        }
        lDict[libaryStateObj.m_Type] = libaryStateObj;

        libaryDict[libaryStateObj.m_Name] = lDict;
    }

    public Sprite GetSprite(string objName)
    {
        return GetObject<Sprite>(objName);
    }

    public Material GetMatiral(string objName)
    {
        return GetObject<Material>(objName);
    }

    public AudioClip GetAudioClip(string objName)
    {
        return GetObject<AudioClip>(objName);
    }

    public VideoClip GetVideoClip(string objName)
    {
        return GetObject<VideoClip>(objName);
    }

    public MovieTexture GetMovieTexture(string objName)
    {
        return GetObject<MovieTexture>(objName);
    }

    public string GetTextAsset(string objName)
    {
        if (libaryDict.ContainsKey(objName))
        {
            LibaryStateObj lobj;
            Dictionary<string, LibaryStateObj> lDict = libaryDict[objName];
            if (lDict.TryGetValue(ResLibaryConfig.ExistType[LibaryTypeEnum.LibaryType_TextAsset], out lobj))
            {
                switch (lobj.m_Status)
                {
                    case LibaryStatusEnum.DIR_ASSETS:
                        return assetsLibary.GetTextAsset(objName);
                    case LibaryStatusEnum.DIR_ASSETBUNDER:
                        return bundleLibary.GetTextAsset(objName);
                    case LibaryStatusEnum.DIR_STREAMINGASSET:
                        return streamming.GetTextAsset(objName);
                    case LibaryStatusEnum.DIR_FILE:
                        return fileLibary.GetTextAsset(objName);
                    case LibaryStatusEnum.DIR_RESOURCE:
                        return resourceLibary.GetTextAsset(objName);
                }
            }
        }
        return null;
    }

    public Texture2D GetTexture2d(string objName)
    {
        return (Texture2D)GetUnityObject(ResLibaryConfig.ExistType[LibaryTypeEnum.LibaryType_Texture2D], objName);
    }

    public T GetObject<T>(string objName) where T : UnityEngine.Object
    {
        string _type = typeof(T).Name;
        return (T)GetUnityObject(_type, objName);
    }

    public Object GetUnityObject(string _type, string objName)
    {
        if (libaryDict.ContainsKey(objName))
        {
            LibaryStateObj lobj;
            Dictionary<string, LibaryStateObj> lDict = libaryDict[objName];
            if (lDict.TryGetValue(_type, out lobj))
            {
                switch (lobj.m_Status)
                {
                    case LibaryStatusEnum.DIR_ASSETS:
                        return assetsLibary.GetUnityObject(_type, objName);
                    case LibaryStatusEnum.DIR_ASSETBUNDER:
                        return bundleLibary.GetUnityObject(_type, objName);
                    case LibaryStatusEnum.DIR_STREAMINGASSET:
                        return streamming.GetUnityObject(_type, objName);
                    case LibaryStatusEnum.DIR_FILE:
                        return fileLibary.GetUnityObject(_type, objName);
                    case LibaryStatusEnum.DIR_RESOURCE:
                        return resourceLibary.GetUnityObject(_type, objName);
                }
            }
        }
        return null;
    }

    public LibaryExistStatusEnum TryGetObject(LibaryTypeEnum libaryTypeEnum, string objName, out object data)
    {
        data = null;
        if (libaryDict.ContainsKey(objName))
        {
            LibaryStateObj lobj;
            Dictionary<string, LibaryStateObj> lDict = libaryDict[objName];
            if (lDict.TryGetValue(ResLibaryConfig.ExistType[libaryTypeEnum], out lobj))
            {
                switch (lobj.m_Status)
                {
                    case LibaryStatusEnum.DIR_ASSETS:
                        return assetsLibary.TryGetObject(libaryTypeEnum, objName, out data);
                    case LibaryStatusEnum.DIR_ASSETBUNDER:
                        return bundleLibary.TryGetObject(libaryTypeEnum, objName, out data);
                    case LibaryStatusEnum.DIR_STREAMINGASSET:
                        return streamming.TryGetObject(libaryTypeEnum, objName, out data);
                    case LibaryStatusEnum.DIR_FILE:
                        return fileLibary.TryGetObject(libaryTypeEnum, objName, out data);
                    case LibaryStatusEnum.DIR_RESOURCE:
                        return resourceLibary.TryGetObject(libaryTypeEnum, objName, out data);
                }
            }
        }
        return LibaryExistStatusEnum.NONE;
    }

    public LibaryStatusEnum TryGetAssetPath<T>(string objName, out string outPath) where T : UnityEngine.Object
    {
        string _type = typeof(T).Name;
        return TryGetAssetPath(_type, objName,out outPath);
    }

    public LibaryStatusEnum TryGetAssetPath(string _type,string objName, out string outPath) 
    {
        if (libaryDict.ContainsKey(objName))
        {
            LibaryStateObj lobj;
            Dictionary<string, LibaryStateObj> lDict = libaryDict[objName];
            if (lDict.TryGetValue(ResLibaryConfig.ExistType[LibaryTypeEnum.LibaryType_TextAsset], out lobj))
            {
                outPath = lobj.m_Path;
                return lobj.m_Status;
            }
        }
        outPath = null;
        return LibaryStatusEnum.DIR_ASSETS;
    }

    public void releaseObj(string _type, string objName)
    {
        if (libaryDict.ContainsKey(objName))
        {
            LibaryStateObj lobj;
            Dictionary<string, LibaryStateObj> lDict = libaryDict[objName];
            if (lDict.TryGetValue(ResLibaryConfig.ExistType[LibaryTypeEnum.LibaryType_TextAsset], out lobj))
            {
                switch (lobj.m_Status)
                {
                    case LibaryStatusEnum.DIR_ASSETS:
                        assetsLibary.releaseObj(_type, objName);
                        break;
                    case LibaryStatusEnum.DIR_ASSETBUNDER:
                        bundleLibary.releaseObj(_type, objName);
                        break;

                    case LibaryStatusEnum.DIR_STREAMINGASSET:
                        streamming.releaseObj(_type, objName);
                        break;
                    case LibaryStatusEnum.DIR_FILE:
                        fileLibary.releaseObj(_type, objName);
                        break;
                    case LibaryStatusEnum.DIR_RESOURCE:
                        resourceLibary.releaseObj(_type, objName);
                        break;
                }
            }
        }
    }

    public void releaseObj(LibaryTypeEnum libaryTypeEnum, string objName)
    {
        releaseObj(ResLibaryConfig.ExistType[libaryTypeEnum], objName);
    }

    public void releaseAll()
    {
        streamming.releaseAll();
        fileLibary.releaseAll();
        bundleLibary.releaseAll();
        assetsLibary.releaseAll();
        resourceLibary.releaseAll();
    }

    public void releaseObj(Object obj)
    {
        releaseObj(obj.GetType().Name, obj.name);
    }


    public object getLibaryObj(string objName)
    {
        object data = null;
        LibaryExistStatusEnum libaryExistStatusEnum = LibaryExistStatusEnum.NONE;
        if (libaryDict.ContainsKey(objName))
        {
            foreach (var item in libaryDict[objName].Keys)
            {
                if (item == ResLibaryConfig.ExistType[LibaryTypeEnum.LibaryType_Sprite])
                {
                    continue;
                }
                if (item == ResLibaryConfig.ExistType[LibaryTypeEnum.LibaryType_TextAsset])
                {
                    data = GetTextAsset(objName);
                    if (data != null)
                        break;
                }
                if (item == ResLibaryConfig.ExistType[LibaryTypeEnum.LibaryType_Texture2D])
                {
                    data = GetTexture2d(objName);
                    if (data != null)
                        break;
                }
                libaryExistStatusEnum = TryGetObject(ResLibaryConfig.ExistTypeNameToEnum[item] ,objName, out data);
                if (libaryExistStatusEnum != LibaryExistStatusEnum.NONE)
                    break;
            }
        }
        return data;
    }

    public void releaseScene()
    {
        streamming.releaseScene();
        fileLibary.releaseScene();
        bundleLibary.releaseScene();
        assetsLibary.releaseScene();
        resourceLibary.releaseScene();
    }
}

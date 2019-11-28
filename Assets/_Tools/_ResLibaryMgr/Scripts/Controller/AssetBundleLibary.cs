using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

namespace ResLibary
{
    public class AssetBundleStateObj
    {
        /// <summary>
        /// 引用次数
        /// </summary>
        public int bundleQuote;

        public string bundlePath;

        public AssetExistStatusEnum bundelExistStatus = AssetExistStatusEnum.Once;

        public AssetBundle bundleAsset;
    }

    public class AssetBundleLibary : MonoBehaviour, ILibaryHandle
    {
        private static volatile AssetBundleLibary instance;
        private static object syncRoot = new object();
        private static bool _applicationIsQuitting = false;
        private static GameObject singletonObj = null;
        public static AssetBundleLibary Instance
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
                            AssetBundleLibary[] instance1 = FindObjectsOfType<AssetBundleLibary>();
                            if (instance1 != null)
                            {
                                for (var i = 0; i < instance1.Length; i++)
                                {
                                    Destroy(instance1[i].gameObject);
                                }
                            }
                        }
                    }

                    GameObject go = new GameObject(typeof(AssetBundleLibary).FullName);
                    singletonObj = go;
                    instance = go.AddComponent<AssetBundleLibary>();
                    instance.OnInit();
                    DontDestroyOnLoad(go);
                    _applicationIsQuitting = false;
                }
                return instance;
            }

        }

        private void Awake()
        {

            AssetBundleLibary t = gameObject.GetComponent<AssetBundleLibary>();
            if (singletonObj == null)
            {
                singletonObj = gameObject;
                DontDestroyOnLoad(gameObject);
                singletonObj.name = typeof(AssetBundleLibary).FullName;
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

        public System.Action<LibaryStateObj> UpdateAssetCallback;
        private Dictionary<string, Dictionary<string, AssetBundleStateObj>> resourceDict;

        private  void OnInit()
        {
            resourceDict = new Dictionary<string, Dictionary<string, AssetBundleStateObj>>();
        }

        public void UpdateAssetBundle(string bundleUrl, AssetExistStatusEnum bundelExistStatus = AssetExistStatusEnum.Scene)
        {
            ResLibaryTool.UTStartCoroutine(AddAssetBundle(bundleUrl, bundelExistStatus));
        }

        private IEnumerator AddAssetBundle(string bundleUrl, AssetExistStatusEnum bundelExistStatus)
        {
            WWW www = new WWW(@"file://" + bundleUrl);
            while (!www.isDone)
            {
                yield return new WaitForFixedUpdate();
            }
            if (string.IsNullOrEmpty(www.error) && www.assetBundle != null)
            {
                AssetBundle bundle = www.assetBundle;
                AssetBundleStateObj assetStateObj = new AssetBundleStateObj();
                assetStateObj.bundelExistStatus = bundelExistStatus;
                assetStateObj.bundlePath = bundleUrl;

                UnityEngine.Object[] data = bundle.LoadAllAssets();
                for (int i = 0; i < data.Length; i++)
                {
                    LibaryStateObj libaryObj = new LibaryStateObj();
                    libaryObj.m_Name = data[i].name;
                    libaryObj.m_Status = LibaryStatusEnum.DIR_ASSETBUNDER;
                    libaryObj.m_Type = data[i].GetType().Name;
                    if (UpdateAssetCallback != null)
                        UpdateAssetCallback(libaryObj);
                    //ResLibaryMgr.Instance.UpdateLibary(libaryObj);
                }
                if (bundelExistStatus == AssetExistStatusEnum.Globle)
                    assetStateObj.bundleAsset = bundle;
                else
                {
                    bundle.Unload(false);
                    Resources.UnloadUnusedAssets();
                }
            }

            www.Dispose();
            yield return 1;
        }

        string ILibaryHandle.GetTextAsset(string objName)
        {
            TextAsset textAsset = ((ILibaryHandle)this).GetObject<TextAsset>(objName);
            if (textAsset != null)
                return textAsset.text;
            return null;
        }

        GameObject GetGameObject(string objName)
        {
            return ((ILibaryHandle)this).GetObject<GameObject>(objName);
        }

        Texture2D ILibaryHandle.GetTexture2d(string objName)
        {
            return ((ILibaryHandle)this).GetObject<Texture2D>(objName);
        }

        RenderTexture GetRenderTexture(string objName)
        {
            return ((ILibaryHandle)this).GetObject<RenderTexture>(objName);
        }

        Sprite ILibaryHandle.GetSprite(string objName)
        {
            return ((ILibaryHandle)this).GetObject<Sprite>(objName);
        }

        Material GetMatiral(string objName)
        {
            return ((ILibaryHandle)this).GetObject<Material>(objName);
        }

        AudioClip GetAudioClip(string objName)
        {
            return ((ILibaryHandle)this).GetObject<AudioClip>(objName);
        }

        VideoClip GetVideoClip(string objName)
        {
            return ((ILibaryHandle)this).GetObject<VideoClip>(objName);
        }

        MovieTexture GetMovieTexture(string objName)
        {
            return ((ILibaryHandle)this).GetObject<MovieTexture>(objName);
        }

        T ILibaryHandle.GetObject<T>(string objName)
        {
            string _type = typeof(T).Name;
            return (T)((ILibaryHandle)this).GetUnityObject(_type, objName);
        }

        UnityEngine.Object ILibaryHandle.GetUnityObject(string _type, string objName)
        {
            if (resourceDict.ContainsKey(_type))
            {
                Dictionary<string, AssetBundleStateObj> _Dict = resourceDict[_type];
                AssetBundleStateObj stateObj;
                _Dict.TryGetValue(objName, out stateObj);
                if (stateObj != null)
                {
                    AssetBundle bundle = null;
                    bundle = stateObj.bundleAsset;
                    if (stateObj.bundleAsset == null)
                    {
                        stateObj.bundleAsset = AssetBundle.LoadFromFile(stateObj.bundlePath);
                        bundle = stateObj.bundleAsset;
                    }
                    UnityEngine.Object t = null;
                    if (bundle != null && ResLibaryConfig.ExistTypeNameToType.ContainsKey(_type))
                    {
                        t = bundle.LoadAsset(objName, ResLibaryConfig.ExistTypeNameToType[_type]);
                    }
                    if (t != null)
                        stateObj.bundleQuote++;
                    if (stateObj.bundelExistStatus == AssetExistStatusEnum.Once)
                    {
                        bundle.Unload(false);
                        stateObj.bundleAsset = null;
                    }

                    return t;
                }
            }
            return null;
        }

        LibaryExistStatusEnum ILibaryHandle.TryGetObject(LibaryTypeEnum libaryTypeEnum, string objName, out object data)
        {
            data = ((ILibaryHandle)this).GetUnityObject(ResLibaryConfig.ExistType[libaryTypeEnum], objName);
            LibaryExistStatusEnum libaryExistStatusEnum = LibaryExistStatusEnum.LibaryExistStatus_UnityEngineObject;
            return libaryExistStatusEnum;
        }

        void ILibaryHandle.InsertLibrary(object data) { }

        void ILibaryHandle.DeleteLiibrary(string _type,string name)
        {
            if (resourceDict.ContainsKey(_type))
            {
                Dictionary<string, AssetBundleStateObj> objectDict = resourceDict[_type];
                if (objectDict.ContainsKey(name))
                {
                    objectDict.Remove(name);
                }
            }
        }

        void ILibaryHandle.releaseObj(LibaryTypeEnum libaryTypeEnum, string objName)
        {
            string _type = ResLibaryConfig.ExistType[libaryTypeEnum];
            ((ILibaryHandle)this).releaseObj(_type, objName);
        }

        void ILibaryHandle.releaseObj(string _type, string objName)
        {
            if (resourceDict.ContainsKey(_type))
            {
                Dictionary<string, AssetBundleStateObj> _Dict = resourceDict[_type];
                AssetBundleStateObj stateObj;
                _Dict.TryGetValue(objName, out stateObj);
                if (stateObj != null)
                {
                    stateObj.bundleQuote = Mathf.Max(0, stateObj.bundleQuote - 1);
                    if (stateObj.bundleQuote == 0 && stateObj.bundelExistStatus == AssetExistStatusEnum.Quote)
                    {
                        if (stateObj.bundleAsset != null)
                        {
                            stateObj.bundleAsset.Unload(true);
                        }
                        stateObj.bundleAsset = null;
                    }
                }
            }
        }

        void ILibaryHandle.releaseObj(UnityEngine.Object obj)
        {
            string _type = obj.GetType().Name;
            ((ILibaryHandle)this).releaseObj(_type, obj.name);
        }

        void ILibaryHandle.releaseAll()
        {
            foreach (var dict in resourceDict.Values)
            {
                List<AssetBundleStateObj> list = new List<AssetBundleStateObj>(dict.Values);
                HashSet<AssetBundleStateObj> array = new HashSet<AssetBundleStateObj>(list);
                foreach (var stateObj in array)
                {
                    if (stateObj != null && stateObj.bundleAsset != null)
                    {
                        if (stateObj.bundleAsset != null)
                        {
                            stateObj.bundleAsset.Unload(true);
                        }
                        stateObj.bundleAsset = null;
                    }
                }
            }
        }

        void ILibaryHandle.releaseScene()
        {
            foreach (var dict in resourceDict.Values)
            {
                List<AssetBundleStateObj> list = new List<AssetBundleStateObj>(dict.Values);
                HashSet<AssetBundleStateObj> array = new HashSet<AssetBundleStateObj>(list);
                foreach (var stateObj in array)
                {
                    if (stateObj != null && stateObj.bundleAsset != null)
                    {
                        if (stateObj.bundleAsset != null && stateObj.bundelExistStatus == AssetExistStatusEnum.Scene)
                        {
                            stateObj.bundleAsset.Unload(true);
                        }
                        stateObj.bundleAsset = null;
                    }
                }
            }
        }
    }
}

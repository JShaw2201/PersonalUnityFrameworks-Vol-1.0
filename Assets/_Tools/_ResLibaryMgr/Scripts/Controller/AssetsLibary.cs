using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Video;

namespace ResLibary
{
    /// <summary>
    /// Asset下的资源
    /// </summary>
    public class AssetsLibary : MonoBehaviour, ILibaryHandle
    {
        private static volatile AssetsLibary instance;
        private static object syncRoot = new object();
        private static bool _applicationIsQuitting = false;
        private static GameObject singletonObj = null;
        public static AssetsLibary Instance
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
                            AssetsLibary[] instance1 = FindObjectsOfType<AssetsLibary>();
                            if (instance1 != null)
                            {
                                for (var i = 0; i < instance1.Length; i++)
                                {
                                    Destroy(instance1[i].gameObject);
                                }
                            }
                        }
                    }

                    GameObject go = new GameObject(typeof(AssetsLibary).FullName);
                    singletonObj = go;
                    instance = go.AddComponent<AssetsLibary>();
                    instance.OnInit();
                    DontDestroyOnLoad(go);
                    _applicationIsQuitting = false;
                }
                return instance;
            }

        }

        private void Awake()
        {

            AssetsLibary t = gameObject.GetComponent<AssetsLibary>();
            if (singletonObj == null)
            {
                singletonObj = gameObject;
                DontDestroyOnLoad(gameObject);
                singletonObj.name = typeof(AssetsLibary).FullName;
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
        private Dictionary<string, Dictionary<string, UnityEngine.Object>> assetDict;

        private void OnInit()
        {
            assetDict = new Dictionary<string, Dictionary<string, UnityEngine.Object>>();

        }
        private IEnumerator InitLibaryAssetSetting<T>(List<T> list, int iterationsPerFrame = 200) where T : UnityEngine.Object
        {
            int iterations = 0;
            string _type = typeof(T).Name;
            if (!assetDict.ContainsKey(_type))
                assetDict[_type] = new Dictionary<string, UnityEngine.Object>();
            HashSet<T> array = new HashSet<T>(list);
            foreach (var item in array)
            {
                if (item == null)
                    continue;
                Dictionary<string, UnityEngine.Object> dict = assetDict[_type];
                dict[item.name] = item;

                LibaryStateObj libaryObj = new LibaryStateObj();
                libaryObj.m_Name = item.name;
                libaryObj.m_Status = LibaryStatusEnum.DIR_ASSETS;
                libaryObj.m_Type = item.GetType().Name;
                if (UpdateAssetCallback != null)
                    UpdateAssetCallback(libaryObj);
                //ResLibaryMgr.Instance.UpdateLibary(libaryObj);
                if (iterationsPerFrame > 0 && ++iterations % iterationsPerFrame == 0)
                    yield return 1;
            }
        }

        void ILibaryHandle.InsertLibrary(object data)
        {
            if (data.GetType().Name != "LibaryAssetSetting")
                return;
            LibaryAssetSetting assetSetting = (LibaryAssetSetting)data;
            ResLibaryTool.UTStartCoroutine(InitLibaryAssetSetting(assetSetting.texture2ds));
            ResLibaryTool.UTStartCoroutine(InitLibaryAssetSetting(assetSetting.renderTextures));
            ResLibaryTool.UTStartCoroutine(InitLibaryAssetSetting(assetSetting.movieTextures));
            ResLibaryTool.UTStartCoroutine(InitLibaryAssetSetting(assetSetting.sprites));
            ResLibaryTool.UTStartCoroutine(InitLibaryAssetSetting(assetSetting.materials));
            ResLibaryTool.UTStartCoroutine(InitLibaryAssetSetting(assetSetting.textAssets));
            ResLibaryTool.UTStartCoroutine(InitLibaryAssetSetting(assetSetting.prefabs));
            ResLibaryTool.UTStartCoroutine(InitLibaryAssetSetting(assetSetting.audios));
            ResLibaryTool.UTStartCoroutine(InitLibaryAssetSetting(assetSetting.videos));
        }

        void ILibaryHandle.DeleteLiibrary(string _type, string name)
        {
            if (assetDict.ContainsKey(_type))
            {
                Dictionary<string, UnityEngine.Object> objectDict = assetDict[_type];
                if (objectDict.ContainsKey(name))
                {
                    objectDict.Remove(name);
                }
            }
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
            return (T)((ILibaryHandle)this).GetUnityObject(typeof(T).Name, objName);
        }

        UnityEngine.Object ILibaryHandle.GetUnityObject(string _type, string objName)
        {
            if (assetDict.ContainsKey(_type))
            {
                Dictionary<string, UnityEngine.Object> objectDict = assetDict[_type];
                if (objectDict.ContainsKey(objName))
                {
                    UnityEngine.Object t = objectDict[objName];
                    return t;
                }
            }
            return null;
        }

        LibaryExistStatusEnum ILibaryHandle.TryGetObject(LibaryTypeEnum libaryTypeEnum, string objName, out object data)
        {
            data = ((ILibaryHandle)this).GetUnityObject(ResLibaryTool.ExistType[libaryTypeEnum], objName);
            LibaryExistStatusEnum libaryExistStatusEnum = LibaryExistStatusEnum.LibaryExistStatus_UnityEngineObject;
            return libaryExistStatusEnum;
        }

        public void releaseObj(string _type, string objName)
        {

        }

        public void releaseObj(string objName)
        {
        }

        public void releaseObj(LibaryTypeEnum libaryTypeEnum, string objName) { }
        void ILibaryHandle.releaseObj(UnityEngine.Object obj) { }
        void ILibaryHandle.releaseAll()
        {
        }
        void ILibaryHandle.releaseScene()
        {

        }

        public void AddLibary(UnityEngine.Object assetObj)
        {
            string typeName = assetObj.GetType().Name;
            if (ResLibaryTool.ExistType.ContainsValue(typeName))
            {
                if (!assetDict.ContainsKey(typeName))
                {
                    assetDict[typeName] = new Dictionary<string, UnityEngine.Object>();
                }
                Dictionary<string, UnityEngine.Object> objectDict = assetDict[typeName];
                if (!objectDict.ContainsKey(assetObj.name))
                {
                    objectDict[assetObj.name] = assetObj;
                }
            }
        }
    }
}
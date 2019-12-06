using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Video;
namespace ResLibary
{
    public class FileLibary : MonoBehaviour, ILibaryHandle
    {
        private static volatile FileLibary instance;
        private static object syncRoot = new object();
        private static bool _applicationIsQuitting = false;
        private static GameObject singletonObj = null;
        public static FileLibary Instance
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
                            FileLibary[] instance1 = FindObjectsOfType<FileLibary>();
                            if (instance1 != null)
                            {
                                for (var i = 0; i < instance1.Length; i++)
                                {
                                    Destroy(instance1[i].gameObject);
                                }
                            }
                        }
                    }

                    GameObject go = new GameObject(typeof(FileLibary).FullName);
                    singletonObj = go;
                    instance = go.AddComponent<FileLibary>();
                    instance.OnInit();
                    DontDestroyOnLoad(go);
                    _applicationIsQuitting = false;
                }
                return instance;
            }

        }

        private void Awake()
        {

            FileLibary t = gameObject.GetComponent<FileLibary>();
            if (singletonObj == null)
            {
                singletonObj = gameObject;
                DontDestroyOnLoad(gameObject);
                singletonObj.name = typeof(FileLibary).FullName;
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
        private Dictionary<string, Dictionary<string, FileStateObj>> resourceDict;

        private void OnInit()
        {
            resourceDict = new Dictionary<string, Dictionary<string, FileStateObj>>();
        }

        void ILibaryHandle.InsertLibrary(object data)
        { }

        void ILibaryHandle.DeleteLiibrary(string _type, string name)
        {
            if (resourceDict.ContainsKey(_type))
            {
                Dictionary<string, FileStateObj> objectDict = resourceDict[_type];
                if (objectDict.ContainsKey(name))
                {
                    objectDict.Remove(name);
                }
            }
        }
        public void UpdateLibary(string localUrl, AssetExistStatusEnum assetStatus = AssetExistStatusEnum.Quote)
        {
            FileInfo file = new FileInfo(localUrl);
            if (file.Exists)
            {
                string existension = file.Extension;
                FileStateObj fileObj = null;
                if (ResLibaryConfig.ResourceTxtExts.Contains(existension))
                {
                    fileObj = new FileStateObj();
                    fileObj.m_Path = localUrl;
                    fileObj.m_Type = ResLibaryConfig.ExistType[LibaryTypeEnum.LibaryType_TextAsset];
                    if (assetStatus == AssetExistStatusEnum.Globle)
                    {
                        fileObj.m_Asset = ResLibaryTool.LoadFileStr(localUrl);
                    }

                }
                else if (ResLibaryConfig.ResourceImgExts.Contains(existension))
                {
                    fileObj = new FileStateObj();
                    fileObj.m_Path = localUrl;
                    fileObj.m_Type = ResLibaryConfig.ExistType[LibaryTypeEnum.LibaryType_Texture2D];
                    FileStateObj sfObj = new FileStateObj();
                    sfObj.m_Path = localUrl;
                    sfObj.m_Type = ResLibaryConfig.ExistType[LibaryTypeEnum.LibaryType_Sprite];
                    if (assetStatus == AssetExistStatusEnum.Globle)
                    {
                        Texture2D tex2d = ResLibaryTool.readLocalTexture2d(localUrl);
                        fileObj.m_Asset = tex2d;
                        sfObj.m_Asset = Sprite.Create(tex2d, new Rect(0, 0, tex2d.width, tex2d.height), Vector2.zero);
                    }
                    string fullFileName = System.IO.Path.GetFileName(localUrl);
                    int position = fullFileName.LastIndexOf(".");
                    string fileName = fullFileName.Substring(0, position - 1);
                    sfObj.m_Name = fileName;
                    if (!resourceDict.ContainsKey(sfObj.m_Type))
                    {
                        resourceDict[sfObj.m_Type] = new Dictionary<string, FileStateObj>();
                    }
                    Dictionary<string, FileStateObj> dict = resourceDict[sfObj.m_Type];
                    dict[sfObj.m_Name] = sfObj;
                    LibaryStateObj libaryObj = new LibaryStateObj();
                    libaryObj.m_Name = sfObj.m_Name;
                    libaryObj.m_Status = LibaryStatusEnum.DIR_FILE;
                    libaryObj.m_Type = sfObj.m_Type;
                    libaryObj.m_Path = localUrl;
                    if (UpdateAssetCallback != null)
                        UpdateAssetCallback(libaryObj);
                    //ResLibaryMgr.Instance.UpdateLibary(libaryObj);
                }
                else if (ResLibaryConfig.ResourceVideoExts.Contains(existension))
                {
                    fileObj = new FileStateObj();
                    fileObj.m_Path = localUrl;
                    fileObj.m_Type = ResLibaryConfig.ExistType[LibaryTypeEnum.LibaryType_VideoClip];
                    fileObj.m_Asset = localUrl;
                }
                else if (ResLibaryConfig.ResourceAudioExts.Contains(existension))
                {
                    fileObj = new FileStateObj();
                    fileObj.m_Path = localUrl;
                    fileObj.m_Type = ResLibaryConfig.ExistType[LibaryTypeEnum.LibaryType_AudioClip];
                    fileObj.m_Asset = localUrl;
                }
                if (fileObj != null)
                {
                    string fullFileName = System.IO.Path.GetFileName(localUrl);
                    int position = fullFileName.LastIndexOf(".");
                    string fileName = fullFileName.Substring(0, position - 1);
                    fileObj.m_Name = fileName;
                    if (!resourceDict.ContainsKey(fileObj.m_Type))
                    {
                        resourceDict[fileObj.m_Type] = new Dictionary<string, FileStateObj>();
                    }
                    Dictionary<string, FileStateObj> dict = resourceDict[fileObj.m_Type];
                    dict[fileObj.m_Name] = fileObj;
                    LibaryStateObj libaryObj = new LibaryStateObj();
                    libaryObj.m_Name = fileObj.m_Name;
                    libaryObj.m_Status = LibaryStatusEnum.DIR_FILE;
                    libaryObj.m_Type = fileObj.m_Type;
                    libaryObj.m_Path = localUrl;
                    if (UpdateAssetCallback != null)
                        UpdateAssetCallback(libaryObj);
                    //ResLibaryMgr.Instance.UpdateLibary(libaryObj);
                }
            }
        }

        string ILibaryHandle.GetTextAsset(string objName)
        {
            string _type = ResLibaryConfig.ExistType[LibaryTypeEnum.LibaryType_TextAsset];
            FileStateObj stateObj = GetFileStateObj(_type, objName);
            if (stateObj != null)
            {
                string path = stateObj.m_Path;
                string str = null;
                if (stateObj.m_Asset == null)
                {
                    stateObj.m_Asset = ResLibaryTool.LoadFileStr(path);
                };
                str = (string)stateObj.m_Asset;
                if (str != null && stateObj.m_ExistStatus == AssetExistStatusEnum.Quote)
                    stateObj.m_Quote++;
                if (stateObj.m_ExistStatus == AssetExistStatusEnum.Once)
                    stateObj.m_Asset = null;
                return str;
            }
            return null;
        }

        Sprite ILibaryHandle.GetSprite(string objName)
        {
            string _type = ResLibaryConfig.ExistType[LibaryTypeEnum.LibaryType_Texture2D];
            FileStateObj stateObj = GetFileStateObj(_type, objName);
            if (stateObj != null)
            {
                Sprite spr = null;
                if (stateObj.m_Asset == null)
                {
                    Texture2D t = ((ILibaryHandle)this).GetTexture2d(objName);
                    if (t != null)
                        stateObj.m_Asset = Sprite.Create(t, new Rect(0, 0, t.width, t.height), Vector2.zero);
                }
                spr = (Sprite)stateObj.m_Asset;
                if (spr != null && stateObj.m_ExistStatus == AssetExistStatusEnum.Quote)
                    stateObj.m_Quote++;
                if (stateObj.m_ExistStatus == AssetExistStatusEnum.Once)
                    stateObj.m_Asset = null;
                return spr;
            }

            return null;
        }

        Texture2D ILibaryHandle.GetTexture2d(string objName)
        {
            string _type = ResLibaryConfig.ExistType[LibaryTypeEnum.LibaryType_Texture2D];
            FileStateObj stateObj = GetFileStateObj(_type, objName);
            if (stateObj != null)
            {
                if (stateObj.m_Asset == null)
                {
                    stateObj.m_Asset = ResLibaryTool.readLocalTexture2d(stateObj.m_Path);
                    stateObj.m_Quote = 0;
                }
                Texture2D t = (Texture2D)stateObj.m_Asset;
                if (t != null && stateObj.m_ExistStatus == AssetExistStatusEnum.Quote)
                    stateObj.m_Quote++;
                if (stateObj.m_ExistStatus == AssetExistStatusEnum.Once)
                    stateObj.m_Asset = null;
                return t;
            }

            return null;
        }

        T ILibaryHandle.GetObject<T>(string objName)
        {
            string _type = typeof(T).Name;
            return (T)((ILibaryHandle)this).GetUnityObject(_type, objName);
        }

        LibaryExistStatusEnum ILibaryHandle.TryGetObject(LibaryTypeEnum libaryTypeEnum, string objName, out object data)
        {
            data = null;
            LibaryExistStatusEnum libaryExistStatusEnum = LibaryExistStatusEnum.NONE;
            if (!ResLibaryConfig.ExistType.ContainsKey(libaryTypeEnum))
                return libaryExistStatusEnum;
            string _type = ResLibaryConfig.ExistType[libaryTypeEnum];

            switch (libaryTypeEnum)
            {
                case LibaryTypeEnum.LibaryType_Texture2D:
                    libaryExistStatusEnum = LibaryExistStatusEnum.LibaryExistStatus_UnityEngineObject;
                    data = ((ILibaryHandle)this).GetTextAsset(objName);
                    break;
                case LibaryTypeEnum.LibaryType_TextAsset:
                    libaryExistStatusEnum = LibaryExistStatusEnum.LibaryExistStatus_NotUnityEngineObject_TextAsset;
                    data = ((ILibaryHandle)this).GetTexture2d(objName);
                    break;
                case LibaryTypeEnum.LibaryType_AudioClip:
                    libaryExistStatusEnum = LibaryExistStatusEnum.LibaryExistStatus_NotUnityEngineObject_NotRead_FilePath;
                    FileStateObj fobjAudio = GetFileStateObj(_type, objName);
                    if (fobjAudio != null)
                    {
                        data = fobjAudio.m_Path;
                    }
                    break;
                case LibaryTypeEnum.LibaryType_VideoClip:
                case LibaryTypeEnum.LibaryType_MovieTexture:
                    libaryExistStatusEnum = LibaryExistStatusEnum.LibaryExistStatus_NotUnityEngineObject_NotRead_FilePath;
                    FileStateObj fobjVideo = GetFileStateObj(_type, objName);
                    if (fobjVideo != null)
                    {
                        data = fobjVideo.m_Path;
                    }
                    break;
            }
            return libaryExistStatusEnum;
        }

        UnityEngine.Object ILibaryHandle.GetUnityObject(string _type, string objName)
        {
            if (resourceDict.ContainsKey(_type))
            {
                if (_type == "Texture2D")
                {
                    return ((UnityEngine.Object)((ILibaryHandle)this).GetTexture2d(objName));
                }
            }
            return null;
        }

        FileStateObj GetFileStateObj(string _type, string objName)
        {
            if (resourceDict.ContainsKey(_type))
            {
                Dictionary<string, FileStateObj> dict = resourceDict[_type];
                if (dict.ContainsKey(objName))
                {
                    return dict[objName];
                }
            }
            return null;
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
                Dictionary<string, FileStateObj> _Dict = resourceDict[_type];
                FileStateObj stateObj;
                _Dict.TryGetValue(objName, out stateObj);
                if (stateObj != null && stateObj.m_ExistStatus == AssetExistStatusEnum.Quote)
                {
                    stateObj.m_Quote = Mathf.Max(0, stateObj.m_Quote - 1);
                    if (stateObj.m_Quote == 0)
                        stateObj.m_Asset = null;
                }
            }
        }

        void ILibaryHandle.releaseObj(UnityEngine.Object obj)
        {
            string _type = obj.GetType().Name;
            ((ILibaryHandle)this).releaseObj(_type, obj.name);
        }

        void ILibaryHandle.releaseScene()
        {
            foreach (var dict in resourceDict.Values)
            {
                List<FileStateObj> list = new List<FileStateObj>(dict.Values);
                HashSet<FileStateObj> array = new HashSet<FileStateObj>(list);
                foreach (var stateObj in array)
                {
                    if (stateObj != null && stateObj.m_Asset != null)
                    {
                        if (stateObj.m_Asset != null && stateObj.m_ExistStatus == AssetExistStatusEnum.Scene)
                        {
                            stateObj.m_Asset = null;
                        }

                    }
                }
            }
            Resources.UnloadUnusedAssets();
        }

        void ILibaryHandle.releaseAll()
        {
            resourceDict.Clear();
            Resources.UnloadUnusedAssets();
        }
    }
}

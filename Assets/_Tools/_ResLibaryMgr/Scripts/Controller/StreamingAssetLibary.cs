using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
namespace ResLibary
{
    public class StreamingAssetLibary : ILibaryHandle
    {
        private System.Action<LibaryStateObj> UpdateAssetCallback;
        private Dictionary<string, Dictionary<string, FileStateObj>> resourceDict;

        public StreamingAssetLibary(System.Action<LibaryStateObj> UpdateAssetCallback)
        {
            this.UpdateAssetCallback = UpdateAssetCallback;
            resourceDict = new Dictionary<string, Dictionary<string, FileStateObj>>();

        }
        void ILibaryHandle.InsertLibrary(object data)
        {
            if (data.GetType().Name != "LibaryStreamingAssetSetting")
                return;
            LibaryStreamingAssetSetting assetSetting = (LibaryStreamingAssetSetting)data;
            ResLibaryTool.UTStartCoroutine(InitLibaryAssetSetting(assetSetting.texture2ds));
            ResLibaryTool.UTStartCoroutine(InitLibaryAssetSetting(assetSetting.textAssets));
            ResLibaryTool.UTStartCoroutine(InitLibaryAssetSetting(assetSetting.audios));
            ResLibaryTool.UTStartCoroutine(InitLibaryAssetSetting(assetSetting.videos));
        }

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
        private IEnumerator InitLibaryAssetSetting(List<ResourceSettingStateObj> list, int iterationsPerFrame = 200)
        {
            int iterations = 0;
            HashSet<ResourceSettingStateObj> array = new HashSet<ResourceSettingStateObj>(list);
            foreach (var item in array)
            {
                string _type = item.m_Type;
                if (!resourceDict.ContainsKey(_type))
                    resourceDict[_type] = new Dictionary<string, FileStateObj>();
                Dictionary<string, FileStateObj> dict = resourceDict[_type];
                FileStateObj resobj = new FileStateObj(item);

                if (item.m_ExistStatus == AssetExistStatusEnum.Globle && ResLibaryConfig.ExistTypeNameToType.ContainsKey(resobj.m_Type))
                {
                    resobj.m_Asset = Resources.Load(resobj.m_Path, ResLibaryConfig.ExistTypeNameToType[resobj.m_Type]);
                }
                dict[item.m_Name] = resobj;
                LibaryStateObj libaryObj = new LibaryStateObj();
                libaryObj.m_Name = item.m_Name;
                libaryObj.m_Status = LibaryStatusEnum.DIR_STREAMINGASSET;
                libaryObj.m_Type = item.m_Type;
                if (UpdateAssetCallback != null)
                    UpdateAssetCallback(libaryObj);
                //ResLibaryMgr.Instance.UpdateLibary(libaryObj);
                if (iterationsPerFrame > 0 && ++iterations % iterationsPerFrame == 0)
                    yield return 1;
            }
        }
        string ILibaryHandle.GetTextAsset(string objName)
        {
            string _type = ResLibaryConfig.ExistType[LibaryTypeEnum.LibaryType_TextAsset];
            string streamingAssetPath = Application.streamingAssetsPath;
            if (resourceDict.ContainsKey(_type))
            {
                Dictionary<string, FileStateObj> _Dict = resourceDict[_type];
                FileStateObj stateObj;
                _Dict.TryGetValue(objName, out stateObj);
                if (stateObj != null && stateObj.m_Asset == null)
                {
                    string path = Path.Combine(streamingAssetPath, stateObj.m_Path);
                    stateObj.m_Asset = ResLibaryTool.LoadFileStr(path);
                    if (stateObj.m_Asset != null && stateObj.m_ExistStatus == AssetExistStatusEnum.Quote)
                        stateObj.m_Quote++;
                    if (stateObj.m_ExistStatus == AssetExistStatusEnum.Once)
                        stateObj.m_Asset = null;
                    return (string)stateObj.m_Asset;
                }
            }
            return null;
        }

        Sprite ILibaryHandle.GetSprite(string objName)
        {
            string _type = ResLibaryConfig.ExistType[LibaryTypeEnum.LibaryType_Texture2D];
            FileStateObj stateObj = GetFileStateObj(_type, objName);
            if (stateObj != null)
            {
                if (stateObj.m_Asset == null)
                {
                    Texture2D t = ((ILibaryHandle)this).GetTexture2d(objName);
                    Sprite spr = null;
                    if (t != null)
                    {
                        spr = Sprite.Create(t, new Rect(0, 0, t.width, t.height), Vector2.zero);
                        if(stateObj.m_ExistStatus == AssetExistStatusEnum.Quote)
                            stateObj.m_Quote++;
                    }

                    if (spr != null)
                        stateObj.m_Asset = spr;
                    if (stateObj.m_ExistStatus == AssetExistStatusEnum.Once)
                        stateObj.m_Asset = null;
                    return spr;
                }
                else
                    return (Sprite)stateObj.m_Asset;
            }

            return null;
        }
        Texture2D ILibaryHandle.GetTexture2d(string objName)
        {
            string _type = ResLibaryConfig.ExistType[LibaryTypeEnum.LibaryType_Texture2D];
            string streamingAssetPath = Application.streamingAssetsPath;
            if (resourceDict.ContainsKey(_type))
            {
                Dictionary<string, FileStateObj> _Dict = resourceDict[_type];
                FileStateObj stateObj;
                _Dict.TryGetValue(objName, out stateObj);
                if (stateObj != null)
                {
                    if (stateObj.m_Asset == null)
                        stateObj.m_Asset = ResLibaryTool.readLocalTexture2d(Path.Combine(streamingAssetPath, stateObj.m_Path));
                    Texture2D t = (Texture2D)stateObj.m_Asset;
                    if (t != null && stateObj.m_ExistStatus == AssetExistStatusEnum.Quote)
                        stateObj.m_Quote++;
                    if (stateObj.m_ExistStatus == AssetExistStatusEnum.Once)
                        stateObj.m_Asset = null;
                    return t;
                }
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
                    libaryExistStatusEnum = LibaryExistStatusEnum.LibaryExistStatus_NotUnityEngineObject_StreamAssetPath;
                    FileStateObj fobjAudio = GetFileStateObj(_type, objName);
                    if (fobjAudio != null)
                    {
                        data = fobjAudio.m_Path;
                    }
                    break;
                case LibaryTypeEnum.LibaryType_VideoClip:
                case LibaryTypeEnum.LibaryType_MovieTexture:
                    libaryExistStatusEnum = LibaryExistStatusEnum.LibaryExistStatus_NotUnityEngineObject_StreamAssetPath;
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

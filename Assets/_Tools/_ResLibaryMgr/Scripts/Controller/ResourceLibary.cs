using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
namespace ResLibary
{
    public class ResourceLibary : ILibaryHandle
    {
        private System.Action<LibaryStateObj> UpdateAssetCallback;
        private Dictionary<string, Dictionary<string, ResourceStateObj>> resourceDict;
        public ResourceLibary(System.Action<LibaryStateObj> UpdateAssetCallback)
        {
            this.UpdateAssetCallback = UpdateAssetCallback;
            resourceDict = new Dictionary<string, Dictionary<string, ResourceStateObj>>();

        }
        void ILibaryHandle.InsertLibrary(object data)
        {
            if (data.GetType().Name != "LibaryResourceSetting")
                return;
            LibaryResourceSetting assetSetting = (LibaryResourceSetting)data;
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
            if (resourceDict.ContainsKey(_type))
            {
                Dictionary<string, ResourceStateObj> objectDict = resourceDict[_type];
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
                    resourceDict[_type] = new Dictionary<string, ResourceStateObj>();
                Dictionary<string, ResourceStateObj> dict = resourceDict[_type];
                ResourceStateObj resobj = new ResourceStateObj(item);

                if (item.m_ExistStatus == AssetExistStatusEnum.Globle && ResLibaryConfig.ExistTypeNameToType.ContainsKey(resobj.m_Type))
                {
                    resobj.m_Asset = Resources.Load(resobj.m_Path, ResLibaryConfig.ExistTypeNameToType[resobj.m_Type]);
                }
                dict[item.m_Name] = resobj;

                LibaryStateObj libaryObj = new LibaryStateObj();
                libaryObj.m_Name = item.m_Name;
                libaryObj.m_Status = LibaryStatusEnum.DIR_RESOURCE;
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
                Dictionary<string, ResourceStateObj> _Dict = resourceDict[_type];
                ResourceStateObj stateObj;
                _Dict.TryGetValue(objName, out stateObj);
                if (stateObj != null)
                {
                    if (stateObj.m_Asset == null)
                        stateObj.m_Asset = Resources.Load(stateObj.m_Path);
                    UnityEngine.Object t = stateObj.m_Asset;
                    if (stateObj.m_ExistStatus == AssetExistStatusEnum.Once)
                        stateObj.m_Asset = null;
                    if (t != null && stateObj.m_ExistStatus == AssetExistStatusEnum.Quote)
                        stateObj.m_Quote++;
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

        void ILibaryHandle.releaseObj(LibaryTypeEnum libaryTypeEnum, string objName)
        {
            string _type = ResLibaryConfig.ExistType[libaryTypeEnum];
            ((ILibaryHandle)this).releaseObj(_type, objName);
        }

        void ILibaryHandle.releaseObj(string _type, string objName)
        {
            if (resourceDict.ContainsKey(_type))
            {
                Dictionary<string, ResourceStateObj> _Dict = resourceDict[_type];
                ResourceStateObj stateObj;
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
                List<ResourceStateObj> list = new List<ResourceStateObj>(dict.Values);
                HashSet<ResourceStateObj> array = new HashSet<ResourceStateObj>(list);
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
        }
    }
}
using ResLibary;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//namespace ResLibary
//{
    [System.Serializable]
    public class ResourceSettingStateObj
    {
        public string m_Name;
        public string m_Path;
        public string m_Type;

        public AssetExistStatusEnum m_ExistStatus = AssetExistStatusEnum.Quote;
    }

    public class FileStateObj : ResourceSettingStateObj
    {
        /// <summary>
        /// 引用次数
        /// </summary>
        public int m_Quote;
        public object m_Asset;
        public FileStateObj() { }
        public FileStateObj(ResourceSettingStateObj stateObj)
        {
            this.m_Name = stateObj.m_Name;
            this.m_Path = stateObj.m_Path;
            this.m_Type = stateObj.m_Type;
            this.m_ExistStatus = stateObj.m_ExistStatus;

        }
    }
    [CreateAssetMenu(fileName = "StreamingAssetLibarySetting", menuName = "LibaryStreamingAssetSetting")]
    [System.Serializable]
    public class LibaryStreamingAssetSetting : ScriptableObject
    {

        public List<ResourceSettingStateObj> texture2ds;
        public List<ResourceSettingStateObj> sprites;
        public List<ResourceSettingStateObj> textAssets;
        public List<ResourceSettingStateObj> audios;
        public List<ResourceSettingStateObj> videos;


        public virtual void AddResToLibary(ResourceSettingStateObj resourceSetting)
        {
            LibaryTypeEnum libaryStatusEnum;
            if (!ResLibaryTool.ExistTypeNameToEnum.TryGetValue(resourceSetting.m_Type, out libaryStatusEnum))
                return;
            switch (libaryStatusEnum)
            {
                case LibaryTypeEnum.LibaryType_Texture2D:
                    if (!texture2ds.Contains(resourceSetting))
                        texture2ds.Add(resourceSetting);
                    break;

                case LibaryTypeEnum.LibaryType_TextAsset:
                    if (!textAssets.Contains(resourceSetting))
                        textAssets.Add(resourceSetting);
                    break;

                case LibaryTypeEnum.LibaryType_Sprite:
                    if (!sprites.Contains(resourceSetting))
                        sprites.Add(resourceSetting);
                    break;

                case LibaryTypeEnum.LibaryType_AudioClip:
                    if (!audios.Contains(resourceSetting))
                        audios.Add(resourceSetting);
                    break;
                case LibaryTypeEnum.LibaryType_VideoClip:
                    if (!videos.Contains(resourceSetting))
                        videos.Add(resourceSetting);
                    break;
            }
        }

        public virtual void DelResToLibary(ResourceSettingStateObj resourceSetting)
        {
            LibaryTypeEnum libaryStatusEnum;
            if (!ResLibaryTool.ExistTypeNameToEnum.TryGetValue(resourceSetting.m_Type, out libaryStatusEnum))
                return;
            switch (libaryStatusEnum)
            {
                case LibaryTypeEnum.LibaryType_Texture2D:
                    if (texture2ds.Contains(resourceSetting))
                        texture2ds.Remove(resourceSetting);
                    break;

                case LibaryTypeEnum.LibaryType_TextAsset:
                    if (textAssets.Contains(resourceSetting))
                        textAssets.Remove(resourceSetting);
                    break;

                case LibaryTypeEnum.LibaryType_Sprite:
                    if (sprites.Contains(resourceSetting))
                        sprites.Remove(resourceSetting);
                    break;
                case LibaryTypeEnum.LibaryType_AudioClip:
                    if (audios.Contains(resourceSetting))
                        audios.Remove(resourceSetting);
                    break;
                case LibaryTypeEnum.LibaryType_VideoClip:
                    if (videos.Contains(resourceSetting))
                        videos.Remove(resourceSetting);
                    break;
            }
        }

        public virtual Dictionary<string, Dictionary<string, ResourceSettingStateObj>> GetSettingMessage()
        {
            Dictionary<string, Dictionary<string, ResourceSettingStateObj>> dict = new Dictionary<string, Dictionary<string, ResourceSettingStateObj>>();
            dict[ResLibaryTool.ExistType[LibaryTypeEnum.LibaryType_Texture2D]] = GetResourceSetting(texture2ds);
            dict[ResLibaryTool.ExistType[LibaryTypeEnum.LibaryType_TextAsset]] = GetResourceSetting(textAssets);
            dict[ResLibaryTool.ExistType[LibaryTypeEnum.LibaryType_AudioClip]] = GetResourceSetting(audios);
            dict[ResLibaryTool.ExistType[LibaryTypeEnum.LibaryType_VideoClip]] = GetResourceSetting(videos);
            dict[ResLibaryTool.ExistType[LibaryTypeEnum.LibaryType_Sprite]] = GetResourceSetting(sprites);
            return dict;
        }
        protected Dictionary<string, ResourceSettingStateObj> GetResourceSetting(List<ResourceSettingStateObj> list)
        {

            if (list != null)
            {
                Dictionary<string, ResourceSettingStateObj> returnList = new Dictionary<string, ResourceSettingStateObj>();
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i] != null && !returnList.ContainsKey(list[i].m_Name))
                        returnList.Add(list[i].m_Name, list[i]);
                }
                return returnList;
            }
            return null;
        }

        public virtual void Clear()
        {
            sprites.Clear();
            texture2ds.Clear();
            audios.Clear();
            videos.Clear();
            textAssets.Clear();
        }
    }
//}

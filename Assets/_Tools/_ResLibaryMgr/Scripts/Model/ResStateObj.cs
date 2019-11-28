using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//namespace ResLibary
//{
    [System.Serializable]
    public struct LibaryStateObj
    {
        public string m_Name;
        public string m_Type;
        public LibaryStatusEnum m_Status;
    }


    public enum AssetExistStatusEnum
    {
        /// <summary>
        /// assetBundle 常驻内存
        /// </summary>
        Globle,

        /// <summary>
        /// 一次性，没有引用就销毁
        /// </summary>
        Once,

        /// <summary>
        /// 切换场景时释放
        /// </summary>
        Scene,

        Quote,
    }

    public enum LibaryTypeEnum
    {
        LibaryType_Texture2D,
        LibaryType_RenderTexture,
        LibaryType_Sprite,
        LibaryType_TextAsset,
        LibaryType_Material,
        LibaryType_GameObject,
        LibaryType_AudioClip,
        LibaryType_VideoClip,
        LibaryType_MovieTexture
    }

    public enum LibaryExistStatusEnum
    {
        NONE,
        /// <summary>
        /// UnityEngine.Object
        /// </summary>
        LibaryExistStatus_UnityEngineObject,

        /// <summary>
        /// streammingasset path
        /// </summary>
        LibaryExistStatus_NotUnityEngineObject_StreamAssetPath,

        /// <summary>
        /// file path
        /// </summary>
        LibaryExistStatus_NotUnityEngineObject_FilePath,

        /// <summary>
        /// textasset  is not unityengine.object
        /// </summary>
        LibaryExistStatus_NotUnityEngineObject_TextAsset
    }

    public enum LibaryStatusEnum
    {
        /// <summary>
        /// assets下的资源
        /// </summary>
        DIR_ASSETS,

        /// <summary>
        /// resource下的资源
        /// </summary>
        DIR_RESOURCE,

        /// <summary>
        /// assetbundle里的资源
        /// </summary>
        DIR_ASSETBUNDER,

        /// <summary>
        /// 单个外部资源
        /// </summary>
        DIR_FILE,

        /// <summary>
        /// Application.streamingAssetsPath路径
        /// </summary>
        DIR_STREAMINGASSET,

    }
//}


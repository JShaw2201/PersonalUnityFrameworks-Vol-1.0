using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ResLibary
{
    public class ResLibaryConfig
    {
        // 支持的资源文件格式
        public static readonly List<string> ResourceExts = new List<string>
        {                        ".prefab", ".fbx",
                             ".png", ".jpg", ".dds", ".gif", ".psd", ".tga", ".bmp",
                             ".txt", ".bytes", ".xml", ".csv", ".json",
                             ".controller", ".shader", ".anim", ".unity", ".mat",
                             ".wav", ".mp3", ".ogg",".mp4",
                             ".ttf",
                             ".shadervariants", ".asset"
         };

        // 支持的资源文件格式
        public static readonly List<string> ResourceImgExts = new List<string> {
        ".png", ".jpg", ".dds", ".gif", ".psd", ".tga", ".bmp"};
        public static readonly List<string> ResourceTxtExts = new List<string> {
        ".txt", ".bytes", ".xml", ".csv", ".json"};
        public static readonly List<string> ResourceAudioExts = new List<string> {
        ".wav", ".mp3"};

        public static readonly List<string> ResourceVideoExts = new List<string> {
        ".mp4",".avi",".ogg"};

        public static readonly Dictionary<LibaryTypeEnum, string> ExistType = new Dictionary<LibaryTypeEnum, string>()
        {
            {LibaryTypeEnum.LibaryType_Texture2D,"Texture2D"},
            {LibaryTypeEnum.LibaryType_RenderTexture,"RenderTexture"},
            {LibaryTypeEnum.LibaryType_Sprite,"Sprite"      },
            {LibaryTypeEnum.LibaryType_TextAsset,"TextAsset"    },
            {LibaryTypeEnum.LibaryType_Material,"Material"    },
            {LibaryTypeEnum.LibaryType_GameObject,"GameObject"   },
            {LibaryTypeEnum.LibaryType_AudioClip,"AudioClip"    },
            {LibaryTypeEnum.LibaryType_VideoClip,"VideoClip"    },
            {LibaryTypeEnum.LibaryType_MovieTexture,"MovieTexture"  }
        };

        public static readonly Dictionary<string, LibaryTypeEnum> ExistTypeNameToEnum = new Dictionary<string, LibaryTypeEnum>()
        {
            {"Texture2D"    ,LibaryTypeEnum.LibaryType_Texture2D    },
            {"RenderTexture",LibaryTypeEnum.LibaryType_RenderTexture},
            {"Sprite"       ,LibaryTypeEnum.LibaryType_Sprite       },
            {"TextAsset"    ,LibaryTypeEnum.LibaryType_TextAsset   },
            {"Material"     ,LibaryTypeEnum.LibaryType_Material     },
            {"GameObject"   ,LibaryTypeEnum.LibaryType_GameObject   },
            {"AudioClip"    ,LibaryTypeEnum.LibaryType_AudioClip    },
            {"VideoClip"    ,LibaryTypeEnum.LibaryType_VideoClip    },
            {"MovieTexture" ,LibaryTypeEnum.LibaryType_MovieTexture }
        };

        public static readonly Dictionary<string, Type> ExistTypeNameToType = new Dictionary<string, Type>()
        {
            {"Texture2D",typeof(Texture2D)},
            {"RenderTexture",typeof(RenderTexture)},
            {"Sprite",typeof(Sprite)},
            {"TextAsset",typeof(TextAsset)},
            {"Material",typeof(Material)},
            {"GameObject",typeof(GameObject)},
            {"AudioClip",typeof(AudioClip)},
            {"VideoClip",typeof(UnityEngine.Video.VideoClip)},
            {"MovieTexture",typeof(MovieTexture)}
        };

    }
}
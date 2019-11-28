using ResLibary;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//namespace ResLibary
//{
[CreateAssetMenu(fileName = "AssetLibarySetting", menuName = "LibaryAssetSetting")]
[System.Serializable]
public class LibaryAssetSetting : ScriptableObject
{
    public List<UnityEngine.Texture2D> texture2ds;
    public List<UnityEngine.RenderTexture> renderTextures;
    public List<UnityEngine.Sprite> sprites;
    public List<UnityEngine.TextAsset> textAssets;
    public List<UnityEngine.Material> materials;
    public List<UnityEngine.GameObject> prefabs;
    public List<UnityEngine.AudioClip> audios;
    public List<UnityEngine.Video.VideoClip> videos;
    public List<UnityEngine.MovieTexture> movieTextures;

    public Dictionary<string, List<string>> GetAssetDict()
    {
        Dictionary<string, List<string>> Dict = new Dictionary<string, List<string>>();
        Dict[ResLibaryTool.ExistType[LibaryTypeEnum.LibaryType_Texture2D]] = GetAssetList(texture2ds);
        Dict[ResLibaryTool.ExistType[LibaryTypeEnum.LibaryType_RenderTexture]] = GetAssetList(renderTextures);
        Dict[ResLibaryTool.ExistType[LibaryTypeEnum.LibaryType_Sprite]] = GetAssetList(sprites);
        Dict[ResLibaryTool.ExistType[LibaryTypeEnum.LibaryType_TextAsset]] = GetAssetList(textAssets);
        Dict[ResLibaryTool.ExistType[LibaryTypeEnum.LibaryType_Material]] = GetAssetList(materials);
        Dict[ResLibaryTool.ExistType[LibaryTypeEnum.LibaryType_GameObject]] = GetAssetList(prefabs);
        Dict[ResLibaryTool.ExistType[LibaryTypeEnum.LibaryType_AudioClip]] = GetAssetList(audios);
        Dict[ResLibaryTool.ExistType[LibaryTypeEnum.LibaryType_VideoClip]] = GetAssetList(videos);
        Dict[ResLibaryTool.ExistType[LibaryTypeEnum.LibaryType_MovieTexture]] = GetAssetList(movieTextures);
        return Dict;
    }
    public List<string> GetAssetList<T>(List<T> dist) where T : UnityEngine.Object
    {
        List<string> list = new List<string>();
        T[] ts = dist == null ? new T[0] : dist.ToArray();
        for (int i = 0; i < ts.Length; i++)
        {
            T t = ts[i];
            if (t != null && !list.Contains(t.name))
            {
                list.Add(t.name);
            }
        }
        return list;
    }
    public void AddResToLibary<T>(T t) where T : UnityEngine.Object
    {
        LibaryTypeEnum libaryStatusEnum;
        string m_Type = t.GetType().Name;
        if (!ResLibaryTool.ExistTypeNameToEnum.TryGetValue(m_Type, out libaryStatusEnum))
            return;
        UnityEngine.Object data = t;
        switch (libaryStatusEnum)
        {
            case LibaryTypeEnum.LibaryType_Texture2D:
                if (texture2ds == null)
                    texture2ds = new List<Texture2D>();
                if (!texture2ds.Contains((UnityEngine.Texture2D)data))
                    texture2ds.Add((UnityEngine.Texture2D)data);
                break;
            case LibaryTypeEnum.LibaryType_RenderTexture:
                if (renderTextures == null)
                    renderTextures = new List<RenderTexture>();
                if (!renderTextures.Contains((UnityEngine.RenderTexture)data))
                    renderTextures.Add((UnityEngine.RenderTexture)data);
                break;
            case LibaryTypeEnum.LibaryType_MovieTexture:
                if (movieTextures == null)
                    movieTextures = new List<MovieTexture>();
                if (!movieTextures.Contains((UnityEngine.MovieTexture)data))
                    movieTextures.Add((UnityEngine.MovieTexture)data);
                break;
            case LibaryTypeEnum.LibaryType_Sprite:
                if (sprites == null)
                    sprites = new List<Sprite>();
                if (!sprites.Contains((UnityEngine.Sprite)data))
                    sprites.Add((UnityEngine.Sprite)data);
                break;
            case LibaryTypeEnum.LibaryType_TextAsset:
                if (textAssets == null)
                    textAssets = new List<TextAsset>();
                if (!textAssets.Contains((UnityEngine.TextAsset)data))
                    textAssets.Add((UnityEngine.TextAsset)data);
                break;
            case LibaryTypeEnum.LibaryType_Material:
                if (materials == null)
                    materials = new List<Material>();
                if (!materials.Contains((UnityEngine.Material)data))
                    materials.Add((UnityEngine.Material)data);
                break;
            case LibaryTypeEnum.LibaryType_GameObject:
                if (prefabs == null)
                    prefabs = new List<GameObject>();
                if (!prefabs.Contains((UnityEngine.GameObject)data))
                    prefabs.Add((UnityEngine.GameObject)data);
                break;
            case LibaryTypeEnum.LibaryType_AudioClip:
                if (audios == null)
                    audios = new List<AudioClip>();
                if (!audios.Contains((UnityEngine.AudioClip)data))
                    audios.Add((UnityEngine.AudioClip)data);
                break;
            case LibaryTypeEnum.LibaryType_VideoClip:
                if (videos == null)
                    videos = new List<UnityEngine.Video.VideoClip>();
                if (!videos.Contains((UnityEngine.Video.VideoClip)data))
                    videos.Add((UnityEngine.Video.VideoClip)data);
                break;
        }
    }

    public void DelResToLibary(string _type, int objIndex)
    {
        LibaryTypeEnum libaryStatusEnum;
        string m_Type = _type;
        if (!ResLibaryTool.ExistTypeNameToEnum.TryGetValue(m_Type, out libaryStatusEnum))
            return;
        switch (libaryStatusEnum)
        {
            case LibaryTypeEnum.LibaryType_Texture2D:
                if (texture2ds == null)
                    texture2ds = new List<Texture2D>();
                if (texture2ds.Count > 0 && objIndex >= 0 && objIndex < texture2ds.Count)
                    texture2ds.RemoveAt(objIndex);
                break;
            case LibaryTypeEnum.LibaryType_RenderTexture:
                if (renderTextures == null)
                    renderTextures = new List<RenderTexture>();
                if (renderTextures.Count > 0 && objIndex >= 0 && objIndex < renderTextures.Count)
                    renderTextures.RemoveAt(objIndex);
                break;
            case LibaryTypeEnum.LibaryType_MovieTexture:
                if (movieTextures == null)
                    movieTextures = new List<MovieTexture>();
                if (movieTextures.Count > 0 && objIndex >= 0 && objIndex < movieTextures.Count)
                    movieTextures.RemoveAt(objIndex);
                break;
            case LibaryTypeEnum.LibaryType_Sprite:
                if (sprites == null)
                    sprites = new List<Sprite>();
                if (sprites.Count > 0 && objIndex >= 0 && objIndex < sprites.Count)
                    sprites.RemoveAt(objIndex);
                break;
            case LibaryTypeEnum.LibaryType_TextAsset:
                if (textAssets == null)
                    textAssets = new List<TextAsset>();
                if (textAssets.Count > 0 && objIndex >= 0 && objIndex < textAssets.Count)
                    textAssets.RemoveAt(objIndex);
                break;
            case LibaryTypeEnum.LibaryType_Material:
                if (materials == null)
                    materials = new List<Material>();
                if (materials.Count > 0 && objIndex >= 0 && objIndex < materials.Count)
                    materials.RemoveAt(objIndex);
                break;
            case LibaryTypeEnum.LibaryType_GameObject:
                if (prefabs == null)
                    prefabs = new List<GameObject>();
                if (prefabs.Count > 0 && objIndex >= 0 && objIndex < prefabs.Count)
                    prefabs.RemoveAt(objIndex);
                break;
            case LibaryTypeEnum.LibaryType_AudioClip:
                if (audios == null)
                    audios = new List<AudioClip>();
                if (audios.Count > 0 && objIndex >= 0 && objIndex < audios.Count)
                    audios.RemoveAt(objIndex);
                break;
            case LibaryTypeEnum.LibaryType_VideoClip:
                if (videos == null)
                    videos = new List<UnityEngine.Video.VideoClip>();
                if (videos.Count > 0 && objIndex >= 0 && objIndex < videos.Count)
                    videos.RemoveAt(objIndex);
                break;
        }
    }
}
//}




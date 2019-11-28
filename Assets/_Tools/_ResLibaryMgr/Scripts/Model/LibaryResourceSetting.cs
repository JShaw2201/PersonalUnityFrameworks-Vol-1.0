using ResLibary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//namespace ResLibary
//{
public class ResourceObj
{
    /// <summary>
    /// 引用次数
    /// </summary>
    public int quoteNum;

    public string objName;

    public UnityEngine.Object obj;
}

public class ResourceStateObj : ResourceSettingStateObj
{
    /// <summary>
    /// 引用次数
    /// </summary>
    public int m_Quote;

    public AssetExistStatusEnum resourceExistStatus = AssetExistStatusEnum.Once;

    public UnityEngine.Object m_Asset;

    public ResourceStateObj(ResourceSettingStateObj stateObj)
    {
        this.m_Name = stateObj.m_Name;
        this.m_Path = stateObj.m_Path;
        this.m_Type = stateObj.m_Type;
    }
}
[CreateAssetMenu(fileName = "ResourceLibarySetting", menuName = "LibaryResourceSetting")]
public class LibaryResourceSetting : LibaryStreamingAssetSetting
{
    public List<ResourceSettingStateObj> renderTextures;
    public List<ResourceSettingStateObj> materials;
    public List<ResourceSettingStateObj> prefabs;
    public List<ResourceSettingStateObj> movieTextures;

    public override void AddResToLibary(ResourceSettingStateObj resourceSetting)
    {
        LibaryTypeEnum libaryStatusEnum;
        if (!ResLibaryConfig.ExistTypeNameToEnum.TryGetValue(resourceSetting.m_Type, out libaryStatusEnum))
            return;
        switch (libaryStatusEnum)
        {

            case LibaryTypeEnum.LibaryType_RenderTexture:
                if (renderTextures == null)
                    renderTextures = new List<ResourceSettingStateObj>();
                if (!renderTextures.Contains(resourceSetting))
                    renderTextures.Add(resourceSetting);
                break;
            case LibaryTypeEnum.LibaryType_MovieTexture:
                if (movieTextures == null)
                    movieTextures = new List<ResourceSettingStateObj>();
                if (!movieTextures.Contains(resourceSetting))
                    movieTextures.Add(resourceSetting);
                break;

            case LibaryTypeEnum.LibaryType_Material:
                if (materials == null)
                    materials = new List<ResourceSettingStateObj>();
                if (!materials.Contains(resourceSetting))
                    materials.Add(resourceSetting);
                break;
            case LibaryTypeEnum.LibaryType_GameObject:
                if (prefabs == null)
                    prefabs = new List<ResourceSettingStateObj>();
                if (!prefabs.Contains(resourceSetting))
                    prefabs.Add(resourceSetting);
                break;
            default:
                base.AddResToLibary(resourceSetting);
                break;
        }
    }

    public override void DelResToLibary(ResourceSettingStateObj resourceSetting)
    {
        LibaryTypeEnum libaryStatusEnum;
        if (!ResLibaryConfig.ExistTypeNameToEnum.TryGetValue(resourceSetting.m_Type, out libaryStatusEnum))
            return;
        switch (libaryStatusEnum)
        {
            case LibaryTypeEnum.LibaryType_RenderTexture:
                if (renderTextures == null)
                    renderTextures = new List<ResourceSettingStateObj>();
                if (renderTextures.Contains(resourceSetting))
                    renderTextures.Remove(resourceSetting);
                break;
            case LibaryTypeEnum.LibaryType_MovieTexture:
                if (movieTextures == null)
                    movieTextures = new List<ResourceSettingStateObj>();
                if (movieTextures.Contains(resourceSetting))
                    movieTextures.Remove(resourceSetting);
                break;


            case LibaryTypeEnum.LibaryType_Material:
                if (materials == null)
                    materials = new List<ResourceSettingStateObj>();
                if (materials.Contains(resourceSetting))
                    materials.Remove(resourceSetting);
                break;
            case LibaryTypeEnum.LibaryType_GameObject:
                if (prefabs == null)
                    prefabs = new List<ResourceSettingStateObj>();
                if (prefabs.Contains(resourceSetting))
                    prefabs.Remove(resourceSetting);
                break;
            default:
                base.DelResToLibary(resourceSetting);
                break;
        }
    }

    public override Dictionary<string, Dictionary<string, ResourceSettingStateObj>> GetSettingMessage()
    {
       
        Dictionary<string, Dictionary<string, ResourceSettingStateObj>> dict = base.GetSettingMessage();
        if (renderTextures == null)
            renderTextures = new List<ResourceSettingStateObj>();
        if (movieTextures == null)
            movieTextures = new List<ResourceSettingStateObj>();
        if (materials == null)
            materials = new List<ResourceSettingStateObj>();
        if (prefabs == null)
            prefabs = new List<ResourceSettingStateObj>();
        dict[ResLibaryConfig.ExistType[LibaryTypeEnum.LibaryType_RenderTexture]] = GetResourceSetting(renderTextures);
        dict[ResLibaryConfig.ExistType[LibaryTypeEnum.LibaryType_MovieTexture]] = GetResourceSetting(movieTextures);
        dict[ResLibaryConfig.ExistType[LibaryTypeEnum.LibaryType_Material]] = GetResourceSetting(materials);
        dict[ResLibaryConfig.ExistType[LibaryTypeEnum.LibaryType_GameObject]] = GetResourceSetting(prefabs);
        return dict;
    }

    public override void Clear()
    {
        base.Clear();
        if (renderTextures == null)
            renderTextures = new List<ResourceSettingStateObj>();
        if (movieTextures == null)
            movieTextures = new List<ResourceSettingStateObj>();
        if (materials == null)
            materials = new List<ResourceSettingStateObj>();
        if (prefabs == null)
            prefabs = new List<ResourceSettingStateObj>();
        renderTextures.Clear();
        sprites.Clear();
        materials.Clear();
        prefabs.Clear();
        movieTextures.Clear();
    }
}
//}

using ResLibary;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Video;

[CustomEditor(typeof(LibaryResourceSetting))]
public class ResourceLibarySettingEditor : Editor
{
    private LibaryResourceSetting libaryCatch;
    private string[] showPopup = new string[]{"Hide","Show"};
    private  Dictionary<string,int> showDict = new Dictionary<string, int>();
    private void OnEnable()
    {
        libaryCatch = target as LibaryResourceSetting;
    }

    public override void OnInspectorGUI()
    {
        OnInspectorObjectGUI(libaryCatch.texture2ds, "texturn2ds");
        EditorGUILayout.Space();
        OnInspectorObjectGUI(libaryCatch.renderTextures, "renderTextures");
        EditorGUILayout.Space();
        OnInspectorObjectGUI(libaryCatch.movieTextures, "movieTextures");
        EditorGUILayout.Space();
        OnInspectorObjectGUI(libaryCatch.sprites, "sprites");
        EditorGUILayout.Space();
        OnInspectorObjectGUI(libaryCatch.textAssets, "textAssets");
        EditorGUILayout.Space();
        OnInspectorObjectGUI(libaryCatch.materials, "materials");
        EditorGUILayout.Space();
        OnInspectorObjectGUI(libaryCatch.prefabs, "prefabs");
        EditorGUILayout.Space();
        OnInspectorObjectGUI(libaryCatch.audios, "audios");
        EditorGUILayout.Space();
        OnInspectorObjectGUI(libaryCatch.videos, "videos");

        if (GUILayout.Button("Update"))
        {
            UpdateAsset();
        }
        if (GUILayout.Button("Clear"))
        {
            libaryCatch.Clear();
        }

        EditorGUILayout.Space();
        if (GUILayout.Button("Save"))
        {
            EditorUtility.SetDirty(target);
            serializedObject.ApplyModifiedProperties();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        OnGUIChanged();
    }

    private void OnGUIChanged()
    {
        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
            serializedObject.ApplyModifiedProperties();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
    private void OnInspectorObjectGUI(List<ResourceSettingStateObj> list, string label)
    {
        if (!showDict.ContainsKey(label))
            showDict[label] = 0;
        float wid = EditorGUIUtility.currentViewWidth;
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(label,GUILayout.MaxWidth(wid/3));
        string CountInfor = "0";
        if (list != null)
            CountInfor = list.Count.ToString();
        EditorGUILayout.LabelField("Count:" + CountInfor, GUILayout.MaxWidth(wid / 3));
        int isShow = showDict[label];
        isShow = EditorGUILayout.Popup(isShow, showPopup,GUILayout.MaxWidth(wid / 3));
        showDict[label] = isShow;       
        EditorGUILayout.EndHorizontal();
        if (isShow == 0)
            return;
        if (list == null)
            return;
        for (int i = 0; i < list.Count; i++)
        {
            ResourceSettingStateObj settingStateObj = list[i];
            EditorGUILayout.BeginHorizontal();
            
            EditorGUILayout.LabelField(settingStateObj.m_Name, GUILayout.Width(wid/3));            
            EditorGUILayout.LabelField(settingStateObj.m_Path,GUILayout.Width(wid/3));
            settingStateObj.m_ExistStatus = (AssetExistStatusEnum)EditorGUILayout.EnumPopup(settingStateObj.m_ExistStatus,GUILayout.Width(wid/3));
            EditorGUILayout.EndHorizontal();
        }
    }

    private void UpdateAsset()
    {
        ResourceLibarySettingWindows myWindow = (ResourceLibarySettingWindows)EditorWindow.GetWindow(typeof(ResourceLibarySettingWindows), false, "ResourceSetting", true);//创建窗口
        myWindow.InitResourceLibarySettingWindows(libaryCatch);//展示
    }

    private void ResourcesList()
    {
        LibaryResourceSetting _resourseSetting = Resources.Load<LibaryResourceSetting>("ResourceLibarySetting");
        if (_resourseSetting == null)
            return;
        string path = AssetDatabase.GetAssetPath(_resourseSetting);
        LibaryResourceSetting resourseSetting = AssetDatabase.LoadAssetAtPath<LibaryResourceSetting>(path);
        Dictionary<string, Dictionary<string, ResourceSettingStateObj>> msgDict = resourseSetting.GetSettingMessage();
        resourseSetting.Clear();
        List<string> listRes = ResLibaryTool.GetAllLocalResourceDirs(Application.dataPath);
        List<string> ret = new List<string>();
        for (int i = 0; i < listRes.Count; i++)
        {
            List<string> list = ResLibaryTool.GetAllSubDirs(listRes[i]);
            if (list != null)
                ret.AddRange(list);
        }
        ret.AddRange(listRes);
        Debug.Log("ret:" + ret.Count);
        for (int i = 0; i < ret.Count; i++)
        {
            string resDir = ret[i];
            if (string.IsNullOrEmpty(resDir) || !ResLibaryTool.DirExistResource(resDir))
                continue;
            string[] files = System.IO.Directory.GetFiles(resDir);
            for (int j = 0; j < files.Length; j++)
            {
                string resfile = files[j];
                string extension = Path.GetExtension(resfile);
                if (!ResLibaryConfig.ResourceExts.Contains(extension))
                    continue;
                resfile = ResLibaryTool.GetAssetRelativePath(resfile);
                int index = resfile.IndexOf("Resources/", StringComparison.CurrentCultureIgnoreCase);
                if (index < 0)
                    continue;
                string filePath = resfile.Substring(index);
                filePath = filePath.Replace("Resources/", "");
                if (Path.HasExtension(filePath))
                    filePath = filePath.Replace(extension, "");
                UnityEngine.Object obj = Resources.Load(filePath);
                if (!ResLibaryConfig.ExistType.ContainsValue(obj.GetType().Name))
                    continue;
                if (obj.GetType().Name == ResLibaryConfig.ExistType[LibaryTypeEnum.LibaryType_Texture2D])
                {
                    TextureImporter textureImporter = TextureImporter.GetAtPath(resfile) as TextureImporter;
                    if (textureImporter.textureType == TextureImporterType.Sprite)
                    {
                        Dictionary<string, ResourceSettingStateObj> sDict = null;
                        msgDict.TryGetValue(ResLibaryConfig.ExistType[LibaryTypeEnum.LibaryType_Sprite], out sDict);
                        UnityEngine.Object[] sprs = AssetDatabase.LoadAllAssetsAtPath(resfile);
                        for (int k = 0; k < sprs.Length; k++)
                        {
                           
                            
                            ResourceSettingStateObj settingStateObjs = new ResourceSettingStateObj();
                            settingStateObjs.m_Name = sprs[k].name;
                            settingStateObjs.m_Path = filePath + "/" + sprs[k].name;
                            settingStateObjs.m_Type = ResLibaryConfig.ExistType[LibaryTypeEnum.LibaryType_Sprite];
                            if (sprs[k].GetType().Name == ResLibaryConfig.ExistType[LibaryTypeEnum.LibaryType_Sprite])
                            {
                                if (sDict != null && sDict.ContainsKey(sprs[k].name))
                                {
                                    resourseSetting.AddResToLibary(sDict[sprs[k].name]);
                                    continue;
                                }
                                resourseSetting.AddResToLibary(settingStateObjs);
                            }
                            else
                            {
                                Dictionary<string, ResourceSettingStateObj> sDictT = null;
                                msgDict.TryGetValue(ResLibaryConfig.ExistType[LibaryTypeEnum.LibaryType_Texture2D], out sDictT);
                                if (sDictT != null && sDictT.ContainsKey(sprs[k].name))
                                {
                                    resourseSetting.AddResToLibary(sDictT[sprs[k].name]);
                                    continue;
                                }
                                settingStateObjs.m_Path = filePath;
                                settingStateObjs.m_Type = ResLibaryConfig.ExistType[LibaryTypeEnum.LibaryType_Texture2D];
                                resourseSetting.AddResToLibary(settingStateObjs);
                            }
                        }
                    }
                    else
                    {
                        Dictionary<string, ResourceSettingStateObj> sDict = null;
                        msgDict.TryGetValue(ResLibaryConfig.ExistType[LibaryTypeEnum.LibaryType_Texture2D], out sDict);
                        if (sDict != null && sDict.ContainsKey(obj.name))
                        {
                            resourseSetting.AddResToLibary(sDict[obj.name]);
                            continue;
                        }
                        ResourceSettingStateObj settingStateObj = new ResourceSettingStateObj() ;
                        settingStateObj.m_Name = obj.name;
                        settingStateObj.m_Path = filePath;
                        settingStateObj.m_Type = obj.GetType().Name;
                        settingStateObj.m_ExistStatus = AssetExistStatusEnum.Quote;
                        resourseSetting.AddResToLibary(settingStateObj);
                    }
                }
                else
                {
                    Dictionary<string, ResourceSettingStateObj> sDict = null;
                    msgDict.TryGetValue(obj.GetType().Name, out sDict);
                    if (sDict != null && sDict.ContainsKey(obj.name))
                    {
                        resourseSetting.AddResToLibary(sDict[obj.name]);
                        continue;
                    }
                    ResourceSettingStateObj settingStateObj = new ResourceSettingStateObj() ;
                    settingStateObj.m_Name = obj.name;
                    settingStateObj.m_Path = filePath;
                    settingStateObj.m_Type = obj.GetType().Name;
                    settingStateObj.m_ExistStatus = AssetExistStatusEnum.Quote;
                    resourseSetting.AddResToLibary(settingStateObj);
                }
            }
        }
        AssetDatabase.ImportAsset(path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
    
  
}

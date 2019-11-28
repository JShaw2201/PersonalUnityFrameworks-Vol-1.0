using ResLibary;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class ResourceLibarySettingWindows : EditorWindow
{
    private LibaryResourceSetting resourseSetting;
    private List<string> listRes = new List<string>();
    private Dictionary<string, bool> assetDict = new Dictionary<string, bool>();
    public void InitResourceLibarySettingWindows(LibaryResourceSetting _resourseSetting)
    {
        this.resourseSetting = _resourseSetting;
       
        this.minSize = new Vector2(960, 720);
        this.Show();
        
    }

    private void OnGUI()
    {
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        foreach (var item in listRes)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(item, GUILayout.Width(position.width*0.75f));
            assetDict[item] = EditorGUILayout.Toggle(assetDict[item]);
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.Space();
        if (GUILayout.Button("+"))
        {
            InsertPath();
        }
        EditorGUILayout.Space();
        if (GUILayout.Button("Update"))
        {
            UpdateAsset();
        }
    }

    private void InsertPath()
    {
        string path = EditorUtility.OpenFolderPanel("选择路径", "new Folder", "new Folder");
        int index = path.IndexOf("Resources", StringComparison.CurrentCultureIgnoreCase);
        if (index < 0)
            return;
        string dataPath = Application.dataPath.Replace("//", "/");
        dataPath = dataPath.Replace("\\", "/");
        dataPath = dataPath.Replace("\\\\", "/");
        dataPath = dataPath.Replace('\\','/');
        path = path.Replace("//", "/");
        path = path.Replace("\\", "/");
        path = path.Replace("\\\\", "/");
        path = path.Replace('\\', '/');
        //path = path.Replace(dataPath, "");
        int dataPathIndex = path.IndexOf("Assets", StringComparison.CurrentCultureIgnoreCase);
        if (dataPathIndex < 0)
            return;
        path = path.Substring(dataPathIndex);
        path = path.Replace("Assets", "");
        foreach (var item in assetDict)
        {
            int cIndex = item.Key.LastIndexOf(path, StringComparison.CurrentCultureIgnoreCase);
            if (cIndex>=0)
            {
                return;
            }
        }
        if (!assetDict.ContainsKey(path))
        {
            assetDict[path] = true;
            listRes = new List<string>(assetDict.Keys);
        }
    }

    private void UpdateAsset()
    {
        string path = AssetDatabase.GetAssetPath(resourseSetting);
        Dictionary<string, Dictionary<string, ResourceSettingStateObj>> msgDict = resourseSetting.GetSettingMessage();
        //List<string> listRes = new List<string>(assetDict.Keys);
        List<string> ret = new List<string>();
        foreach (var item in assetDict)
        {
            if (item.Value)
            {
                string url = Application.dataPath + item.Key;//Path.Combine(Application.dataPath, item.Key);
                ret.Add(url);
                List<string> list = ResLibaryTool.GetAllSubDirs(url);
                if (list != null)
                    ret.AddRange(list);
            }
        }
        //for (int i = 0; i < listRes.Count; i++)
        //{
        //    List<string> list = ResLibaryTool.GetAllSubDirs(listRes[i]);
        //    if (list != null)
        //        ret.AddRange(list);
        //}
        //ret.AddRange(listRes);
        Debug.Log("ret:" + ret.Count);
        resourseSetting.Clear();
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
                if (!ResLibaryTool.ResourceExts.Contains(extension))
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
                if (!ResLibaryTool.ExistType.ContainsValue(obj.GetType().Name))
                    continue;
                if (obj.GetType().Name == ResLibaryTool.ExistType[LibaryTypeEnum.LibaryType_Texture2D])
                {
                    TextureImporter textureImporter = TextureImporter.GetAtPath(resfile) as TextureImporter;
                    if (textureImporter == null)
                        continue;
                    if (textureImporter.textureType == TextureImporterType.Sprite)
                    {
                        Dictionary<string, ResourceSettingStateObj> sDict = null;
                        msgDict.TryGetValue(ResLibaryTool.ExistType[LibaryTypeEnum.LibaryType_Sprite], out sDict);
                        UnityEngine.Object[] sprs = AssetDatabase.LoadAllAssetsAtPath(resfile);
                        for (int k = 0; k < sprs.Length; k++)
                        {
                            ResourceSettingStateObj settingStateObjs = new ResourceSettingStateObj();
                            settingStateObjs.m_Name = sprs[k].name;
                            settingStateObjs.m_Path = filePath + "/" + sprs[k].name;
                            settingStateObjs.m_Type = ResLibaryTool.ExistType[LibaryTypeEnum.LibaryType_Sprite];
                            if (sprs[k].GetType().Name == ResLibaryTool.ExistType[LibaryTypeEnum.LibaryType_Sprite])
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
                                msgDict.TryGetValue(ResLibaryTool.ExistType[LibaryTypeEnum.LibaryType_Texture2D], out sDictT);
                                if (sDictT != null && sDictT.ContainsKey(sprs[k].name))
                                {
                                    resourseSetting.AddResToLibary(sDictT[sprs[k].name]);
                                    continue;
                                }
                                settingStateObjs.m_Path = filePath;
                                settingStateObjs.m_Type = ResLibaryTool.ExistType[LibaryTypeEnum.LibaryType_Texture2D];
                                resourseSetting.AddResToLibary(settingStateObjs);
                            }
                        }
                    }
                    else
                    {
                        Dictionary<string, ResourceSettingStateObj> sDict = null;
                        msgDict.TryGetValue(ResLibaryTool.ExistType[LibaryTypeEnum.LibaryType_Texture2D], out sDict);
                        if (sDict != null && sDict.ContainsKey(obj.name))
                        {
                            resourseSetting.AddResToLibary(sDict[obj.name]);
                            continue;
                        }
                        ResourceSettingStateObj settingStateObj = new ResourceSettingStateObj();
                        settingStateObj.m_Name = obj.name;
                        settingStateObj.m_Path = filePath;
                        settingStateObj.m_Type = obj.GetType().Name;
                        settingStateObj.m_ExistStatus = AssetExistStatusEnum.Scene;
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
                    ResourceSettingStateObj settingStateObj = new ResourceSettingStateObj();
                    settingStateObj.m_Name = obj.name;
                    settingStateObj.m_Path = filePath;
                    settingStateObj.m_Type = obj.GetType().Name;
                    settingStateObj.m_ExistStatus = AssetExistStatusEnum.Scene;
                    resourseSetting.AddResToLibary(settingStateObj);
                }
            }
        }
        AssetDatabase.ImportAsset(path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}

using ResLibary;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class StreamingAssetLibarySettingWindows : EditorWindow
{

    private LibaryStreamingAssetSetting resourseSetting;
    private List<string> listRes = new List<string>();
    private Dictionary<string, bool> assetDict = new Dictionary<string, bool>();
    public void InitStreamingAssetSettingWindows(LibaryStreamingAssetSetting _resourseSetting)
    {
        this.resourseSetting = _resourseSetting;
        this.minSize = new Vector2(960, 720);
        this.Show();

    }

    private void OnGUI()
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Count:"+ listRes.Count);
        EditorGUILayout.Space();
        foreach (var item in listRes)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(item, GUILayout.Width(position.width * 0.75f));
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
        int index = path.IndexOf("StreamingAssets", StringComparison.CurrentCultureIgnoreCase);
        if (index < 0)
            return;
        string dataPath = Application.dataPath.Replace("//", "/");
        dataPath = dataPath.Replace("\\", "/");
        dataPath = dataPath.Replace("\\\\", "/");
        dataPath = dataPath.Replace('\\', '/');
        path = path.Replace("//", "/");
        path = path.Replace("\\", "/");
        path = path.Replace("\\\\", "/");
        path = path.Replace('\\', '/');
        //path = path.Replace(dataPath, "");
       
        path = path.Substring(index);
        path = path.Replace("StreamingAssets", "/StreamingAssets");
        foreach (var item in assetDict)
        {
            int cIndex = item.Key.LastIndexOf(path, StringComparison.CurrentCultureIgnoreCase);
            if (cIndex >= 0)
            {
                Debug.Log(cIndex);
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
                string typeName = null;
                resfile = resfile.Replace("\\", "/");
                int index = resfile.IndexOf("StreamingAssets/", StringComparison.CurrentCultureIgnoreCase);
                if (index < 0)
                    continue;
                string filePath = resfile.Substring(index);
                filePath = filePath.Replace("StreamingAssets/", "");
                string fileName = Path.GetFileNameWithoutExtension(resfile);
                if (ResLibaryConfig.ResourceImgExts.Contains(extension))
                {
                    typeName = ResLibaryConfig.ExistType[LibaryTypeEnum.LibaryType_Texture2D];
                    ResourceSettingStateObj resourceSettingStateObjs = new ResourceSettingStateObj();
                    resourceSettingStateObjs.m_Name = fileName;
                    resourceSettingStateObjs.m_Path = filePath;
                    resourceSettingStateObjs.m_Type = ResLibaryConfig.ExistType[LibaryTypeEnum.LibaryType_Sprite];
                    resourceSettingStateObjs.m_ExistStatus = AssetExistStatusEnum.Quote;
                    resourseSetting.AddResToLibary(resourceSettingStateObjs);
                }
                else if (ResLibaryConfig.ResourceTxtExts.Contains(extension))
                {
                    typeName = ResLibaryConfig.ExistType[LibaryTypeEnum.LibaryType_TextAsset];
                }
                else if (ResLibaryConfig.ResourceAudioExts.Contains(extension))
                {
                    typeName = ResLibaryConfig.ExistType[LibaryTypeEnum.LibaryType_AudioClip];
                }
                else if (ResLibaryConfig.ResourceVideoExts.Contains(extension))
                {
                    typeName = ResLibaryConfig.ExistType[LibaryTypeEnum.LibaryType_VideoClip];
                }
                else
                    continue;


                Dictionary<string, ResourceSettingStateObj> sDict = null;
                msgDict.TryGetValue(typeName, out sDict);
                if (sDict != null && sDict.ContainsKey(fileName))
                {
                    resourseSetting.AddResToLibary(sDict[fileName]);
                    continue;
                }
                ResourceSettingStateObj resourceSettingStateObj = new ResourceSettingStateObj();
                resourceSettingStateObj.m_Name = fileName;
                resourceSettingStateObj.m_Path = filePath;
                resourceSettingStateObj.m_Type = typeName;
                resourceSettingStateObj.m_ExistStatus = AssetExistStatusEnum.Scene;
                resourseSetting.AddResToLibary(resourceSettingStateObj);
            }

        }
        AssetDatabase.ImportAsset(path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}

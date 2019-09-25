using ResLibary;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Video;

[CustomEditor(typeof(LibaryStreamingAssetSetting))]
public class StreamingAssetLibarySettingEditor : Editor
{
    private LibaryStreamingAssetSetting libaryCatch;
    private string[] showPopup = new string[]{"Hide","Show"};
    private  Dictionary<string,int> showDict = new Dictionary<string, int>();
    private void OnEnable()
    {
        libaryCatch = target as LibaryStreamingAssetSetting;
    }

    public override void OnInspectorGUI()
    {
        OnInspectorObjectGUI(libaryCatch.texture2ds, "texturn2ds");
        EditorGUILayout.Space();

        EditorGUILayout.Space();
        OnInspectorObjectGUI(libaryCatch.textAssets, "textAssets");
        EditorGUILayout.Space();
        OnInspectorObjectGUI(libaryCatch.sprites, "sprites");
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
            //float wid = EditorGUIUtility.currentViewWidth;
            EditorGUILayout.LabelField(settingStateObj.m_Name, GUILayout.Width(wid/3));
            EditorGUILayout.LabelField(settingStateObj.m_Path,GUILayout.Width(wid/3));
            settingStateObj.m_ExistStatus = (AssetExistStatusEnum)EditorGUILayout.EnumPopup(settingStateObj.m_ExistStatus,GUILayout.Width(wid/3));
            EditorGUILayout.EndHorizontal();
        }
    }

    private void UpdateAsset()
    {
        StreamingAssetLibarySettingWindows myWindow = (StreamingAssetLibarySettingWindows)EditorWindow.GetWindow(typeof(StreamingAssetLibarySettingWindows), false, "StreamingAssetSetting", true);//创建窗口
        myWindow.InitStreamingAssetSettingWindows(libaryCatch);//展示
    }
    private  void ResourcesList()
    {
        LibaryStreamingAssetSetting _resourseSetting = Resources.Load<LibaryStreamingAssetSetting>("StreamingAssetLibarySetting");
        if (_resourseSetting == null)
            return;
        string path = AssetDatabase.GetAssetPath(_resourseSetting);
        LibaryStreamingAssetSetting resourseSetting = AssetDatabase.LoadAssetAtPath<LibaryStreamingAssetSetting>(path);
        Dictionary<string, Dictionary<string, ResourceSettingStateObj>> msgDict = resourseSetting.GetSettingMessage();
        resourseSetting.Clear();
        List<string> listRes = ResLibaryTool.GetAllLocalStreamingAssetsDirs(Application.dataPath);
        List<string> ret = new List<string>();
        for (int i = 0; i < listRes.Count; i++)
        {
            List<string> list = ResLibaryTool.GetAllSubDirs(listRes[i]);
            if (list != null)
                ret.AddRange(list);
        }
        ret.AddRange(listRes);
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
                if (ResLibaryTool.ResourceImgExts.Contains(extension))
                {
                    typeName =ResLibaryTool.ExistType[LibaryTypeEnum.LibaryType_Texture2D];
                    ResourceSettingStateObj resourceSettingStateObjs = new ResourceSettingStateObj();
                    resourceSettingStateObjs.m_Name = fileName;
                    resourceSettingStateObjs.m_Path = filePath;
                    resourceSettingStateObjs.m_Type = ResLibaryTool.ExistType[LibaryTypeEnum.LibaryType_Sprite];
                    resourceSettingStateObjs.m_ExistStatus = AssetExistStatusEnum.Quote;
                    resourseSetting.AddResToLibary(resourceSettingStateObjs);
                }
                else if (ResLibaryTool.ResourceTxtExts.Contains(extension))
                {
                    typeName = ResLibaryTool.ExistType[LibaryTypeEnum.LibaryType_TextAsset];
                }
                else if (ResLibaryTool.ResourceAudioExts.Contains(extension))
                {
                    typeName = ResLibaryTool.ExistType[LibaryTypeEnum.LibaryType_AudioClip];
                }
                else if (ResLibaryTool.ResourceVideoExts.Contains(extension))
                {
                    typeName = ResLibaryTool.ExistType[LibaryTypeEnum.LibaryType_VideoClip];
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
                resourceSettingStateObj.m_ExistStatus = AssetExistStatusEnum.Quote;
                resourseSetting.AddResToLibary(resourceSettingStateObj);
            }

        }
        AssetDatabase.ImportAsset(path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}

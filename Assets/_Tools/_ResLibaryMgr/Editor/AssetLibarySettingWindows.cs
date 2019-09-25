using ResLibary;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class AssetLibarySettingWindows : EditorWindow
{

    private LibaryAssetSetting resourseSetting;
    private List<string> listRes = new List<string>();
    private Dictionary<string, bool> assetDict = new Dictionary<string, bool>();
    public void InitAssetLibarySettingWindows(LibaryAssetSetting _resourseSetting)
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
        int rindex = path.IndexOf("Resources/", StringComparison.CurrentCultureIgnoreCase);
        if (rindex >= 0)
            return;
        int sindex = path.IndexOf("StreamingAssets/", StringComparison.CurrentCultureIgnoreCase);
        if (sindex >= 0)
            return;
        string dataPath = Application.dataPath.Replace("//", "/");
        dataPath = dataPath.Replace("\\", "/");
        dataPath = dataPath.Replace("\\\\", "/");
        path = path.Replace("//", "/");
        path = path.Replace("\\", "/");
        path = path.Replace("\\\\", "/");
        path = path.Replace(dataPath, "");
        foreach (var item in assetDict)
        {
            int cIndex = item.Key.LastIndexOf(path, StringComparison.CurrentCultureIgnoreCase);
            if (cIndex >= 0)
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
        Dictionary<string, List<string>> msgDict = resourseSetting.GetAssetDict();
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
        string dataPath = Application.dataPath.Replace("//", "/");
        dataPath = dataPath.Replace("\\", "/");
        dataPath = dataPath.Replace("\\\\", "/");
        for (int i = 0; i < ret.Count; i++)
        {          
            string[] files = System.IO.Directory.GetFiles(ret[i]);
            for (int j = 0; j < files.Length; j++)
            {
                string file = files[j];

                string extension = Path.GetExtension(file);
                if (!ResLibaryTool.ResourceExts.Contains(extension))
                    continue;

                file = file.Replace("//", "/");
                file = file.Replace("\\", "/");
                file = file.Replace("\\\\", "/");
                file = file.Replace(dataPath,"Assets");
                UnityEngine.Object obj = AssetDatabase.LoadMainAssetAtPath(file);
                if (obj == null)
                {
                    continue;
                }        
                if (!ResLibaryTool.ExistType.ContainsValue(obj.GetType().Name))
                    continue;
                if (obj.GetType().Name == ResLibaryTool.ExistType[LibaryTypeEnum.LibaryType_Texture2D])
                {
                    string resfile = AssetDatabase.GetAssetPath(obj);
                    TextureImporter textureImporter = TextureImporter.GetAtPath(resfile) as TextureImporter;
                    if (textureImporter == null)
                        continue;
                    if (textureImporter.textureType == TextureImporterType.Sprite)
                    {
                        List<string> sDict = null;
                        msgDict.TryGetValue(ResLibaryTool.ExistType[LibaryTypeEnum.LibaryType_Sprite], out sDict);
                        UnityEngine.Object[] sprs = AssetDatabase.LoadAllAssetsAtPath(resfile);
                        for (int k = 0; k < sprs.Length; k++)
                        {

                            if (sprs[k].GetType().Name == ResLibaryTool.ExistType[LibaryTypeEnum.LibaryType_Sprite])
                            {
                                if (sDict != null && sDict.Contains(sprs[k].name))
                                {
                                    continue;
                                }
                                resourseSetting.AddResToLibary(sprs[k]);
                            }
                            else
                            {
                                List<string> sDictT = null;
                                msgDict.TryGetValue(ResLibaryTool.ExistType[LibaryTypeEnum.LibaryType_Texture2D], out sDictT);
                                if (sDictT != null && sDictT.Contains(sprs[k].name))
                                {
                                    continue;
                                }                               
                                resourseSetting.AddResToLibary(sprs[k]);
                            }
                        }
                    }
                    else
                    {
                        List<string> sDict = null;
                        msgDict.TryGetValue(ResLibaryTool.ExistType[LibaryTypeEnum.LibaryType_Texture2D], out sDict);
                        if (sDict != null && sDict.Contains(obj.name))
                        {
                            continue;
                        }
                        resourseSetting.AddResToLibary(obj);
                    }
                }
                else
                {
                    List<string> sDict = null;
                    msgDict.TryGetValue(obj.GetType().Name, out sDict);
                    if (sDict != null && sDict.Contains(obj.name))
                    {
                        continue;
                    }
                    resourseSetting.AddResToLibary(obj);
                }
            }
        }
        AssetDatabase.ImportAsset(path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}

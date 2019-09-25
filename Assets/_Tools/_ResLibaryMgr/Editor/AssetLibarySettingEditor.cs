using ResLibary;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Video;

[CustomEditor(typeof(LibaryAssetSetting))]
public class AssetLibarySettingEditor : Editor
{    
    private LibaryAssetSetting resLibaryAsset;
    private string[] showPopup = new string[]{"Hide","Show"};

    private int modeIndex = 0;
    private string[] modePopup = new string[]{"DisplayMode","EditorMode"};
    private Dictionary<string,int> showDict = new Dictionary<string, int>();
    private void OnEnable()
    {
        resLibaryAsset = target as LibaryAssetSetting;
    }

    public override void OnInspectorGUI()
    {
//        modeIndex = EditorGUILayout.Popup(modeIndex, modePopup);
//        EditorGUILayout.Space();
        OnInspectorObjectGUI<Texture2D>(resLibaryAsset.texture2ds, "texturn2ds");
        EditorGUILayout.Space();
        OnInspectorObjectGUI<RenderTexture>(resLibaryAsset.renderTextures, "renderTextures");
        EditorGUILayout.Space();
        OnInspectorObjectGUI<MovieTexture>(resLibaryAsset.movieTextures, "movieTextures");
        EditorGUILayout.Space();
        OnInspectorObjectGUI<Sprite>(resLibaryAsset.sprites, "sprites");
        EditorGUILayout.Space();
        OnInspectorObjectGUI<TextAsset>(resLibaryAsset.textAssets, "textAssets");
        EditorGUILayout.Space();
        OnInspectorObjectGUI<Material>(resLibaryAsset.materials, "materials");
        EditorGUILayout.Space();
        OnInspectorObjectGUI<GameObject>(resLibaryAsset.prefabs, "prefabs");
        EditorGUILayout.Space();
        OnInspectorObjectGUI<AudioClip>(resLibaryAsset.audios, "audios");
        EditorGUILayout.Space();
        OnInspectorObjectGUI<VideoClip>(resLibaryAsset.videos, "videos");
        OnGUIChanged();
        EditorGUILayout.Space();
        if (GUILayout.Button("Update"))
        {
            UpdateAsset();
        }
        EditorGUILayout.Space();
        if (GUILayout.Button("Save"))
        {
            EditorUtility.SetDirty(target);
            serializedObject.ApplyModifiedProperties();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
    private void UpdateAsset()
    {
        AssetLibarySettingWindows myWindow = (AssetLibarySettingWindows)EditorWindow.GetWindow(typeof(AssetLibarySettingWindows), false, "AssetSetting", true);//创建窗口
        myWindow.InitAssetLibarySettingWindows(resLibaryAsset);//展示
    }

    private void OnInspectorObjectGUI<T>(List<T> list, string label) where T : UnityEngine.Object
    {

        EditorGUILayout.BeginHorizontal();
        if (!showDict.ContainsKey(label))
            showDict[label] = 0;
        float wid = EditorGUIUtility.currentViewWidth;
        EditorGUILayout.LabelField(label,GUILayout.MaxWidth(wid/4));
        string CountInfor = "0";
        if (list != null)
            CountInfor = list.Count.ToString();
        EditorGUILayout.LabelField("Count:"+ CountInfor, GUILayout.MaxWidth(wid/4));
        int isShow = showDict[label];
        isShow = EditorGUILayout.Popup(isShow, showPopup,GUILayout.MaxWidth(wid / 4));
        showDict[label] = isShow;                  
        if (GUILayout.Button("Clear",GUILayout.MaxWidth(wid / 4)))
        {
            if(list!=null)
                list.Clear();
        }
        EditorGUILayout.EndHorizontal();
        if (isShow == 0)
            return;
        if (list == null)
            return;
        T[] ts = list.ToArray();
        for (int i = 0; i < ts.Length; i++)
        {
            bool _break = false;
            if (ts.Length == list.Count)
            {              
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(ts[i] == null ? "" : ts[i].name, GUILayout.MinWidth(10));
                list[i] = (T)EditorGUILayout.ObjectField(ts[i], typeof(T), false);
                if (GUILayout.Button("-", GUILayout.Width(50)))
                {
                    list.RemoveAt(i);
                    _break = true;
                }
                
                EditorGUILayout.EndHorizontal();
            }

            if (_break)
                break;
        }

        if (modeIndex == 1)
        {
            if (GUILayout.Button("+"))
            {
                list.Add(null);
            }
        }
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

    //[MenuItem("Assets/InsertAssetLibary")]
    static void ExportResource()
    {
        LibaryAssetSetting _assetSetting = Resources.Load<LibaryAssetSetting>("AssetLibarySetting");
        if (_assetSetting == null)
            return;
        string path = AssetDatabase.GetAssetPath(_assetSetting);
        LibaryAssetSetting assetSetting = AssetDatabase.LoadAssetAtPath<LibaryAssetSetting>(path);
        if (assetSetting == null)
            return;
        // 选择的要保存的对象  
        Object[] selection = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);
        foreach (Object item in selection)
        {
            if (item == null || item.GetType() == typeof(DefaultAsset))
                continue;
            if (ResLibaryTool.ExistType.ContainsValue(item.GetType().Name))
            {
                if (item.GetType().Name == ResLibaryTool.ExistType[LibaryTypeEnum.LibaryType_Texture2D])
                {
                    string tex2dpath = AssetDatabase.GetAssetPath(item);
                    TextureImporter textureImporter = TextureImporter.GetAtPath(tex2dpath) as TextureImporter;
                    //SpriteMetaData[] sprs = textureImporter.spritesheet;
                    if (textureImporter.textureType == TextureImporterType.Sprite)
                    {
                        Object[] sprs = AssetDatabase.LoadAllAssetsAtPath(tex2dpath);
                        for (int i = 0; i < sprs.Length; i++)
                        {
                            //Sprite spr = AssetDatabase.LoadAssetAtPath<Sprite>(textPath + "/"+ sprs[i].name);
                            Object spr = sprs[i];
                            if (spr != null && spr.GetType().Name == ResLibaryTool.ExistType[LibaryTypeEnum.LibaryType_Sprite])
                                assetSetting.AddResToLibary((Sprite)spr);
                        }
                        if (textureImporter.spriteImportMode != SpriteImportMode.Multiple)
                        {
                            assetSetting.AddResToLibary(item);
                        }
                    }
                }
                else
                {
                    assetSetting.AddResToLibary(item);
                }
            }
        }
        EditorUtility.SetDirty(assetSetting);
        AssetDatabase.ImportAsset(path);
        
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    [MenuItem("Assets/RemoveAssetLibary")]
    static void RemoveResource()
    {
        LibaryAssetSetting _assetSetting = Resources.Load<LibaryAssetSetting>("AssetLibarySetting");
        if (_assetSetting == null)
            return;
        string path = AssetDatabase.GetAssetPath(_assetSetting);
        LibaryAssetSetting assetSetting = AssetDatabase.LoadAssetAtPath<LibaryAssetSetting>(path);
        if (assetSetting == null)
            return;
        // 选择的要保存的对象 
        Dictionary<string, List<string>> dictMsg = assetSetting.GetAssetDict();
        Object[] selection = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);
        foreach (Object item in selection)
        {
            if (item == null || item.GetType() == typeof(DefaultAsset))
                continue;
            if (ResLibaryTool.ExistType.ContainsValue(item.GetType().Name))
            {
                if (item.GetType() == typeof(Texture2D))
                {
                    string tex2dpath = AssetDatabase.GetAssetPath(item);
                    TextureImporter textureImporter = TextureImporter.GetAtPath(tex2dpath) as TextureImporter;
                    if (textureImporter.textureType == TextureImporterType.Sprite)
                    {
                        Object[] sprs = AssetDatabase.LoadAllAssetsAtPath(tex2dpath);
                        for (int i = 0; i < sprs.Length; i++)
                        {
                            Object spr = sprs[i];

                            if (spr != null && spr.GetType() == typeof(Sprite))
                            {
                                List<string> sprslist;
                                dictMsg.TryGetValue(spr.GetType().Name,out sprslist);
                                if (sprslist != null && sprslist.Contains(spr.name))
                                {
                                    int dindex = sprslist.FindLastIndex(x=>x==spr.name);
                                    assetSetting.DelResToLibary(spr.GetType().Name,dindex);
                                }
                                
                            }
                        }

                        if (textureImporter.spriteImportMode != SpriteImportMode.Multiple)
                        {
                            List<string> sprslist;
                            dictMsg.TryGetValue(item.GetType().Name,out sprslist);
                            if (sprslist != null && sprslist.Contains(item.name))
                            {
                                int dindex = sprslist.FindLastIndex(x=>x==item.name);
                                assetSetting.DelResToLibary(item.GetType().Name,dindex);
                            }
                        }                     
                     }
                }
             }
            else
            {
                List<string> sprslist;
                dictMsg.TryGetValue(item.GetType().Name,out sprslist);
                if (sprslist != null && sprslist.Contains(item.name))
                {
                    int dindex = sprslist.FindLastIndex(x=>x==item.name);
                    assetSetting.DelResToLibary(item.GetType().Name,dindex);
                }
               
            }
        }
        AssetDatabase.ImportAsset(path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
       
}
     
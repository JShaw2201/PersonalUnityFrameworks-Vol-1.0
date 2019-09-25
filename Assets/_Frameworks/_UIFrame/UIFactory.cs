using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;

namespace JXFrame.View
{
    public delegate object LoadAction(string assetName);

    public class UIFactory : MonoSingleton<UIFactory>
    {
        public static bool configAssstBundle = false;
        public static string configUrl = "UIPrefabConfig";
        public static string configName = "UIPrefabConfig";
        public static string configDirType = "Resources";/*Resources,streamingAssetsPath,dataPath,persistentDataPath**/

        public static LoadAction LoadString;
        public static LoadAction LoadPrefab;

        private static Dictionary<string, UIPrefabConfigNode> m_PrefabsDict;
        private static Dictionary<string, string> AndroidStreamingAssetsPathText;
        private static Dictionary<string, GameObject> AndroidStreamingAssetsPathPrefab;
        public override void OnInit() { OnLoad(); }
        public static void OnLoad()
        {
            if (m_PrefabsDict == null)
                m_PrefabsDict = new Dictionary<string, UIPrefabConfigNode>();
            if (AndroidStreamingAssetsPathText == null)
                AndroidStreamingAssetsPathText = new Dictionary<string, string>();
            if (AndroidStreamingAssetsPathPrefab == null)
                AndroidStreamingAssetsPathPrefab = new Dictionary<string, GameObject>();
            System.Action<string> v1 = (string str) =>
            {
                if (string.IsNullOrEmpty(str))
                {
                    Debug.LogError("config is null!");
                    return;
                }
                UIPrefabConfigInfo info = JsonUtility.FromJson<UIPrefabConfigInfo>(str);
                for (int i = 0; i < info.UIPrefabInfo.Count; i++)
                {
                    m_PrefabsDict[info.UIPrefabInfo[i].UIFormName] = info.UIPrefabInfo[i];
#if UNITY_ANDROID && !UNITY_EDITOR
                   LoadAndroidSteamingAsset(info.UIPrefabInfo[i]);
#endif
                }
            };

            if (LoadString != null)
            {
                v1((string)LoadString(configName));
                return;
            }
            if (configDirType == "Resources")
            {
                TextAsset uiConfig = Resources.Load<TextAsset>(configUrl);
                if (uiConfig != null)
                    v1(uiConfig.text);
            }
            else
            {
                if (configDirType == "streamingAssetsPath")
                {
#if UNITY_ANDROID && !UNITY_EDITOR
                   //android平台的streamingAssetsPath路径
                    if (configAssstBundle)
                    {
                        Instance.StartCoroutine(WWWLoadAssetBundle(GetPath(configDirType, configUrl), (bundle) => 
                        {
                            TextAsset text = LoadAssetBundleObj<TextAsset>(bundle, configName);
                             v1(text == null ? null : text.text);
                        }));
                    }
                    else
                    {
                        Instance.StartCoroutine(WWWLoadFileStr(GetPath(configDirType, configUrl), (text) => 
                        {
                            v1(text);
                        }));
                    }
#else
                    //非android平台的streamingAssetsPath路径
                    if (configAssstBundle)
                    {
                        TextAsset text = LoadAssetBundleObj<TextAsset>(GetPath(configDirType, configUrl), configName);
                        v1(text == null ? null : text.text);
                    }
                    else
                        v1(IOLoadFileStr(GetPath(configDirType, configUrl)));
#endif
                }
                else
                {
                    if (configAssstBundle)
                    {
                        TextAsset text = LoadAssetBundleObj<TextAsset>(GetPath(configDirType, configUrl), configName);
                        v1(text == null ? null : text.text);
                    }
                    else
                        v1(IOLoadFileStr(GetPath(configDirType, configUrl)));
                }

            }
        }

        //将streamingAssetsPath路径下的资源加载到缓存
        private static void LoadAndroidSteamingAsset(UIPrefabConfigNode uiPrefabInfoNode)
        {
            //加载预设
            if (uiPrefabInfoNode.UIFormPrefabDirType == "streamingAssetsPath")
            {
                Instance.StartCoroutine(WWWLoadAssetBundle(GetPath(uiPrefabInfoNode.UIFormPrefabDirType, uiPrefabInfoNode.UIFormPrefabUrl), (bundle) =>
                 {
                     if (bundle != null)
                     {
                         GameObject prefab = bundle.LoadAsset<GameObject>(uiPrefabInfoNode.UIFormPrefabName);
                         AndroidStreamingAssetsPathPrefab[uiPrefabInfoNode.UIFormName] = prefab;
                     }
                 }));
            }
            //加载Lua脚本
            if (uiPrefabInfoNode.UIFormLuaScripDirType == "streamingAssetsPath")
            {
                if (uiPrefabInfoNode.UIFormLuaScriptAssetBudle)
                {
                    Instance.StartCoroutine(WWWLoadAssetBundle(GetPath(uiPrefabInfoNode.UIFormLuaScripDirType, uiPrefabInfoNode.UIFormLuaScriptUrl), (bundle) =>
                    {
                        if (bundle != null)
                        {
                            TextAsset text = bundle.LoadAsset<TextAsset>(uiPrefabInfoNode.UIFormClassName);
                            AndroidStreamingAssetsPathText[uiPrefabInfoNode.UIFormClassName] = text == null ? null : text.text;
                        }
                    }));
                }
                else
                {
                    Instance.StartCoroutine(WWWLoadFileStr(GetPath(uiPrefabInfoNode.UIFormLuaScripDirType, uiPrefabInfoNode.UIFormLuaScriptUrl), (text) =>
                    {
                        AndroidStreamingAssetsPathText[uiPrefabInfoNode.UIFormClassName] = text;
                    }));
                }
            }
        }

        /// <summary>
        /// 添加lua
        /// </summary>
        /// <param name="formName"></param>
        /// <param name="luaStr"></param>
        public static void AddUIPrefabNode(UIPrefabConfigNode node)
        {
            if (!m_PrefabsDict.ContainsKey(node.UIFormName))
            {
                m_PrefabsDict[node.UIFormName] = node;
#if UNITY_ANDROID && !UNITY_EDITOR
                LoadAndroidSteamingAsset(node);
#endif
            }
        }

        public override void OnDispose()
        {
            m_PrefabsDict = null;
            AndroidStreamingAssetsPathText = null;
            AndroidStreamingAssetsPathPrefab = null;
        }

        public static GameObject LoadUIMask()
        {
            GameObject clone = Resources.Load<GameObject>("UIPrefabs/UIMaskPanel");
            if (clone == null)
                return null;
            GameObject go = GameObject.Instantiate(clone);
            go.name = "UIMaskPanel";
            return go;
        }

        /// <summary>
        /// 获取android平台下streamingAsset路径下的lua脚本缓存
        /// </summary>
        /// <param name="uiFormName"></param>
        /// <returns></returns>
        public static string GetAndoidStreamingAssetLuaScripts(string uiFormName)
        {
            string str = null;
            AndroidStreamingAssetsPathText.TryGetValue(uiFormName, out str);
            return str;
        }

        /// <summary>
        /// 获取预设
        /// </summary>
        /// <param name="uiFormName"></param>
        /// <returns></returns>
        public static GameObject LoadUIPrefab(string uiFormName)
        {
            UIPrefabConfigNode uiPrefabInfoNode = null;
            m_PrefabsDict.TryGetValue(uiFormName, out uiPrefabInfoNode);
            if (uiPrefabInfoNode == null)
            {
                Debug.LogError(uiFormName + ":UIFormName is null!");
                return null;
            }

            GameObject prefab = null;
            if (LoadPrefab != null)
            {
                prefab = (GameObject)LoadPrefab(uiPrefabInfoNode.UIFormPrefabName);
            }
            else
            {
                if (uiPrefabInfoNode.UIFormPrefabDirType == "Resources")
                    prefab = Resources.Load<GameObject>(uiPrefabInfoNode.UIFormPrefabUrl);
                else if (uiPrefabInfoNode.UIFormPrefabDirType == "streamingAssetsPath")
                {
#if UNITY_ANDROID && !UNITY_EDITOR
                AndroidStreamingAssetsPathPrefab.TryGetValue(uiPrefabInfoNode.UIFormName,out prefab);
#else
                    prefab = LoadAssetBundleObj<GameObject>(GetPath(uiPrefabInfoNode.UIFormPrefabDirType, uiPrefabInfoNode.UIFormPrefabUrl), uiPrefabInfoNode.UIFormPrefabName);
#endif
                }
            }
            if (prefab == null)
            {
                Debug.LogError("prefab is null!" + uiPrefabInfoNode.UIFormPrefabUrl);
                return null;
            }

            GameObject go = GameObject.Instantiate(prefab);
            go.name = prefab.name;

            Component uibase = go.GetComponent<UIBase>();

            if (uibase == null)
            {
                System.Type monoType = null;
#if HOTFIX_ENABLE
                if (!uiPrefabInfoNode.UIFormLuaScript)
                    monoType = System.Type.GetType(uiPrefabInfoNode.UIFormClassName);
                else
                    monoType = System.Type.GetType("JXFrame.View.LuaUIBehavior");
#else
                monoType = System.Type.GetType(uiPrefabInfoNode.UIFormClassName);
#endif
                if (monoType == null)
                {
                    Debug.LogError("monoType is null!:" + uiPrefabInfoNode.UIFormLuaScript);
                    return null;
                }
                uibase = go.AddComponent(monoType);
#if HOTFIX_ENABLE
                if (uibase is LuaUIBehavior)
                {
                    LuaUIBehavior luaui = uibase as LuaUIBehavior;
                    luaui.LoadLuaString(uiPrefabInfoNode.UIFormLuaScriptAssetBudle, uiPrefabInfoNode.UIFormLuaScripDirType, uiPrefabInfoNode.UIFormClassName, uiPrefabInfoNode.UIFormLuaScriptUrl);
                }
#endif
            }
            return uibase.gameObject;
        }

        public static IHandleUIManager CreateInternalUIManager()
        {
            return new InternalUIManager();
        }

        private static T LoadAssetBundleObj<T>(AssetBundle bundle, string resName) where T : Object
        {
            if (bundle != null)
            {
                return bundle.LoadAsset<T>(resName);
            }
            return null;
        }

        public static T LoadAssetBundleObj<T>(string bundlePath, string resName) where T : Object
        {
            AssetBundle bundle = AssetBundle.LoadFromFile(bundlePath);
            return LoadAssetBundleObj<T>(bundle, resName);
        }

        public static string GetPath(string DirType, string url)
        {
            string fpath = url;
            switch (DirType)
            {
                case "streamingAssetsPath":
                    fpath = string.Format("{0}/{1}", Application.streamingAssetsPath, url);
                    break;
                case "dataPath":
                    fpath = string.Format("{0}/{1}", Application.dataPath, url);
                    break;
                case "persistentDataPath":
                    fpath = string.Format("{0}/{1}", Application.persistentDataPath, url);
                    break;
            }
            return fpath;
        }

        /// <summary>
        /// IO方式加载
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string IOLoadFileStr(string path)
        {
            string str = "";
            if (!File.Exists(path))
            {
                return null;
            }
            StreamReader sr = new StreamReader(path, System.Text.Encoding.GetEncoding("gb2312"));
            str = sr.ReadToEnd();
            return str;
        }

        /// <summary>
        /// WWW方式加载
        /// </summary>
        /// <param name="path"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public static IEnumerator WWWLoadFileStr(string path, System.Action<string> callback)
        {
            WWW www = new WWW(path);
            string id = Time.time.ToString() + Random.Range(1, 100).ToString();
            yield return www;
            if (www.isDone && !string.IsNullOrEmpty(www.error))
            {
                if (callback != null) callback(www.text);
            }
            else
            {
                if (callback != null) callback(null);
            }
        }

        /// <summary>
        /// WWW方式加载assetbundle
        /// </summary>
        /// <param name="path"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public static IEnumerator WWWLoadAssetBundle(string path, System.Action<AssetBundle> callback)
        {
            WWW www = new WWW(path);
            string id = Time.time.ToString() + Random.Range(1, 100).ToString();
            yield return www;
            if (www.isDone && !string.IsNullOrEmpty(www.error))
            {
                if (callback != null) callback(www.assetBundle);
            }
            else
            {
                if (callback != null) callback(null);
            }
        }

    }





}
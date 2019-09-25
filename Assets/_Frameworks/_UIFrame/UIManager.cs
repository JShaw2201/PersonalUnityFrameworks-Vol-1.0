using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System;
using System.Linq;

namespace JXFrame.View
{
    public delegate object LoadAction(string assetName);

    public class UIFactory : MonoBehaviour
    {
        public static bool UNITY_Android;
        public static bool UNITY_Editor;

        private static volatile UIFactory instance;
        private static object syncRoot = new object();
        private static bool _applicationIsQuitting = false;
        private static GameObject singletonObj = null;
        public static UIFactory Instance
        {
            get
            {
                if (_applicationIsQuitting)
                {
                    return null;
                }
                if (instance == null)
                {

                    lock (syncRoot)
                    {
                        if (instance == null)
                        {
                            UIFactory[] instance1 = FindObjectsOfType<UIFactory>();
                            if (instance1 != null)
                            {
                                for (var i = 0; i < instance1.Length; i++)
                                {
                                    Destroy(instance1[i].gameObject);
                                }
                            }
                        }
                    }

                    GameObject go = new GameObject(typeof(UIFactory).FullName);
                    singletonObj = go;
                    instance = go.AddComponent<UIFactory>();
                    instance.OnInit();
                    DontDestroyOnLoad(go);
                    _applicationIsQuitting = false;
                }
                return instance;
            }

        }

        private void Awake()
        {

            UIFactory t = gameObject.GetComponent<UIFactory>();
            if (singletonObj == null)
            {
                singletonObj = gameObject;
                DontDestroyOnLoad(gameObject);
                singletonObj.name = typeof(UIFactory).FullName;
                instance = t;
                OnInit();
                _applicationIsQuitting = false;
            }
            else if (singletonObj != gameObject)
            {
                MonoBehaviour[] monos = gameObject.GetComponents<MonoBehaviour>();
                if (monos.Length > 1)
                {
                    Destroy(t);
                }
                else
                {
                    Destroy(gameObject);
                }
            }
        }

        public static bool configAssstBundle = false;
        public static string configUrl = "UIPrefabConfig";
        public static string configName = "UIPrefabConfig";
        public static string configDirType = "Resources";/*Resources,streamingAssetsPath,dataPath,persistentDataPath**/

        public static LoadAction LoadString;
        public static LoadAction LoadPrefab;


        private static Dictionary<string, UIPrefabConfigNode> m_PrefabsDict;
        private static Dictionary<string, string> AndroidStreamingAssetsPathText;
        private static Dictionary<string, GameObject> AndroidStreamingAssetsPathPrefab;
        private static Dictionary<string, System.Type> _uibaseTypes;
        private static Dictionary<string, System.Type> uibaseTypes
        {
            get
            {
                InitUIBaseType();
                return _uibaseTypes;
            }
        }
        private static void InitUIBaseType()
        {
            if (_uibaseTypes == null)
            {
                _uibaseTypes = new Dictionary<string, System.Type>();
                var types = AppDomain.CurrentDomain.GetAssemblies()
                  .SelectMany(a => a.GetTypes()).ToArray();
                for (int i = 0; i < types.Length; i++)
                {
                    System.Type v = types[i];// System.Type.GetType(autoRegistorList[i]);
                    if (v.IsSubclassOf(typeof(UIBase)))
                        _uibaseTypes[v.FullName] = v;
                }
            }
        }
        public void OnInit() { OnLoad(); }
        public static void OnLoad()
        {
            InitUIBaseType();
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
                    if (UNITY_Android && !UNITY_Editor)
                        LoadAndroidSteamingAsset(info.UIPrefabInfo[i]);
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
                    if (UNITY_Android && !UNITY_Editor)
                    {
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
                    }
                    else
                    {
                        //非android平台的streamingAssetsPath路径
                        if (configAssstBundle)
                        {
                            TextAsset text = LoadAssetBundleObj<TextAsset>(GetPath(configDirType, configUrl), configName);
                            v1(text == null ? null : text.text);
                        }
                        else
                            v1(IOLoadFileStr(GetPath(configDirType, configUrl)));
                    }
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
                if (UNITY_Android && !UNITY_Editor)
                    LoadAndroidSteamingAsset(node);
            }
        }

        public void OnDispose()
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
                    if (UNITY_Android && !UNITY_Editor)
                        AndroidStreamingAssetsPathPrefab.TryGetValue(uiPrefabInfoNode.UIFormName, out prefab);
                    else
                        prefab = LoadAssetBundleObj<GameObject>(GetPath(uiPrefabInfoNode.UIFormPrefabDirType, uiPrefabInfoNode.UIFormPrefabUrl), uiPrefabInfoNode.UIFormPrefabName);

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
                if (!uiPrefabInfoNode.UIFormLuaScript)
                {
                    uibaseTypes.TryGetValue(uiPrefabInfoNode.UIFormClassName, out monoType);
                    //monoType = System.Type.GetType(uiPrefabInfoNode.UIFormClassName);
                }
                else
                {
                    uibaseTypes.TryGetValue("JXFrame.View.LuaUIBehavior", out monoType);
                    //monoType = System.Type.GetType("JXFrame.View.LuaUIBehavior");
                }
                if (monoType == null)
                {
                    Debug.LogError("monoType is null!:" + uiPrefabInfoNode.UIFormLuaScript);
                    return null;
                }
                uibase = go.AddComponent(monoType);
                if (uibase.GetType().Name == "LuaUIBehavior")
                {
                    System.Reflection.MethodInfo mi = uibase.GetType().GetMethod("LoadLuaString", System.Reflection.BindingFlags.Public);
                    if (mi != null)
                    {
                        mi.Invoke(uibase, new System.Object[]
                        {
                        uiPrefabInfoNode.UIFormLuaScriptAssetBudle,
                        uiPrefabInfoNode.UIFormLuaScripDirType,
                        uiPrefabInfoNode.UIFormClassName,
                        uiPrefabInfoNode.UIFormLuaScriptUrl
                        });
                    }
                    else
                    {
                        Debug.LogError("LoadLuaString(bool isAssetBundle, string dirType, string scriptName, string scriptPath) is null");
                    }
                    //LuaUIBehavior luaui = uibase as LuaUIBehavior;
                    //luaui.LoadLuaString(uiPrefabInfoNode.UIFormLuaScriptAssetBudle, uiPrefabInfoNode.UIFormLuaScripDirType, uiPrefabInfoNode.UIFormClassName, uiPrefabInfoNode.UIFormLuaScriptUrl);
                }
            }
            return uibase.gameObject;
        }

        public static IHandleUIManager CreateInternalUIManager()
        {
            return new InternalUIManager();
        }

        private static T LoadAssetBundleObj<T>(AssetBundle bundle, string resName) where T : UnityEngine.Object
        {
            if (bundle != null)
            {
                return bundle.LoadAsset<T>(resName);
            }
            return null;
        }

        public static T LoadAssetBundleObj<T>(string bundlePath, string resName) where T : UnityEngine.Object
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
            string id = Time.time.ToString() + UnityEngine.Random.Range(1, 100).ToString();
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
            string id = Time.time.ToString() + UnityEngine.Random.Range(1, 100).ToString();
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
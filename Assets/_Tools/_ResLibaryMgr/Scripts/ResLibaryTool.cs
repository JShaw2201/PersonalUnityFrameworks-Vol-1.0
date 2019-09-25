using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace ResLibary
{
    public class ResLibaryTool :MonoBehaviour
    {
        private static volatile ResLibaryTool instance;
        private static object syncRoot = new object();
        private static bool _applicationIsQuitting = false;
        private static GameObject singletonObj = null;
        public static ResLibaryTool Instance
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
                            ResLibaryTool[] instance1 = FindObjectsOfType<ResLibaryTool>();
                            if (instance1 != null)
                            {
                                for (var i = 0; i < instance1.Length; i++)
                                {
                                    Destroy(instance1[i].gameObject);
                                }
                            }
                        }
                    }

                    GameObject go = new GameObject(typeof(ResLibaryTool).FullName);
                    singletonObj = go;
                    instance = go.AddComponent<ResLibaryTool>();
                    instance.OnInit();
                    DontDestroyOnLoad(go);
                    _applicationIsQuitting = false;
                }
                return instance;
            }

        }

        private void Awake()
        {

            ResLibaryTool t = gameObject.GetComponent<ResLibaryTool>();
            if (singletonObj == null)
            {
                singletonObj = gameObject;
                DontDestroyOnLoad(gameObject);
                singletonObj.name = typeof(ResLibaryTool).FullName;
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

        // 支持的资源文件格式
        public static readonly List<string> ResourceExts = new List<string>
    {                        ".prefab", ".fbx",
                             ".png", ".jpg", ".dds", ".gif", ".psd", ".tga", ".bmp",
                             ".txt", ".bytes", ".xml", ".csv", ".json",
                             ".controller", ".shader", ".anim", ".unity", ".mat",
                             ".wav", ".mp3", ".ogg",".mp4",
                             ".ttf",
                             ".shadervariants", ".asset"
    };

        // 支持的资源文件格式
        public static readonly List<string> ResourceImgExts = new List<string> {
        ".png", ".jpg", ".dds", ".gif", ".psd", ".tga", ".bmp"};
        public static readonly List<string> ResourceTxtExts = new List<string> {
        ".txt", ".bytes", ".xml", ".csv", ".json"};
        public static readonly List<string> ResourceAudioExts = new List<string> {
        ".wav", ".mp3"};

        public static readonly List<string> ResourceVideoExts = new List<string> {
        ".mp4",".avi",".ogg"};



        public static readonly Dictionary<LibaryTypeEnum, string> ExistType = new Dictionary<LibaryTypeEnum, string>()
    {
        {LibaryTypeEnum.LibaryType_Texture2D,"Texture2D"},
        {LibaryTypeEnum.LibaryType_RenderTexture,"RenderTexture"},
        {LibaryTypeEnum.LibaryType_Sprite,"Sprite"      },
        {LibaryTypeEnum.LibaryType_TextAsset,"TextAsset"    },
        {LibaryTypeEnum.LibaryType_Material,"Material"    },
        {LibaryTypeEnum.LibaryType_GameObject,"GameObject"   },
        {LibaryTypeEnum.LibaryType_AudioClip,"AudioClip"    },
        {LibaryTypeEnum.LibaryType_VideoClip,"VideoClip"    },
        {LibaryTypeEnum.LibaryType_MovieTexture,"MovieTexture"  }
    };

        public static readonly Dictionary<string, LibaryTypeEnum> ExistTypeNameToEnum = new Dictionary<string, LibaryTypeEnum>()
    {
        {"Texture2D"    ,LibaryTypeEnum.LibaryType_Texture2D    },
        {"RenderTexture",LibaryTypeEnum.LibaryType_RenderTexture},
        {"Sprite"       ,LibaryTypeEnum.LibaryType_Sprite       },
        {"TextAsset"    ,LibaryTypeEnum.LibaryType_TextAsset   },
        {"Material"     ,LibaryTypeEnum.LibaryType_Material     },
        {"GameObject"   ,LibaryTypeEnum.LibaryType_GameObject   },
        {"AudioClip"    ,LibaryTypeEnum.LibaryType_AudioClip    },
        {"VideoClip"    ,LibaryTypeEnum.LibaryType_VideoClip    },
        {"MovieTexture" ,LibaryTypeEnum.LibaryType_MovieTexture }
    };

        public static readonly Dictionary<string, Type> ExistTypeNameToType = new Dictionary<string, Type>()
    {
        {"Texture2D",typeof(Texture2D)},
        {"RenderTexture",typeof(RenderTexture)},
        {"Sprite",typeof(Sprite)},
        {"TextAsset",typeof(TextAsset)},
        {"Material",typeof(Material)},
        {"GameObject",typeof(GameObject)},
        {"AudioClip",typeof(AudioClip)},
        {"VideoClip",typeof(UnityEngine.Video.VideoClip)},
        {"MovieTexture",typeof(MovieTexture)}
    };

        public static string LoadFileStr(string path)
        {
            string str = "";
            if (!File.Exists(path))
                return null;
            StreamReader sr = new StreamReader(path);
            str = sr.ReadToEnd();
            if (string.IsNullOrEmpty(str))
                return null;
            return str;
        }

        public static Texture2D readLocalTexture2d(string filePath)
        {
            if (!File.Exists(filePath))
                return null;
            //读取文件
            FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            int byteLength = (int)fs.Length;
            byte[] imgBytes = new byte[byteLength];
            fs.Read(imgBytes, 0, byteLength);
            fs.Close();
            fs.Dispose();
            //转化为Texture2D
            Texture2D t2d = new Texture2D(1024, 1024);
            t2d.LoadImage(imgBytes);
            t2d.Apply();
            return t2d;
        }

        public static List<string> GetAllLocalResourceDirs(string rootPath)
        {
            if (string.IsNullOrEmpty(rootPath))
                return null;
            string fullRootPath = System.IO.Path.GetFullPath(rootPath);
            if (string.IsNullOrEmpty(fullRootPath))
                return null;

            string[] dirs = System.IO.Directory.GetDirectories(fullRootPath);
            if ((dirs == null) || (dirs.Length <= 0))
                return null;
            List<string> ret = new List<string>();

            for (int i = 0; i < dirs.Length; ++i)
            {
                DirectoryInfo dir = new DirectoryInfo(dirs[i]);
                if (dir.Name == "Resources")
                {
                    ret.Add(dir.FullName);

                    continue;
                }
                List<string> list = GetAllLocalResourceDirs(dir.FullName);
                if (list != null)
                    ret.AddRange(list);

            }
            return ret;
        }

        public static List<string> GetAllLocalStreamingAssetsDirs(string rootPath, bool FoundDir = false)
        {
            if (string.IsNullOrEmpty(rootPath))
                return null;
            string fullRootPath = System.IO.Path.GetFullPath(rootPath);
            if (string.IsNullOrEmpty(fullRootPath))
                return null;

            string[] dirs = System.IO.Directory.GetDirectories(fullRootPath);
            if ((dirs == null) || (dirs.Length <= 0))
                return null;
            List<string> ret = new List<string>();

            for (int i = 0; i < dirs.Length; ++i)
            {
                DirectoryInfo dir = new DirectoryInfo(dirs[i]);
                if (dir.Name == "StreamingAssets" && !FoundDir)
                {
                    ret.Add(dir.FullName);
                    List<string> list = GetAllLocalStreamingAssetsDirs(dir.FullName, true);
                    if (list != null)
                        ret.AddRange(list);
                    continue;
                }
                if (FoundDir)
                {
                    ret.Add(dir.FullName);
                    List<string> list = GetAllLocalStreamingAssetsDirs(dir.FullName, true);
                    if (list != null)
                        ret.AddRange(list);
                }

            }
            return ret;
        }

        public static List<string> GetAllSubDirs(string rootPath)
        {
            if (string.IsNullOrEmpty(rootPath))
                return null;
            string fullRootPath = System.IO.Path.GetFullPath(rootPath);
            if (string.IsNullOrEmpty(fullRootPath))
                return null;
            string[] dirs = System.IO.Directory.GetDirectories(fullRootPath);
            if ((dirs == null) || (dirs.Length <= 0))
                return null;
            List<string> ret = new List<string>();

            for (int i = 0; i < dirs.Length; ++i)
            {
                string dir = dirs[i];
                ret.Add(dir);
            }
            for (int i = 0; i < dirs.Length; ++i)
            {
                string dir = dirs[i];
                List<string> list = GetAllSubDirs(dir);
                if (list != null)
                    ret.AddRange(list);
            }

            return ret;
        }
        // 获得根据Assets目录的局部目录
        public static string GetLocalPath(string path)
        {
            return GetAssetRelativePath(path);
        }
        public static string GetAssetRelativePath(string fullPath)
        {
            if (string.IsNullOrEmpty(fullPath))
                return string.Empty;
            fullPath = fullPath.Replace("\\", "/");
            int index = fullPath.IndexOf("Assets/", StringComparison.CurrentCultureIgnoreCase);
            if (index < 0)
                return fullPath;
            string ret = fullPath.Substring(index);
            return ret;
        }

        // 根据目录判断是否有资源文件
        public static bool DirExistResource(string path)
        {
            if (string.IsNullOrEmpty(path))
                return false;
            string fullPath = Path.GetFullPath(path);
            if (string.IsNullOrEmpty(fullPath))
                return false;

            string[] files = System.IO.Directory.GetFiles(fullPath);
            if ((files == null) || (files.Length <= 0))
                return false;
            for (int i = 0; i < files.Length; ++i)
            {
                string ext = System.IO.Path.GetExtension(files[i]);
                if (string.IsNullOrEmpty(ext))
                    continue;
                for (int j = 0; j < ResourceExts.Count; ++j)
                {
                    if (string.Compare(ext, ResourceExts[j], true) == 0)
                    {
                        if ((ResourceExts[j] == ".fbx"))
                        {
                            // ingore xxx@idle.fbx
                            string name = Path.GetFileNameWithoutExtension(files[i]);
                            if (name.IndexOf('@') >= 0)
                                return false;
                        }
                        //else if (ResourceExts[j] == ".unity")
                        //{
                        //    if (!IsVaildSceneResource(files[i]))
                        //        return false;
                        //}
                        return true;
                    }
                }
            }

            return false;
        }

        private static bool IsVaildSceneResource(string fileName)
        {
            bool ret = false;

            if (string.IsNullOrEmpty(fileName))
                return ret;

            string localFileName = GetLocalPath(fileName);
            if (string.IsNullOrEmpty(localFileName))
                return ret;

#if UNITY_EDITOR

        var scenes = UnityEditor.EditorBuildSettings.scenes;
        if (scenes == null)
            return ret;
        var iter = scenes.GetEnumerator();
        while (iter.MoveNext())
        {
            UnityEditor.EditorBuildSettingsScene scene = iter.Current as UnityEditor.EditorBuildSettingsScene;
            if ((scene != null) && scene.enabled)
            {
                if (string.Compare(scene.path, localFileName, true) == 0)
                {
                    ret = true;
                    break;
                }
            }
        }
#endif
            return ret;
        }

        private void OnInit()
        {

        }
        public static void UTStopCoroutine(string methodName)
        {
            ResLibaryTool.Instance.StopCoroutine(methodName);
        }
        public static void UTStopCoroutine(IEnumerator routine)
        {
            ResLibaryTool.Instance.StopCoroutine(routine);
        }

        public static Coroutine UTStartCoroutine(IEnumerator routine)
        {
            return ResLibaryTool.Instance.StartCoroutine(routine);
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace ResLibary
{
    public class ResLibaryTool : MonoBehaviour
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
                for (int j = 0; j < ResLibaryConfig.ResourceExts.Count; ++j)
                {
                    if (string.Compare(ext, ResLibaryConfig.ResourceExts[j], true) == 0)
                    {
                        if ((ResLibaryConfig.ResourceExts[j] == ".fbx"))
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
        private ResLibary.GlobalCoroutineMgr GlobalCoroutine;
        private void OnInit()
        {
            GlobalCoroutine = gameObject.AddComponent<ResLibary.GlobalCoroutineMgr>();
            GlobalCoroutine.OnInit();
        }

        public void m_UTStartCoroutine(string id, IEnumerator routine)
        {
            GlobalCoroutine.UTStartCoroutine(id, routine);
        }
        public IEnumerator m_BGStartCoroutine(string id, IEnumerator routine)
        {
            return GlobalCoroutine.GBStartCoroutine(id, routine);
        }

        public void m_UTStopCoroutine(string id)
        {
            GlobalCoroutine.UTStopCoroutine(id);
        }

        public static void UTStopCoroutine(string id)
        {
            Instance.m_UTStopCoroutine(id);
        }

        //public static void UTStopCoroutine(IEnumerator routine)
        //{
        //    Instance.StopCoroutine(routine);
        //}

        public static void UTStartCoroutine(IEnumerator routine)
        {
            string id = Time.time.ToString() + UnityEngine.Random.Range(1, 100).ToString();
            UTStartCoroutine(id, routine);
        }

        public static void UTStartCoroutine(string id, IEnumerator routine)
        {
            Instance.m_UTStartCoroutine(id, routine);
        }

    }
}
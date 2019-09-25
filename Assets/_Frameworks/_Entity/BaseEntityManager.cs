using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace JXFrame.Entity
{

    public class BaseEntityManager : MonoBehaviour
    {
        protected virtual string configUrl { get { return Application.streamingAssetsPath + "/EntityConfig/SceneEntityConfig/"; } }

        [SerializeField]
        protected TextAsset autoRegistorText;
        [SerializeField]
        protected List<string> autoRegistorList = new List<string>();

        protected Dictionary<string, object> map = new Dictionary<string, object>();
        protected Dictionary<string, IEntityBehavior> autoRegistorDict;

        public object this[string Key]
        {
            get
            {
                if(map == null)
                    map = new Dictionary<string, object>();
                if (!map.ContainsKey(Key))
                    return null;
                return map[Key];
            }
            set
            {
                if (map == null)
                    map = new Dictionary<string, object>();
                map[Key] = value;
            }
        }

        public void SetMap(string Key,object data)
        {
            this[Key] = data;
        }
        public object GetMap(string Key)
        {
            return this[Key];
        }

        protected virtual void Awake()
        {
            
            autoRegistorDict = new Dictionary<string, IEntityBehavior>();
           
            LitJson.JsonData jd = null;
            if (autoRegistorText == null)
            {
                string str = LoadFileStr(configUrl);
                if (!string.IsNullOrEmpty(str))
                    jd = LitJson.JsonMapper.ToObject(str);
                else
                {
                    TextAsset text = Resources.Load<TextAsset>(configUrl);
                    if(text != null)
                        jd = LitJson.JsonMapper.ToObject(text.text);
                }
            }
            else
                jd = LitJson.JsonMapper.ToObject(autoRegistorText.text);
            List<string> typeNames = new List<string>();
            if (jd != null)
            {
                for (int i = 0; i < jd.Count; i++)
                {
                    if(jd[i] != null && !string.IsNullOrEmpty(jd[i].ToString()) && !typeNames.Contains(jd[i].ToString()))
                        typeNames.Add(jd[i].ToString());
                    //if (!autoRegistorDict.ContainsKey(jd[i].ToString()))
                    //{
                    //    System.Type v = System.Type.GetType(jd[i].ToString());
                    //    if (v == null)
                    //    {
                    //        Debug.Log(autoRegistorList[i] + " is null!");
                    //        continue;
                    //    }
                    //    IEntityBehavior obj = null;
                    //    if (v.IsSubclassOf(typeof(Component)))
                    //        obj = gameObject.AddComponent(v) as IEntityBehavior;
                    //    else
                    //        obj = System.Activator.CreateInstance(v) as IEntityBehavior;
                    //    if (obj != null)
                    //        autoRegistorDict[v.Name] = obj;
                    //}
                }
            }
            for (int i = 0; i < autoRegistorList.Count; i++)
            {
                if (!string.IsNullOrEmpty(autoRegistorList[i]) && !typeNames.Contains(autoRegistorList[i]))
                    typeNames.Add(autoRegistorList[i]);
                //if (!autoRegistorDict.ContainsKey(autoRegistorList[i]))
                //{
                //    System.Type v = System.Type.GetType(autoRegistorList[i]);
                //    if (v == null)
                //    {
                //        Debug.Log(autoRegistorList[i]+" is null!");
                //        continue;
                //    }
                //    IEntityBehavior obj = null;
                //    if (v.IsSubclassOf(typeof(Component)))
                //        obj = gameObject.AddComponent(v) as IEntityBehavior;
                //    else
                //        obj = System.Activator.CreateInstance(v) as IEntityBehavior;
                //    if (obj != null)
                //        autoRegistorDict[v.Name] = obj;
                //}
            }

            var types = AppDomain.CurrentDomain.GetAssemblies()
                       .SelectMany(a => a.GetTypes()).ToArray();
            for (int i = 0; i < types.Length; i++)
            {
                if (typeNames.Contains(types[i].Name) || typeNames.Contains(types[i].FullName))
                {
                    System.Type v = types[i];// System.Type.GetType(autoRegistorList[i]);
                   
                    object obj = null;
                    if (v.IsSubclassOf(typeof(Component)))
                        obj = gameObject.AddComponent(v);
                    else
                        obj = System.Activator.CreateInstance(v);
                    if (obj != null && obj is IEntityBehavior)
                    {
                        autoRegistorDict[v.Name] = obj as IEntityBehavior;
                    }
                }
            }
        }

        protected virtual void Start()
        {

            List<IEntityBehavior> list = new List<IEntityBehavior>(autoRegistorDict.Values);
            list.Sort((x, y) => x.Order.CompareTo(y.Order));
            for (int i = 0; i < list.Count; i++)
            {
                list[i].OnStart();
            }
        }

        protected virtual void OnDestroy()
        {
            List<IEntityBehavior> list = new List<IEntityBehavior>();
            list.Sort((x, y) => -x.Order.CompareTo(y.Order));
            for (int i = 0; i < list.Count; i++)
            {
                list[i].OnDispose();
            }
            autoRegistorDict.Clear();
            map.Clear();
        }

        protected virtual void OnApplicationQuit()
        {
            List<IEntityBehavior> list = new List<IEntityBehavior>();
            list.Sort((x, y) => -x.Order.CompareTo(y.Order));
            for (int i = 0; i < list.Count; i++)
            {
                list[i].OnDispose();
            }
            autoRegistorDict.Clear();
            map.Clear();
        }

        public string LoadFileStr(string path)
        {
            string str = "";
            if (!File.Exists(path))
                return null;
            StreamReader sr = new StreamReader(path);
            str = sr.ReadToEnd();
            if (string.IsNullOrEmpty(str))
                return null;
            sr.Close();
            sr.Dispose();
            return str;
        }
    }
}

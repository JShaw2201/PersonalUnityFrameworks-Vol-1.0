using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace JXFrame.Entity
{
#if HOTFIX_ENABLE
    [XLua.LuaCallCSharp]
#endif
    public class GlobalEntityManager : BaseEntityManager
    {
        public static GlobalEntityManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (syncRoot)
                    {
                        if (_instance == null)
                        {
                            GlobalEntityManager[] instance1 = FindObjectsOfType<GlobalEntityManager>();
                            if (instance1 != null)
                            {
                                for (var i = 0; i < instance1.Length; i++)
                                {
                                    Destroy(instance1[i].gameObject);
                                }
                            }
                        }
                    }

                    GameObject go = new GameObject(typeof(GlobalEntityManager).Name);
                    singletonObj = go;
                    DontDestroyOnLoad(go);
                    _instance = go.AddComponent<GlobalEntityManager>();
                }
                return _instance;
            }
        }
        private static GlobalEntityManager _instance;
        private static object syncRoot = new object();
        private static GameObject singletonObj = null;

        protected override string configUrl { get {return Application.streamingAssetsPath + "/EntityConfig/GlobalEntityConfig.json"; } }

        protected override void Awake()
        {
            GlobalEntityManager t = gameObject.GetComponent<GlobalEntityManager>();
            if (singletonObj == null)
            {
                singletonObj = gameObject;
                DontDestroyOnLoad(gameObject);

                _instance = t;
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

            base.Awake();
        }
      
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace JXFrame.Entity
{
#if HOTFIX_ENABLE
    [XLua.LuaCallCSharp]
#endif
    public class SceneEntityManager : BaseEntityManager
    {
        public static SceneEntityManager Instance;

        protected override string configUrl
        {
            get {
                string sname = SceneManager.GetActiveScene().name;
                if (sname == null)
                    sname = "";
                return Application.streamingAssetsPath + "/EntityConfig/SceneEntityConfig_" + sname+".json";
            }
        }

        protected override void Awake()
        {
            Instance = this;
            base.Awake();
        }
       
    }
}

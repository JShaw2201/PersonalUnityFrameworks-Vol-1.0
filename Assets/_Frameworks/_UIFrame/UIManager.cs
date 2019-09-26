using UnityEngine;
using System.Collections.Generic;

namespace JXFrame.View
{
#if HOTFIX_ENABLE
    [XLua.LuaCallCSharp]
#endif
    public class UIManager : MonoBehaviour, IHandleUIManager
    {
        public static UIManager Instance { get; private set; }

        public static bool configAssstBundle { set { UIFactory.configAssstBundle = value; InitFactory(); } }

        public static string configUrl       { set { UIFactory.configUrl = value; InitFactory(); } }
        public static string configName      { set { UIFactory.configName = value; InitFactory(); } }
        public static string configDirType   { set { UIFactory.configDirType = value; InitFactory(); } }/*Resources,streamingAssetsPath,dataPath,persistentDataPath**/

        public static LoadAction LoadString { set { UIFactory.LoadString = value; InitFactory(); } }
        public static LoadAction LoadPrefab { set { UIFactory.LoadPrefab = value; InitFactory(); } }

#if HOTFIX_ENABLE
        public static XLua.LuaEnv luaenv = new XLua.LuaEnv();
#endif
        private IHandleUIManager m_UIMgr;
        private static void InitFactory()
        {
#if UNITY_ANDROID
            UIFactory.UNITY_Android = true;
#else
            UIFactory.UNITY_Android = false;
#endif

#if UNITY_EDITOR
            UIFactory.UNITY_Editor = true;
#else
            UIFactory.UNITY_Editor = false;
#endif
            UIFactory.OnLoad();
        }
        private void Awake()
        {
            InitFactory();
            InitView();
        }

        private void OnDestroy()
        {
            ReleaseView();
        }

        public void InitView()
        {
            if (Instance != null && Instance != this)
            {
                MonoBehaviour[] monos = gameObject.GetComponents<MonoBehaviour>();
                if (monos.Length > 1)
                {
                    Destroy(this);
                }
                else
                {
                    Destroy(gameObject);
                }
                return;
            }
            Instance = this;
            m_UIMgr = UIFactory.CreateInternalUIManager();
            m_UIMgr.InitView();
        }

        /// <summary>
        /// 显示
        /// </summary>
        /// <param name="uIFormName"></param>
        public void ShowUIForm(string uIFormName)
        {

            m_UIMgr.ShowUIForm(uIFormName);
        }

        public void ShowUIForm(string uIFormName, string InfoState, params object[] data)
        {
            m_UIMgr.ShowUIForm(uIFormName, InfoState, data);
        }

        /// <summary>
        /// 关闭
        /// </summary>
        /// <param name="uIFormName"></param>
        public void CloseUIForm(string uIFormName)
        {
            m_UIMgr.CloseUIForm(uIFormName);
        }

        public void UpdateView(string uiFormName, string InfoState, params object[] data)
        {
            m_UIMgr.UpdateView(uiFormName, InfoState, data);
        }

        public void AddUIFormToCatch(UIBase uIBase)
        {
            m_UIMgr.AddUIFormToCatch(uIBase);
        }

        public void RemoveUIFormToCatch(string uIFormName)
        {
            m_UIMgr.RemoveUIFormToCatch(uIFormName);
        }

        public void CloseAllUIForm()
        {
            m_UIMgr.CloseAllUIForm();
        }

        public void ReleaseView()
        {
            m_UIMgr.ReleaseView();
        }
    }
}
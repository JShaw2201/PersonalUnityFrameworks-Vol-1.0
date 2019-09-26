using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JXFrame.View
{
#if HOTFIX_ENABLE
    [XLua.LuaCallCSharp]
#endif
    public class LuaUIBehavior : UIBase
    {
        protected System.Action luaOnInit;
        protected OnUpdateAction luaOnExcute;
        protected System.Action luaOnDisplay;
        protected System.Action luaOnHide;
        protected System.Action luaOnReDisplay;
        protected System.Action luaOnFreese;
        protected System.Action luaOnRelease;
#if HOTFIX_ENABLE
      
        [XLua.CSharpCallLua]
        public delegate void OnUpdateAction(string str, Array arr);
        

        private  XLua.LuaTable scriptEnv;
      
        [XLua.CSharpCallLua]
         public delegate UIType luaUiFormType();
#else
        public delegate UIType luaUiFormType();
        public delegate void OnUpdateAction(string str, Array arr);
#endif

        protected luaUiFormType _luaUiFormType;
        protected override string uiFormName()
        {
#if HOTFIX_ENABLE
            return scriptEnv.Get<string>("uiFormName");
#else
            return null;
#endif
        }

        protected override string canvasName()
        {
#if HOTFIX_ENABLE
            return scriptEnv.Get<string>("canvasName");
#else
            return null;
#endif
        }

        protected override UIType uiFormType()
        {
            if (_luaUiFormType == null)
                Debug.LogError("uiFormType is null!");
            return _luaUiFormType == null ? null : _luaUiFormType();// luaenv.Global.Get<UIType>("uiFormType");
        }

        public void LoadLuaString(bool isAssetBundle, string dirType, string scriptName, string scriptPath)
        {
#if HOTFIX_ENABLE
            scriptEnv = UIManager.luaenv.NewTable();
            XLua.LuaTable meta = UIManager.luaenv.NewTable();
            meta.Set("__index", UIManager.luaenv.Global);
            scriptEnv.SetMetaTable(meta);
            meta.Dispose();
            scriptEnv.Set("self", this);

            if (dirType == "Resources")
            {
                TextAsset luaStr = Resources.Load<TextAsset>(scriptPath);
                if (luaStr != null)
                    UIManager.luaenv.DoString(luaStr.text, "LuaUIBehaviour", scriptEnv);
                else
                    Debug.Log(scriptPath + "is null!");
            }
            else if (dirType == "streamingAssetsPath")
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                    string str = UIFactory.GetAndoidStreamingAssetLuaScripts(scriptName);
                if (!string.IsNullOrEmpty(str))
                    UIManager.luaenv.DoString(str ,"LuaUIBehaviour", scriptEnv);
                else
                    Debug.Log(scriptPath + "is null!");
                
#else
                LoadLuaScript(isAssetBundle, dirType, scriptName, scriptPath);
#endif
            }
            else
            {
                LoadLuaScript(isAssetBundle, dirType, scriptName, scriptPath);
            }

            scriptEnv.Get("uiFormType", out _luaUiFormType);
            scriptEnv.Get("OnInit", out luaOnInit);
            scriptEnv.Get("OnExcute", out luaOnExcute);
            scriptEnv.Get("OnDisplay", out luaOnDisplay);
            scriptEnv.Get("OnHide", out luaOnHide);
            scriptEnv.Get("OnReDisplay", out luaOnReDisplay);
            scriptEnv.Get("OnFreese", out luaOnFreese);
            scriptEnv.Get("OnRelease", out luaOnRelease);
#endif
        }
#if HOTFIX_ENABLE
        private void LoadLuaScript(bool isAssetBundle, string dirType, string scriptName, string scriptPath)
        {
            if (UIFactory.LoadString != null)
            {
                string content = (string)UIFactory.LoadString(scriptName);
                if (!string.IsNullOrEmpty(content))
                    UIManager.luaenv.DoString(content, "LuaUIBehaviour", scriptEnv);
                else
                    Debug.Log(scriptName + "is null!");
                return;
            }
            if (isAssetBundle)
            {
                TextAsset luaStr = UIFactory.LoadAssetBundleObj<TextAsset>(UIFactory.GetPath(dirType, scriptPath), scriptName);
                if (luaStr != null)
                    UIManager.luaenv.DoString(luaStr.text, "LuaUIBehaviour", scriptEnv);
                else
                    Debug.Log(scriptPath + "is null!");
            }
            else
            {
                string filePath = UIFactory.GetPath(dirType, scriptPath);
                string dir = System.IO.Path.GetDirectoryName(filePath);
                string fl = System.IO.Path.GetFileNameWithoutExtension(filePath);
                if (!System.IO.Directory.Exists(dir))
                {
                    Debug.LogError(dir+"is null");
                }

                string[] fls = System.IO.Directory.GetFiles(dir);
                string path = scriptPath;//完整路径
                for (int i = 0; i < fls.Length; i++)
                {
                    if (string.IsNullOrEmpty((fls[i])))
                        continue;
                    System.IO.FileInfo fs = new System.IO.FileInfo(fls[i]);
                    if (!fs.Exists)
                        continue;
                    if (fs.Extension == ".meta")
                        continue;
                    int findex = fs.Name.LastIndexOf(fl);
                    if (findex != 0)
                        continue;
                    path = fs.FullName;
                }
                Debug.Log(fl+":"+path + ":" + fls.Length);
                string content = UIFactory.IOLoadFileStr(path);
                if (!string.IsNullOrEmpty(content))
                    UIManager.luaenv.DoString(content, "LuaUIBehaviour", scriptEnv);
                else
                    Debug.Log(scriptPath + "is null!");

            }
        }

#endif

        protected override void OnInit()
        {
            if (luaOnInit != null)
                luaOnInit();
        }

        protected override void OnExcute(string InfoState, params object[] data)
        {
            if (luaOnExcute != null)
                luaOnExcute(InfoState, data);
        }
        protected override void OnDisplay()
        {
            if (luaOnDisplay != null)
                luaOnDisplay();
        }

        protected override void OnHide()
        {
            if (luaOnHide != null)
                luaOnHide();
        }

        protected override void OnReDisplay()
        {
            if (luaOnReDisplay != null)
                luaOnReDisplay();
        }

        protected override void OnFreese()
        {
            if (luaOnFreese != null)
                luaOnFreese();
        }

        protected override void OnRelease()
        {
            if (luaOnRelease != null)
                luaOnRelease();
#if HOTFIX_ENABLE
            scriptEnv.Dispose();
#endif

        }

    }
}

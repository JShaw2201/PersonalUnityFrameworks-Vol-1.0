using UnityEngine;
using System.Collections.Generic;

namespace JXFrame.View
{
    public interface IHandleUIManager
    {
        void InitView();
        void UpdateView(string uiFormName, string InfoState, params object[] data);
        void ReleaseView();
        void AddUIFormToCatch(UIBase uIBase);
        void RemoveUIFormToCatch(string uIFormName);
        void CloseAllUIForm();
        void ShowUIForm(string uIFormName);
        void ShowUIForm(string uIFormName, string InfoState, params object[] data);
        void CloseUIForm(string uIFormName);
    }

    internal class InternalUIManager : IHandleUIManager
    {

        private UIMaskMgr uIMaskMgr;

        private Stack<UIBase> _stackUIForm;
        private Dictionary<string, UIBase> _dicCurUIForm;
        private Dictionary<string, UIBase> _dicAllUIForm;

        void IHandleUIManager.InitView()
        {
            _stackUIForm = new Stack<UIBase>();
            _dicCurUIForm = new Dictionary<string, UIBase>();
            _dicAllUIForm = new Dictionary<string, UIBase>();
            UIFactory.OnLoad();
            uIMaskMgr = new UIMaskMgr();
        }

        /// <summary>
        /// 显示
        /// </summary>
        /// <param name="uIFormName"></param>
        void IHandleUIManager.ShowUIForm(string uIFormName)
        {
            ((IHandleUIManager)this).ShowUIForm(uIFormName, null, null);
        }

        void IHandleUIManager.ShowUIForm(string uIFormName, string InfoState, params object[] data)
        {
            UIBase _UIBase = LoadUIBase(uIFormName);
            if (_UIBase == null)
            {
                Debug.LogError("UIBase is null");
                return;
            }

            switch (_UIBase.m_uIFormType.UIForms_ShowMode)
            {
                case UIFormShowMode.Normal:
                    if (_dicCurUIForm.ContainsKey(uIFormName))
                    {
                        _UIBase.Excute(InfoState, data);
                        return;
                    }
                    _dicCurUIForm[uIFormName] = _UIBase;
                    break;
                case UIFormShowMode.ReverseChange:
                    if (_stackUIForm.Contains(_UIBase))
                    {
                        _UIBase.Excute(InfoState, data);
                        return;
                    }
                    else
                    {
                        if (_stackUIForm.Count > 0)
                        {
                            UIBase topUIForm = _stackUIForm.Peek();
                            //栈顶元素作冻结处理
                            topUIForm.Freeze();
                        }
                        _stackUIForm.Push(_UIBase);
                    }
                    break;
                case UIFormShowMode.HideOther:
                    {
                        if (_dicCurUIForm.ContainsKey(uIFormName))
                        {
                            _UIBase.Excute(InfoState, data);
                            return;
                        }
                        foreach (var stack in _stackUIForm)
                        {
                            if (stack.m_canvasName == _UIBase.m_canvasName)
                                stack.Hiding();
                        }
                        foreach (var dic in _dicCurUIForm)
                        {
                            if (dic.Value.m_canvasName == _UIBase.m_canvasName)
                                dic.Value.Hiding();
                        }
                        _dicCurUIForm[uIFormName] = _UIBase;
                    }
                    break;
            }
            if (!string.IsNullOrEmpty(_UIBase.m_canvasName) && _dicAllUIForm.ContainsKey(_UIBase.m_canvasName) && !_UIBase.m_uIFormType.IsNewCanvas)
            {
                UIBase P_UIBase = GetParentComponentScript<UIBase>(_UIBase.transform); //_UIBase.transform.parent.parent.GetComponent<UIBase>();
                if (P_UIBase != null && _UIBase.m_canvasName == P_UIBase.m_uIFormName)
                {
                    UIFormSetParent(_UIBase);
                }
            }
            uIMaskMgr.SetUIMask(_UIBase);
            _UIBase.transform.SetAsLastSibling();
            _UIBase.Display();
            _UIBase.Excute(InfoState, data);
        }

        /// <summary>
        /// 关闭
        /// </summary>
        /// <param name="uIFormName"></param>
        void IHandleUIManager.CloseUIForm(string uIFormName)
        {
            if (string.IsNullOrEmpty(uIFormName) || !_dicAllUIForm.ContainsKey(uIFormName))
                return;
            UIBase _UIBase = null;
            //_dicAllUIForm.TryGetValue(uIFormName,out _UIBase);
            UIBase jS_UIBase = _dicAllUIForm[uIFormName];
            if (jS_UIBase == null)
                return;
            switch (jS_UIBase.m_uIFormType.UIForms_ShowMode)
            {
                case UIFormShowMode.Normal:
                    {
                        jS_UIBase.transform.SetAsFirstSibling();
                        jS_UIBase.Hiding();
                        _dicCurUIForm.TryGetValue(uIFormName, out _UIBase);
                        if (_UIBase == null)
                            return;
                        _dicCurUIForm.Remove(uIFormName);

                    }
                    break;
                case UIFormShowMode.ReverseChange:
                    {
                        if (_stackUIForm.Count >= 2)
                        {
                            //出栈处理
                            UIBase topUIForms = _stackUIForm.Pop();
                            //做隐藏处理
                            topUIForms.Hiding();
                            //出栈后，下一个窗体做“重新显示”处理。
                            UIBase nextUIForms = _stackUIForm.Peek();
                            nextUIForms.ReDisplay();
                        }
                        else if (_stackUIForm.Count == 1)
                        {
                            //出栈处理
                            UIBase topUIForms = _stackUIForm.Pop();
                            //做隐藏处理
                            topUIForms.transform.SetAsFirstSibling();
                            topUIForms.Hiding();
                        }
                    }
                    break;
                case UIFormShowMode.HideOther:
                    jS_UIBase.Hiding();
                    jS_UIBase.transform.SetAsFirstSibling();
                    _dicCurUIForm.TryGetValue(uIFormName, out _UIBase);
                    if (_UIBase == null)
                        return;
                    _dicCurUIForm.Remove(uIFormName);
                    foreach (var stack in _stackUIForm)
                    {
                        stack.ReDisplay();
                    }
                    foreach (var dic in _dicCurUIForm.Values)
                    {
                        dic.ReDisplay();
                    }
                    break;
            }
            UIBase _preUIBase = null;
            if (_stackUIForm.Count > 0)
            {
                _preUIBase = _stackUIForm.Peek();
            }
            else
            {
                List<string> list = new List<string>(_dicCurUIForm.Keys);
                if (list.Count > 0)
                    _preUIBase = _dicCurUIForm[list[list.Count - 1]];
            }
            if (_preUIBase != null)
            {
                uIMaskMgr.SetUIMask(_preUIBase);
                _preUIBase.transform.SetAsLastSibling();
            }
        }

        void IHandleUIManager.UpdateView(string uiFormName, string InfoState, params object[] data)
        {
            if (_dicAllUIForm.ContainsKey(uiFormName))
            {
                UIBase jS_UIBase = _dicAllUIForm[uiFormName];
                jS_UIBase.Excute(InfoState, data);
            }
        }

        void IHandleUIManager.AddUIFormToCatch(UIBase uIBase)
        {
            if (!_dicAllUIForm.ContainsKey(uIBase.m_uIFormName))
            {
                _dicAllUIForm[uIBase.m_uIFormName] = uIBase;
                UIFormSetParent(uIBase);
            }
        }

        void IHandleUIManager.RemoveUIFormToCatch(string uIFormName)
        {
            if (_dicAllUIForm.ContainsKey(uIFormName))
            {
                _dicAllUIForm.Remove(uIFormName);
            }
            if (_dicCurUIForm.ContainsKey(uIFormName))
            {
                _dicCurUIForm.Remove(uIFormName);
            }
            bool found = false;
            UIBase[] uIBases = _stackUIForm.ToArray();
            for (int i = 0; i < uIBases.Length; i++)
            {
                if (uIBases[i] == null || uIBases[i].m_uIFormName == uIFormName)
                {
                    found = true;
                }
            }

            if (found)
            {
                _stackUIForm.Clear();
                for (int i = 0; i < uIBases.Length; i++)
                {
                    UIBase uIBase = uIBases[i];
                    if (uIBase == null || uIBase.m_uIFormName == uIFormName)
                    {
                        continue;
                    }
                    _stackUIForm.Push(uIBase);
                }
            }
        }

        void IHandleUIManager.CloseAllUIForm()
        {
            foreach (var stack in _stackUIForm)
            {
                if (stack != null)
                    stack.Hiding();
            }
            _stackUIForm.Clear();
            List<string> list = new List<string>(_dicCurUIForm.Keys);
            foreach (var dic in list)
            {
                if (_dicCurUIForm[dic] != null)
                    _dicCurUIForm[dic].Hiding();
            }
            _dicCurUIForm.Clear();
        }

        /// <summary>
        /// 加载
        /// </summary>
        /// <param name="uIFormName"></param>
        /// <returns></returns>
        private UIBase LoadUIBase(string uIFormName)
        {
            if (string.IsNullOrEmpty(uIFormName))
            {
                Debug.LogError("名称是空的");
                return null;
            }
            UIBase uiBase = null;
            _dicAllUIForm.TryGetValue(uIFormName, out uiBase);
            if (uiBase == null)
                uiBase = LoadResUIBase(uIFormName);

            return uiBase;
        }


        private UIBase LoadResUIBase(string uIFormName)
        {
            GameObject go = UIFactory.LoadUIPrefab(uIFormName);
            if (go == null)
            {
                Debug.LogError("预设是空的");
                return null;
            }
            UIBase uiBase = go.GetComponent<UIBase>();
            if (uiBase != null)
            {
                uiBase.InitView();
                _dicAllUIForm[uIFormName] = uiBase;
                Debug.Log(uIFormName + "current:" + uiBase.m_uIFormName);

                UIFormSetParent(uiBase);
            }
            else
            {
                Debug.LogError("预设是空的");

            }
            return uiBase;
        }

        private Transform LoadCanvasRoot()
        {
            GameObject croot = GameObject.Find("Canvas");
            if (croot == null)
            {
                GameObject _croot = Resources.Load<GameObject>("Canvas");
                if (_croot != null)
                {
                    croot = GameObject.Instantiate(_croot);
                    return croot.transform;
                }
                else
                {
                    Debug.Log("Canvas is null!");
                }
            }
            return croot.transform;
        }

        private void UIFormSetParent(UIBase uiBase)
        {
            GameObject go = uiBase.Container;
            Transform NorTran;
            Transform FixTran;
            Transform PopTran;
            Transform CanvasT = null;

            if (uiBase.m_uIFormType.IsNewCanvas)
            {
                CanvasT = go.transform;
            }
            else if (!string.IsNullOrEmpty(uiBase.m_canvasName) && _dicAllUIForm.ContainsKey(uiBase.m_canvasName))
            {
                CanvasT = _dicAllUIForm[uiBase.m_canvasName].transform;
            }
            else
            {
                CanvasT = LoadCanvasRoot();
            }
            CheckUIFormTypeTrans(CanvasT);

            NorTran = CanvasT.Find("Normal");
            FixTran = CanvasT.Find("Fixed");
            PopTran = CanvasT.Find("PopUp");

            switch (uiBase.m_uIFormType.UIForms_Type)
            {
                case UIFormType.Normal:
                    if (NorTran == null)
                        Debug.Log("NorTran is null");
                    go.transform.SetParent(NorTran);
                    break;
                case UIFormType.Fixed:
                    if (FixTran == null)
                        Debug.Log("FixTran is null");
                    go.transform.SetParent(FixTran);
                    break;
                case UIFormType.PopUp:
                    if (PopTran == null)
                        Debug.Log("PopTran is null");
                    go.transform.SetParent(PopTran);
                    break;

            }
            go.transform.localPosition = Vector3.zero;
            go.transform.localScale = Vector3.one;
            go.transform.localEulerAngles = Vector3.zero;
        }

        private void CheckUIFormTypeTrans(Transform CanvasT)
        {
            Transform NorTran;
            Transform FixTran;
            Transform PopTran;
            NorTran = CanvasT.Find("Normal");
            FixTran = CanvasT.Find("Fixed");
            PopTran = CanvasT.Find("PopUp");

            bool IsSetIndex = false;

            if (NorTran == null)
            {
                NorTran = new GameObject("Normal").transform;
                NorTran.SetParent(CanvasT);
                NorTran.localPosition = Vector3.zero;
                NorTran.localRotation = Quaternion.identity;
                NorTran.localScale = Vector3.one;

                IsSetIndex = true;
            }
            if (FixTran == null)
            {
                FixTran = new GameObject("Fixed").transform;
                FixTran.SetParent(CanvasT);
                FixTran.localPosition = Vector3.zero;
                FixTran.localRotation = Quaternion.identity;
                FixTran.localScale = Vector3.one;
                IsSetIndex = true;
            }

            if (PopTran == null)
            {
                PopTran = new GameObject("PopUp").transform;
                PopTran.SetParent(CanvasT);
                PopTran.localPosition = Vector3.zero;
                PopTran.localRotation = Quaternion.identity;
                PopTran.localScale = Vector3.one;
                IsSetIndex = true;
            }
            if (IsSetIndex)
            {
                NorTran.SetAsLastSibling();
                FixTran.SetAsLastSibling();
                PopTran.SetAsLastSibling();
            }
        }

        void IHandleUIManager.ReleaseView()
        {
            ((IHandleUIManager)this).CloseAllUIForm();
            List<string> templist = new List<string>(_dicAllUIForm.Keys);
            foreach (var uiform in templist)
            {
                var uibase = _dicAllUIForm[uiform];
                if (uibase != null)
                    uibase.ReleaseView();
            }
        }

        private T GetParentComponentScript<T>(Transform go) where T : Component
        {
            T t = null;
            if (go.parent != null)
            {
                t = go.parent.GetComponent<T>();
                if (go.root != go.parent && t == null)
                {
                    t = GetParentComponentScript<T>(go.parent);
                }
            }
            return t;
        }
    }
}
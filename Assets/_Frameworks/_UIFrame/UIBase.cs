using UnityEngine;
using System.Collections;

namespace JXFrame.View
{
    public abstract class UIBase : MonoBehaviour
    {

        public string m_canvasName { get { if (_canvasName == null) _canvasName = canvasName(); return _canvasName; } }
        public string m_uIFormName { get { if (_uiFormName == null) _uiFormName = uiFormName(); return _uiFormName; } }
        public UIType m_uIFormType { get { if (_uiFormType == null) _uiFormType = uiFormType(); return _uiFormType; } }

        public GameObject Container
        {
            get
            {
                if (m_Container == null)
                    m_Container = gameObject;
                return m_Container;
            }
        }

        protected virtual string canvasName() { return null; }
        protected virtual UIType uiFormType() { return new UIType(); }
        protected abstract string uiFormName();

        [SerializeField]
        protected GameObject m_Container;

        public CanvasGroup canvasGroup
        {
            get
            {
                if (m_Container == null)
                    m_Container = gameObject;
                if (_canvasGroup == null)
                {
                    _canvasGroup = m_Container.GetComponent<CanvasGroup>();
                    if (_canvasGroup == null)
                        _canvasGroup = m_Container.AddComponent<CanvasGroup>();
                }
                return _canvasGroup;
            }
        }

        private string _canvasName;
        private string _uiFormName;
        private UIType _uiFormType;
        private CanvasGroup _canvasGroup;

        public void InitView()
        {
            //_canvasName = canvasName();
            //_uiFormName = uiFormName();
            //_uiFormType = uiFormType();
            OnInit();
        }

        public void Excute(string InfoState, params object[] data) { OnExcute(InfoState, data); }

        /// <summary>
        /// 显示
        /// </summary>
        public void Display()
        {
            if (m_Container == null)
                m_Container = gameObject;
            if (gameObject != null)
                m_Container.SetActive(true);
            canvasGroup.alpha = 1;
            canvasGroup.interactable = true;
            OnDisplay();
        }

        /// <summary>
        /// 隐藏
        /// </summary>
        public void Hiding()
        {
            if (m_Container == null)
                m_Container = gameObject;
            if (gameObject != null)
                m_Container.SetActive(false);
            OnHide();
        }

        /// <summary>
        /// 冻结
        /// </summary>
        public void Freeze()
        {
            if (m_Container == null)
                m_Container = gameObject;
            if (gameObject != null)
                m_Container.SetActive(true);
            canvasGroup.interactable = false;
            OnFreese();
        }

        /// <summary>
        /// 再显示
        /// </summary>
        public void ReDisplay()
        {
            if (m_Container == null)
                m_Container = gameObject;
            if (gameObject != null)
                m_Container.SetActive(true);
            canvasGroup.alpha = 1;
            canvasGroup.interactable = true;
            OnReDisplay();
        }

        public void ReleaseView()
        {
            Destroy(gameObject);
        }

        private void OnDestroy()
        {
            OnRelease();
        }

        protected abstract void OnInit();

        protected abstract void OnExcute(string InfoState, params object[] data);

        protected virtual void OnDisplay() { }

        protected virtual void OnHide() { }

        protected virtual void OnReDisplay() { }

        protected virtual void OnFreese() { }

        protected virtual void OnRelease() { }
    }
}
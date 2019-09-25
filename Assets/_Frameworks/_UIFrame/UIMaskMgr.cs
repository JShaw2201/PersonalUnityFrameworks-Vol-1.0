using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace JXFrame.View
{
    public class UIMaskMgr
    {
        private Dictionary<string, GameObject> dicMask;
        
        public UIMaskMgr()
        {
            dicMask = new Dictionary<string, GameObject>();
        }

        /// <summary>
        /// 设置UI遮罩
        /// </summary>
        /// <param name="uIBase">面板</param>
        public void SetUIMask(UIBase uIBase)
        {
            GameObject mask;
            string canvasName = uIBase.m_uIFormType.IsNewCanvas ? uIBase.m_uIFormName : !string.IsNullOrEmpty(uIBase.m_canvasName) ?uIBase.m_canvasName:"default";
            mask = LoadUIMaskObj(canvasName, uIBase.Container.transform.parent);
            if (mask == null)
                return;
            CanvasGroup uimask = mask.GetComponent<CanvasGroup>();
            switch (uIBase.m_uIFormType.UIForm_LucencyType)
            {
                case UIFormLucenyType.Lucency:
                    uimask.alpha = 0;
                    uimask.blocksRaycasts = true;
                    break;
                case UIFormLucenyType.Translucence:
                    uimask.alpha = 0.5f;
                    uimask.blocksRaycasts = true;
                    break;
                case UIFormLucenyType.ImPenetrable:
                    uimask.alpha = 0.25f;
                    uimask.blocksRaycasts = true;
                    break;
                case UIFormLucenyType.Pentrate:
                    uimask.alpha = 0;
                    uimask.blocksRaycasts = false;
                    break;
            }
            mask.transform.SetParent(uIBase.Container.transform.parent);
            mask.transform.SetAsLastSibling();
        }

        private GameObject LoadUIMaskObj(string canvasName,Transform ParentObj)
        {
            if (dicMask.ContainsKey(canvasName))
            {
                return dicMask[canvasName];
            }
            else
            {
                GameObject GO = UIFactory.LoadUIMask();
                if (GO == null)
                    return null;
                GO.transform.SetParent(ParentObj);
                GO.transform.localPosition = Vector3.zero;
                GO.transform.localEulerAngles = Vector3.zero;
                GO.transform.localScale = Vector3.one;
                dicMask[canvasName] = GO;
                return GO;
            }
        }

        public void OnUpdate()
        {

        }
        public void OnDispose()
        {
            
        }

       
    }
}

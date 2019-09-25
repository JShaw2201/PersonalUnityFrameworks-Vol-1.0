using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Canvas3d
{ 
    public class CanvasObject3d : BaseCanvas3d
    {       
        #region UnityMethod

        private void Awake()
        {
            OnInit();
        }

        private void Update()
        {
            OnUpdate();
        }

        private void OnDestroy()
        {
            OnDispose();
        }

        #endregion

        protected override BaseClickObject3d CreateClickObject3d()
        {
            BaseClickObject3d _clickCheck = null;
            try
            {
                _clickCheck = (BaseClickObject3d)Activator.CreateInstance(Type.GetType(ClickCheckTypeName));
            }
            catch { }
            return _clickCheck;
        }
    }

}
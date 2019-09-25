using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Canvas3d
{
    public enum TriggerMethod
    {
        Mouse,
        VR_Left,
        VR_Right,
        VR_Both
    }
    public abstract class BaseCanvas3d : MonoBehaviour
    {
        public TriggerMethod triggerMethod = TriggerMethod.Mouse;
        public bool AutoGetRayObj;
        public string ClickCheckTypeName = "Canvas3d.ClickObject3d";
        public Camera Camera3d;
        public Transform leftHandle;
        public Transform rightHandle;
        public float rayDistance = 1000;
        public LayerMask mask;

        private BaseClickObject3d clickCheck;
        private Dictionary<GameObject, Base3dInterface> base3dDict = new Dictionary<GameObject, Base3dInterface>();
     
        #region Public Method

        public void AddChildObject3d(BaseObject3d base3DObject)
        {
            if (!base3dDict.ContainsKey(base3DObject.gameObject))
            {
                base3dDict[base3DObject.gameObject] = base3DObject;
            }
        }

        public void DelChildObject3d(BaseObject3d base3DObject)
        {
            if (base3dDict.ContainsKey(base3DObject.gameObject))
            {
                base3dDict.Remove(base3DObject.gameObject);
            }
        }

        protected void OnInit()
        {
            NewClickObject3d(CreateClickObject3d());
        }

        protected void OnUpdate()
        {
            if (clickCheck != null)
                CheckClick();
        }

        protected void OnDispose()
        {
            base3dDict.Clear();
        }

        protected virtual BaseClickObject3d CreateClickObject3d()
        {
            BaseClickObject3d _clickCheck = null;
            try
            {
                _clickCheck = (BaseClickObject3d)Activator.CreateInstance(Type.GetType(ClickCheckTypeName));
            }
            catch { }
            return _clickCheck;
        }

        #endregion

        #region Private Method

        private void NewClickObject3d(BaseClickObject3d clickObj)
        {
            clickCheck = clickObj;
        }

        private void CheckClick()
        {
            Base3dInterface base3Dc = null;
            Base3dInterface base3Dr = null;
            Base3dInterface base3Dl = null;

            switch (triggerMethod)
            {
                case TriggerMethod.Mouse:
                    base3Dc = GetMouseClick();
                    break;
                case TriggerMethod.VR_Both:
                    base3Dl = GetRightHandClick();
                    base3Dr = GetLeftHandClick();
                    break;
                case TriggerMethod.VR_Left:
                    base3Dl = GetLeftHandClick();
                    break;
                case TriggerMethod.VR_Right:
                    base3Dr = GetRightHandClick();
                    break;
            }

            foreach (var base3dObj in base3dDict.Values)
            {
                if (base3dObj != base3Dl && base3dObj != base3Dr && base3dObj != base3Dc)
                {
                    switch (base3dObj.statusEnum3d)
                    {
                        case StatusEnum3d.Highlight:
                            base3dObj.OnNormal();
                            break;
                        case StatusEnum3d.Pressed:
                            base3dObj.OnNormal();
                            break;
                    }
                }
            }
        }

        private Base3dInterface GetMouseClick()
        {
            if (AutoGetRayObj)
            {
                Camera3d = clickCheck.Camera3d();
            }
            if (Camera3d == null)
                return null;
            return GetClickObj(Camera3d.ScreenPointToRay(Input.mousePosition));
        }

        private Base3dInterface GetLeftHandClick()
        {
            if (AutoGetRayObj)
            {
                if (leftHandle == clickCheck.GetLeftHand())
                {
                    leftHandle = clickCheck.GetLeftHand();
                }
            }
            if (leftHandle == null)
            {
                return null;
            }
            return GetClickObj(new Ray(leftHandle.position, leftHandle.forward), TriggerMethod.VR_Left);
        }

        private Base3dInterface GetRightHandClick()
        {
            if (AutoGetRayObj)
            {
                if (rightHandle == clickCheck.GetRightHand())
                {
                    rightHandle = clickCheck.GetRightHand();
                }
                if (rightHandle == null)
                {
                    return null;
                }
            }
            return GetClickObj(new Ray(rightHandle.position, rightHandle.forward), TriggerMethod.VR_Right);
        }

        private Base3dInterface GetClickObj(Ray ray, TriggerMethod triggerMethod = TriggerMethod.Mouse)
        {
            Base3dInterface base3D = null;
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, rayDistance, mask))
            {
                if (base3dDict.ContainsKey(hit.collider.gameObject))
                {
                    base3D = base3dDict[hit.collider.gameObject];
                    if (!base3D.RaycastTarget)
                        return null;
                    switch (base3D.statusEnum3d)
                    {
                        case StatusEnum3d.Highlight:
                            if (clickCheck.CheckTiggherDown(triggerMethod))
                            {
                                base3D.OnPresseDown();
                            }
                            break;
                        case StatusEnum3d.Normal:
                            if (clickCheck.CheckTiggherDown(triggerMethod))
                            {
                                base3D.OnPresseDown();
                            }
                            else
                            {
                                base3D.OnHighlight();
                            }
                            break;
                        case StatusEnum3d.Pressed:
                            if (clickCheck.CheckTiggherUp(triggerMethod))
                            {
                                base3D.OnPresseUp();
                            }
                            break;
                    }
                }

            }
            return base3D;
        }

        #endregion

    }

}
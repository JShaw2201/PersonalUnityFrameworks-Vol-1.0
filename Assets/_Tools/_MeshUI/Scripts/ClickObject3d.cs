using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Canvas3d
{
    public class ClickObject3d: BaseClickObject3d
    {
        protected Camera Cam3d;
   
        public override Camera Camera3d()
        {
            if (Cam3d == null)
            {
                Cam3d = Camera.main;
                if (Camera.main == null)
                    return null;
            }
            if (!Cam3d.gameObject.activeInHierarchy)
            {
                if (Camera.main != null && Camera.main.gameObject.activeInHierarchy)
                {
                    Cam3d = Camera.main;
                }
                else
                {
                    GameObject[] cams = GameObject.FindGameObjectsWithTag("MainCamera");
                    for (int i = 0; i < cams.Length; i++)
                    {
                        if (cams[i].activeInHierarchy && cams[i].GetComponent<Camera>())
                        {
                            Cam3d = cams[i].GetComponent<Camera>();
                            break;
                        }
                    }
                }
            }
            return Cam3d;
        }

        public override Transform GetLeftHand()
        {
#if VRTK_DEFINE_SDK_STEAMVR
            if (VRTK_DeviceFinder.GetControllerLeftHand() != null)
            {
                return VRTK_DeviceFinder.GetControllerLeftHand().transform;

            }
#endif
            return null;
        }

        public override Transform GetRightHand()
        {
#if VRTK_DEFINE_SDK_STEAMVR
            if (VRTK_DeviceFinder.GetControllerRightHand() != null)
            {
                return VRTK_DeviceFinder.GetControllerRightHand().transform;

            }
#endif
            return null;
        }

        protected override bool CheckMouseClickDown()
        {
            return Input.GetMouseButtonDown(0);
        }

        protected override bool CheckMouseClickUp()
        {
            return Input.GetMouseButtonUp(0);
        }

        protected override bool CheckLeftClicDown()
        {
            bool clicked = false;
#if VRTK_DEFINE_SDK_STEAMVR
            if (VRTK_DeviceFinder.GetControllerRightHand() != null)
            {
                var deviceright = SteamVR_Controller.Input((int)VRTK_DeviceFinder.GetControllerIndex(VRTK_DeviceFinder.GetControllerRightHand()));
                clicked = deviceright.GetPressDown(SteamVR_Controller.ButtonMask.Trigger);

            }
#endif
            return clicked;
        }

        protected override bool CheckRightClicDown()
        {
            bool clicked = false;
#if VRTK_DEFINE_SDK_STEAMVR
            if (VRTK_DeviceFinder.GetControllerRightHand() != null)
            {
                var deviceright = SteamVR_Controller.Input((int)VRTK_DeviceFinder.GetControllerIndex(VRTK_DeviceFinder.GetControllerRightHand()));
                clicked = deviceright.GetPressDown(SteamVR_Controller.ButtonMask.Trigger);

            }
#endif
            return clicked;
        }

        protected override bool CheckLeftClicUp()
        {
            bool clicked = false;
#if VRTK_DEFINE_SDK_STEAMVR
            if (VRTK_DeviceFinder.GetControllerRightHand() != null)
            {
                var deviceright = SteamVR_Controller.Input((int)VRTK_DeviceFinder.GetControllerIndex(VRTK_DeviceFinder.GetControllerRightHand()));
                clicked = deviceright.GetPressUp(SteamVR_Controller.ButtonMask.Trigger);

            }
#endif
            return clicked;
        }

        protected override bool CheckRightClicUp()
        {
            bool clicked = false;
#if VRTK_DEFINE_SDK_STEAMVR
            if (VRTK_DeviceFinder.GetControllerRightHand() != null)
            {
                var deviceright = SteamVR_Controller.Input((int)VRTK_DeviceFinder.GetControllerIndex(VRTK_DeviceFinder.GetControllerRightHand()));
                clicked = deviceright.GetPressUp(SteamVR_Controller.ButtonMask.Trigger);

            }
#endif
            return clicked;
        }

        
    }
}
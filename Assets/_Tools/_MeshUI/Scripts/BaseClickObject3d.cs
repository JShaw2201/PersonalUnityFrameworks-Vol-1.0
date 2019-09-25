using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Canvas3d
{
    public abstract class BaseClickObject3d
    {
        public bool CheckTiggherDown(TriggerMethod triggerMethod)
        {
            bool clicked = false;
            switch (triggerMethod)
            {
                case TriggerMethod.Mouse:
                    clicked = CheckMouseClickDown();
                    break;
                case TriggerMethod.VR_Right:
                    clicked = CheckRightClicDown();
                    break;
                case TriggerMethod.VR_Left:
                    clicked = CheckLeftClicDown();
                    break;
                case TriggerMethod.VR_Both:
                    break;
            }
            return clicked;
        }



        public bool CheckTiggherUp(TriggerMethod triggerMethod)
        {
            bool clicked = false;
            switch (triggerMethod)
            {
                case TriggerMethod.Mouse:
                    clicked = CheckMouseClickUp();
                    break;
                case TriggerMethod.VR_Right:
                    clicked = CheckRightClicUp();
                    break;
                case TriggerMethod.VR_Left:
                    clicked = CheckLeftClicUp();
                    break;
            }
            return clicked;
        }

        public abstract Camera Camera3d();
        public abstract Transform GetLeftHand();
        public abstract Transform GetRightHand();
        protected abstract bool CheckMouseClickDown();
        protected abstract bool CheckMouseClickUp();
        protected abstract bool CheckLeftClicDown();
        protected abstract bool CheckRightClicDown();
        protected abstract bool CheckLeftClicUp();
        protected abstract bool CheckRightClicUp();
    }

}

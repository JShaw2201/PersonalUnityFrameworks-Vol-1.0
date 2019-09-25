using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Canvas3d
{
    public enum Transition
    {
        //
        // 摘要:
        //     No Transition.
        None = 0,
        //
        // 摘要:
        //     Use an color tint transition.
        ColorTint = 1,

        MaterialSwap = 2,

        //
        // 摘要:
        //     Use a sprite swap transition.
        SpriteSwap = 3,
        //
        // 摘要:
        //     Use an animation transition.
        Animation = 4
    }

    public abstract class BaseObject3d : MonoBehaviour, Base3dInterface
    {
        protected bool _RaycastTarget = true;
        public bool RaycastTarget { get { return _RaycastTarget; } set { _RaycastTarget = value; } }

        protected StatusEnum3d _StatusEnum3d = StatusEnum3d.Normal;
        public StatusEnum3d statusEnum3d { get { return _StatusEnum3d; } set { _StatusEnum3d = value; } }

        public virtual void OnDisabled(){ }
        public virtual void OnHighlight() { }
        public virtual void OnNormal() { }
        public virtual void OnPresseDown() { }
        public virtual void OnPresseUp() { }

        protected BaseCanvas3d canvas3D;
        // Use this for initialization
        protected virtual void Awake()
        {
            canvas3D = ToolObject3d.GetParentComponentScript<BaseCanvas3d>(transform);
            if (canvas3D != null)
                canvas3D.AddChildObject3d(this);
        }
        protected virtual void OnDestroy()
        {
            if (canvas3D != null)
                canvas3D.DelChildObject3d(this);
        }      
    }
}
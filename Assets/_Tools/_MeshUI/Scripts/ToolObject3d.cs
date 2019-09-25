using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Canvas3d
{ 
    public class ToolObject3d 
    {
        public static T GetParentComponentScript<T>(Transform go) where T : Component
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
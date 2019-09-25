using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Canvas3d
{
    public class ButtonObject3d : BaseButton3d
    {
        [HideInInspector]
        public UnityEngine.UI.Button.ButtonClickedEvent OnClick;

        public override void OnPresseDown()
        {
            base.OnPresseDown();
            OnClick.Invoke();
        }
    }
}
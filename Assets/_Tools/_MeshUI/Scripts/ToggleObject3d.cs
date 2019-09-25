using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Canvas3d
{

    public enum GraphicTransition
    {
        //
        // 摘要:
        //     No Transition.
        None = 0,

        Transition = 1,

        GameObject = 2

    }

    public class ToggleObject3d : BaseButton3d
    {
        #region Public Varials

        public bool IsOn
        {
            get { return _IsOn; }
            set
            {
                if (_IsOn != value)
                {
                    _IsOn = value;
                    _IsOnCatch = _IsOn;
                    ValueChanged(value);
                }              
                
            }
        }

        [HideInInspector]
        public GraphicTransition _GraphicTransition = GraphicTransition.None;

        [HideInInspector]
        public Color _GraphicNormalColor = new Color(1, 1, 1, 1);
        [HideInInspector]
        public Color _GraphicHighlightColor = new Color(1, 1, 1, 1);
        [HideInInspector]
        public Color _GraphicPressedColor = new Color(1, 1, 1, 1);

        [HideInInspector]
        public string _GraphicNormalAnim;
        [HideInInspector]
        public string _GraphicHighlightAnim;
        [HideInInspector]
        public string _GraphicPressedAnim;

        [HideInInspector]
        public Texture _GraphicNormalTex;
        [HideInInspector]
        public Texture _GraphicHighlightTex;
        [HideInInspector]
        public Texture _GraphicPresssedTex;


        [HideInInspector]
        public Material _GraphicNormalMat;
        [HideInInspector]
        public Material _GraphicHighlightMat;
        [HideInInspector]
        public Material _GraphicPressedMat;

        [HideInInspector]
        public GameObject _GraphicObject;

        [HideInInspector]
        public UnityEngine.UI.Toggle.ToggleEvent OnValueChange;

        [HideInInspector]
        public UnityEngine.UI.Button.ButtonClickedEvent OnShowGraphic;

        [HideInInspector]
        public UnityEngine.UI.Button.ButtonClickedEvent OnHideGraphic;

        #endregion

        #region Private Varials

        [SerializeField]
        private bool _IsOn = false;

        private bool _IsOnCatch = false;

        #endregion

        protected override void Awake()
        {
            base.Awake();
        }
        protected override void OnValidate()
        {
            base.OnValidate();
            if (_IsOnCatch != _IsOn)
            {
                _IsOnCatch = _IsOn;
                switch (_GraphicTransition)
                {
                    case GraphicTransition.GameObject:
                        if (_GraphicObject != null)
                            _GraphicObject.SetActive(_IsOn);
                        break;
                    case GraphicTransition.Transition:
                        switch (_Transition)
                        {
                            case Transition.ColorTint:
                                OnNormalColorStatus();
                                break;
                            case Transition.SpriteSwap:                            
                                OnNormalTexStatus();
                                break;
                            case Transition.MaterialSwap:                               
                                OnNormalMatStatus();
                                break;
                            case Transition.Animation:
                                OnNormalAnimStatus();
                                break;
                        }
                        break;
                }
            }

        }

        public override void OnPresseDown()
        {
            base.OnPresseDown();
            IsOn = !IsOn;
        }


        #region NormalStatus

        protected override void OnNormalColorStatus()
        {
            if (_GraphicTransition == GraphicTransition.Transition && _IsOn)
            {
                OnStatusColor(_GraphicNormalColor);
            }
            else
            {
                base.OnNormalColorStatus();
            }
        }

        protected override void OnNormalMatStatus()
        {

            if (_GraphicTransition == GraphicTransition.Transition && _IsOn)
            {
                OnStatusMat(_GraphicNormalMat);
            }
            else
            {
                OnStatusMat(_normalMat);
                //base.OnNormalMatStatus();
            }
        }

        protected override void OnNormalAnimStatus()
        {

            if (_GraphicTransition == GraphicTransition.Transition && _IsOn)
            {
                OnStatusAnim(_GraphicNormalAnim, true);
                OnStatusAnim(_GraphicHighlightAnim, false);
                OnStatusAnim(_GraphicPressedAnim, false);
                OnStatusAnim(_disabledAnim, false);
            }
            else
            {
                base.OnNormalAnimStatus();
            }
        }

        protected override void OnNormalTexStatus()
        {

            if (_GraphicTransition == GraphicTransition.Transition && _IsOn)
            {
                OnStatusTex(_GraphicNormalTex);
            }
            else
            {
                base.OnNormalTexStatus();
            }
        }

        #endregion

        #region HighlightStatus

        protected override void OnHighlightColorStatus()
        {
            if (_GraphicTransition == GraphicTransition.Transition && _IsOn)
            {
                OnStatusColor(_GraphicHighlightColor);
            }
            else
            {
                base.OnHighlightColorStatus();
            }
        }

        protected override void OnHighlightMatStatus()
        {
            if (_GraphicTransition == GraphicTransition.Transition && _IsOn)
            {
                OnStatusMat(_GraphicHighlightMat);
            }
            else
            {
                OnStatusMat(_highlightMat);
                //base.OnHighlightMatStatus();
            }
        }

        protected override void OnHighlightAnimStatus()
        {

            if (_GraphicTransition == GraphicTransition.Transition && _IsOn)
            {
                OnStatusAnim(_GraphicNormalAnim, false);
                OnStatusAnim(_GraphicHighlightAnim, true);
                OnStatusAnim(_GraphicPressedAnim, false);
                OnStatusAnim(_disabledAnim, false);
            }
            else
            {
                base.OnHighlightAnimStatus();
            }

        }

        protected override void OnHighlightTexStatus()
        {
            if (_GraphicTransition == GraphicTransition.Transition && _IsOn)
            {
                OnStatusTex(_GraphicHighlightTex);
            }
            else
            {
                base.OnHighlightTexStatus();
            }
        }

        #endregion

        #region PressedStatus

        protected override void OnPressedColorStatus()
        {
            if (_GraphicTransition == GraphicTransition.Transition && _IsOn)
            {
                OnStatusColor(_GraphicPressedColor);
            }
            else
            {
                base.OnPressedColorStatus();
            }
        }

        protected override void OnPressedMatStatus()
        {
            if (_GraphicTransition == GraphicTransition.Transition && _IsOn)
            {
                OnStatusMat(_GraphicPressedMat);
            }
            else
            {
                OnStatusMat(_pressedMat);
                //base.OnPressedMatStatus();
            }
        }

        protected override void OnPressedAnimStatus()
        {
            if (_GraphicTransition == GraphicTransition.Transition && _IsOn)
            {
                OnStatusAnim(_GraphicNormalAnim, false);
                OnStatusAnim(_GraphicHighlightAnim, false);
                OnStatusAnim(_GraphicPressedAnim, true);
                OnStatusAnim(_disabledAnim, false);
            }
            else
            {
                base.OnPressedAnimStatus();
            }
        }

        protected override void OnPressedTexStatus()
        {
            if (_GraphicTransition == GraphicTransition.Transition && _IsOn)
            {
                OnStatusTex(_GraphicPresssedTex);
            }
            else
            {
                base.OnPressedTexStatus();
            }
        }

        #endregion


        private void ValueChanged(bool m_IsOn)
        {
            if (_GraphicTransition == GraphicTransition.GameObject && _GraphicObject != null)
                _GraphicObject.SetActive(m_IsOn);
            else if (_GraphicTransition == GraphicTransition.Transition)
            {
                switch (statusEnum3d)
                {
                    case StatusEnum3d.Normal:
                        OnNormal();
                        break;
                    case StatusEnum3d.Highlight:
                        OnHighlight();
                        break;
                    case StatusEnum3d.Pressed:
                        base.OnPresseDown();
                        break;
                }
            }

            if (!m_IsOn)
            {
                OnHideGraphic.Invoke();
            }
            else
            {
                OnShowGraphic.Invoke();
            }
            OnValueChange.Invoke(m_IsOn);
            OnValueChanged(m_IsOn);
        }

        protected virtual void OnValueChanged(bool m_IsOn) { }
    }
}

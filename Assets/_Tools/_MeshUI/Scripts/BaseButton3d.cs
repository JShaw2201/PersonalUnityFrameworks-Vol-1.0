using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Canvas3d
{
    public abstract class BaseButton3d : BaseObject3d
    {
        #region Public Varials

        [HideInInspector]
        public Color _normalColor = new Color(1, 1, 1, 1);

        [HideInInspector]
        public Color _highlightColor = new Color(0.95f, 0.95f, 0.95f, 1);

        [HideInInspector]
        public Color _pressedColor = new Color(0.78f, 0.78f, 0.78f, 1);

        [HideInInspector]
        public Color _disabledColor = new Color(0.78f, 0.78f, 0.78f, 0.5f);

        [HideInInspector]
        public Texture _normalTex;

        [HideInInspector]
        public Texture _highlightTex;

        [HideInInspector]
        public Texture _pressedTex;

        [HideInInspector]
        public Texture _disabledTex;

        [HideInInspector]
        public Material _normalMat;

        [HideInInspector]
        public Material _highlightMat;

        [HideInInspector]
        public Material _pressedMat;

        [HideInInspector]
        public Material _disabledMat;

        [HideInInspector]
        public string _normalAnim = "Normal";

        [HideInInspector]
        public string _highlightAnim = "Highlighted";

        [HideInInspector]
        public string _pressedAnim = "Pressed";

        [HideInInspector]
        public string _disabledAnim = "Disabled";

        [HideInInspector]
        public Transition _Transition = Transition.ColorTint;

        #endregion

        #region Protected Varials


        protected Renderer _renderer;
        protected Animator _animator;

        #endregion

        #region Unity Method

        protected virtual void OnValidate()
        {
            _renderer = GetComponent<Renderer>();
            switch (_Transition)
            {              
                case Transition.Animation:
                    _animator = GetComponent<Animator>();
                    if (!GetComponent<Animator>())
                        gameObject.AddComponent<Animator>();
                    break;
            }
        }

        protected override void Awake()
        {
            base.Awake();
            _renderer = GetComponent<Renderer>();
            _animator = GetComponent<Animator>();
            OnNormal();
        }

        #endregion

        #region NormalStatus

        protected virtual void OnNormalColorStatus()
        {
            OnStatusColor(_normalColor);
        }

        protected virtual void OnNormalMatStatus()
        {
            OnStatusMat(_normalMat);
        }

        protected virtual void OnNormalAnimStatus()
        {

            OnStatusAnim(_normalAnim, true);
            OnStatusAnim(_highlightAnim, false);
            OnStatusAnim(_pressedAnim, false);
            OnStatusAnim(_disabledAnim, false);
        }

        protected virtual void OnNormalTexStatus()
        {
            OnStatusTex(_normalTex);
        }


        #endregion

        #region HighlightStatus

        protected virtual void OnHighlightColorStatus()
        {
            OnStatusColor(_highlightColor);
        }

        protected virtual void OnHighlightMatStatus()
        {
            OnStatusMat(_highlightMat);
        }

        protected virtual void OnHighlightAnimStatus()
        {
            OnStatusAnim(_normalAnim, false);
            OnStatusAnim(_highlightAnim, true);
            OnStatusAnim(_pressedAnim, false);
            OnStatusAnim(_disabledAnim, false);
        }

        protected virtual void OnHighlightTexStatus()
        {
            OnStatusTex(_highlightTex);
        }

        #endregion

        #region PressedStatus

        protected virtual void OnPressedColorStatus()
        {
            OnStatusColor(_pressedColor);
        }

        protected virtual void OnPressedMatStatus()
        {
            OnStatusMat(_pressedMat);
        }

        protected virtual void OnPressedAnimStatus()
        {
            OnStatusAnim(_normalAnim, false);
            OnStatusAnim(_highlightAnim, false);
            OnStatusAnim(_pressedAnim, true);
            OnStatusAnim(_disabledAnim, false);
        }

        protected virtual void OnPressedTexStatus()
        {
            OnStatusTex(_pressedTex);
        }

        #endregion

        #region DisabledStatus

        protected virtual void OnDisabledColorStatus()
        {
            OnStatusColor(_disabledColor);
        }

        protected virtual void OnDisabledMatStatus()
        {
            OnStatusMat(_disabledMat);
        }

        protected virtual void OnDisabledAnimStatus()
        {
            OnStatusAnim(_normalAnim, false);
            OnStatusAnim(_highlightAnim, false);
            OnStatusAnim(_pressedAnim, false);
            OnStatusAnim(_disabledAnim, true);
        }

        protected virtual void OnDisabledTexStatus()
        {
            OnStatusTex(_disabledTex);
        }

        #endregion

        #region GameObjectCtrl

        protected void OnStatusColor(Color color)
        {
            if (_renderer != null)
            {
                _renderer.material.color = color;
            }
        }

        protected void OnStatusAnim(string anim, bool animActive)
        {
            if (_animator != null)
            {
                _animator.SetBool(anim, animActive);
            }
        }

        protected void OnStatusMat(Material mat)
        {
            if (_renderer != null && mat != null)
            {
                _renderer.sharedMaterial = mat;

            }
        }
        protected void OnStatusTex(Texture tex)
        {
            if (_renderer != null)
            {
                _renderer.material.mainTexture = tex;
            }
        }

        #endregion

        #region BaseObject3d

        public override void OnNormal()
        {
            _StatusEnum3d = StatusEnum3d.Normal;
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
        }

        public override void OnHighlight()
        {
            _StatusEnum3d = StatusEnum3d.Highlight;
            switch (_Transition)
            {
                case Transition.ColorTint:
                    OnHighlightColorStatus();
                    break;
                case Transition.SpriteSwap:
                    OnHighlightTexStatus();
                    break;
                case Transition.MaterialSwap:
                    OnHighlightMatStatus();
                    break;
                case Transition.Animation:
                    OnHighlightAnimStatus();
                    break;
            }
        }

        public override void OnPresseDown()
        {
            _StatusEnum3d = StatusEnum3d.Pressed;
            switch (_Transition)
            {
                case Transition.ColorTint:
                    OnPressedColorStatus();
                    break;
                case Transition.SpriteSwap:
                    OnPressedTexStatus();
                    break;
                case Transition.MaterialSwap:
                    OnPressedMatStatus();
                    break;
                case Transition.Animation:
                    OnPressedAnimStatus();
                    break;
            }

        }

        public override void OnPresseUp()
        {
            OnHighlight();
        }

        public override void OnDisabled()
        {
            _StatusEnum3d = StatusEnum3d.Disabled;
            switch (_Transition)
            {
                case Transition.ColorTint:
                    OnDisabledColorStatus();
                    break;
                case Transition.SpriteSwap:
                    OnDisabledTexStatus();
                    break;
                case Transition.MaterialSwap:
                    OnDisabledMatStatus();
                    break;
                case Transition.Animation:
                    OnDisabledAnimStatus();
                    break;
            }
        }

        #endregion

    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Canvas3d
{
    [CustomEditor(typeof(BaseButton3d))]
    public class BaseBottun3dEditor : Editor
    {       
        protected BaseButton3d _buttonObject3d;
        protected virtual void OnEnable()
        {           
            _buttonObject3d = target as BaseButton3d;
        }

        public override void OnInspectorGUI()
        {
            this.serializedObject.Update();
            base.OnInspectorGUI();
            OnDrawInspectorGUI();
            SetDirt();
        }

        protected virtual void OnDrawInspectorGUI()
        {
            _buttonObject3d.RaycastTarget = EditorGUILayout.Toggle("RaycastTarget", _buttonObject3d.RaycastTarget);
            EditorGUILayout.Space();
            _buttonObject3d._Transition = (Transition)EditorGUILayout.EnumPopup("Transition", _buttonObject3d._Transition);
            switch (_buttonObject3d._Transition)
            {
                case Transition.Animation:
                    _buttonObject3d._normalAnim = EditorGUILayout.TextField("Normal", _buttonObject3d._normalAnim);
                    _buttonObject3d._highlightAnim = EditorGUILayout.TextField("Highlighted", _buttonObject3d._highlightAnim);
                    _buttonObject3d._pressedAnim = EditorGUILayout.TextField("Pressed", _buttonObject3d._pressedAnim);
                    _buttonObject3d._disabledAnim = EditorGUILayout.TextField("Disabled", _buttonObject3d._disabledAnim);
                    break;
                case Transition.ColorTint:
                    _buttonObject3d._normalColor = EditorGUILayout.ColorField("Normal", _buttonObject3d._normalColor);
                    _buttonObject3d._highlightColor = EditorGUILayout.ColorField("Highlighted", _buttonObject3d._highlightColor);
                    _buttonObject3d._pressedColor = EditorGUILayout.ColorField("Pressed", _buttonObject3d._pressedColor);
                    _buttonObject3d._disabledColor = EditorGUILayout.ColorField("Disabled", _buttonObject3d._disabledColor);
                    break;
                case Transition.MaterialSwap:
                    _buttonObject3d._normalMat = (Material)EditorGUILayout.ObjectField("Normal", _buttonObject3d._normalMat, typeof(Material), false);
                    _buttonObject3d._highlightMat = (Material)EditorGUILayout.ObjectField("Highlighted", _buttonObject3d._highlightMat, typeof(Material), false);
                    _buttonObject3d._pressedMat = (Material)EditorGUILayout.ObjectField("Pressed", _buttonObject3d._pressedMat, typeof(Material), false);
                    _buttonObject3d._disabledMat = (Material)EditorGUILayout.ObjectField("Disabled", _buttonObject3d._disabledMat, typeof(Material), false);
                    break;
                case Transition.SpriteSwap:
                    _buttonObject3d._normalTex = (Texture)EditorGUILayout.ObjectField("Normal", _buttonObject3d._normalTex, typeof(UnityEngine.Object), false);
                    _buttonObject3d._highlightTex = (Texture)EditorGUILayout.ObjectField("Highlighted", _buttonObject3d._highlightTex, typeof(UnityEngine.Object), false);
                    _buttonObject3d._pressedTex = (Texture)EditorGUILayout.ObjectField("Pressed", _buttonObject3d._pressedTex, typeof(UnityEngine.Object), false);
                    _buttonObject3d._disabledTex = (Texture)EditorGUILayout.ObjectField("Disabled", _buttonObject3d._disabledTex, typeof(UnityEngine.Object), false);
                    break;
            }
        }

        protected virtual void SetDirt()
        {
            
            serializedObject.ApplyModifiedProperties();
        }
    }
}

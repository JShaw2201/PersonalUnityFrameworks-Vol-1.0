using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Canvas3d
{
    [CustomEditor(typeof(ToggleObject3d), true)]
    public class ToggleObject3dEditor : BaseBottun3dEditor
    {
        protected ToggleObject3d toggle;
        protected SerializedProperty graphicTransitionProperty;
        protected SerializedProperty onValueProperty;
        protected SerializedProperty onShowProperty;
        protected SerializedProperty onHideProperty;
        protected override void OnEnable()
        {
            base.OnEnable();
            toggle = target as ToggleObject3d;
            graphicTransitionProperty = serializedObject.FindProperty("_GraphicTransition");
            onValueProperty = serializedObject.FindProperty("OnValueChange");
            onShowProperty = serializedObject.FindProperty("OnShowGraphic");
            onHideProperty = serializedObject.FindProperty("OnHideGraphic");
        }
        protected override void OnDrawInspectorGUI()
        {
            base.OnDrawInspectorGUI();
            EditorGUILayout.Space();
            //toggle.IsOn = EditorGUILayout.Toggle("IsOn", toggle.IsOn);
            EditorGUILayout.PropertyField(graphicTransitionProperty);
            if (toggle._GraphicTransition == GraphicTransition.Transition)
            {
                switch (toggle._Transition)
                {
                    case Transition.ColorTint:
                        toggle._GraphicNormalColor = EditorGUILayout.ColorField("GraphicNomal", toggle._GraphicNormalColor);
                        toggle._GraphicHighlightColor = EditorGUILayout.ColorField("GraphicHighlight", toggle._GraphicHighlightColor);
                        toggle._GraphicPressedColor = EditorGUILayout.ColorField("GraphicPressed", toggle._GraphicPressedColor);
                        break;
                    case Transition.SpriteSwap:
                        toggle._GraphicNormalTex = (Texture)EditorGUILayout.ObjectField("GraphicNomal", toggle._GraphicNormalTex, typeof(Texture), true);
                        toggle._GraphicHighlightTex = (Texture)EditorGUILayout.ObjectField("GraphicHighlight", toggle._GraphicHighlightTex, typeof(Texture), true);
                        toggle._GraphicPresssedTex = (Texture)EditorGUILayout.ObjectField("GraphicPressed", toggle._GraphicPresssedTex, typeof(Texture), true);
                        break;
                    case Transition.MaterialSwap:
                        toggle._GraphicNormalMat = (Material)EditorGUILayout.ObjectField("GraphicNomal", toggle._GraphicNormalMat, typeof(Material), true);
                        toggle._GraphicHighlightMat = (Material)EditorGUILayout.ObjectField("GraphicHighlight", toggle._GraphicHighlightMat, typeof(Material), true);
                        toggle._GraphicPressedMat = (Material)EditorGUILayout.ObjectField("GraphicPressed", toggle._GraphicPressedMat, typeof(Material), true);
                        break;

                    case Transition.Animation:
                        toggle._GraphicNormalAnim = EditorGUILayout.TextField("GraphicNomal", toggle._GraphicNormalAnim);
                        toggle._GraphicHighlightAnim = EditorGUILayout.TextField("GraphicHighlight", toggle._GraphicHighlightAnim);
                        toggle._GraphicPressedAnim = EditorGUILayout.TextField("GraphicPressed", toggle._GraphicPressedAnim);
                        break;
                }
            }
            else if (toggle._GraphicTransition == GraphicTransition.GameObject)
                toggle._GraphicObject = (GameObject)EditorGUILayout.ObjectField("Graphic", toggle._GraphicObject, typeof(GameObject), true);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(onValueProperty);
            EditorGUILayout.PropertyField(onShowProperty);
            EditorGUILayout.PropertyField(onHideProperty);
        }
    }
}

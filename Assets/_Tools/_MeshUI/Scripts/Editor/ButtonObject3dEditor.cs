using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Canvas3d
{
    [CustomEditor(typeof(ButtonObject3d),true)]
    public class ButtonObject3dEditor : BaseBottun3dEditor
    {
        protected SerializedProperty onClickProperty;
        protected override void OnEnable()
        {
            base.OnEnable();
            onClickProperty = serializedObject.FindProperty("OnClick");
        }

        protected override void OnDrawInspectorGUI()
        {
            base.OnDrawInspectorGUI();
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(onClickProperty);
        }

    }
}

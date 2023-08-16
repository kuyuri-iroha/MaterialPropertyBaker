﻿using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace sui4.MaterialPropertyBaker.Timeline
{
    [CustomEditor(typeof(TargetGroupClip))]
    public class TargetGroupClipInspectorEditor : Editor
    {
        private SerializedProperty _mpbProfileProp;
        private SerializedProperty _editable;
        private Editor _presetEditor;
        private TargetGroupClip Target => (TargetGroupClip)target;

        private void OnEnable()
        {
            _mpbProfileProp = serializedObject.FindProperty("_mpbProfile");
            _editable = serializedObject.FindProperty("_editable");
            if(Target.MpbProfile != null)
                _presetEditor = CreateEditor(Target.MpbProfile);
        }

        public override void OnInspectorGUI()
        {
            PresetRefGUI();
            EditorGUILayout.Separator();
            using (new EditorGUILayout.VerticalScope("box"))
            {
                using (new EditorGUI.DisabledScope(!_editable.boolValue)) 
                    PropertyGroupEditor(Target.MpbProfile);
            }
        }

        private void PresetRefGUI()
        {
            using (var changeCheck = new EditorGUI.ChangeCheckScope())
            {
                EditorGUILayout.PropertyField(_mpbProfileProp, new GUIContent("Preset Profile"));

                // Load Save buttons
                if (Target.MpbProfile != null)
                {
                    EditorGUILayout.PropertyField(_editable, new GUIContent("Edit Preset"));
                }

                if (changeCheck.changed)
                {
                    serializedObject.ApplyModifiedProperties();
                    EditorUtility.SetDirty(target);

                    serializedObject.Update();
                }
            }
        }

        private void PropertyGroupEditor(MpbProfile mpbProfileProp)
        {
            if (mpbProfileProp == null)
                return;
            if (_presetEditor == null)
            {
                _presetEditor = CreateEditor(mpbProfileProp);
            }
            else if (_presetEditor.target != mpbProfileProp)
            {
                DestroyImmediate(_presetEditor);
                _presetEditor = null;
                _presetEditor = CreateEditor(mpbProfileProp);
            }

            if (_presetEditor != null)
            {
                _presetEditor.OnInspectorGUI();
            }
        }
    }
}
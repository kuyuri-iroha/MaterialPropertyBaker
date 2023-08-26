using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace sui4.MaterialPropertyBaker
{
    [CustomEditor(typeof(MpbProfile))]
    public class MpbProfileEditor : Editor
    {
        private readonly List<bool> _propFoldoutList = new();
        private readonly List<bool> _colorsFoldoutList = new();
        private readonly List<bool> _floatsFoldoutList = new();
        private readonly List<bool> _intsFoldoutList = new();
        private bool _globalColorsFoldout = true;
        private bool _globalFloatsFoldout = true;
        private bool _globalIntsFoldout = true;

        private SerializedProperty _globalPropsProp;
        private SerializedProperty _materialPropsListProp;
        private SerializedProperty _materialPropsProp;
        private MpbProfile Target => (MpbProfile)target;

        private void OnEnable()
        {
            if (target == null) return;
            _materialPropsListProp = serializedObject.FindProperty("_materialPropsList");
            _globalPropsProp = serializedObject.FindProperty("_globalProps");
            Validate();
        }

        private string PropFoldoutKeyAt(string id) => $"{Target.name}_propFoldout_{id}";
        private string ColorsFoldoutKeyAt(string id) => $"{Target.name}_colorsFoldout_{id}";
        private string FloatsFoldoutKeyAt(string id) => $"{Target.name}_floatsFoldout_{id}";
        private string IntsFoldoutKeyAt(string id) => $"{Target.name}_intsFoldout_{id}";

        private void Validate()
        {
            for (var i = _propFoldoutList.Count; i < _materialPropsListProp.arraySize; i++)
            {
                var key = string.IsNullOrWhiteSpace(Target.MaterialPropsList[i].ID)
                    ? i.ToString()
                    : Target.MaterialPropsList[i].ID;
                _propFoldoutList.Add(SessionState.GetBool(PropFoldoutKeyAt(key), true));
                _colorsFoldoutList.Add(SessionState.GetBool(ColorsFoldoutKeyAt(key), true));
                _floatsFoldoutList.Add(SessionState.GetBool(FloatsFoldoutKeyAt(key), true));
                _intsFoldoutList.Add(SessionState.GetBool(IntsFoldoutKeyAt(key), true));
            }
        }

        public override void OnInspectorGUI()
        {
            if (target == null) return;
            serializedObject.Update();
            if (_materialPropsListProp.arraySize > _propFoldoutList.Count)
                Validate();

            EditorUtils.WarningGUI(Target.Warnings);
            using (var change = new EditorGUI.ChangeCheckScope())
            {
                // global settings
                using (new EditorGUILayout.VerticalScope("box"))
                {
                    GlobalPropertyGUI(_globalPropsProp);
                }

                using (new GUILayout.VerticalScope("box"))
                {
                    EditorGUILayout.LabelField("Per Property Settings", EditorStyles.boldLabel);
                    EditorGUI.indentLevel++;
                    // per id settings
                    for (var i = 0; i < _materialPropsListProp.arraySize; i++)
                    {
                        _materialPropsProp = _materialPropsListProp.GetArrayElementAtIndex(i);
                        string key, title;
                        if (string.IsNullOrWhiteSpace(Target.MaterialPropsList[i].ID))
                        {
                            key = i.ToString();
                            title = $"Material Property {i}";
                        }
                        else
                        {
                            key = title = Target.MaterialPropsList[i].ID;
                        }

                        _propFoldoutList[i] = EditorGUILayout.Foldout(_propFoldoutList[i], title);
                        SessionState.SetBool(PropFoldoutKeyAt(key), _propFoldoutList[i]);
                        if (_propFoldoutList[i])
                        {
                            EditorGUI.indentLevel++;
                            MaterialPropsGUI(_materialPropsProp, i);
                            EditorGUI.indentLevel--;
                        }
                    }

                    EditorGUI.indentLevel--;
                }

                if (change.changed)
                {
                    serializedObject.ApplyModifiedProperties();
                }
            }
        }

        private void GlobalPropertyGUI(SerializedProperty globalPropsProp)
        {
            var shader = globalPropsProp.FindPropertyRelative("_shader");
            var colors = globalPropsProp.FindPropertyRelative("_colors");
            var floats = globalPropsProp.FindPropertyRelative("_floats");
            var ints = globalPropsProp.FindPropertyRelative("_ints");

            EditorGUILayout.LabelField("Global Properties", EditorStyles.boldLabel);

            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(shader);

            // Colors
            _globalColorsFoldout = EditorGUILayout.Foldout(_globalColorsFoldout, "Colors");
            SessionState.SetBool(ColorsFoldoutKeyAt("global"), _globalColorsFoldout);
            if (_globalColorsFoldout)
            {
                EditorGUI.indentLevel++;
                PropsGUI(colors, Target.GlobalProps, ShaderPropertyType.Color);
                EditorGUI.indentLevel--;
            }

            // Floats
            _globalFloatsFoldout = EditorGUILayout.Foldout(_globalFloatsFoldout, "Floats");
            SessionState.SetBool(FloatsFoldoutKeyAt("global"), _globalFloatsFoldout);
            if (_globalFloatsFoldout)
            {
                EditorGUI.indentLevel++;
                PropsGUI(floats, Target.GlobalProps, ShaderPropertyType.Float);
                EditorGUI.indentLevel--;
            }
            
            // // Ints
            // _globalIntsFoldout = EditorGUILayout.Foldout(_globalIntsFoldout, "Ints");
            // SessionState.SetBool(IntsFoldoutKeyAt("global"), _globalIntsFoldout);
            // if (_globalIntsFoldout)
            // {
            //     EditorGUI.indentLevel++;
            //     PropsGUI(ints, Target.GlobalProps, ShaderPropertyType.Int);
            //     EditorGUI.indentLevel--;
            // }

            EditorGUI.indentLevel--;
        }

        private void MaterialPropsGUI(SerializedProperty materialPropsProp, int index)
        {
            var id = materialPropsProp.FindPropertyRelative("_id");
            var material = materialPropsProp.FindPropertyRelative("_material");
            var shader = materialPropsProp.FindPropertyRelative("_shader");
            var colors = materialPropsProp.FindPropertyRelative("_colors");
            var floats = materialPropsProp.FindPropertyRelative("_floats");
            var ints = materialPropsProp.FindPropertyRelative("_ints");

            EditorGUILayout.PropertyField(id, new GUIContent("ID"));
            EditorGUILayout.PropertyField(material);
            using (new EditorGUI.DisabledScope(material.objectReferenceValue != null))
            {
                EditorGUILayout.PropertyField(shader);
            }

            var key = string.IsNullOrWhiteSpace(id.stringValue) ? index.ToString() : id.stringValue;
            // Colors
            _colorsFoldoutList[index] = EditorGUILayout.Foldout(_colorsFoldoutList[index], "Colors");
            SessionState.SetBool(ColorsFoldoutKeyAt(key), _colorsFoldoutList[index]);
            if (_colorsFoldoutList[index])
            {
                EditorGUI.indentLevel++;
                PropsGUI(colors, Target.MaterialPropsList[index], ShaderPropertyType.Color);
                EditorGUI.indentLevel--;
            }

            // Floats
            _floatsFoldoutList[index] = EditorGUILayout.Foldout(_floatsFoldoutList[index], "Floats");
            SessionState.SetBool(FloatsFoldoutKeyAt(key), _floatsFoldoutList[index]);
            if (_floatsFoldoutList[index])
            {
                EditorGUI.indentLevel++;
                PropsGUI(floats, Target.MaterialPropsList[index], ShaderPropertyType.Float);
                EditorGUI.indentLevel--;
            }
            
            // // Ints
            // _intsFoldoutList[index] = EditorGUILayout.Foldout(_intsFoldoutList[index], "Ints");
            // SessionState.SetBool(IntsFoldoutKeyAt(key), _intsFoldoutList[index]);
            // if (_intsFoldoutList[index])
            // {
            //     EditorGUI.indentLevel++;
            //     PropsGUI(ints, Target.MaterialPropsList[index], ShaderPropertyType.Int);
            //     EditorGUI.indentLevel--;
            // }
        }

        private void PropsGUI(SerializedProperty propsList, MaterialProps matProps, ShaderPropertyType targetPropType)
        {
            if (propsList.arraySize == 0)
            {
                EditorGUILayout.LabelField("List is Empty");
            }

            for (int i = 0; i < propsList.arraySize; i++)
            {
                SerializedProperty prop = propsList.GetArrayElementAtIndex(i);
                var property = prop.FindPropertyRelative("_name");
                var valueProp = prop.FindPropertyRelative("_value");
                var label = Utils.UnderscoresToSpaces(property.stringValue);
                label = label.Length == 0 ? " " : label;
                using (new GUILayout.HorizontalScope())
                {
                    if (targetPropType == ShaderPropertyType.Color)
                        valueProp.colorValue =
                            EditorGUILayout.ColorField(new GUIContent(label), valueProp.colorValue, true, true, true);
                    else
                        EditorGUILayout.PropertyField(valueProp, new GUIContent(label));

                    if (GUILayout.Button("-", GUILayout.Width(20)))
                    {
                        propsList.DeleteArrayElementAtIndex(i);
                        serializedObject.ApplyModifiedProperties();
                    }
                }
            }

            using (new GUILayout.HorizontalScope())
            {
                EditorGUILayout.Space();
                if (GUILayout.Button("+", GUILayout.Width(20)))
                {
                    ShowNewRecorderMenu(matProps, targetPropType);
                }
            }
        }

        private bool IsMatchShaderType(ShaderPropertyType s1, ShaderPropertyType s2)
        {
            if(s1 == s2) return true;
            switch (s1)
            {
                case ShaderPropertyType.Float when s2 == ShaderPropertyType.Range:
                case ShaderPropertyType.Range when s2 == ShaderPropertyType.Float:
                    return true;
                case ShaderPropertyType.Color:
                case ShaderPropertyType.Vector:
                case ShaderPropertyType.Texture:
                case ShaderPropertyType.Int:
                default:
                    return false;
            }
        }

        private void ShowNewRecorderMenu(MaterialProps matProps, ShaderPropertyType targetPropType)
        {
            var addPropertyMenu = new GenericMenu();
            var shader = matProps.Shader;
            for (var pi = 0; pi < shader.GetPropertyCount(); pi++)
            {
                var propName = shader.GetPropertyName(pi);
                var propType = shader.GetPropertyType(pi);
                if(!IsMatchShaderType(targetPropType, propType)) continue;
                switch (propType)
                {
                    // すでに同じ名前のプロパティがある場合は追加しない
                    case ShaderPropertyType.Color when matProps.Colors.All(c => c.Name != propName):
                        AddPropertyToMenu(propName, addPropertyMenu, matProps, propType);
                        break;
                    case ShaderPropertyType.Float or ShaderPropertyType.Range when matProps.Floats.All(f => f.Name != propName):
                        AddPropertyToMenu(propName, addPropertyMenu, matProps, propType);
                        break;
                    case ShaderPropertyType.Int when matProps.Ints.All(f => f.Name != propName):
                        AddPropertyToMenu(propName, addPropertyMenu, matProps, propType);
                        break;
                }
            }

            if (addPropertyMenu.GetItemCount() == 0)
            {
                addPropertyMenu.AddDisabledItem(new GUIContent("No Property to Add"));
            }

            addPropertyMenu.ShowAsContext();
        }

        private void AddPropertyToMenu(string propName, GenericMenu menu, MaterialProps props, ShaderPropertyType propType)
        {
            menu.AddItem(new GUIContent(propName), false, data => OnAddProperty((string)data, props, propType),
                propName);
        }

        private void OnAddProperty(string propName, MaterialProps props, ShaderPropertyType propType)
        {
            var material = props.Material;
            if (propType is ShaderPropertyType.Color)
            {
                var defaultColor = material == null ? Color.black : material.GetColor(propName);
                var matProp = new MaterialProp<Color>(propName, defaultColor);
                props.Colors.Add(matProp);
            }
            else if (propType is ShaderPropertyType.Float or ShaderPropertyType.Range)
            {
                var defaultFloat = material == null ? 0.0f : material.GetFloat(propName);
                var matProp = new MaterialProp<float>(propName, defaultFloat);
                props.Floats.Add(matProp);
            }
            else if (propType is ShaderPropertyType.Int)
            {
                var defaultInt = material == null ? 0 : material.GetInteger(propName);
                var matProp = new MaterialProp<int>(propName, defaultInt);
                props.Ints.Add(matProp);
            }

            EditorUtility.SetDirty(target);
            AssetDatabase.SaveAssetIfDirty(target);
            serializedObject.Update();
        }
    }
}
using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Pigeon.Math
{
    /// <summary>
    /// Serializable EaseFunctions delegate for selecting functions from <see cref="EaseFunctions"/> in the editor
    /// </summary>
    [Serializable]
    public struct EaseFunction
    {
        [SerializeField] EaseFunctions.EaseMode mode;
        EaseFunctions.EvaluateMode evaluateMode;

        /// <summary>
        /// Get or set the ease mode enum & delegate
        /// </summary>
        public EaseFunctions.EaseMode Mode
        {
            get => mode;
            set
            {
                mode = value;
                evaluateMode = EaseFunctions.SetEaseMode(mode);
            }
        }

        public EaseFunction(EaseFunctions.EaseMode mode)
        {
            this.mode = mode;
            evaluateMode = null;
        }

        /// <summary>
        /// Evaluate a 0-1 t value using the selected ease mode
        /// </summary>
        public float Evaluate(float t)
        {
#if UNITY_EDITOR
            evaluateMode = EaseFunctions.SetEaseMode(mode);
#endif
            if (evaluateMode == null)
            {
                evaluateMode = EaseFunctions.SetEaseMode(mode);
            }

            return evaluateMode(t);
        }

#if UNITY_EDITOR
        [CustomPropertyDrawer(typeof(EaseFunction))]
        class OverrideFieldDrawer : PropertyDrawer
        {
            // Draw the property inside the given rect
            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
            {
                // Using BeginProperty / EndProperty on the parent property means that
                // prefab override logic works on the entire property.
                EditorGUI.BeginProperty(position, label, property);

                // Draw label
                position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

                // Don't make child fields be indented
                var indent = EditorGUI.indentLevel;
                EditorGUI.indentLevel = 0;

                // Draw fields - passs GUIContent.none to each so they are drawn without labels
                EditorGUI.PropertyField(position, property.FindPropertyRelative("mode"), GUIContent.none);
                //((EaseFunction)Util.EditorUtil.GetTargetObjectOfProperty(property)).UpdateEvaluateMode();

                // Set indent back to what it was
                EditorGUI.indentLevel = indent;

                EditorGUI.EndProperty();
            }
        }
#endif
    }
}
/// Credit Ben MacKinnon @Dover8
/// Sourced from - https://github.com/Dover8/Unity-UI-Extensions/tree/range-slider
/// Usage: Extension of the standard slider. Two handles determine a low and high value between a Min and Max. 
/// Raises a UnityEvent passing the low and high values

using UnityEditor;
using UnityEditor.UI;

namespace UnityEngine.UI.Extensions
{
    [CustomEditor(typeof(RangeSlider), true)]
    [CanEditMultipleObjects]
    public class RangeSliderEditor : SelectableEditor
    {
        SerializedProperty m_Direction;
        SerializedProperty m_LowHandleRect;
        SerializedProperty m_HighHandleRect;
        SerializedProperty m_FillRect;

        SerializedProperty m_MinValue;
        SerializedProperty m_MaxValue;
        SerializedProperty m_WholeNumbers;

        SerializedProperty m_LowValue;
        SerializedProperty m_HighValue;

        //need ref values for the editor MinMaxSlider
        float low = 0;
        float high = 1;

        SerializedProperty m_OnValueChanged;


        protected override void OnEnable()
        {
            base.OnEnable();
            m_LowHandleRect = serializedObject.FindProperty("m_LowHandleRect");
            m_HighHandleRect = serializedObject.FindProperty("m_HighHandleRect");
            m_FillRect = serializedObject.FindProperty("m_FillRect");
            m_Direction = serializedObject.FindProperty("m_Direction");

            m_MinValue = serializedObject.FindProperty("m_MinValue");
            m_MaxValue = serializedObject.FindProperty("m_MaxValue");
            m_WholeNumbers = serializedObject.FindProperty("m_WholeNumbers");

            m_LowValue = serializedObject.FindProperty("m_LowValue");
            low = m_LowValue.floatValue;
            m_HighValue = serializedObject.FindProperty("m_HighValue");
            high = m_HighValue.floatValue;

            m_OnValueChanged = serializedObject.FindProperty("m_OnValueChanged");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.Space();

            serializedObject.Update();
            //grab the updated value affected by m_WholeNumbers
            low = m_LowValue.floatValue;
            high = m_HighValue.floatValue;

            EditorGUILayout.PropertyField(m_LowHandleRect);
            EditorGUILayout.PropertyField(m_HighHandleRect);
            EditorGUILayout.PropertyField(m_FillRect);

            if (m_LowHandleRect.objectReferenceValue != null && m_HighHandleRect.objectReferenceValue != null)
            {
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(m_Direction);
                if (EditorGUI.EndChangeCheck())
                {
                    RangeSlider.Direction direction = (RangeSlider.Direction)m_Direction.enumValueIndex;
                    foreach (var obj in serializedObject.targetObjects)
                    {
                        RangeSlider rangeSlider = obj as RangeSlider;
                        rangeSlider.SetDirection(direction, true);
                    }
                }

                EditorGUILayout.PropertyField(m_MinValue);
                EditorGUILayout.PropertyField(m_MaxValue);
                EditorGUILayout.PropertyField(m_WholeNumbers);

                //We're going to do a fair bit of layout here
                EditorGUILayout.BeginHorizontal();
                //Low Label and value
                EditorGUILayout.BeginVertical();
                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.Label("Low");
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
                low = EditorGUILayout.DelayedFloatField(low, GUILayout.MaxWidth(100));
                EditorGUILayout.EndVertical();

                GUILayout.FlexibleSpace();

                //Slider
                EditorGUILayout.BeginVertical();
                GUILayout.FlexibleSpace();
                EditorGUILayout.MinMaxSlider(ref low, ref high, m_MinValue.floatValue, m_MaxValue.floatValue, GUILayout.ExpandWidth(true));
                EditorGUILayout.EndVertical();

                GUILayout.FlexibleSpace();

                //High label and value
                EditorGUILayout.BeginVertical();
                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.Label("High");
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
                high = EditorGUILayout.DelayedFloatField(high, GUILayout.MaxWidth(100));
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndHorizontal();

                m_LowValue.floatValue = low;
                m_HighValue.floatValue = high;

                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(m_OnValueChanged);
            }
            else
            {
                EditorGUILayout.HelpBox("Specify a RectTransform for the RangeSlider fill or the RangeSlider handles or both. Each must have a parent RectTransform that it can slide within.", MessageType.Info);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }

}

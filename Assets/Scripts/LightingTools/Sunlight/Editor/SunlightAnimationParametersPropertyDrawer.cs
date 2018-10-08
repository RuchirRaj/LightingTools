using UnityEditor;
using UnityEngine;
using LightingTools;

[CustomPropertyDrawer(typeof(SunlightAnimationParameters))]
public class SunlightAnimationParametersPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        EditorGUI.indentLevel = 0;
        EditorLightingUtilities.DrawSplitter();

        property.FindPropertyRelative("animate").boolValue = EditorLightingUtilities.DrawHeader("Animation", property.FindPropertyRelative("animate").boolValue);
        EditorGUI.indentLevel = 1;

        if (property.FindPropertyRelative("animate").boolValue)
        {
            EditorGUILayout.PropertyField(property.FindPropertyRelative("animationMode"));
            if(property.FindPropertyRelative("animationMode").enumValueIndex>0)
            {
                EditorGUILayout.PropertyField(property.FindPropertyRelative("dayLength"));
                EditorGUILayout.PropertyField(property.FindPropertyRelative("colorGradient"));
            }
        }

        EditorGUI.EndProperty();
    }
}
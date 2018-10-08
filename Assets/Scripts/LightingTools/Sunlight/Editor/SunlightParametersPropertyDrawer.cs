using UnityEditor;
using UnityEngine;
using LightingTools;

[CustomPropertyDrawer(typeof(SunlightParameters))]
public class SunlightParametersPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        EditorGUI.indentLevel = 0;
        EditorLightingUtilities.DrawSplitter();

        property.FindPropertyRelative("animationParameters.animate").boolValue = EditorLightingUtilities.DrawHeader("Animation", property.FindPropertyRelative("animationParameters.animate").boolValue);
        EditorGUI.indentLevel = 1;

        if (property.FindPropertyRelative("animationParameters.animate").boolValue)
        {
            EditorGUILayout.PropertyField(property.FindPropertyRelative("animationParameters.animationMode"));
            if (property.FindPropertyRelative("animationParameters.animationMode").enumValueIndex > 0)
            {
                EditorGUILayout.PropertyField(property.FindPropertyRelative("animationParameters.dayLength"));
                EditorGUILayout.PropertyField(property.FindPropertyRelative("animationParameters.colorGradient"));
            }
        }

        EditorGUI.indentLevel = 0;
        EditorLightingUtilities.DrawSplitter();

        property.FindPropertyRelative("orientationParameters.yAxis").isExpanded = EditorLightingUtilities.DrawHeaderFoldout("Orientation", property.FindPropertyRelative("orientationParameters.yAxis").isExpanded);
        EditorGUI.indentLevel = 1;

        if (property.FindPropertyRelative("orientationParameters.yAxis").isExpanded)
        {
            EditorGUILayout.PropertyField(property.FindPropertyRelative("orientationParameters.yAxis"));
            EditorGUILayout.PropertyField(property.FindPropertyRelative("orientationParameters.lattitude"));
            EditorGUILayout.PropertyField(property.FindPropertyRelative("orientationParameters.timeOfDay"), GUILayout.MaxWidth(EditorGUIUtility.labelWidth + 250 + EditorGUIUtility.fieldWidth));
            if (property.FindPropertyRelative("lightParameters.lightCookie").objectReferenceValue != null)
            {
                EditorGUILayout.PropertyField(property.FindPropertyRelative("orientationParameters.Roll"));
            }
        }

        EditorGUI.indentLevel = 0;
        EditorLightingUtilities.DrawSplitter();
        property.FindPropertyRelative("lightParameters.intensity").isExpanded = EditorLightingUtilities.DrawHeaderFoldout("Light", property.FindPropertyRelative("lightParameters.intensity").isExpanded);
        EditorGUI.indentLevel = 1;

        if(property.FindPropertyRelative("lightParameters.intensity").isExpanded)
        {
            EditorGUILayout.PropertyField(property.FindPropertyRelative("lightParameters.intensity"));
            if (property.FindPropertyRelative("animationParameters.animationMode").enumValueIndex != 1)
                EditorGUILayout.PropertyField(property.FindPropertyRelative("lightParameters.colorFilter"));
            
            //EditorGUILayout.PropertyField(property.FindPropertyRelative("useColorTemperature"));
            //if (property.FindPropertyRelative("useColorTemperature").boolValue == true)
            //{
            //    EditorGUILayout.PropertyField(property.FindPropertyRelative("colorTemperature"));
           // }
            EditorGUILayout.PropertyField(property.FindPropertyRelative("lightParameters.mode"), new GUIContent("Light mode / Indirect Intensity"), GUILayout.MaxWidth(EditorGUIUtility.labelWidth + 80));
            var modeRect = GUILayoutUtility.GetLastRect();
            var indirectRect = new Rect(modeRect.x + EditorGUIUtility.labelWidth + 100, modeRect.y, position.xMax * 0.3f, EditorGUIUtility.singleLineHeight);
            EditorGUI.PropertyField(indirectRect, property.FindPropertyRelative("lightParameters.indirectIntensity"), GUIContent.none);
            EditorGUILayout.PropertyField(property.FindPropertyRelative("lightParameters.lightCookie"));
            if(property.FindPropertyRelative("lightParameters.lightCookie").objectReferenceValue != null)
                EditorGUILayout.PropertyField(property.FindPropertyRelative("lightParameters.cookieSize"));
        }
        

        EditorGUI.indentLevel = 0;
        EditorLightingUtilities.DrawSplitter();
        property.FindPropertyRelative("lightParameters.enableShadows").boolValue = property.FindPropertyRelative("lightParameters.shadows").enumValueIndex == 0 ? false : true;
        property.FindPropertyRelative("lightParameters.enableShadows").boolValue = EditorLightingUtilities.DrawHeader("Shadows", property.FindPropertyRelative("lightParameters.enableShadows").boolValue);
        EditorGUI.indentLevel = 1;

        if (property.FindPropertyRelative("lightParameters.enableShadows").boolValue)
            property.FindPropertyRelative("lightParameters.shadows").enumValueIndex = (int)Mathf.Clamp(property.FindPropertyRelative("lightParameters.shadows").enumValueIndex, 1, 2);
        else
            property.FindPropertyRelative("lightParameters.shadows").enumValueIndex = 0;

        if (property.FindPropertyRelative("lightParameters.shadows").enumValueIndex != 0)
        {
            EditorGUILayout.PropertyField(property.FindPropertyRelative("lightParameters.shadows"));
            EditorGUILayout.PropertyField(property.FindPropertyRelative("lightParameters.shadowQuality"));
            EditorGUILayout.PropertyField(property.FindPropertyRelative("lightParameters.shadowBias"));
            EditorGUILayout.PropertyField(property.FindPropertyRelative("lightParameters.shadowNormalBias"));
            EditorGUILayout.PropertyField(property.FindPropertyRelative("lightParameters.shadowMaxDistance"));
        }

        EditorGUI.indentLevel = 0;
        EditorLightingUtilities.DrawSplitter();
        property.FindPropertyRelative("proceduralSkyParameters.sunSize").isExpanded = EditorLightingUtilities.DrawHeaderFoldout("Procedural Skybox", property.FindPropertyRelative("proceduralSkyParameters.sunSize").isExpanded);
        EditorGUI.indentLevel = 1;

        if(property.FindPropertyRelative("proceduralSkyParameters.sunSize").isExpanded)
        {
            EditorGUILayout.PropertyField(property.FindPropertyRelative("proceduralSkyParameters.sunSize"));
            EditorGUILayout.PropertyField(property.FindPropertyRelative("proceduralSkyParameters.atmosphereThickness"));
            EditorGUILayout.PropertyField(property.FindPropertyRelative("proceduralSkyParameters.skyTint"));
            EditorGUILayout.PropertyField(property.FindPropertyRelative("proceduralSkyParameters.Ground"));
            EditorGUILayout.PropertyField(property.FindPropertyRelative("proceduralSkyParameters.exposure"));
        }



        EditorGUI.EndProperty();
    }
}
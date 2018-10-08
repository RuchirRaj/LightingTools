using LightUtilities;
using UnityEditor;
using UnityEngine;

// IngredientDrawer
[CustomPropertyDrawer (typeof (LightParameters))]
public class LightParametersPropertyDrawer : PropertyDrawer {

    SerializedProperty cullingMask;

	// Draw the property inside the given rect
	public override void OnGUI (Rect position, SerializedProperty property, GUIContent label) 
	{
		// Using BeginProperty / EndProperty on the parent property means that
		// prefab override logic works on the entire property.
		EditorGUI.BeginProperty (position, label, property);

        EditorGUI.indentLevel--;
        LightUIUtilities.DrawSplitter();
        LightUIUtilities.DrawHeader("Light");
        EditorGUI.indentLevel++;

		EditorGUILayout.PropertyField (property.FindPropertyRelative ("intensity"));
        EditorGUILayout.PropertyField(property.FindPropertyRelative("colorFilter"));
        EditorGUILayout.PropertyField(property.FindPropertyRelative("type"));
        EditorGUILayout.PropertyField(property.FindPropertyRelative ("mode"), new GUIContent("Light mode"));
		EditorGUILayout.PropertyField(property.FindPropertyRelative("range"));
		EditorGUILayout.PropertyField(property.FindPropertyRelative("indirectIntensity"));
		EditorGUILayout.PropertyField(property.FindPropertyRelative("lightCookie"));
		
		if (property.FindPropertyRelative("lightCookie").objectReferenceValue != null)
		{
			EditorGUILayout.PropertyField(property.FindPropertyRelative("cookieSize"));	
		}

        // Draw label
        EditorGUILayout.Space();
        EditorGUI.indentLevel--;
        LightUIUtilities.DrawSplitter();
        LightUIUtilities.DrawHeader("Shape");
        EditorGUI.indentLevel++;
        EditorGUILayout.PropertyField(property.FindPropertyRelative("shape"));
        if (property.FindPropertyRelative("type").enumValueIndex == 0) //if spotlight
		{
			EditorGUILayout.PropertyField (property.FindPropertyRelative ("lightAngle"));
			EditorGUILayout.PropertyField (property.FindPropertyRelative ("innerSpotPercent"));
			EditorGUILayout.PropertyField (property.FindPropertyRelative ("maxSmoothness"));
		}
		
		// Draw label
		EditorGUILayout.Space();
        EditorGUI.indentLevel--;
        LightUIUtilities.DrawSplitter();
        LightUIUtilities.DrawHeader("Shadows");
        EditorGUI.indentLevel++;
        // Draw fields
        EditorGUILayout.PropertyField (property.FindPropertyRelative ("shadows"));
		if (property.FindPropertyRelative("shadows").enumValueIndex != 0)
		{
			EditorGUILayout.PropertyField (property.FindPropertyRelative ("ShadowNearClip"));
			EditorGUILayout.PropertyField (property.FindPropertyRelative ("shadowResolution"));
            EditorGUILayout.PropertyField(property.FindPropertyRelative("shadowStrength"));
            EditorGUILayout.PropertyField(property.FindPropertyRelative("shadowBias"));
            EditorGUILayout.PropertyField(property.FindPropertyRelative("shadowNormalBias"));
        }
        

        // Draw label
        EditorGUILayout.Space();
        EditorGUI.indentLevel--;
        LightUIUtilities.DrawSplitter();
        cullingMask = property.FindPropertyRelative("cullingMask");
        cullingMask.isExpanded = LightUIUtilities.DrawHeaderFoldout("Additional settings",cullingMask.isExpanded);
        EditorGUI.indentLevel++;

        if(cullingMask.isExpanded)
        {
            EditorGUILayout.PropertyField(cullingMask);
            EditorGUILayout.PropertyField(property.FindPropertyRelative("affectDiffuse"));
            EditorGUILayout.PropertyField(property.FindPropertyRelative("affectSpecular"));
            EditorGUILayout.PropertyField(property.FindPropertyRelative("applyRangeAttenuation"));
            EditorGUILayout.PropertyField(property.FindPropertyRelative("fadeDistance"));
            EditorGUILayout.PropertyField(property.FindPropertyRelative("shadowFadeDistance"));
        }

        EditorGUI.EndProperty ();
	}

}

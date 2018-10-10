using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using EditorLightUtilities;

[CustomEditor(typeof(Sunlight))]
public class SunlightEditor : Editor
{
    [MenuItem("GameObject/Light/Sunlight", false, 10)]
    static void CreateCustomGameObject(MenuCommand menuCommand)
    {
        // Create a custom game object
        GameObject sunlight = new GameObject("Sunlight");
        // Ensure it gets reparented if this was a context click (otherwise does nothing)
        GameObjectUtility.SetParentAndAlign(sunlight, menuCommand.context as GameObject);
        // Register the creation in the undo system
        Undo.RegisterCreatedObjectUndo(sunlight, "Create " + sunlight.name);
        Selection.activeObject = sunlight;
        sunlight.AddComponent<Sunlight>();
    }

    public Sunlight sunlight;
    SerializedProperty intensityCurve;
    SerializedProperty gradient;
    SerializedProperty sunlightParameters;
    SerializedProperty drawGizmo;
    SerializedProperty gizmoSize;
    public List<Vector3> lightPivots;

    void OnEnable()
    {
        sunlight = (Sunlight)serializedObject.targetObject;
        sunlightParameters = serializedObject.FindProperty("sunlightParameters");
        intensityCurve = serializedObject.FindProperty("sunlightParameters.intensityCurve");
        gradient = serializedObject.FindProperty("sunlightParameters.colorGradient");
        drawGizmo = serializedObject.FindProperty("drawGizmo");
        gizmoSize = serializedObject.FindProperty("gizmoSize");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        GUILayout.Space(EditorGUIUtility.singleLineHeight);
        EditorGUILayout.LabelField("Default Values", EditorStyles.boldLabel);

        EditorGUILayout.PropertyField(sunlightParameters, true);

        LightUIUtilities.DrawSplitter();
        LightUIUtilities.DrawHeader("Visualization");
        EditorGUI.indentLevel = 1;

        EditorGUILayout.PropertyField(drawGizmo);
        EditorGUILayout.PropertyField(gizmoSize);

        serializedObject.ApplyModifiedProperties();
    }
}

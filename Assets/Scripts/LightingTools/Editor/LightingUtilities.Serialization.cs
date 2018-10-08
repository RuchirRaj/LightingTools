﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Experimental.Rendering.HDPipeline;
#if UNITY_EDITOR
using UnityEditor;
#endif
using Object = UnityEngine.Object;

namespace LightUtilities
{
#if UNITY_EDITOR
    public static class EditorLightingUtilities
    {
        public static void AssignSerializedProperty(SerializedProperty sp, object source)
        {
            var valueType = source.GetType();
            if (valueType.IsEnum)
            {
                sp.enumValueIndex = (int)source;
            }
            else if (valueType == typeof(Color))
            {
                sp.colorValue = (Color)source;
            }
            else if (valueType == typeof(float))
            {
                sp.floatValue = (float)source;
            }
            else if (valueType == typeof(Vector3))
            {
                sp.vector3Value = (Vector3)source;
            }
            else if (valueType == typeof(bool))
            {
                sp.boolValue = (bool)source;
            }
            else if (valueType == typeof(string))
            {
                sp.stringValue = (string)source;
            }
            else if (typeof(int).IsAssignableFrom(valueType))
            {
                sp.intValue = (int)source;
            }
            else if (typeof(Object).IsAssignableFrom(valueType))
            {
                sp.objectReferenceValue = (Object)source;
            }
            else
            {
                Debug.LogError("Missing type : " + valueType);
            }
        }
    }
#endif
}
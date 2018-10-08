using System.Collections;
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
    [System.Serializable]
    public class SunlightOrientationParameters
    {
        [Range(-180f, 180f)]
        public float yAxis = 0f;
        [Range(0f, 24f)]
        public float timeOfDay = 10f;
        [Range(0f, 90f)]
        public float lattitude = 35f;
        [Range(-180f, 180f)]
        public float Roll = 0.1f;
    }

        [System.Serializable]
    public class SunlightParameters
    {
        public SunlightAnimationParameters animationParameters = new SunlightAnimationParameters();
        public SunlightOrientationParameters orientationParameters = new SunlightOrientationParameters();
        public LightParameters lightParameters = new LightParameters();
        public ProceduralSkyboxParameters proceduralSkyParameters = new ProceduralSkyboxParameters();
    }

    [System.Serializable]
    public enum SunlightAnimationMode
    {
        Custom = 0,
        DayCycle = 1
    }

    [System.Serializable]
    public class SunlightAnimationParameters
    {
        public bool animate;
        public SunlightAnimationMode animationMode;
        public float dayLength = 1;
        public Gradient colorGradient;
    }

    [System.Serializable]
    public class ProceduralSkyboxParameters
    {
        [Range(0.01f,1)]
        public float sunSize = 0.05f;
        [Range(0.1f,4.5f)]
        public float atmosphereThickness = 1;
        public Color skyTint = Color.gray;
        public Color Ground = Color.gray;
        [Range(0,8)]
        public float exposure = 1.5f;
    }

    public static class SunlightLightingUtilities
    {
        public static SunlightParameters LerpSunlightParameters(SunlightParameters from, SunlightParameters to, float weight)
        {
            var lerpSunlightParameters = new SunlightParameters();
            //Orientation
            lerpSunlightParameters.orientationParameters.lattitude = Mathf.Lerp(from.orientationParameters.lattitude, to.orientationParameters.lattitude, weight);
            lerpSunlightParameters.orientationParameters.yAxis = Mathf.Lerp(from.orientationParameters.yAxis, to.orientationParameters.yAxis, weight);
            lerpSunlightParameters.orientationParameters.timeOfDay = Mathf.Lerp(from.orientationParameters.timeOfDay, to.orientationParameters.timeOfDay, weight);

            lerpSunlightParameters.lightParameters = LightingUtilities.LerpLightParameters(from.lightParameters, to.lightParameters, weight);

            return lerpSunlightParameters;
        }

        public static void SetProceduralSkyboxParameters(ProceduralSkyboxParameters skyParameters)
        {
            var skyboxMaterial = RenderSettings.skybox;
            if(skyboxMaterial.shader.name == "Skybox/Procedural")
            {
                skyboxMaterial.SetFloat("_SunSize", skyParameters.sunSize);
                skyboxMaterial.SetFloat("_AtmosphereThickness", skyParameters.atmosphereThickness);
                skyboxMaterial.SetColor("_SkyTint", skyParameters.skyTint);
                skyboxMaterial.SetColor("_GroundColor", skyParameters.Ground);
                skyboxMaterial.SetFloat("_Exposure", skyParameters.exposure);
            }
            else
            {
                Debug.Log("Skybox material not using Procedural Skybox shader");
            }
        }
    }
}
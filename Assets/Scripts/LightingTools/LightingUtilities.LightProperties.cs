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
    public enum LightmapPresetBakeType
    {
        //Simplify serialization
        Baked = 0,
        Mixed = 1,
        Realtime = 2
    }

    public enum ShadowQuality
    {
        FromQualitySettings = 0,
        Low = 1,
        Medium = 2,
        High = 3,
        VeryHigh = 4
    }

    [System.Serializable]
    public enum LightShape
    {
        Point = 0,
        Spot = 1,
        Directional = 2,
        Rectangle = 3,
        Sphere = 4,
        Line = 5,
        Disc = 6,
        Frustum = 7
    }

    [System.Serializable]
    public class CascadeParameters
    {
        [Range(1, 4)]
        public int count = 4;
        [Range(0, 1)]
        public float split0 = 0.05f;
        [Range(0, 1)]
        public float split1 = 0.1f;
        [Range(0, 1)]
        public float split2 = 0.2f;
    }

    [System.Serializable]
	public class LightParameters
	{
        public LightParameters() { }

        public LightParameters(LightType specificType, LightmapPresetBakeType specificBakeMode)
        {
            type = specificType;
            mode = specificBakeMode;
        }

        public LightParameters(LightType specificType, LightmapPresetBakeType specificBakeMode, bool isNeutral)
        {
            if(isNeutral)
            {
                range = 0;
                intensity = 0;
                colorFilter = Color.black;
                indirectIntensity = 0;
                lightAngle = 0;
                innerSpotPercent = 0;
                cookieSize = 0;
                ShadowNearClip = 0;
                shadowStrength = 0;
                shadowBias = 0;
                shadowNormalBias = 0;
                maxSmoothness = 0;
                fadeDistance = 0;
                shadowFadeDistance = 0;
                shadowResolution = 0;
            }
            type = specificType;
            mode = specificBakeMode;
        }

        public LightType type = LightType.Point;
        public LightmapPresetBakeType mode = LightmapPresetBakeType.Mixed;
		public float range = 8;
		public bool useColorTemperature;
		public float colorTemperature = 6500;
		public Color colorFilter = Color.white;
		public float intensity = 1;
		public float indirectIntensity = 1;
        [Range(0,180)]
        public float lightAngle = 45;
		public LightShadows shadows = LightShadows.Soft ;
        public ShadowQuality shadowQuality = ShadowQuality.Medium;
		[Range(0.01f,10f)]
		public float ShadowNearClip = 0.1f;
        public float shadowBias = 0.01f;
        public float shadowNormalBias = 0.1f;
        public Texture lightCookie;
		public float cookieSize = 5 ;
        public LightShape shape = LightShape.Point;
		[Range(0f,100f)]
        public float innerSpotPercent = 40;
        public float length;
        public float width;
        public float fadeDistance = 50;
        public float shadowFadeDistance = 10 ;
        public bool affectDiffuse = true;
        public bool affectSpecular = true;
        [Range(0, 1)]
        public float shadowStrength = 1;
        public LayerMask cullingMask = -1 ;
		[Range(0,1)]
		public float maxSmoothness = 1;
		public int shadowResolution = 128;
        public bool applyRangeAttenuation = true;
    }

    public static class LightingUtilities
    {

        public static void ApplyLightParameters(Light light, LightParameters lightParameters)
        {
			//HD
			var additionalLightData = light.gameObject.GetComponent<HDAdditionalLightData>();
			var additionalShadowData = light.gameObject.GetComponent<AdditionalShadowData>();

            light.type = lightParameters.type;

#if UNITY_EDITOR
            switch (lightParameters.mode)
            {
                case LightmapPresetBakeType.Realtime: light.lightmapBakeType = LightmapBakeType.Realtime; break;
                case LightmapPresetBakeType.Baked: light.lightmapBakeType = LightmapBakeType.Baked; break;
                case LightmapPresetBakeType.Mixed: light.lightmapBakeType = LightmapBakeType.Mixed; break;
            }
#endif
            light.shadows = lightParameters.shadows;
            light.shadowStrength = lightParameters.shadowStrength;
            light.shadowNearPlane = lightParameters.ShadowNearClip;
            light.shadowResolution = (LightShadowResolution)lightParameters.shadowQuality;
            light.shadowNormalBias = lightParameters.shadowNormalBias;
            light.shadowBias = lightParameters.shadowBias;
            light.intensity = lightParameters.intensity;
            light.color = lightParameters.colorFilter;
            light.range = lightParameters.range;
            light.spotAngle = lightParameters.lightAngle;
            light.cookie = lightParameters.lightCookie;
            light.cullingMask = lightParameters.cullingMask;

			additionalLightData.affectDiffuse = lightParameters.affectDiffuse;
			additionalLightData.affectSpecular = lightParameters.affectSpecular;
			additionalLightData.maxSmoothness = lightParameters.maxSmoothness;
			additionalLightData.fadeDistance = lightParameters.fadeDistance;
			additionalLightData.m_InnerSpotPercent = lightParameters.innerSpotPercent;
            additionalLightData.applyRangeAttenuation = lightParameters.applyRangeAttenuation;

			additionalShadowData.shadowFadeDistance = lightParameters.shadowFadeDistance;
			additionalShadowData.shadowResolution = lightParameters.shadowResolution;
			additionalShadowData.shadowDimmer = lightParameters.shadowStrength;
        }

        public static LightParameters LerpLightParameters(LightParameters from, LightParameters to, float weight)
        {
            var lerpLightParameters = new LightParameters();

            lerpLightParameters.intensity = Mathf.Lerp(from.intensity, to.intensity, weight);
            lerpLightParameters.indirectIntensity = Mathf.Lerp(from.indirectIntensity, to.indirectIntensity, weight);
            lerpLightParameters.range = Mathf.Lerp(from.range, to.range, weight);
            lerpLightParameters.lightAngle = Mathf.Lerp(from.lightAngle, to.lightAngle, weight);
            lerpLightParameters.type = from.type;
            lerpLightParameters.colorFilter = Color.Lerp(from.colorFilter, to.colorFilter, weight);
			lerpLightParameters.maxSmoothness = Mathf.Lerp (from.maxSmoothness, to.maxSmoothness, weight);
			lerpLightParameters.innerSpotPercent = Mathf.Lerp (from.innerSpotPercent, to.innerSpotPercent, weight);
            
            if (from.shadows == LightShadows.None && to.shadows != LightShadows.None)
            {
                lerpLightParameters.shadows = to.shadows;
            }
            if (from.shadows != LightShadows.None && to.shadows == LightShadows.None)
            {
                lerpLightParameters.shadows = from.shadows;
            }
            if (from.shadows == LightShadows.None && to.shadows == LightShadows.None)
            {
                lerpLightParameters.shadows = LightShadows.None;
            }

			lerpLightParameters.lightCookie = weight > 0.5f ? to.lightCookie : from.lightCookie;
            lerpLightParameters.fadeDistance = Mathf.Lerp(from.fadeDistance, to.fadeDistance, weight);
            lerpLightParameters.shadowStrength = Mathf.Lerp(from.shadowStrength, to.shadowStrength, weight);
            lerpLightParameters.shadowBias = Mathf.Lerp(from.shadowBias, to.shadowBias, weight);
            lerpLightParameters.shadowNormalBias = Mathf.Lerp(from.shadowNormalBias, to.shadowNormalBias, weight);
            lerpLightParameters.shadowFadeDistance = Mathf.Lerp(from.shadowFadeDistance, to.shadowFadeDistance, weight);
            lerpLightParameters.ShadowNearClip = Mathf.Lerp(from.ShadowNearClip, to.ShadowNearClip, weight);
			lerpLightParameters.shadowResolution = (int)Mathf.Lerp(from.shadowResolution, to.shadowResolution, weight);

			lerpLightParameters.affectDiffuse = weight > 0.5f ? to.affectDiffuse : from.affectDiffuse;
			lerpLightParameters.affectSpecular = weight > 0.5f ? to.affectSpecular : from.affectSpecular ;

			lerpLightParameters.cullingMask = weight > 0.5f ? to.cullingMask : from.cullingMask ;
			lerpLightParameters.shadowQuality = weight > 0.5f ? to.shadowQuality : from.shadowQuality ;

            return lerpLightParameters;
        }
    }
}
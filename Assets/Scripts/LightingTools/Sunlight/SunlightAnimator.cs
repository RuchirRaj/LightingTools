using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(Sunlight))]
public class SunlightAnimator : MonoBehaviour {

    public bool animateOrientation;
    [Range(-180f, 180f)]
    public float Yaw = 0.1f;
    [Range(90f, -90f)]
    public float Pitch = 0.1f;
    [Range(-180f, 180f)]
    public float Roll = 0.1f;

	public bool animateTransform;
    public float distance = 2f;
    public Vector3 offset;
    
	public bool animateLightProperties;
	public float range = 3;
    public float colorTemperature = 6500;
    public Color colorFilter = Color.white;
    public float intensity = 1;
    public float indirectIntensity = 1;
    [Range(0f, 180f)]
    public float lightAngle = 45;
    public LightShadows shadows = LightShadows.Soft;
    public LightShadowResolution shadowsResolution = LightShadowResolution.High;
    public LightUtilities.ShadowQuality shadowQuality = LightUtilities.ShadowQuality.High;
    [Range(0.01f, 10f)]
    public float ShadowNearClip = 0.1f;
    [Range(0f, 2f)]
    public float shadowBias = 0.005f;

	private Sunlight lightTarget;

	void OnEnable()
	{
		intensity = lightTarget.sunlightParameters.lightParameters.intensity;
		indirectIntensity = lightTarget.sunlightParameters.lightParameters.indirectIntensity;
		//colorTemperature =lightTarget.sunlightParameters.colorTemperature;
		colorFilter=lightTarget.sunlightParameters.lightParameters.colorFilter;
		shadows=lightTarget.sunlightParameters.lightParameters.shadows;
        shadowQuality = lightTarget.sunlightParameters.lightParameters.shadowQuality;
		shadowBias=lightTarget.sunlightParameters.lightParameters.shadowBias;
	}

    // Use this for initialization
    void Start ()
    {
        lightTarget = GetComponent<Sunlight>();

    }
	
	// Update is called once per frame
	void Update ()
    {
		if (animateOrientation) {
			lightTarget.sunlightParameters.orientationParameters.yAxis = Yaw;
			lightTarget.sunlightParameters.orientationParameters.timeOfDay = Pitch;
			lightTarget.sunlightParameters.orientationParameters.Roll = Roll;
		}
		if (animateLightProperties) {
			lightTarget.sunlightParameters.lightParameters.intensity = intensity;
			lightTarget.sunlightParameters.lightParameters.indirectIntensity = indirectIntensity;
			//lightTarget.sunlightParameters.colorTemperature = colorTemperature;
			lightTarget.sunlightParameters.lightParameters.colorFilter = colorFilter;
			lightTarget.sunlightParameters.lightParameters.shadows = shadows;
			//lightTarget.sunlightParameters.shadowsResolution = shadowsResolution;
            lightTarget.sunlightParameters.lightParameters.shadowQuality = shadowQuality;
			lightTarget.sunlightParameters.lightParameters.shadowBias = shadowBias;
		}

	}
}

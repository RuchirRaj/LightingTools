using UnityEngine;
using LightUtilities;
using UnityEngine.Experimental.Rendering;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class Sunlight : MonoBehaviour
{
    //public SunlightAnimationParameters animationParameters = new SunlightAnimationParameters();
    public SunlightParameters sunlightParameters;
    [SerializeField][HideInInspector]
    private GameObject sunlight;
    [SerializeField][HideInInspector]
    private GameObject sunlightLattitude;
    [SerializeField][HideInInspector]
    private GameObject sunlightYAxis;
    [SerializeField][HideInInspector]
    private GameObject sunlightTimeofdayDummy;
    public bool drawGizmo = false;
    public float gizmoSize = 1;
    public bool showEntities = true;
    private float initialTimeOfDay;
    private float updatedTimeOfDay;

    private void OnEnable()
    {
        CreateLightYAxis();
        CreateLightLattitude();
        CreateSunlightTimeofdayDymmy();
        CreateSunlight();
        //Enable if it has been disabled
        if (sunlight != null) { sunlight.GetComponent<Light>().enabled = true; }
    }

    private void OnDisable()
    {
        if (sunlight != null) { sunlight.GetComponent<Light>().enabled = false; }
    }

    // Use this for initialization
    void Start ()
    {
        initialTimeOfDay = sunlightParameters.orientationParameters.timeOfDay;
        updatedTimeOfDay = initialTimeOfDay;

    }
	
	// Update is called once per frame
	void Update ()
    {
        if(Application.isPlaying)
        {
            switch (sunlightParameters.animationParameters.animationMode)
            {
                case SunlightAnimationMode.Custom:
                    {
                        SetSunlightTransform();
                        SetLightSettings();
                        break;
                    }
                case SunlightAnimationMode.DayCycle:
                    {
                        updatedTimeOfDay = (updatedTimeOfDay + Time.deltaTime * sunlightParameters.animationParameters.dayLength * 24 / 60) % 24;
                        sunlightParameters.orientationParameters.timeOfDay = updatedTimeOfDay;
                        SetSunlightTransform();
                        SetLightSettings();
                        break;
                    }


            }
        }
        if(Application.isEditor && !Application.isPlaying)
        {
            SetSunlightTransform();
            if (sunlight != null && sunlightParameters != null)
            {
                SetLightSettings();
            }
            ApplyShowFlags(showEntities);
        }
            
    }

#if UNITY_EDITOR

    private void OnDrawGizmos()
    {
        if(drawGizmo && sunlight != null && sunlightParameters.animationParameters.colorGradient != null)
        {
            Gizmos.color = Handles.color = new Color(1,1,1,0.3f);
            Handles.DrawWireDisc(gameObject.transform.position, gameObject.transform.up, gizmoSize);
            Handles.DrawWireArc(gameObject.transform.position, sunlightLattitude.transform.right, sunlightLattitude.transform.forward, 180, gizmoSize);
            var gizmoColor = sunlightParameters.animationParameters.colorGradient.Evaluate(sunlightParameters.orientationParameters.timeOfDay / 24);
            Gizmos.color = Handles.color = gizmoColor;
            LightingGizmos.DrawDirectionalLightGizmo(sunlight.transform);
            Gizmos.DrawLine(sunlightTimeofdayDummy.transform.position + sunlight.transform.forward * -gizmoSize, gameObject.transform.position);
            Gizmos.DrawWireSphere(sunlightTimeofdayDummy.transform.position + sunlight.transform.forward * -gizmoSize, gizmoSize/10);
        }
    }
#endif

    void CreateLightYAxis()
    {
        if(sunlightYAxis == null)
            sunlightYAxis = new GameObject("SunlightYAxis");
        sunlightYAxis.transform.parent = gameObject.transform;
        sunlightYAxis.transform.localPosition = Vector3.zero;
        sunlightYAxis.transform.rotation = Quaternion.identity;
    }

    void CreateLightLattitude()
    {
        if (sunlightLattitude == null)
            sunlightLattitude = new GameObject("SunlightLattitude");
        sunlightLattitude.transform.parent = sunlightYAxis.transform;
        sunlightLattitude.transform.localPosition = Vector3.zero;
        sunlightLattitude.transform.localRotation = Quaternion.identity;
    }

    void CreateSunlightTimeofdayDymmy()
    {
        if (sunlightTimeofdayDummy == null)
            sunlightTimeofdayDummy = new GameObject("SunlightTimeofdayDummy");
        sunlightTimeofdayDummy.transform.parent = sunlightLattitude.transform;
        sunlightTimeofdayDummy.transform.localPosition = Vector3.zero;
        sunlightTimeofdayDummy.transform.localRotation = Quaternion.identity;
    }

    void CreateSunlight()
    {
        if (sunlight == null)
            sunlight = new GameObject("DirectionalLight");
        sunlight.transform.parent = sunlightTimeofdayDummy.transform;
        sunlight.transform.localPosition = -Vector3.forward * gizmoSize;
        var directionalLight = sunlight.GetComponent<Light>() == null ? sunlight.AddComponent<Light>() : sunlight.GetComponent<Light>();
        directionalLight.type = LightType.Directional;
    }



    public void SetSunlightTransform()
    {
        SetSunlightTransform(sunlightParameters.orientationParameters.timeOfDay);
    }

    void SetSunlightTransform(float timeOfDay)
    {
        if (sunlightYAxis != null && sunlightLattitude != null && sunlight != null && sunlightParameters != null && sunlightYAxis.transform.parent == gameObject.transform)
        {
            sunlightYAxis.transform.localRotation = Quaternion.Euler(new Vector3(0, sunlightParameters.orientationParameters.yAxis, 0));
            sunlightLattitude.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 180 - sunlightParameters.orientationParameters.lattitude));
            sunlightTimeofdayDummy.transform.localRotation = Quaternion.Euler(new Vector3(timeOfDay * 15f + 90, 0, 0));
            sunlight.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, sunlightParameters.orientationParameters.Roll));
        }
    }

    public void SetLightSettings()
    {
        var directionalLight = sunlight.GetComponent<Light>();
        var shadowData = directionalLight.GetComponent<AdditionalShadowData>();

#if UNITY_EDITOR
        switch (sunlightParameters.lightParameters.mode)
        {
            case LightmapPresetBakeType.Realtime: directionalLight.lightmapBakeType = LightmapBakeType.Realtime; break;
            case LightmapPresetBakeType.Baked: directionalLight.lightmapBakeType = LightmapBakeType.Baked; break;
            case LightmapPresetBakeType.Mixed: directionalLight.lightmapBakeType = LightmapBakeType.Mixed; break;
        }
#endif

        directionalLight.shadows = sunlightParameters.lightParameters.shadows;
        directionalLight.intensity = sunlightParameters.lightParameters.intensity;
        directionalLight.bounceIntensity = sunlightParameters.lightParameters.indirectIntensity;
        if (sunlightParameters.animationParameters.colorGradient != null && sunlightParameters.animationParameters.animationMode == SunlightAnimationMode.DayCycle)
        {
            directionalLight.color = sunlightParameters.animationParameters.colorGradient.Evaluate(sunlightParameters.orientationParameters.timeOfDay / 24);
        }
        else
            directionalLight.color = sunlightParameters.lightParameters.colorFilter;
        directionalLight.shadowBias = sunlightParameters.lightParameters.shadowBias;
        switch (sunlightParameters.lightParameters.shadowQuality)
        {
            case LightingTools.ShadowQuality.VeryHigh: shadowData.shadowResolution = 2048; break;
            case LightingTools.ShadowQuality.High: shadowData.shadowResolution = 1024; break;
            case LightingTools.ShadowQuality.Medium: shadowData.shadowResolution = 512; break;
            case LightingTools.ShadowQuality.Low: shadowData.shadowResolution = 256; break;
        }
        directionalLight.cookie = sunlightParameters.lightParameters.lightCookie;
        directionalLight.shadowStrength = sunlightParameters.lightParameters.shadowStrength;
        QualitySettings.shadowDistance = sunlightParameters.lightParameters.shadowMaxDistance;
    }

    private void OnDestroy()
    {
        DestroyImmediate(sunlight);
        DestroyImmediate(sunlightYAxis);
        DestroyImmediate(sunlightLattitude);
    }

    void ApplyShowFlags(bool show)
    {
        if (sunlight != null)
        {
            if (!show) { sunlight.hideFlags = HideFlags.HideInHierarchy; }
            if (show)
            {
                sunlight.hideFlags = HideFlags.None;
            }
        }
        if (sunlightYAxis != null)
        {
            if (!show) { sunlightYAxis.hideFlags = HideFlags.HideInHierarchy; }
            if (show)
            {
                sunlightYAxis.hideFlags = HideFlags.None;
            }
        }
        if (sunlightLattitude != null)
        {
            if (!show) { sunlightLattitude.hideFlags = HideFlags.HideInHierarchy; }
            if (show)
            {
                sunlightLattitude.hideFlags = HideFlags.None;
            }
        }
    }

}

using UnityEngine;
using LightUtilities;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Experimental.Rendering.HDPipeline;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class Sunlight : MonoBehaviour
{
    public SunlightParameters sunlightParameters;
    [SerializeField][HideInInspector]
    private GameObject sunlight;
    [SerializeField][HideInInspector]
    private GameObject sunlightLattitude;
    [SerializeField][HideInInspector]
    private GameObject sunlightYAxis;
    [SerializeField][HideInInspector]
    private GameObject sunlightTimeofdayDummy;
    public bool drawGizmo = true;
    public float gizmoSize = 5;
    public bool showEntities = true;
    private float initialTimeOfDay;

    [SerializeField]
    [HideInInspector]
    private Light directionalLight;
    [SerializeField]
    [HideInInspector]
    private HDAdditionalLightData additionalLightData;
    [SerializeField]
    [HideInInspector]
    private AdditionalShadowData shadowData;

    private VolumeStack stack;

    private void OnEnable()
    {
        CreateLightYAxis();
        CreateLightLattitude();
        CreateSunlightTimeofdayDymmy();
        CreateSunlight();
        //Enable if it has been disabled
        if (sunlight != null) { sunlight.GetComponent<Light>().enabled = true; }

        stack = VolumeManager.instance.stack;
    }

    private void OnDisable()
    {
        if (sunlight != null) { sunlight.GetComponent<Light>().enabled = false; }
    }

    // Use this for initialization
    void Start ()
    {
        initialTimeOfDay = sunlightParameters.orientationParameters.timeOfDay;
    }
	
	// Update is called once per frame
	void Update ()
    {
        GatherOverrides();

        SetSunlightTransform();
        SetLightSettings();
        ApplyShowFlags(showEntities);
    }

    private void GatherOverrides()
    {
        var sunProps = stack.GetComponent<SunlightProperties>();

        if (sunProps.lattitude.overrideState)
            sunlightParameters.orientationParameters.lattitude = sunProps.lattitude.value;
        if (sunProps.YAxis.overrideState)
            sunlightParameters.orientationParameters.yAxis = sunProps.YAxis.value;
        if (sunProps.timeOfDay.overrideState)
            sunlightParameters.orientationParameters.timeOfDay = sunProps.timeOfDay.value;

        //If overridden intensity is constant, otherwise drive by curve
        if (sunProps.intensity.overrideState)
            sunlightParameters.lightParameters.intensity = sunProps.intensity;
        else
            sunlightParameters.lightParameters.intensity = sunlightParameters.intensityCurve.Evaluate(sunlightParameters.orientationParameters.timeOfDay);
        //If overridden intensity is constant, otherwise driven by gradient
        if (sunProps.color.overrideState)
            sunlightParameters.lightParameters.colorFilter = sunProps.color;
        else
            sunlightParameters.lightParameters.colorFilter = sunlightParameters.colorGradient.Evaluate(sunlightParameters.orientationParameters.timeOfDay/24);

        if (sunProps.indirectMultiplier.overrideState)
            sunlightParameters.lightParameters.indirectIntensity = sunProps.indirectMultiplier;
        if (sunProps.cookieTexture.overrideState)
            sunlightParameters.lightParameters.lightCookie = sunProps.cookieTexture;
        if (sunProps.cookieSize.overrideState)
            sunlightParameters.lightParameters.cookieSize = sunProps.cookieSize;
        if (sunProps.shadowResolution.overrideState)
            sunlightParameters.lightParameters.shadowResolution = sunProps.shadowResolution;
    }

    public void SetLightSettings()
    {
        LightingUtilities.ApplyLightParameters(directionalLight, sunlightParameters.lightParameters);
    }


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
        {
            sunlight = new GameObject("DirectionalLight");
            //Init defaults
            sunlightParameters = new SunlightParameters();
            sunlightParameters.lightParameters.type = LightType.Directional;
        }

        sunlight.transform.parent = sunlightTimeofdayDummy.transform;
        sunlight.transform.localPosition = -Vector3.forward * gizmoSize;

        directionalLight = sunlight.GetComponent<Light>() ?? sunlight.AddComponent<Light>();
        additionalLightData = sunlight.GetComponent<HDAdditionalLightData>() ?? sunlight.AddComponent<HDAdditionalLightData>();
        shadowData = sunlight.GetComponent<AdditionalShadowData>() ?? sunlight.AddComponent<AdditionalShadowData>();

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
            sunlightYAxis.transform.rotation = Quaternion.Euler(new Vector3(0, sunlightParameters.orientationParameters.yAxis, 0));
            sunlightLattitude.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 180 - sunlightParameters.orientationParameters.lattitude));
            sunlightTimeofdayDummy.transform.localRotation = Quaternion.Euler(new Vector3(timeOfDay * 15f + 90, 0, 0));
            sunlight.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, sunlightParameters.orientationParameters.Roll));
        }
    }

    private void OnDestroy()
    {
        DestroyImmediate(sunlight);
        DestroyImmediate(sunlightYAxis);
        DestroyImmediate(sunlightLattitude);
        DestroyImmediate(sunlightTimeofdayDummy);
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

#if UNITY_EDITOR

    private void OnDrawGizmos()
    {
        if(drawGizmo && sunlight != null)
        {
            Gizmos.color = Handles.color = new Color(1,1,1,0.3f);
            Handles.DrawWireDisc(sunlightYAxis.transform.position, sunlightYAxis.transform.up, gizmoSize);
            Handles.DrawWireArc(gameObject.transform.position, sunlightLattitude.transform.right, sunlightLattitude.transform.forward, 180, gizmoSize);
            var gizmoColor = sunlightParameters.lightParameters.colorFilter;
            Gizmos.color = Handles.color = gizmoColor;
            //LightingGizmos.DrawDirectionalLightGizmo(sunlight.transform);
            Gizmos.DrawLine(sunlightTimeofdayDummy.transform.position + sunlight.transform.forward * -gizmoSize, gameObject.transform.position);
            Handles.DrawWireDisc(sunlight.transform.position, Camera.current.transform.forward, gizmoSize / 10);
        }
    }
#endif
}

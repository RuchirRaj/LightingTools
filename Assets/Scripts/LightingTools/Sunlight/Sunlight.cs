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

    public bool useManager = false;

    [SerializeField]
    [HideInInspector]
    private Light directionalLight;
    [SerializeField]
    [HideInInspector]
    private HDAdditionalLightData additionalLightData;
    [SerializeField]
    [HideInInspector]
    private AdditionalShadowData shadowData;

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
    }
	
	// Update is called once per frame
	void Update ()
    {
        if(Application.isEditor && !Application.isPlaying && !useManager)
        {
            SetSunlightTransform();
            if (sunlight != null && sunlightParameters != null)
            {
                SetLightSettings();
            }
            ApplyShowFlags(showEntities);
        }
        if(useManager)
        {
            var stack = VolumeManager.instance.stack;
            var sunProps = stack.GetComponent<SunlightProperties>();
            sunlightParameters.orientationParameters.lattitude = sunProps.lattitude.value;
            sunlightParameters.orientationParameters.yAxis = sunProps.YAxis.value;
            sunlightParameters.orientationParameters.timeOfDay = sunProps.timeOfDay.value;
            sunlightParameters.lightParameters.intensity = sunProps.intensity;
            sunlightParameters.lightParameters.indirectIntensity = sunProps.indirectMultiplier;
            sunlightParameters.lightParameters.colorFilter = sunProps.color;
            sunlightParameters.lightParameters.lightCookie = sunProps.cookieTexture;
            sunlightParameters.lightParameters.cookieSize = sunProps.cookieSize;
            sunlightParameters.lightParameters.shadowResolution = sunProps.shadowResolution;
            SetSunlightTransform();
            SetLightSettings();
            ApplyShowFlags(showEntities);
        }
            
    }

    public void SetLightSettings()
    {
        LightingUtilities.ApplyLightParameters(directionalLight, sunlightParameters.lightParameters);
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
        directionalLight = sunlight.GetComponent<Light>() == null ? sunlight.AddComponent<Light>() : sunlight.GetComponent<Light>();
        directionalLight.type = LightType.Directional;
        if(!sunlight.GetComponent<HDAdditionalLightData>())
        {
            additionalLightData = sunlight.AddComponent<HDAdditionalLightData>();
            HDAdditionalLightData.InitDefaultHDAdditionalLightData(additionalLightData);
        }
        else
        {
            additionalLightData = sunlight.GetComponent<HDAdditionalLightData>();
        }
        shadowData = sunlight.GetComponent<AdditionalShadowData>() == null ? sunlight.AddComponent<AdditionalShadowData>() : sunlight.GetComponent<AdditionalShadowData>();
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

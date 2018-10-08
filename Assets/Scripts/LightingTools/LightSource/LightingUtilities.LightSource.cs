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
	public class LightSourceMeshParameters
	{
		public GameObject lightSourceObject;
		public ShadowCastingMode meshShadowMode;
		public bool showObjectInHierarchy;
	}
	
	[System.Serializable]
	public class LightSourceMaterialsParameters
	{
		public bool linkEmissiveIntensityWithLight = true ;
		public bool linkEmissiveColorWithLight = true ;
		public float emissiveMultiplier = 1f;
	}

    [System.Serializable]
    public class LightSourceAnimationParameters
    {
	    public bool enabledFromStart = true ;
	    public bool enableFunctionalAnimation = false ;
	    public bool enableSwithOnAnimation = false ;
	    public bool enableSwithOffAnimation = false ;
	    public bool enableBreakAnimation = false ;

        //public LightAnimationParameters functionalAnimationParameters;
	    //public LightAnimationParameters switchOnAnimationParameters;
	    //public LightAnimationParameters switchOffAnimationParameters;
	    //public LightAnimationParameters breakAnimationParameters;
    }
}
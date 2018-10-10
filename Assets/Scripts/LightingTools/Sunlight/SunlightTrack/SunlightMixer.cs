using UnityEngine.Playables;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

public class SunlightMixer : PlayableBehaviour
{
    // Called each frame the mixer is active, after inputs are processed
    public override void ProcessFrame(Playable handle, FrameData info, object playerData) {

        Volume volume = playerData as Volume;
        VolumeProfile volumeProfile = Application.isPlaying ? volume.profile : volume.sharedProfile;
        SunlightProperties sunprops = ScriptableObject.CreateInstance<SunlightProperties>();

        if(volumeProfile.TryGet<SunlightProperties>(out sunprops))
        {
            var count = handle.GetInputCount();
            for (var i = 0; i < count; i++)
            {
                var inputHandle = handle.GetInput(i);
                var weight = handle.GetInputWeight(i);

                if (inputHandle.IsValid() &&
                    inputHandle.GetPlayState() == PlayState.Playing &&
                    weight > 0)
                {
                    var data = ((ScriptPlayable<SunlightClipPlayable>)inputHandle).GetBehaviour();
                    if (data != null)
                    {
                        //var lerpedSunlightParameters = SunlightLightingUtilities.LerpSunlightParameters(data.sunlightParameters, data.sunlightParameters, weight);

                        sunprops.YAxis.value = data.sunlightParameters.orientationParameters.yAxis;
                        sunprops.lattitude.value = data.sunlightParameters.orientationParameters.lattitude;
                        sunprops.timeOfDay.value = data.sunlightParameters.orientationParameters.timeOfDay;
                        sunprops.intensity.value = data.sunlightParameters.lightParameters.intensity;
                        sunprops.color.value = data.sunlightParameters.lightParameters.colorFilter;
                        sunprops.cookieTexture.value = data.sunlightParameters.lightParameters.lightCookie;
                        sunprops.cookieSize.value = data.sunlightParameters.lightParameters.cookieSize;
                    }
                }
            }
        }
    }
}

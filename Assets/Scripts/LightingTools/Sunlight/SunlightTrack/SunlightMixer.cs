using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;
using System.Collections.Generic;
using LightUtilities;

public class SunlightMixer : PlayableBehaviour {

    // Called each frame the mixer is active, after inputs are processed
    public override void ProcessFrame(Playable handle, FrameData info, object playerData) {
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
                    data.sunlight.sunlightParameters = SunlightLightingUtilities.LerpSunlightParameters(data.sunlight.sunlightParameters, data.sunlightParameters, weight);
                    data.sunlight.SetSunlightTransform();
                    data.sunlight.SetLightSettings();
                }
            }

        }

    }
}

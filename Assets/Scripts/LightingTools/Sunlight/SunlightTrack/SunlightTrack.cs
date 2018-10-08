using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.UI;

[TrackColor(1.0f, 0.96f, 0.85f)]
[TrackClipType(typeof(SunlightClip))]
public class SunlightTrack : TrackAsset {

    public Sunlight trackSunlight;

    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount) {
        return ScriptPlayable<SunlightMixer>.Create(graph, inputCount);
    }
}

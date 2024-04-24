using UnityEngine;
using System.Collections;

public class MusicPlayer : MonoBehaviour {
    public AudioClip introClip;
    public AudioClip loopClip;
    public AudioSequence sequence { get; private set; }

    void Start () {
        sequence = gameObject.AddComponent<AudioSequence>();

        // Play method initializes and plays the introClip
        sequence.Play(introClip, loopClip);

        // Retrieve AudioSource for introClip if it's different from loopClip's AudioSource
        AudioSequenceData introData = sequence.GetData(introClip);
        if (introData != null && introData.source != null) {
            introData.source.volume = 0.3f;  // Set volume for introClip
        }

        // Ensure loop settings and volume for loopClip
        AudioSequenceData loopData = sequence.GetData(loopClip);
        if (loopData != null && loopData.source != null) {
            loopData.source.loop = true;
            loopData.source.volume = 0.3f;  // Set volume for loopClip
        }
    }
}

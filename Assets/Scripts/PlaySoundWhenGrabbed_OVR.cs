using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(OVRGrabbable))]
public class PlaySoundWhenGrabbed_OVR : MonoBehaviour
{
    AudioSource audioSource;
    OVRGrabbable grabbable;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        grabbable = GetComponent<OVRGrabbable>();

        audioSource.playOnAwake = false;
        audioSource.loop = true;
        audioSource.Stop();
    }

    void Update()
    {
        Debug.Log($"Grabbed: {grabbable.isGrabbed}");

        if (grabbable.isGrabbed)
        {
            if (!audioSource.isPlaying)
                audioSource.Play();
        }
        else
        {
            if (audioSource.isPlaying)
                audioSource.Stop();
        }
    }
}

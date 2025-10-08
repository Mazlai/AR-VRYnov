using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(OVRGrabbable))]
public class PlaySoundWhenGrabbed : MonoBehaviour
{
    private AudioSource audioSource;
    private OVRGrabbable grabbable;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        grabbable = GetComponent<OVRGrabbable>();

        // On s’assure que le son ne démarre pas par défaut
        audioSource.loop = true;
        audioSource.playOnAwake = false;
        audioSource.Stop();
    }

    void Update()
    {
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
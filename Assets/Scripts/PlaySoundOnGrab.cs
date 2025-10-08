using UnityEngine;
using Oculus.Interaction;   // <- nécessaire pour accéder à Grabbable et PointerEvent

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Grabbable))]
public class PlaySoundOnGrab : MonoBehaviour
{
    private AudioSource audioSource;
    private Grabbable grabbable;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        grabbable = GetComponent<Grabbable>();

        // s'assurer que le son n'est pas lancé d’avance
        audioSource.playOnAwake = false;
        audioSource.loop = true;
        audioSource.Stop();
    }

    private void OnEnable()
    {
        grabbable.WhenPointerEventRaised += OnPointerEvent;
    }

    private void OnDisable()
    {
        grabbable.WhenPointerEventRaised -= OnPointerEvent;
    }

    private void OnPointerEvent(PointerEvent evt)
    {
        if (evt.Type == PointerEventType.Select)
        {
            if (!audioSource.isPlaying)
                audioSource.Play();
        }
        else if (evt.Type == PointerEventType.Unselect)
        {
            if (audioSource.isPlaying)
                audioSource.Stop();
        }
    }
}

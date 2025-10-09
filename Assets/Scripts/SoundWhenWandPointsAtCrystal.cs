using UnityEngine;
using Oculus.Interaction;
using System.Collections.Generic;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Grabbable))]
public class SoundWhenWandPointsAtCrystal : MonoBehaviour
{
    [Header("Réglages")]
    public Transform wandTip;             // le bout de la baguette
    public string crystalTag = "Crystal"; // tag des cristaux
    public float angleThreshold = 15f;    // tolérance en degrés
    public float hapticFrequency = 1f;    // vibration
    public float hapticIntensity = 0.5f;

    private AudioSource audioSource;
    private Grabbable grabbable;
    private bool isHeld = false;

    // Pour gérer la vibration manuelle
    private OVRInput.Controller holdingController = OVRInput.Controller.None;
    private bool isVibrating = false;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        grabbable = GetComponent<Grabbable>();

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
        StopHaptics();
    }

    private void OnPointerEvent(PointerEvent evt)
    {
        if (evt.Type == PointerEventType.Select)
        {
            isHeld = true;
            // On récupère la manette à partir de l'ID du pointeur
            holdingController = GetControllerFromPointer(evt.Identifier);
        }
        else if (evt.Type == PointerEventType.Unselect || evt.Type == PointerEventType.Cancel)
        {
            isHeld = false;
            holdingController = OVRInput.Controller.None;
            if (audioSource.isPlaying) audioSource.Stop();
            StopHaptics();
        }
    }

    private void Update()
    {
        if (!isHeld || wandTip == null)
            return;

        bool pointingAtCrystal = false;

        GameObject[] crystals = GameObject.FindGameObjectsWithTag(crystalTag);
        foreach (GameObject crystal in crystals)
        {
            if (crystal == this.gameObject) continue;

            Vector3 toCrystal = (crystal.transform.position - wandTip.position).normalized;
            float angle = Vector3.Angle(wandTip.forward, toCrystal);

            if (angle <= angleThreshold)
            {
                pointingAtCrystal = true;
                break;
            }
        }

        if (pointingAtCrystal)
        {
            if (!audioSource.isPlaying)
                audioSource.Play();
            if (!isVibrating)
                StartHaptics();
        }
        else
        {
            if (audioSource.isPlaying)
                audioSource.Stop();
            if (isVibrating)
                StopHaptics();
        }
    }

    private void StartHaptics()
    {
        if (holdingController != OVRInput.Controller.None)
        {
            OVRInput.SetControllerVibration(hapticFrequency, hapticIntensity, holdingController);
            isVibrating = true;
        }
    }

    private void StopHaptics()
    {
        if (holdingController != OVRInput.Controller.None)
        {
            OVRInput.SetControllerVibration(0, 0, holdingController);
            isVibrating = false;
        }
    }

    // Exemple simple pour récupérer la manette depuis l'ID du pointeur
    private OVRInput.Controller GetControllerFromPointer(int pointerId)
    {
        // Si tu utilises le standard Oculus Interaction, les IDs 0 et 1 correspondent généralement aux deux mains
        if (pointerId == 0) return OVRInput.Controller.LTouch;
        if (pointerId == 1) return OVRInput.Controller.RTouch;
        return OVRInput.Controller.None;
    }
}

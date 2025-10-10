using UnityEngine;
using Oculus.Interaction;
using System.Collections.Generic;

public class GrabSoundManager : MonoBehaviour
{
    [Header("Sons par d�faut")]
    public AudioClip defaultGrabSound;
    public AudioClip defaultReleaseSound;

    [Header("Param�tres Audio")]
    [Range(0f, 1f)]
    public float volume = 0.7f;

    [Header("Param�tres Haptiques")]
    [Range(0f, 1f)]
    public float grabHapticAmplitude = 0.5f;
    [Range(0f, 1f)]
    public float releaseHapticAmplitude = 0.3f;
    [Range(0f, 0.5f)]
    public float hapticDuration = 0.1f;

    private AudioSource audioSource;
    private static GrabSoundManager instance;

    // Dictionnaire pour tracker les interactors et leurs manettes
    private Dictionary<int, OVRInput.Controller> interactorToController = new Dictionary<int, OVRInput.Controller>();

    // Dictionnaire pour se souvenir quelle main utilise chaque interactor ID lors du grab
    private Dictionary<int, OVRInput.Controller> activeGrabsByIdentifier = new Dictionary<int, OVRInput.Controller>();

    void Awake()
    {
        // Singleton
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Cr�e l'AudioSource
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f; // 2D sound (ou 1f pour 3D)
    }

    void Start()
    {
        // Trouve tous les Grabbables et abonne-toi � leurs �v�nements
        RegisterAllGrabbables();

        // Mappe les interactors aux manettes
        MapInteractorsToControllers();
    }

    void MapInteractorsToControllers()
    {
        // Trouve tous les interactors dans la sc�ne
        var allInteractors = FindObjectsOfType<MonoBehaviour>();

        foreach (var component in allInteractors)
        {
            if (component is IInteractor interactor)
            {
                Transform current = component.transform;

                while (current != null)
                {
                    string name = current.name.ToLower();

                    if (name.Contains("left") || name.Contains("gauche"))
                    {
                        interactorToController[interactor.Identifier] = OVRInput.Controller.LTouch;
                        Debug.Log($"Interactor gauche mapp�: {component.name} (ID: {interactor.Identifier})");
                        break;
                    }
                    else if (name.Contains("right") || name.Contains("droite"))
                    {
                        interactorToController[interactor.Identifier] = OVRInput.Controller.RTouch;
                        Debug.Log($"Interactor droit mapp�: {component.name} (ID: {interactor.Identifier})");
                        break;
                    }

                    current = current.parent;
                }
            }
        }

        Debug.Log($"GrabSoundManager: {interactorToController.Count} interactors mapp�s");
    }

    void RegisterAllGrabbables()
    {
        Grabbable[] allGrabbables = FindObjectsOfType<Grabbable>();
        foreach (Grabbable grabbable in allGrabbables)
        {
            grabbable.WhenPointerEventRaised += HandleGrabEvent;
        }
        Debug.Log($"GrabSoundManager: {allGrabbables.Length} objets grabbables enregistr�s");
    }

    private void HandleGrabEvent(PointerEvent evt)
    {
        OVRInput.Controller controller = OVRInput.Controller.None;

        switch (evt.Type)
        {
            case PointerEventType.Select:
                // D�tecte quelle main grab
                controller = GetControllerFromIdentifier(evt.Identifier);
                // Stocke la manette pour cet identifier
                activeGrabsByIdentifier[evt.Identifier] = controller;
                Debug.Log($"Grab d�tect� avec {controller} (ID: {evt.Identifier})");

                PlaySound(defaultGrabSound);
                TriggerHaptic(controller, grabHapticAmplitude);
                break;

            case PointerEventType.Unselect:
            case PointerEventType.Cancel:
                // R�cup�re la manette qui avait grab via l'identifier
                if (activeGrabsByIdentifier.TryGetValue(evt.Identifier, out controller))
                {
                    Debug.Log($"Release d�tect� avec {controller} (ID: {evt.Identifier})");
                    activeGrabsByIdentifier.Remove(evt.Identifier);
                }
                else
                {
                    // Fallback au cas o�
                    controller = GetControllerFromIdentifier(evt.Identifier);
                    Debug.LogWarning($"Release sans grab pr�alable trouv�, utilisation fallback: {controller}");
                }

                PlaySound(defaultReleaseSound);
                TriggerHaptic(controller, releaseHapticAmplitude);
                break;
        }
    }

    void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip, volume);
        }
    }

    void TriggerHaptic(OVRInput.Controller controller, float amplitude)
    {
        if (controller != OVRInput.Controller.None && controller != OVRInput.Controller.Touch)
        {
            OVRInput.SetControllerVibration(1f, amplitude, controller);
            StartCoroutine(StopHapticAfterDelay(controller));
        }
    }

    private System.Collections.IEnumerator StopHapticAfterDelay(OVRInput.Controller controller)
    {
        yield return new WaitForSeconds(hapticDuration);
        OVRInput.SetControllerVibration(0f, 0f, controller);
    }

    private OVRInput.Controller GetControllerFromIdentifier(int identifier)
    {
        // Cherche dans le dictionnaire pr�-mapp�
        if (interactorToController.TryGetValue(identifier, out OVRInput.Controller controller))
        {
            return controller;
        }

        // Fallback: essaie de d�tecter avec OVRInput en temps r�el
        if (OVRInput.Get(OVRInput.Button.PrimaryHandTrigger, OVRInput.Controller.LTouch))
        {
            Debug.Log("D�tection fallback: main gauche");
            return OVRInput.Controller.LTouch;
        }
        else if (OVRInput.Get(OVRInput.Button.PrimaryHandTrigger, OVRInput.Controller.RTouch))
        {
            Debug.Log("D�tection fallback: main droite");
            return OVRInput.Controller.RTouch;
        }

        // Si vraiment impossible de d�tecter
        Debug.LogWarning($"Impossible de d�terminer la main pour l'identifier {identifier}");
        return OVRInput.Controller.Touch;
    }

    // M�thode publique pour jouer un son custom
    public static void PlayCustomGrabSound(AudioClip clip, float customVolume = 1f)
    {
        if (instance != null && clip != null)
        {
            instance.audioSource.PlayOneShot(clip, customVolume);
        }
    }

    // M�thode publique pour d�clencher une vibration custom sur une main sp�cifique
    public static void PlayCustomHaptic(OVRInput.Controller controller, float amplitude, float duration)
    {
        if (instance != null)
        {
            OVRInput.SetControllerVibration(1f, amplitude, controller);
            instance.StartCoroutine(instance.StopHapticCustom(controller, duration));
        }
    }

    private System.Collections.IEnumerator StopHapticCustom(OVRInput.Controller controller, float duration)
    {
        yield return new WaitForSeconds(duration);
        OVRInput.SetControllerVibration(0f, 0f, controller);
    }
}
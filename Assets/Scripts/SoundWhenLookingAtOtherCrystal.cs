using UnityEngine;
using Oculus.Interaction;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Grabbable))]
public class SoundWhenLookingAtOtherCrystal : MonoBehaviour
{
    [Header("Réglages")]
    public Transform playerCamera;            // la caméra VR (ex : CenterEyeAnchor)
    public string tag = "Crystal";            // tag utilisé pour les autres cristaux
    public float angleThreshold = 15f;        // tolérance en degrés (horizontal uniquement)

    private AudioSource audioSource;
    private Grabbable grabbable;
    private bool isHeld = false;

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
    }

    private void OnPointerEvent(PointerEvent evt)
    {
        if (evt.Type == PointerEventType.Select)
        {
            isHeld = true;
        }
        else if (evt.Type == PointerEventType.Unselect)
        {
            isHeld = false;
            if (audioSource.isPlaying)
                audioSource.Stop();
        }
    }

    private void Update()
    {
        if (!isHeld || playerCamera == null)
            return;

        // direction du regard du joueur (projetée sur le plan horizontal)
        Vector3 lookDir = playerCamera.forward;
        lookDir.y = 0f;              // ignore la hauteur
        lookDir.Normalize();

        // recherche d’un cristal aligné horizontalement
        GameObject[] allCrystals = GameObject.FindGameObjectsWithTag(tag);
        bool foundAlignedCrystal = false;

        foreach (GameObject c in allCrystals)
        {
            if (c == this.gameObject) continue; // on ignore le cristal tenu

            // direction vers le cristal (projetée sur le plan horizontal)
            Vector3 toCrystal = c.transform.position - playerCamera.position;
            toCrystal.y = 0f;            // ignore la hauteur
            toCrystal.Normalize();

            float angle = Vector3.Angle(lookDir, toCrystal);

            if (angle <= angleThreshold)
            {
                foundAlignedCrystal = true;
                break;
            }
        }

        if (foundAlignedCrystal)
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

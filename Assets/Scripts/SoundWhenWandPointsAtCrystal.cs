using UnityEngine;
using Oculus.Interaction;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Grabbable))]
public class SoundWhenWandPointsAtCrystal : MonoBehaviour
{
    [Header("Réglages")]
    [Tooltip("Le point de référence de la baguette, par exemple l’extrémité avant du modèle (Empty GameObject).")]
    public Transform wandTip;                // le bout de la baguette magique
    [Tooltip("Le tag utilisé pour les cristaux dans la scène.")]
    public string crystalTag = "Crystal";    // tag des cristaux
    [Tooltip("Tolérance angulaire en degrés.")]
    public float angleThreshold = 15f;       // angle d'acceptation
    [Tooltip("Ignore la différence de hauteur entre la baguette et les cristaux.")]
    public bool ignoreHeight = true;         // optionnel : ignorer la hauteur

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
        if (!isHeld || wandTip == null)
            return;

        // direction du "faisceau" de la baguette magique
        Vector3 wandDir = wandTip.forward;

        GameObject[] allCrystals = GameObject.FindGameObjectsWithTag(crystalTag);
        bool foundAlignedCrystal = false;

        foreach (GameObject crystal in allCrystals)
        {
            if (crystal == this.gameObject) continue;

            Vector3 toCrystal = (crystal.transform.position - wandTip.position);

            if (ignoreHeight)
            {
                wandDir.y = 0f;
                toCrystal.y = 0f;
            }

            wandDir.Normalize();
            toCrystal.Normalize();

            float angle = Vector3.Angle(wandDir, toCrystal);

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

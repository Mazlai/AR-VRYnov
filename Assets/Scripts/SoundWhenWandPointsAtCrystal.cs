using UnityEngine;
using Oculus.Interaction;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Grabbable))]
public class SoundWhenWandPointsAtCrystal : MonoBehaviour
{
    [Header("Réglages")]
    public Transform wandTip;
    public string crystalTag = "Crystal";
    public float angleThreshold = 15f;
    public bool ignoreHeight = true;

    private AudioSource audioSource;
    private Grabbable grabbable;
    private bool isHeld = false;
    private float baseScale;

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

            if (audioSource.isPlaying) audioSource.Stop();
        }
    }

    private void Update()
    {
        if (!isHeld || wandTip == null) return;

        Vector3 wandDir = wandTip.forward;
        GameObject[] allCrystals = GameObject.FindGameObjectsWithTag(crystalTag);
        bool foundAlignedCrystal = false;

        foreach (GameObject crystal in allCrystals)
        {
            if (crystal == this.gameObject) continue;

            Vector3 toCrystal = crystal.transform.position - wandTip.position;

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

        // Son
        if (foundAlignedCrystal)
        {
            if (!audioSource.isPlaying) audioSource.Play();
        }
        else
        {
            if (audioSource.isPlaying) audioSource.Stop();
        }
    }
}

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

    [Header("Sphère lumineuse")]
    public GameObject pulseSpherePrefab;
    public float pulseAmplitude = 0.1f;
    public float pulseSpeed = 3f;
    public float maxScale = 0.3f;

    private GameObject pulseSphereInstance;
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

        if (pulseSpherePrefab != null && wandTip != null)
        {
            pulseSphereInstance = Instantiate(pulseSpherePrefab, wandTip);
            pulseSphereInstance.transform.localPosition = Vector3.zero;
            pulseSphereInstance.transform.localScale = Vector3.zero;
            pulseSphereInstance.SetActive(false);
            baseScale = maxScale * 0.5f;
        }
    }

    private void OnEnable()
    {
        grabbable.WhenPointerEventRaised += OnPointerEvent;
    }

    private void OnDisable()
    {
        grabbable.WhenPointerEventRaised -= OnPointerEvent;
        if (pulseSphereInstance != null)
            pulseSphereInstance.SetActive(false);
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
            if (pulseSphereInstance != null)
                pulseSphereInstance.SetActive(false);
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

        // Sphère pulsante
        if (pulseSphereInstance != null)
        {
            if (foundAlignedCrystal)
            {
                if (!pulseSphereInstance.activeSelf)
                    pulseSphereInstance.SetActive(true);

                float scale = baseScale + Mathf.Sin(Time.time * pulseSpeed) * pulseAmplitude;
                pulseSphereInstance.transform.localScale = Vector3.one * Mathf.Clamp(scale, 0f, maxScale);
            }
            else
            {
                if (pulseSphereInstance.activeSelf)
                    pulseSphereInstance.SetActive(false);
            }
        }
    }
}

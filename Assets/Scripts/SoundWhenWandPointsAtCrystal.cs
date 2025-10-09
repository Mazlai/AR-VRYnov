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
<<<<<<< HEAD
    public GameObject pulseSpherePrefab;   // prefab d'une sphère semi-transparente
    public float pulseAmplitude = 0.1f;    // variation de scale
    public float pulseSpeed = 3f;          // vitesse de pulsation
    public float maxScale = 0.3f;          // scale max de la sphère
=======
    public GameObject pulseSpherePrefab;
    public float pulseAmplitude = 0.1f;
    public float pulseSpeed = 3f;
    public float maxScale = 0.3f;
>>>>>>> 53bbf6b5720e1121eb29d8e8f3a4ed2dfef1b136

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

<<<<<<< HEAD
        // Crée la sphère pulsante en tant qu'enfant de wandTip
=======
>>>>>>> 53bbf6b5720e1121eb29d8e8f3a4ed2dfef1b136
        if (pulseSpherePrefab != null && wandTip != null)
        {
            pulseSphereInstance = Instantiate(pulseSpherePrefab, wandTip);
            pulseSphereInstance.transform.localPosition = Vector3.zero;
            pulseSphereInstance.transform.localScale = Vector3.zero;
<<<<<<< HEAD
            pulseSphereInstance.SetActive(false);
=======
            pulseSphereInstance.SetActive(false); // désactive au départ
>>>>>>> 53bbf6b5720e1121eb29d8e8f3a4ed2dfef1b136
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
<<<<<<< HEAD
            if (audioSource.isPlaying) audioSource.Stop();
=======
            audioSource.Stop();
>>>>>>> 53bbf6b5720e1121eb29d8e8f3a4ed2dfef1b136
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

<<<<<<< HEAD
        // Son
=======
        // Gestion du son
>>>>>>> 53bbf6b5720e1121eb29d8e8f3a4ed2dfef1b136
        if (foundAlignedCrystal)
        {
            if (!audioSource.isPlaying) audioSource.Play();
        }
        else
        {
            if (audioSource.isPlaying) audioSource.Stop();
        }

<<<<<<< HEAD
        // Sphère pulsante
=======
        // Gestion de la sphère lumineuse
>>>>>>> 53bbf6b5720e1121eb29d8e8f3a4ed2dfef1b136
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

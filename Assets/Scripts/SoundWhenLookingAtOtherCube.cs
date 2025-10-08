using UnityEngine;
using Oculus.Interaction;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Grabbable))]
public class SoundWhenLookingAtOtherCube : MonoBehaviour
{
    [Header("Réglages")]
    public Transform playerCamera;            // la caméra VR (ex : CenterEyeAnchor)
    public string cubeTag = "TargetCube";     // tag utilisé pour les autres cubes
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

        // recherche d’un cube aligné horizontalement
        GameObject[] allCubes = GameObject.FindGameObjectsWithTag(cubeTag);
        bool foundAlignedCube = false;

        foreach (GameObject c in allCubes)
        {
            if (c == this.gameObject) continue; // on ignore le cube tenu

            // direction vers le cube (projetée sur le plan horizontal)
            Vector3 toCube = c.transform.position - playerCamera.position;
            toCube.y = 0f;            // ignore la hauteur
            toCube.Normalize();

            float angle = Vector3.Angle(lookDir, toCube);

            if (angle <= angleThreshold)
            {
                foundAlignedCube = true;
                break;
            }
        }

        if (foundAlignedCube)
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

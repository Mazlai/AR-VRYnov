using UnityEngine;
using Oculus.Interaction;
using System.Linq;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Grabbable))]
public class SoundWhenLookingAtOtherCube : MonoBehaviour
{
    [Header("Réglages")]
    public Transform playerCamera;      // la caméra VR (ex : CenterEyeAnchor)
    public string cubeTag = "Cube";     // tag utilisé pour les autres cubes
    public float angleThreshold = 15f;  // tolérance en degrés

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

        // direction du regard du joueur
        Vector3 lookDir = playerCamera.forward;

        // recherche du cube le plus aligné avec le regard
        GameObject[] allCubes = GameObject.FindGameObjectsWithTag(cubeTag);
        bool foundAlignedCube = false;

        foreach (GameObject c in allCubes)
        {
            if (c == this.gameObject) continue; // on ignore le cube qu'on tient

            Vector3 toCube = (c.transform.position - playerCamera.position).normalized;
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

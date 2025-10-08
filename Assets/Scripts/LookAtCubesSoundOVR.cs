using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction;

[RequireComponent(typeof(OVRGrabbable))]
[RequireComponent(typeof(AudioSource))]
public class LookAtCubesSoundOVR : MonoBehaviour
{
    [Header("Références")]
    public Transform playerCamera;       // Glisser CenterEyeAnchor ici
    public float angleThreshold = 15f;   // Cône de détection

    private AudioSource audioSource;
    private OVRGrabbable grabbable;

    private List<GameObject> targetCubes = new List<GameObject>();
    private bool hasPlayedThisFrame = false;

    void Awake()
    {
        grabbable = GetComponent<OVRGrabbable>();
        audioSource = GetComponent<AudioSource>();

        // Récupère tous les cubes à trouver (taggés "TargetCube")
        GameObject[] found = GameObject.FindGameObjectsWithTag("TargetCube");
        targetCubes.AddRange(found);

        if (playerCamera == null)
        {
            // Cherche automatiquement le CenterEyeAnchor dans l’OVRCameraRig
            OVRCameraRig rig = FindObjectOfType<OVRCameraRig>();
            if (rig != null)
            {
                playerCamera = rig.centerEyeAnchor;
            }
            else if (Camera.main != null)
            {
                playerCamera = Camera.main.transform;
            }
        }
    }

    void Update()
    {
        // Ne fait rien si le cube n’est pas tenu
        if (!grabbable.isGrabbed || playerCamera == null || targetCubes.Count == 0)
        {
            hasPlayedThisFrame = false;
            return;
        }

        hasPlayedThisFrame = false;

        foreach (var cube in targetCubes)
        {
            if (cube == null || cube == gameObject) continue; // ignore le cube tenu

            Vector3 toCube = (cube.transform.position - playerCamera.position).normalized;
            float angle = Vector3.Angle(playerCamera.forward, toCube);

            if (angle <= angleThreshold)
            {
                if (!hasPlayedThisFrame)
                {
                    audioSource.Play();
                    hasPlayedThisFrame = true;
                }
                break; // ne joue qu'une fois même si plusieurs cubes sont dans le cône
            }
        }
    }
}

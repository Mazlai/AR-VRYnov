using UnityEngine;
using UnityEngine.UI;

public class MenuVR : MonoBehaviour
{
    [Header("R�f�rences UI")]
    public Canvas menuCanvas;
    public GameObject menuPanel;
    public GameObject welcomeText;
    public Button boutonJouer;
    public Button boutonQuitter;

    [Header("R�f�rences XR (Meta)")]
    public GameObject ovrInteractionComprehensive;
    public GameObject locomotor;

    [Header("Configuration Camera")]
    public Camera vrCamera;

    [Header("Viseurs (Rays)")]
    public LineRenderer leftHandRay;    // Line Renderer main gauche
    public LineRenderer rightHandRay;   // Line Renderer main droite
    public float rayLength = 10f;       // Longueur du rayon
    public Color rayColor = Color.cyan; // Couleur du rayon

    private bool menuActif = true;      // Pour savoir si le menu est visible

    void Start()
    {
        ConfigureCanvasForVR();

        // Configure les viseurs
        ConfigureRayVisuals();

        if (menuPanel != null)
            menuPanel.SetActive(true);
        if (welcomeText != null)
            welcomeText.SetActive(false);

        ToggleXRInteractions(false);

        // Active les viseurs au d�marrage (menu visible)
        SetRayVisibility(true);
        menuActif = true;

        if (boutonJouer != null)
            boutonJouer.onClick.AddListener(OnJouerClick);
        if (boutonQuitter != null)
            boutonQuitter.onClick.AddListener(OnQuitterClick);
    }

    void ConfigureRayVisuals()
    {
        // Trouve automatiquement les LineRenderers si non assign�s
        if (leftHandRay == null || rightHandRay == null)
        {
            FindRayVisuals();
        }

        // Configure les LineRenderers
        ConfigureLineRenderer(leftHandRay);
        ConfigureLineRenderer(rightHandRay);
    }

    void FindRayVisuals()
    {
        // Cherche dans toute la hi�rarchie les LineRenderers des contr�leurs
        LineRenderer[] allLineRenderers = FindObjectsOfType<LineRenderer>();

        foreach (LineRenderer lr in allLineRenderers)
        {
            // Cherche dans les parents si c'est li� � LeftHand ou RightHand
            Transform parent = lr.transform;
            while (parent != null)
            {
                if (parent.name.Contains("LeftHand") || parent.name.Contains("Left"))
                {
                    leftHandRay = lr;
                    Debug.Log("Line Renderer gauche trouv� : " + lr.name);
                    break;
                }
                else if (parent.name.Contains("RightHand") || parent.name.Contains("Right"))
                {
                    rightHandRay = lr;
                    Debug.Log("Line Renderer droit trouv� : " + lr.name);
                    break;
                }
                parent = parent.parent;
            }
        }

        if (leftHandRay == null)
            Debug.LogWarning("Line Renderer de la main gauche non trouv� !");
        if (rightHandRay == null)
            Debug.LogWarning("Line Renderer de la main droite non trouv� !");
    }

    void ConfigureLineRenderer(LineRenderer lineRenderer)
    {
        if (lineRenderer == null) return;

        // Configure l'apparence
        lineRenderer.startWidth = 0.002f;
        lineRenderer.endWidth = 0.002f;
        lineRenderer.startColor = rayColor;
        lineRenderer.endColor = rayColor;

        // Configure les positions (2 points : d�but et fin du rayon)
        lineRenderer.positionCount = 2;

        // Assure qu'il y a un mat�riau
        if (lineRenderer.material == null)
        {
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        }

        Debug.Log("Line Renderer configur� : " + lineRenderer.name);
    }

    void Update()
    {
        // Met � jour la longueur des rayons seulement si le menu est actif
        if (menuActif)
        {
            UpdateRayVisual(leftHandRay);
            UpdateRayVisual(rightHandRay);
        }
    }

    void UpdateRayVisual(LineRenderer lineRenderer)
    {
        if (lineRenderer == null || !lineRenderer.enabled) return;

        Transform rayOrigin = lineRenderer.transform;

        // Point de d�part du rayon
        lineRenderer.SetPosition(0, rayOrigin.position);

        // Point de fin du rayon
        Vector3 endPoint = rayOrigin.position + rayOrigin.forward * rayLength;
        lineRenderer.SetPosition(1, endPoint);
    }

    // Nouvelle fonction pour activer/d�sactiver les viseurs
    void SetRayVisibility(bool visible)
    {
        if (leftHandRay != null)
        {
            leftHandRay.enabled = visible;
            leftHandRay.gameObject.SetActive(visible);
        }

        if (rightHandRay != null)
        {
            rightHandRay.enabled = visible;
            rightHandRay.gameObject.SetActive(visible);
        }

        Debug.Log("Viseurs " + (visible ? "activ�s" : "d�sactiv�s"));
    }

    void ConfigureCanvasForVR()
    {
        if (menuCanvas != null)
        {
            menuCanvas.renderMode = RenderMode.ScreenSpaceCamera;

            if (menuCanvas.worldCamera == null && vrCamera != null)
            {
                menuCanvas.worldCamera = vrCamera;
            }

            if (menuCanvas.worldCamera == null)
            {
                Camera mainCam = Camera.main;
                if (mainCam != null)
                {
                    menuCanvas.worldCamera = mainCam;
                    Debug.Log("Cam�ra VR assign�e automatiquement au Canvas");
                }
                else
                    Debug.LogWarning("Aucune cam�ra VR assign�e au Canvas !");
            }

            menuCanvas.planeDistance = 2.5f;

            GraphicRaycaster oldRaycaster = menuCanvas.GetComponent<GraphicRaycaster>();
            OVRRaycaster ovrRaycaster = menuCanvas.GetComponent<OVRRaycaster>();

            if (oldRaycaster != null && ovrRaycaster == null)
            {
                Destroy(oldRaycaster);
                menuCanvas.gameObject.AddComponent<OVRRaycaster>();
                Debug.Log("OVRRaycaster ajout� au Canvas");
            }
            else if (ovrRaycaster == null)
            {
                menuCanvas.gameObject.AddComponent<OVRRaycaster>();
                Debug.Log("OVRRaycaster ajout� au Canvas");
            }
        }
    }

    void OnJouerClick()
    {
        Debug.Log("Bouton Jouer cliqu� !");

        if (menuPanel != null)
            menuPanel.SetActive(false);

        if (welcomeText != null)
        {
            welcomeText.SetActive(true);
            Animator animator = welcomeText.GetComponent<Animator>();
            if (animator != null)
            {
                animator.enabled = true;
                animator.Play(animator.GetCurrentAnimatorStateInfo(0).fullPathHash, 0, 0f);
            }
        }

        // D�sactive les viseurs quand on quitte le menu
        SetRayVisibility(false);
        menuActif = false;

        ToggleXRInteractions(true);
    }

    void OnQuitterClick()
    {
        Debug.Log("Quitter le jeu");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void RetourMenu()
    {
        if (welcomeText != null)
            welcomeText.SetActive(false);
        if (menuPanel != null)
            menuPanel.SetActive(true);

        // R�active les viseurs quand on revient au menu
        SetRayVisibility(true);
        menuActif = true;

        ToggleXRInteractions(false);
    }

    private void ToggleXRInteractions(bool state)
    {
        if (ovrInteractionComprehensive != null)
            ovrInteractionComprehensive.SetActive(state);

        if (locomotor != null)
            locomotor.SetActive(state);
    }
}
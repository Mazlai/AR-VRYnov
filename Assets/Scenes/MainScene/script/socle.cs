using UnityEngine;

public class socle : MonoBehaviour
{
    private Renderer socleRenderer;
    private Material socleMaterial;
    private Color couleurOriginale;
    private Color couleurDoree = new Color(1f, 0.84f, 0f);
    private bool objetSurSocle = false;
    private float vitesseFlash = 2f;
    private float intensiteLumiereMax = 5f;

    private Light lumiere;

    // Son
    public AudioClip sonActivation;
    public AudioClip sonDesactivation;
    private AudioSource audioSource;

    void Start()
    {
        socleRenderer = GetComponent<Renderer>();
        socleMaterial = socleRenderer.material;
        couleurOriginale = socleMaterial.color;

        // Ajouter un AudioSource pour jouer les sons
        audioSource = gameObject.AddComponent<AudioSource>();

        // Créer une vraie lumière Unity
        GameObject lightObj = new GameObject("LumiereSocle");
        lightObj.transform.parent = transform;
        lightObj.transform.localPosition = new Vector3(-3.32f, 1.83f, 0.5f); // Position décalée

        lumiere = lightObj.AddComponent<Light>();
        lumiere.type = LightType.Point;
        lumiere.color = couleurDoree;
        lumiere.range = 10f;
        lumiere.intensity = 0f;

        // S'enregistrer auprès du manager
        if (SocleManager.Instance != null)
        {
            SocleManager.Instance.EnregistrerSocle(this);
        }
    }

    void Update()
    {
        if (objetSurSocle && lumiere != null)
        {
            float lerp = Mathf.PingPong(Time.time * vitesseFlash, 1f);
            lumiere.intensity = Mathf.Lerp(2f, intensiteLumiereMax, lerp);
            socleMaterial.color = Color.Lerp(couleurOriginale, couleurDoree, lerp * 0.5f);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Crystal"))
        {
            print("🔹 Crystal posé sur: " + gameObject.name);
            objetSurSocle = true;

            if (sonActivation != null && audioSource != null)
            {
                audioSource.PlayOneShot(sonActivation);
                print("🔊 Son d'activation joué !");
            }

            if (SocleManager.Instance != null)
            {
                SocleManager.Instance.VerifierSocles();
            }
        }
    }


    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Crystal"))
        {
            print("🔸 Crystal retiré de: " + gameObject.name);
            objetSurSocle = false;

            if (sonDesactivation != null && audioSource != null)
            {
                audioSource.PlayOneShot(sonDesactivation);
            }

            if (lumiere != null)
            {
                lumiere.intensity = 0f;
            }

            socleMaterial.color = couleurOriginale;

            if (SocleManager.Instance != null)
            {
                SocleManager.Instance.VerifierSocles();
            }
        }
    }

    public bool AObjetDessus()
    {
        return objetSurSocle;
    }

    void OnDestroy()
    {
        if (SocleManager.Instance != null)
        {
            SocleManager.Instance.RetirerSocle(this);
        }
    }
}
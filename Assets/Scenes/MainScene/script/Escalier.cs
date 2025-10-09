using UnityEngine;

public class Escalier : MonoBehaviour
{
    public GameObject porteTP;
    public float vitesseApparition = 3f;
    public bool avecLumiere = true;
    public Color couleurLumiere = new Color(0.5f, 0f, 1f);
    public AudioClip sonApparition;

    private bool porteApparue = false;
    private Vector3 positionApparition;
    private Light lumiere;
    private float scaleOriginal;
    private AudioSource audioSource;

    void Start()
    {
        // Ajouter un AudioSource pour jouer le son
        audioSource = gameObject.AddComponent<AudioSource>();

        if (porteTP != null)
        {
            // Sauvegarder la position et taille d'origine
            positionApparition = porteTP.transform.position;
            scaleOriginal = porteTP.transform.localScale.x;

            // Cacher la porte (très petite)
            porteTP.transform.localScale = Vector3.zero;

            // Ajouter une lumière magique à la porte
            if (avecLumiere)
            {
                GameObject lightObj = new GameObject("LumierePorteTP");
                lightObj.transform.parent = porteTP.transform;
                lightObj.transform.localPosition = Vector3.zero;

                lumiere = lightObj.AddComponent<Light>();
                lumiere.type = LightType.Point;
                lumiere.color = couleurLumiere;
                lumiere.range = 8f;
                lumiere.intensity = 0f;
            }
        }
        else
        {
            Debug.LogWarning("⚠️ Aucune porte TP assignée à l'escalier !");
        }
    }

    void Update()
    {
        // Animation d'apparition de la porte
        if (porteApparue && porteTP != null)
        {
            // Grandir progressivement
            porteTP.transform.localScale = Vector3.Lerp(
                porteTP.transform.localScale,
                Vector3.one * scaleOriginal,
                Time.deltaTime * vitesseApparition
            );

            // Augmenter l'intensité de la lumière
            if (lumiere != null)
            {
                lumiere.intensity = Mathf.Lerp(lumiere.intensity, 3f, Time.deltaTime * vitesseApparition);

                // Petit effet de scintillement magique
                lumiere.intensity += Mathf.Sin(Time.time * 5f) * 0.3f;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Même logique que les socles
        print("🔍 COLLISION DÉTECTÉE avec : " + collision.gameObject.name);

        if (!porteApparue)
        {
            print("✨ Objet sur l'escalier : " + collision.gameObject.name);

            if (porteTP != null)
            {
                print("🚪 La porte de téléportation apparaît par magie !");
                porteApparue = true;

                // Jouer le son d'apparition
                if (sonApparition != null && audioSource != null)
                {
                    audioSource.PlayOneShot(sonApparition);
                    print("🔊 Son d'apparition joué !");
                }
            }
            else
            {
                print("❌ ERREUR : Aucune porte TP assignée !");
            }
        }
    }
}
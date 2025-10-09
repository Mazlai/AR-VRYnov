// ========================================
// Script 2: À mettre sur un GameObject vide nommé "SocleManager"
using UnityEngine;
using System.Collections.Generic;

public class SocleManager : MonoBehaviour
{
    public static SocleManager Instance;
    
    public GameObject porte;
    public Vector3 positionApparition = new Vector3(0, 0, 10);
    public float vitesseApparition = 2f;
    
    // Sons
    public AudioClip sonCinematic; // Son cinématique/dramatique
    public AudioClip sonBuild; // Son de construction
    
    private List<socle> tousLesSocles = new List<socle>();
    private bool tousActivesAvant = false;
    private Vector3 positionCachee;
    private bool porteApparue = false;
    private AudioSource audioSource1;
    private AudioSource audioSource2;
    
    void Awake()
    {
        // Singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        
        // Créer 2 AudioSources pour jouer 2 sons en même temps
        audioSource1 = gameObject.AddComponent<AudioSource>();
        audioSource2 = gameObject.AddComponent<AudioSource>();
    }
    
    void Start()
    {
        if (porte != null)
        {
            // Sauvegarder la position d'apparition actuelle de la porte
            positionApparition = porte.transform.position;
            // Cacher la porte au début (sous le sol)
            positionCachee = positionApparition - new Vector3(0, 10, 0);
            porte.transform.position = positionCachee;
        }
    }
    
    void Update()
    {
        // Animation de la porte qui monte
        if (porteApparue && porte != null)
        {
            porte.transform.position = Vector3.Lerp(
                porte.transform.position, 
                positionApparition, 
                Time.deltaTime * vitesseApparition
            );
        }
    }
    
    public void EnregistrerSocle(socle s)
    {
        if (!tousLesSocles.Contains(s))
        {
            tousLesSocles.Add(s);
            print("Socle enregistré: " + s.gameObject.name + " (Total: " + tousLesSocles.Count + ")");
        }
    }
    
    public void RetirerSocle(socle s)
    {
        tousLesSocles.Remove(s);
    }
    
    public void VerifierSocles()
    {
        int soclesActifs = 0;
        
        foreach (socle s in tousLesSocles)
        {
            if (s.AObjetDessus())
            {
                soclesActifs++;
            }
        }
        
        print("Socles avec objets: " + soclesActifs + "/" + tousLesSocles.Count);
        
        bool tousActifs = (soclesActifs == tousLesSocles.Count && tousLesSocles.Count > 0);
        
        // Déclencher l'événement seulement au moment où tous deviennent actifs
        if (tousActifs && !tousActivesAvant)
        {
            TousLesSoclesActifs();
        }
        
        tousActivesAvant = tousActifs;
    }
    
    private void TousLesSoclesActifs()
    {
        print("🎉 TOUS LES SOCLES ONT UN OBJET DESSUS ! 🎉");
        
        if (porte != null && !porteApparue)
        {
            print("🚪 La porte apparaît !");
            porteApparue = true;
            
            // Jouer les 2 sons en même temps
            if (sonCinematic != null && audioSource1 != null)
            {
                audioSource1.PlayOneShot(sonCinematic);
                print("🎬 Son cinématique joué !");
            }
            
            if (sonBuild != null && audioSource2 != null)
            {
                audioSource2.PlayOneShot(sonBuild);
                print("🔨 Son de construction joué !");
            }
        }
        else if (porte == null)
        {
            Debug.LogWarning("⚠️ Aucune porte assignée dans le SocleManager !");
        }
    }
}
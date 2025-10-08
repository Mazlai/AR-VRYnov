// ========================================
// Script 2: À mettre sur un GameObject vide nommé "SocleManager"
using UnityEngine;
using System.Collections.Generic;

public class SocleManager : MonoBehaviour
{
    public static SocleManager Instance;
    
    private List<socle> tousLesSocles = new List<socle>();
    private bool tousActivesAvant = false;
    
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
    
    [Header("Porte à faire apparaître")]
    public GameObject porte; // Glisse ta porte ici dans l'Inspector
    public Vector3 positionApparition = new Vector3(0, 0, 10);
    public float vitesseApparition = 2f;
    
    private Vector3 positionCachee;
    private bool porteApparue = false;
    
    void Start()
    {
        if (porte != null)
        {
            // Cacher la porte au début (sous le sol par exemple)
            positionCachee = porte.transform.position - new Vector3(0, 10, 0);
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
    
    private void TousLesSoclesActifs()
    {
        print("🎉 TOUS LES SOCLES ONT UN OBJET DESSUS ! 🎉");
        
        if (porte != null && !porteApparue)
        {
            print("🚪 La porte apparaît !");
            porteApparue = true;
        }
        else if (porte == null)
        {
            Debug.LogWarning("⚠️ Aucune porte assignée dans le SocleManager !");
        }
    }
}
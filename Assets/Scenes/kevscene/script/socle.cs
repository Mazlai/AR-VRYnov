// ========================================
// Script 1: socle.cs - À mettre sur CHAQUE socle (cube)
// ========================================
using UnityEngine;

public class socle : MonoBehaviour
{
    private Renderer socleRenderer;
    private Color couleurOriginale;
    private Color couleurDoree = new Color(1f, 0.84f, 0f);
    private bool objetSurSocle = false;
    private float vitesseFlash = 3f;
    
    void Start()
    {
        socleRenderer = GetComponent<Renderer>();
        couleurOriginale = socleRenderer.material.color;
        
        // S'enregistrer auprès du manager
        if (SocleManager.Instance != null)
        {
            SocleManager.Instance.EnregistrerSocle(this);
        }
    }
    
    void Update()
    {
        if (objetSurSocle)
        {
            float lerp = Mathf.PingPong(Time.time * vitesseFlash, 1f);
            socleRenderer.material.color = Color.Lerp(couleurOriginale, couleurDoree, lerp);
        }
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        print("Objet posé sur: " + gameObject.name);
        objetSurSocle = true;
        
        if (SocleManager.Instance != null)
        {
            SocleManager.Instance.VerifierSocles();
        }
    }
    
    private void OnCollisionExit(Collision collision)
    {
        print("Objet retiré de: " + gameObject.name);
        objetSurSocle = false;
        socleRenderer.material.color = couleurOriginale;
        
        if (SocleManager.Instance != null)
        {
            SocleManager.Instance.VerifierSocles();
        }
    }
    
    // MÉTHODE IMPORTANTE : permet au Manager de vérifier l'état
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
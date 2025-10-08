using UnityEngine;
using UnityEngine.UI;

public class MenuVR : MonoBehaviour
{
    [Header("R�f�rences UI")]
    public GameObject menuPanel;        // Le panel du menu
    public GameObject welcomeText;      // Votre texte avec animation
    public Button boutonJouer;
    public Button boutonQuitter;

    void Start()
    {
        // Assure que le menu est visible et le texte cach� au d�part
        if (menuPanel != null)
            menuPanel.SetActive(true);

        if (welcomeText != null)
            welcomeText.SetActive(false);

        // Assigne les fonctions aux boutons
        if (boutonJouer != null)
            boutonJouer.onClick.AddListener(OnJouerClick);

        if (boutonQuitter != null)
            boutonQuitter.onClick.AddListener(OnQuitterClick);
    }

    void OnJouerClick()
    {
        // Cache le menu
        if (menuPanel != null)
            menuPanel.SetActive(false);

        // Affiche le texte de bienvenue (qui jouera son animation)
        if (welcomeText != null)
        {
            welcomeText.SetActive(true);

            // R�active l'Animator au cas o�
            Animator animator = welcomeText.GetComponent<Animator>();
            if (animator != null)
            {
                animator.enabled = true;
                // Rejoue l'animation depuis le d�but
                animator.Play(animator.GetCurrentAnimatorStateInfo(0).fullPathHash, 0, 0f);
            }
        }
    }

    void OnQuitterClick()
    {
        Debug.Log("Quitter le jeu");

        // Ferme l'application
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // Fonction optionnelle pour revenir au menu
    public void RetourMenu()
    {
        if (welcomeText != null)
            welcomeText.SetActive(false);

        if (menuPanel != null)
            menuPanel.SetActive(true);
    }
}
using UnityEngine;
using UnityEngine.SceneManagement;

public class PorteFin : MonoBehaviour
{
    public string messageVictoire = "🎉 VICTOIRE ! PARTIE TERMINÉE ! 🎉";
    public AudioClip sonVictoire; // Son optionnel de victoire
    public float delaiAvantFin = 2f; // Temps avant de fermer le jeu (en secondes)

    private AudioSource audioSource;
    private bool partieTerminee = false;

    void Start()
    {
        // Ajouter un AudioSource pour le son de victoire
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        #if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
        #else
                Application.Quit();
        #endif

        // Vérifier si c'est le joueur qui touche la porte
        if (other.CompareTag("Player") && !partieTerminee)
        {
            partieTerminee = true;
            TerminerPartie();
        }
    }

    private void TerminerPartie()
    {
        print(messageVictoire);

        // Jouer le son de victoire
        if (sonVictoire != null && audioSource != null)
        {
            audioSource.PlayOneShot(sonVictoire);
        }

        // Arrêter le temps (freeze le jeu)
        Time.timeScale = 0f;

        // Attendre puis fermer ou recharger
        Invoke("FermerJeu", delaiAvantFin);
    }

    private void FermerJeu()
    {
        // Remettre le temps à la normale avant de recharger
        Time.timeScale = 1f;

        // Recharger la scène actuelle pour rejouer
        print("🔄 Rechargement du jeu...");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
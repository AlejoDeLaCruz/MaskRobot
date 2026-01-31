using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Singleton simple (opcional, pero útil)
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else Destroy(gameObject);
    }

    // Método para "matar" al jugador y reiniciar la escena actual
    public void PlayerDied()
    {
        Debug.Log("Jugador muerto -> reiniciando escena");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
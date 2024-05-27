using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneReloader : MonoBehaviour
{
    public void ReloadScene()
    {
        // Obtém a cena ativa atualmente
        Scene currentScene = SceneManager.GetActiveScene();
        // Recarrega a cena ativa
        SceneManager.LoadScene(currentScene.name);
    }
}


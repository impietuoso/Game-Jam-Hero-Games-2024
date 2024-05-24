using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public CanvasGroup loadingScreen;

    public void LoadScene(int scene) {
        SceneManager.LoadSceneAsync(scene);
    }

    private IEnumerator LoadingScene() {
        yield return null;
    }

    public void ExitGame() {
        Application.Quit();
    }
}

using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour {
    private void Start() {
        VsnController.instance.StartVSN("start_scene");
    }

    public void LoadScene(int scene) {
        if(Time.timeScale == 0) Time.timeScale = 1;
        switch (scene) {
            case 0:
                VsnController.instance.StartVSN("loading_menu");
                break;
            case 1:
                VsnController.instance.StartVSN("loading_base");
                break;
            case 2:
                VsnController.instance.StartVSN("loading_island");
                break;
            case 3:
                VsnController.instance.StartVSN("loading_finalbattle");
                break;
        }
    }

    public void ExitGame() {
        Application.Quit();
    }
}
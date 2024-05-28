using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour {
    [TextArea(5, 10)]
    public string baseObjective;
    [TextArea(5, 10)]
    public string islandObjective;
    [TextArea(5, 10)]
    public string finalbattleObjective;

    public Save save;

    private void Start() {
        VsnController.instance.StartVSN("start_scene");
    }

    public void LoadScene(int scene) {
        if(Time.timeScale == 0) Time.timeScale = 1;
        switch (scene) {
            case 0:
                ChangeObjetive("");
                VsnController.instance.StartVSN("loading_menu");
                break;
            case 1:
                ChangeObjetive(baseObjective);
                VsnController.instance.StartVSN("loading_base");
                break;
            case 2:
                ChangeObjetive(islandObjective);
                VsnController.instance.StartVSN("loading_island");
                break;
            case 3:
                ChangeObjetive(finalbattleObjective);
                VsnController.instance.StartVSN("loading_finalbattle");
                break;
        }
    }

    public void Reload() {
        LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ChangeObjetive(string nextObjective) {
        save.objective = nextObjective;
    }

    public void ExitGame() {
        Application.Quit();
    }
}
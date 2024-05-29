using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour {
    public Objective[] objectives;

    public Save save;
    [SerializeField] int scene;

    private void Awake() {
        
    }

    private void Start() {
        StartScene();
    }

    void StartScene() {
        scene = SceneManager.GetActiveScene().buildIndex;
        var newObjective = scene > 0 ? objectives[scene - 1].objetiveText : "";
        switch (scene) {
            case 0:
                ChangeObjetive("");
                VsnController.instance.StartVSN("open_menu");
                break;
            case 1:
                ChangeObjetive(newObjective);
                VsnController.instance.StartVSN("open_base");
                break;
            case 2:
                ChangeObjetive(newObjective);
                VsnController.instance.StartVSN("open_island");
                break;
            case 3:
                ChangeObjetive(newObjective);
                VsnController.instance.StartVSN("open_finalbattle");
                break;
        }
    }

    public void LoadScene(int scene) {
        if(Time.timeScale == 0) Time.timeScale = 1;
        var newObjective = scene > 0 ? objectives[scene - 1].objetiveText : "";
        switch (scene) {
            case 0:
                ChangeObjetive("");
                VsnController.instance.StartVSN("loading_menu");
                break;
            case 1:
                ChangeObjetive(newObjective);
                VsnController.instance.StartVSN("loading_base");
                break;
            case 2:
                ChangeObjetive(newObjective);
                VsnController.instance.StartVSN("loading_island");
                break;
            case 3:
                ChangeObjetive(newObjective);
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

[Serializable]
public class Objective {
    [TextArea(10, 15)]
    public string objetiveText;
}
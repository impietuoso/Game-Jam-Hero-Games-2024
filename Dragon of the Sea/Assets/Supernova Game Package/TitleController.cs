using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;


public class TitleController : MonoBehaviour {
  public static TitleController instance;

  public Panel optionsPanel;
  public Transform logoImage;
  public bool demoVersion = false;


  public void Awake() {
    instance = this;
  }

  public void Start() {
    Time.timeScale = 1f;
    VsnSaveSystem.CleanAllData();
    //VsnAudioManager.instance.PlayMusic("", "Playlist");
    VsnController.instance.StartVSN("fade in");

    logoImage.localScale = Vector3.zero;
    StartCoroutine(logoAnim());
  }

  public void OnLoad(string loadedData) {
    Debug.LogWarning("LOADED DATA: " + loadedData);
  }


  public IEnumerator logoAnim() {
    yield return new WaitForSeconds(0.1f);
    logoImage.AnimTweenPopAppear();
    yield return new WaitForSeconds(0.7f);
    logoImage.DOScale(1.05f, 1.5f).SetLoops(-1, LoopType.Yoyo);
  }



  public void Update() {
    if(Input.GetKeyDown(KeyCode.Return) ||
       Input.GetKeyDown(KeyCode.Space)) {
      LoadStage();
    }

    if(Application.isEditor && Input.GetKeyDown(KeyCode.F5)) {
      SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
  }


  public void ClickedNewGameButton() {
    VsnController.instance.StartVSN("goto gameplay");
    //LoadStage();
  }

  public void ClickedContinueButton() {
    //VsnController.instance.state = ExecutionState.WAITINGSAVEORLOAD;
    VsnSaveSystem.Load(0);
    //VsnController.instance.StartVSNContent("load 0\nwait 0.1", "custom");
    VsnController.instance.StartVSN("goto gameplay");
    //LoadStage();
  } 
     
  public void ClickedOptionsButton() {
    //VsnController.instance.StartVSN("stage select");
    optionsPanel.ShowPanel();
  }

  public void ClickedExitButton() {
    VsnController.instance.StartVSN("exit game");
  }

  public void LoadStage() {
    SceneManager.LoadScene("Stage Select");
  }
}

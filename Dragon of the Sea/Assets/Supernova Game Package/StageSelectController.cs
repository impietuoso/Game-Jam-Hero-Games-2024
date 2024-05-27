using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StageSelectController : MonoBehaviour {

  public static StageSelectController instance;

  public Transform[] worldPanelContents;
  public World[] worlds;

  public static int[] worldStageCount = new int[] {3, 4, 3, 3};
  public static int[] worldRequirement = new int[] {0, 2, 5, 10};

  public bool instantiateStageButtons = true;


  public void Awake() {
    instance = this;
  }


  public void Start() {
    Time.timeScale = 1f;
    int numStages = CountStages();
    VsnSaveSystem.SetVariable("total_stages", numStages);

    ///// load save
    //if(VsnSaveSystem.IsSaveSlotBusy(0)) {
    //  VsnSaveSystem.Load(0);
    //}

    InitializeWorlds(numStages);

    VsnController.instance.StartVSN("fade in");
  }

  public static int CountStages() {
    return 13;
    //Texture2D[] ts = Resources.LoadAll<Texture2D>("Pics");
    //return ts.Length;
  }

  public void InitializeWorlds(int numButtons) {
    for(int i=0; i<worlds.Length; i++) {
      worlds[i].Initialize(i);
    }
  }

  public static int GetWorldByStageId(int stageId) {
    for(int i=0; i< worldStageCount.Length; i++) {
      if(stageId <= worldStageCount[i]) {
        return i;
      }
      stageId -= worldStageCount[i];
    }
    return 0;
  }

  public static bool LastStageInWorld(int stageId) {
    for(int i = 0; i < worldStageCount.Length; i++) {
      if(stageId == worldStageCount[i]) {
        return true;
      }
      stageId -= worldStageCount[i];
    }
    return false;
  }

  public void Test() {
    Debug.LogWarning("Finished: "+ CountFinishedStages());
  }

  public bool IsStageUnlocked(int stageId) {
    if(stageId == 1) {
      return true;
    }
    return VsnSaveSystem.GetBoolVariable("stage" + (stageId-1) + "_finished");
  }

  public static int CountFinishedStages() {
    int count = 0;
    for(int i=1; i <= CountStages(); i++) {
      if(VsnSaveSystem.GetBoolVariable("stage"+i+"_finished")){
        count++;
      }
    }
    return count;
  }
}

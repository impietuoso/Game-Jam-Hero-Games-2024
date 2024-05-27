using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadVsnScriptWhenActivated : MonoBehaviour {
  public string scriptName;
  public bool loadWhenDeactivated;
  public bool disableWhenUsed = false;

  public void OnEnable() {
    if(!loadWhenDeactivated) {
      LoadScript();
    }
  }

  public void OnDisable() {
    if(loadWhenDeactivated) {
      LoadScript();
    }
  }

  public void LoadScript() {
    VsnController.instance.StartVSN(scriptName);
    if(disableWhenUsed) {
      enabled = false;
    }
  }
}

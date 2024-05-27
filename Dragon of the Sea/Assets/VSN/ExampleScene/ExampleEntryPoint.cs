using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Example entry point for the new VSN system. Just call VsnController.instance.StartVSN with the script path.
/// </summary>
public class ExampleEntryPoint : MonoBehaviour {

  public string scriptToPlay;
  public bool loadScriptOnStart = false;
  public bool disableWhenUsed = false;

  public float waitTimeBeforeLoad = 0.01f;


  // Script is in the Resources folder. (e.g "VSN Scripts/example3" loads from "Resources/VSN Scripts/example3").
  // "example1": basic Say, goto, waypoint
  // "example2": say with rich tags, choices
  // "example3": characters, alpha, move, say with text
  // "example4": example of transition with movex and wait with a character

  void Start () {
    if(loadScriptOnStart){
      //  		VsnSaveSystem.Load(0);
      if(waitTimeBeforeLoad == 0f) {
        LoadScript();
      } else {
        StartCoroutine(WaitToLoadScript(waitTimeBeforeLoad));
      }
    }
	}

  public IEnumerator WaitToLoadScript(float time) {
    yield return new WaitForSeconds(time);
    LoadScript();
  }

  public void LoadScript(){
    VsnController.instance.StartVSN(scriptToPlay);
    if(disableWhenUsed) {
      enabled = false;
    }
  }
}

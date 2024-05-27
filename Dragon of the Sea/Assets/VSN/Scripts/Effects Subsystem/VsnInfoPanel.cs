using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class VsnInfoPanel : Panel {
  public TextMeshProUGUI infoText;
  public bool waitToResume;
  public float animTime;


  public void Initialize(string text, float argAnimTime, bool waitToResumeVsn) {
    infoText.text = text;
    animTime = argAnimTime;
    waitToResume = waitToResumeVsn;
    gameObject.SetActive(false);
    ShowPanel();
    StopAllCoroutines();
    StartCoroutine(WaitToCloseInfo());
  }

  public override void PosHidePanel() {
    if(waitToResume && VsnController.instance.state == ExecutionState.WAITINGTOSHOWINFO) {
      VsnController.instance.state = ExecutionState.PLAYING;
    }
  }

  public IEnumerator WaitToCloseInfo() {
    yield return new WaitForSecondsRealtime(fadeTime); 
    yield return new WaitForSecondsRealtime(animTime - fadeTime*2f);
    HidePanel();
  }
}

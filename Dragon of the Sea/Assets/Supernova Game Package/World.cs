using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class World : MonoBehaviour {

  public Transform buttonsContent;
  public GameObject lockedIcon;
  public TextMeshProUGUI lockedText;
  public GameObject buttonPrefab;

  public int worldId;

  public void Initialize(int id) {
    worldId = id;
    int worldRequirement = StageSelectController.worldRequirement[worldId];
    int stagesFinished = StageSelectController.CountFinishedStages();

    if(stagesFinished < worldRequirement) {
      lockedIcon.SetActive(true);
      lockedText.text = "<sprite=\"Nonogram Icons\" index=0>" + stagesFinished.ToString() + " <size=70%>/" + worldRequirement+"</size>";
    } else {
      lockedText.gameObject.SetActive(false);
    }

    if(StageSelectController.instance.instantiateStageButtons) {
      InstantiateButtons();
    }
  }

  public void InstantiateButtons() {
    GameObject newobj;
    int numButtons = StageSelectController.worldStageCount[worldId];
    int firstId = InitialStage();

    for(int i = 1; i <= numButtons; i++) {
      //Debug.LogWarning("Button number: " + i);
      newobj = Instantiate(buttonPrefab, buttonsContent);
      newobj.GetComponentInChildren<StageSelectButton>().Initialize(firstId + i);
    }
  }

  public int InitialStage() {
    int count = 0;
    for(int i = 0; i < worldId; i++) {
      count += StageSelectController.worldStageCount[i];
    }
    return count;
  }
}

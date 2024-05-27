using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class StageSelectButton : MonoBehaviour {

  public int stageId = 0;
  public Image finishedImage;
  public TextMeshProUGUI stageText;
  public TextMeshProUGUI dimensionText;
  public GameObject completedIcon;

  public Image lockImage;
  public Image unlockImage;

  public bool isUnlocked;


  public bool IsStageFinished {
    get { return VsnSaveSystem.GetBoolVariable("stage" + stageId + "_finished"); }
  }

  public bool IsUnlocked {
    get {
      return StageSelectController.instance.IsStageUnlocked(stageId);
    }
  }


  public bool IsBeingEditted {
    get { return false; }
    //get { return VsnSaveSystem.GetStringVariable("stage" + stageId + "_progress") != "";  }
  }


  public void Initialize(int stage) {
    //Debug.LogWarning("Initializing Button");
    stageId = stage;

    UpdateGraphics();
  }


  public void Start() {
    UpdateGraphics();
  }

  public void UpdateGraphics() {
    if(IsUnlocked) {
      stageText.text = stageId.ToString();
      lockImage.gameObject.SetActive(false);
      unlockImage.gameObject.SetActive(true);
    } else {
      lockImage.gameObject.SetActive(true);
      unlockImage.gameObject.SetActive(false);
      stageText.text = "";
    }
    completedIcon.SetActive(IsStageFinished);
    //completedIcon.SetActive(IsStageFinished || true); // debug: show stars
  }

  public Texture2D AnswerSprite() {
    Texture2D texture = Resources.Load<Texture2D>("Pics/" + stageId);
    if(texture == null) {
      return null;
    }
    Color[] textureColors = texture.GetPixels();
    int width, height;

    width = texture.width / 2;
    height = texture.height;

    Texture2D visibleTexture = new Texture2D(width, height, TextureFormat.ARGB32, false);

    for(int i = 0; i < texture.width / 2; i++) {
      for(int j = 0; j < texture.height; j++) {
        int index = i + j * texture.width;
        visibleTexture.SetPixel(i, j, textureColors[index]);
      }
    }
    visibleTexture.Apply();
    visibleTexture.wrapMode = TextureWrapMode.Clamp;
    visibleTexture.filterMode = FilterMode.Point;
    return visibleTexture;
  }


  public void ClickedLoadStage() {
    if(IsUnlocked) {
      VsnSaveSystem.SetVariable("stage_to_load", "Stage " + stageId);
      VsnSaveSystem.SetVariable("restart_stage", false);
      VsnController.instance.StartVSN("goto gameplay");
    } else {
      VsnAudioManager.instance.PlaySfx("ui_locked");
      VsnController.instance.StartVSN("stage_locked");
    }
  }
}

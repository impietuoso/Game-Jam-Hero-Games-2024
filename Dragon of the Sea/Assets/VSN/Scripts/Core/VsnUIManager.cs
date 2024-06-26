using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Command;
using TMPro;
using DG.Tweening;


public class VsnUIManager : MonoBehaviour {

  public static VsnUIManager instance;

  [Header("- Dialog Box -")]
  public VsnConsoleSimulator consoleSimulator;
  public GameObject vsnMessagePanel;
  public GameObject clickMessageIcon;
  public TextMeshProUGUI vsnMessageText;
  public TextMeshProUGUI vsnMessageTitle;
  public Image vsnMessageTitlePanel;
  public GameObject talkingDialogArrow;
  public Image[] dialogBoxImages;
  public bool isTextAppearing;
  public int charsToShowPerSecond = 50;

  public Button screenButton;
  public RectTransform charactersPanel;
  public Image backgroundImage;
  public GameObject choicesPanel;
  public Button[] choicesButtons;
  public TextMeshProUGUI[] choicesTexts;

  [Header("- Info Panel -")]
  public VsnInfoPanel infoPanel;

  public Image vsnFaceIcon;

  [Header("- Letter Panel -")]
  public GameObject letterPanel;
  public Image letterImage;
  public TextMeshProUGUI letterTitleText;
  public TextMeshProUGUI letterText;
  public ScrollRect letterScrollRect;

  [Header("- Text Input Panel -")]
  public GameObject textInputPanel;
  public TextMeshProUGUI textInputPanelDescriptionText;
  public TMP_InputField textInputField;

  [Header("- Log Panel -")]
  public VsnLogWindow logWindow;

  [Header("- Debug Panel -")]
  public VsnVariablesDebugPanel variablesDebugPanel;

  [Header("- Skip Button -")]
  public Button skipButton;
  public string skipButtonWaypoint;

  [Header("- Characters Layer -")]
  public GameObject vsnCharacterPrefab;
  public List<VsnCharacter> characters;



  void Awake() {
    if(instance == null) {
      instance = this;
    }

    screenButton.onClick.AddListener(AdvanceTextInput);
    characters = new List<VsnCharacter>();
  }


  public static void SelectUiElement(GameObject toSelect) {
    EventSystem.current.SetSelectedGameObject(toSelect);
  }


  public void ShowDialogPanel(bool value) {
    vsnMessagePanel.SetActive(value);
  }

  public void ShowText(string msg) {
    ShowClickMessageIcon(false);
    Utils.SelectUiElement(screenButton.gameObject);
    if(!string.IsNullOrEmpty(vsnMessageTitle.text)) {
      if(msg[0] == '(') {
        vsnMessageText.text = "(" + msg.Substring(1, msg.Length - 2) + ")";
      } else {
        vsnMessageText.text = "\"" + msg + "\"";
      }
    } else{
    vsnMessageText.text = msg;
    }
    consoleSimulator.callAfterShowCharacters = FinishShowingCharacters;
    consoleSimulator.StartShowingCharacters();
  }


  public void FinishShowingCharacters(){
    ShowClickMessageIcon(true);
    isTextAppearing = false;
    if(consoleSimulator.autopass) {
      AdvanceTextInput();
      consoleSimulator.autopass = false;
    }
  }

  public void SetTextAuto() {
    consoleSimulator.SetAutoPassText();
  }

  public void ShowClickMessageIcon(bool value){
    clickMessageIcon.SetActive(value);
  }

  public void SetTextTitle(string messageTitle) {
    vsnMessageTitle.text = messageTitle;
    if(string.IsNullOrEmpty(messageTitle)) {
      vsnMessageTitlePanel.gameObject.SetActive(false);
    } else {
      vsnMessageTitlePanel.gameObject.SetActive(true);
    }
    /// TODO: Implement support for VSN facesets
    //if(GlobalData.instance.GetFaceByName(messageTitle) != null) {
    //  vsnFaceIcon.transform.parent.gameObject.SetActive(true);
    //  vsnFaceIcon.sprite = GlobalData.instance.GetFaceByName(messageTitle);
    //  VsnAudioManager.instance.SetDialogSfxPitch(GlobalData.instance.GetPitchByName(messageTitle));
    //  VsnAudioManager.instance.SetDialogSfx(GlobalData.instance.GetDialogSfxByName(messageTitle));
    //} else {
    //  vsnFaceIcon.transform.parent.gameObject.SetActive(false);
    //  VsnAudioManager.instance.SetDialogSfxPitch(1f);
    //  VsnAudioManager.instance.SetDialogSfx(null);
    //}
  }

  public void AdvanceTextInput() {
    if(isTextAppearing) {
      isTextAppearing = false;
      consoleSimulator.FinishShowingCharacters();
    } else if(VsnController.instance.state == ExecutionState.WAITINGTOUCH) {
      VsnAudioManager.instance.PlayDialogAdvanceSfx();

      /// Log text
      logWindow.AddToLog(vsnMessageTitle.text, vsnMessageText.text);

      VsnController.instance.state = ExecutionState.PLAYING;
      ShowClickMessageIcon(false);
      ShowDialogPanel(false);
    }
  }

  private void AddChoiceButtonListener(Button button, string label) {
    button.onClick.RemoveAllListeners();
    button.onClick.AddListener(() => {
      string myLabel = label;
      if(!string.IsNullOrEmpty(myLabel)) {
        GotoCommand.StaticExecute(myLabel);
      }
      VsnAudioManager.instance.PlaySfx("ui_confirm");
      ShowChoicesPanel(false, 0);
      ShowDialogPanel(false);
      VsnController.instance.state = ExecutionState.PLAYING;
    });
  }

  public void ShowChoicesPanel(bool enable, int numberOfChoices) {
    choicesPanel.SetActive(enable);

    if(enable) {
      for(int i = 0; i < choicesButtons.Length; i++) {
        bool willSetActive = (i < numberOfChoices);
        choicesButtons[i].gameObject.SetActive(willSetActive);
      }

      Utils.GenerateNavigation(choicesButtons);
      Utils.SelectUiElement(choicesButtons[0].gameObject);
    }
  }

  public void SetChoicesTexts(string[] choices) {
    for(int i = 0; i < choices.Length; i++) {
      choicesTexts[i].text = choices[i];
    }
  }

  public void SetChoicesLabels(string[] labels) {
    for(int i = 0; i < labels.Length; i++) {
      AddChoiceButtonListener(choicesButtons[i], labels[i]);
    }
  }

  public void OpenLetterPanel(Sprite image, string letterTitle, string letterContent){
    letterImage.sprite = image;
    letterTitleText.text = letterTitle;
    letterText.text = letterContent;
    OpenLetterPanel();
  }

  public void OpenLetterPanel(){
    letterScrollRect.verticalNormalizedPosition = 1f;
    letterPanel.SetActive(true);
  }

  public void CloseLetterPanel(){
    letterPanel.SetActive(false);
    VsnController.instance.GotCustomInput();
  }

  public void SetCharacterSprite(string characterFilename, string characterLabel) {
    Sprite characterSprite = Resources.Load<Sprite>("Characters/" + characterFilename);
    if(characterSprite == null) {
      Debug.LogError("Error loading " + characterFilename + " character sprite. Please check its path");
      return;
    }

    foreach(VsnCharacter character in characters) {
      if(character.label == characterLabel) {
        character.SetData(characterSprite, characterLabel);
        return;
      }
    }
    CreateNewCharacter(characterSprite, characterFilename, characterLabel);
  }

  public void CreateNewCharacter(Sprite characterSprite, string characterFilename, string characterLabel) {
    GameObject vsnCharacterObject = Instantiate(vsnCharacterPrefab, charactersPanel.transform) as GameObject;
    vsnCharacterObject.transform.localScale = Vector3.one;
    VsnCharacter vsnCharacter = vsnCharacterObject.GetComponent<VsnCharacter>();

    Vector2 newPosition = Vector2.zero;
    vsnCharacter.GetComponent<RectTransform>().anchoredPosition = newPosition;

    vsnCharacter.SetData(characterSprite, characterLabel);
    characters.Add(vsnCharacter);
  }

  public void MoveCharacterX(string characterLabel, float position, float duration) {
    float screenPosition = GetCharacterScreenPositionX(position);
    VsnCharacter character = FindCharacterByLabel(characterLabel);

    //Debug.LogWarning("Original pos: "+position+", final pos: " + screenPosition);

    if(character != null) {
      Vector2 newPosition = new Vector2(screenPosition, character.GetComponent<RectTransform>().anchoredPosition.y);
      if(duration != 0) {
        character.GetComponent<RectTransform>().DOAnchorPos(newPosition, duration).SetUpdate(true).SetEase(Ease.Linear);
      } else {
        character.GetComponent<RectTransform>().anchoredPosition = newPosition;
      }
    }
  }

  public void MoveCharacterY(string characterLabel, float position, float duration) {
    float screenPosition = GetCharacterScreenPositionY(position);
    VsnCharacter character = FindCharacterByLabel(characterLabel);

    if(character != null) {
      Vector2 newPosition = new Vector2(character.GetComponent<RectTransform>().anchoredPosition.x, screenPosition);
      if(duration != 0) {
        character.GetComponent<RectTransform>().DOAnchorPos(newPosition, duration).SetUpdate(true);
      } else {
        character.GetComponent<RectTransform>().anchoredPosition = newPosition;
      }
    }

  }

  public void SetCharacterAlpha(string characterLabel, float alphaValue, float duration) {
    VsnCharacter character = FindCharacterByLabel(characterLabel);

    if(character != null) {
      Image characterImage = character.GetComponent<Image>();
      if(duration > 0f) {
        characterImage.DOFade(alphaValue, duration).SetUpdate(true);
      } else {
        characterImage.color = new Color(characterImage.color.r,
                                         characterImage.color.g,
                                         characterImage.color.b, alphaValue);
      }
    }
  }

  private float GetCharacterScreenPositionX(float normalizedPositionX) {
    float zeroPoint = -charactersPanel.rect.width/2f;
    float onePoint = charactersPanel.rect.width/2f;
    float totalSize = onePoint - zeroPoint;

    float finalPositionX = zeroPoint + normalizedPositionX * totalSize;
    return finalPositionX;
  }

  private float GetCharacterScreenPositionY(float normalizedPositionY) {
    int maxPoint = 500;
    int minPoint = 200;
    int totalPoints = Mathf.Abs(maxPoint) + Mathf.Abs(minPoint);

    if(normalizedPositionY < 0f)
      return minPoint;
    else if(normalizedPositionY > 1f)
      return maxPoint;

    float finalPositionY = normalizedPositionY * totalPoints;
    Debug.Log("Final Y: " + finalPositionY);
    return finalPositionY;
  }

  private VsnCharacter FindCharacterByLabel(string characterLabel) {
    foreach(VsnCharacter character in characters) {
      if(character.label == characterLabel) {
        return character;
      }
    }
    return null;
  }

  public void FlipCharacterSprite(string characterLabel) {
    VsnCharacter character = FindCharacterByLabel(characterLabel);

    if(character == null){
      Debug.LogError("Error flipping character " + characterLabel + ". Character not found with this label.");
      return;
    }

    Vector3 localScale = character.transform.localScale;
    character.transform.localScale = new Vector3(-1f*localScale.x, localScale.y, localScale.z);
    Debug.LogWarning("Flipping character " + character.name + ". New scale: " + character.transform.localScale);
  }

  public void ScaleCharacter(string characterLabel, float scaleValue, float duration) {
    VsnCharacter character = FindCharacterByLabel(characterLabel);

    Vector3 currentScale = character.transform.localScale;
    Vector3 endScale = scaleValue * new Vector3(currentScale.x / Mathf.Abs(currentScale.x),
                                                currentScale.y / Mathf.Abs(currentScale.y),
                                                currentScale.z / Mathf.Abs(currentScale.z));

    if(character != null) {
      if(duration != 0) {
        character.GetComponent<RectTransform>().DOScale(endScale, duration).SetUpdate(true);
      } else {
        character.GetComponent<RectTransform>().localScale = endScale;
      }
    }
  }


  public void ResetAllCharacters() {
    foreach(VsnCharacter character in characters) {
      Destroy(character.gameObject);
    }
    characters.Clear();
  }

  public void SetBackground(Sprite backgroundSprite) {
    if(backgroundSprite != null) {
      backgroundImage.sprite = backgroundSprite;
      backgroundImage.gameObject.SetActive(true);
    } else {
      ResetBackground();
    }
  }

  public void ResetBackground() {
    backgroundImage.gameObject.SetActive(false);
  }


  public void SetSkipButtonWaypoint(string waypoint){
    skipButtonWaypoint = waypoint;
    AddChoiceButtonListener(skipButton, waypoint);
  }

  public void ShowSkipButton(bool show){
    skipButton.gameObject.SetActive(show);
  }

  public void SetTextInputDescription(string msg){
    textInputPanelDescriptionText.text = msg;
  }

  public void SetTextInputCharacterLimit(int limit){
    textInputField.characterLimit = limit;
  }

  public void ShowTextInput(bool show){
    textInputPanel.SetActive(show);
    if(show == true){
      SelectUiElement(null);
      textInputField.text = "";
      SelectUiElement(textInputField.gameObject);
    }
  }

  public void OnTextInputConfirm() {
    if(string.IsNullOrEmpty(textInputField.text)){
//      SoundManager.PlayForbiddenSound();
      return;
    }

    VsnController.instance.state = ExecutionState.PLAYING;
    if(textInputField.text != ""){
      VsnSaveSystem.SetVariable("text_input", textInputField.text);
    }
    ShowTextInput(false);
  }

  public void SetDialogBoxPosition(string position) {
    RectTransform rect = vsnMessageText.transform.parent.GetComponent<RectTransform>();

    switch(position) {
      case "center":
        rect.anchoredPosition = Vector2.zero;
        break;
      case "up":
        rect.anchoredPosition = new Vector2(0f, 176f);
        break;
      case "down":
        rect.anchoredPosition = new Vector2(0f, -176f);
        break;
    }
  }

  public void SetTextBaseColor(Color color) {
    vsnMessageText.color = color;
  }

  public void SetDialogBoxInvisible(bool isInvisible) {
    foreach(Image img in dialogBoxImages) {
      if(isInvisible) {
        img.enabled = false;
      } else {
        img.enabled = true;
      }      
    }
  }

  public void ShowInfo(string textToShow, float animTime = 2f, bool waitToResumeVsn = false) {
    if(waitToResumeVsn) {
      VsnController.instance.state = ExecutionState.WAITINGTOSHOWINFO;
    }
    infoPanel.Initialize(textToShow, animTime, waitToResumeVsn);
  }
}


using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Linq;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;


public static class Constants {
  public const string gameplayContextName = "Gameplay";
  public const string pauseContextName = "Pause Menu";
  public const string dialogContextName = "VSN Dialog";
  public const string choiceContextName = "VSN Choice";

  public static string grayColorTag = "<color=#234067>";
  public static string highlightColorTag = "<color=#7C3047>";
  public static string greenHighlightColorTag = "<color=#166033>";
  public static string closeColorTag = "</color>";

  public static string soulsTag = "<sprite=\"TextIcons\" index=1 tint>";
  public static string crownTag = "<sprite=\"TextIcons\" index=0 tint>";
}



public class Utils {

  public static void UnselectButton() {
    EventSystem.current.SetSelectedGameObject(null);
  }

  public static void SetButtonDisabledGraphics(Button but) {
    //SpriteState st = new SpriteState();
    //st.disabledSprite = ResourcesController.instance.buttonSprites[3];
    //st.highlightedSprite = ResourcesController.instance.buttonSprites[4];
    //st.pressedSprite = ResourcesController.instance.buttonSprites[5];
    //but.spriteState = st;
    //but.GetComponent<Image>().sprite = ResourcesController.instance.buttonSprites[3];
  }

  public static void SetButtonEnabledGraphics(Button but) {
    //SpriteState st = new SpriteState();
    //st.disabledSprite = ResourcesController.instance.buttonSprites[0];
    //st.highlightedSprite = ResourcesController.instance.buttonSprites[1];
    //st.pressedSprite = ResourcesController.instance.buttonSprites[2];
    //but.spriteState = st;
    //but.GetComponent<Image>().sprite = ResourcesController.instance.buttonSprites[0];
  }


  public static void SelectUiElement(GameObject toSelect) {
    EventSystem.current.SetSelectedGameObject(toSelect);
  }

  public static GameObject GetSelectedElement() {
    return EventSystem.current.currentSelectedGameObject;
  }

  public static void PrintSelectedElement() {
    if(GetSelectedElement() != null) {
      Debug.LogWarning("Element selected name: " + Utils.GetSelectedElement().name);
    } else {
      Debug.LogWarning("No element selected!");
    }
  }

  public static void GenerateNavigation(Button[] navigatableObjects) {
    Navigation navi = new Navigation {
      mode = Navigation.Mode.Explicit
    };

    if(navigatableObjects.Length <= 1) {
      return;
    }

    for(int i = 0; i < navigatableObjects.Length; i++) {
      if(i == 0) {
        navi.selectOnDown = navigatableObjects[i + 1];
        navi.selectOnUp = null;
      } else if(i == navigatableObjects.Length - 1) {
        navi.selectOnDown = null;
        navi.selectOnUp = navigatableObjects[i - 1];
      } else {
        navi.selectOnUp = navigatableObjects[i - 1];
        navi.selectOnDown = navigatableObjects[i + 1];
      }
      navigatableObjects[i].navigation = navi;
    }
  }

  public static Color GetColorByString(string colorName) {
    switch(colorName) {
      case "red":
        return Color.red;
      case "green":
        return Color.green;
      case "blue":
        return Color.blue;
      case "yellow":
        return Color.yellow;
      case "cyan":
        return Color.cyan;
      case "magenta":
        return Color.magenta;
      case "gray":
      case "grey":
        return Color.gray;
      case "white":
        return Color.white;
      case "black":
        return Color.black;
    }
    Color c;
    if(ColorUtility.TryParseHtmlString(colorName, out c)) {
      return c;
    }
    return Color.magenta;
  }

  public static Color ChangeColorAlpha(Color c, float alpha) {
    c.a = alpha;
    return c;
  }

  public static string[] SeparateTags(string raw) {
    if(string.IsNullOrEmpty(raw)) {
      return new string[0];
    }

    string[] loadedTags = raw.Split(',');
    for(int i = 0; i < loadedTags.Length; i++) {
      loadedTags[i] = loadedTags[i].Trim();
    }
    return loadedTags;
  }

  public static int[] SeparateInts(string raw) {
    if(string.IsNullOrEmpty(raw)) {
      return new int[0];
    }

    string[] loadedTags = raw.Split(',');
    int[] ints = new int[loadedTags.Length];
    for(int i = 0; i < loadedTags.Length; i++) {
      ints[i] = int.Parse(loadedTags[i].Trim());
    }
    return ints;
  }

  public static string GetStringArgument(string clause) {
    int start = clause.IndexOf("(");
    int end = clause.IndexOf(")");

    if(start == -1 || end == -1) {
      return null;
    }

    string argumentName = clause.Substring(start + 1, (end - start - 1));
    Debug.Log("GET ARGUMENT: '" + argumentName + "'  ");
    return argumentName;
  }


  public static bool TagIsInArray(string tagToCheck, string[] tags) {
    foreach(string tag in tags) {
      if(tag == tagToCheck) {
        return true;
      }
    }
    return false;
  }

  public static int[] IntArray(string raw) {
    if(string.IsNullOrEmpty(raw)) {
      return new int[0];
    }

    string[] stringParts = raw.Split(',');
    int[] intArray = new int[stringParts.Length];
    for(int i = 0; i < stringParts.Length; i++) {
      stringParts[i] = stringParts[i].Trim();
      intArray[i] = int.Parse(stringParts[i]);
    }
    return intArray;
  }

  public static string GetTimeFormattedAsString(float timePassed) {
    string timeString = "";
    int timeInt = (int)timePassed;

    timeString = (timeInt / 60).ToString();

    timeString += ":";

    if(timeInt % 60 < 10) {
      timeString += "0" + (timeInt % 60);
    } else {
      timeString += (timeInt % 60).ToString();
    }

    return timeString;
  }

  public static Vector2 RandomPointInCircle(float radius) {
    Vector2 selectedPoint;
    do {
      selectedPoint = new Vector2(UnityEngine.Random.Range(-radius, radius), UnityEngine.Random.Range(-radius, radius));
    } while(selectedPoint.sqrMagnitude > radius * radius);
    return selectedPoint;
  }

  //public static Direction GetDirectionFromAngle(float angle) {
  //  angle += 22.5f;
  //  angle %= 360f;
  //  if(angle <= 45f) {
  //    return Direction.right;
  //  }
  //  if(angle <= 90f) {
  //    return Direction.upright;
  //  }
  //  if(angle <= 135f) {
  //    return Direction.up;
  //  }
  //  if(angle <= 180f) {
  //    return Direction.upleft;
  //  }
  //  if(angle <= 225f) {
  //    return Direction.left;
  //  }
  //  if(angle <= 270f) {
  //    return Direction.downleft;
  //  }
  //  if(angle <= 315f) {
  //    return Direction.down;
  //  }
  //  //if(angle <= 360f) {
  //  return Direction.downright;
  //  //}
  //}

  public int GetValueByString<T>(string valueName, Type t) {
    //T val;
    string[] names = t.GetEnumNames();
    //var valuea;
    foreach(string currentName in names) {
      if(currentName == valueName) {
        return 0;
      }
    }

    return -1;
  }


  public static void CopyFilesRecursively(string sourcePath, string targetPath) {
    //Now Create all of the directories
    foreach(string dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories)) {
      Directory.CreateDirectory(dirPath.Replace(sourcePath, targetPath));
    }

    //Copy all the files & Replaces any files with the same name
    foreach(string newPath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories)) {
      File.Copy(newPath, newPath.Replace(sourcePath, targetPath), true);
    }
  }

  public static Color NewColor(string color) {
    Color newCol;
    if(ColorUtility.TryParseHtmlString(color, out newCol)) {
      return newCol;
    }
    return new Color(0f, 0f, 0f);
  }


  public static float CalculateFogEffectByDist(float sqrDist) {
    float min = 15f;
    float max = 35f;
    float maxFogEffect = 0.93f;
    //float maxFogEffect = 1f;
    int colorCount = 8;
    float step = (max - min) / colorCount;

    if(sqrDist <= min * min) {
      return 0f;
    }

    if(sqrDist >= max * max) {
      return maxFogEffect;
    }

    for(int i = 0; i < colorCount; i++) {
      if(sqrDist <= (min + step * i) * (min + step * i)) {
        return Mathf.Lerp(0f, maxFogEffect, (float)i / colorCount);
      }
    }
    return maxFogEffect;
  }

  public static float FogByDy(float dy, float camSize) {
    float cameraHeight = camSize*2f;
    float min = -camSize;
    float value = (dy - min) / cameraHeight;
    return value;
  }

  public static float DifficultyMultiplierByPlayerCount(int playerCount) {
    switch(playerCount) {
      case 1:
        return 1f;
      case 2:
        return 1.65f;
      case 3:
        return 2.3f;
      case 4:
        return 2.9f;
    }
    return 1f;
  }
}

public static class MyExtensions {
  private static System.Random rng = new System.Random();

  public static void Shuffle<T>(this IList<T> list) {
    int n = list.Count;
    while(n > 1) {
      n--;
      int k = rng.Next(n + 1);
      T value = list[k];
      list[k] = list[n];
      list[n] = value;
    }
  }

  public static void SetAlpha(this Image img, float alpha) {
    Color c = img.color;
    c.a = alpha;
    img.color = c;
  }

  public static void SetAlpha(this SpriteRenderer renderer, float alpha) {
    Color c = renderer.color;
    c.a = alpha;
    renderer.color = c;
  }

  public static Vector2 Rotate(this Vector2 v, float degrees) {
    float radians = degrees * Mathf.Deg2Rad;
    float sin = Mathf.Sin(radians);
    float cos = Mathf.Cos(radians);

    float tx = v.x;
    float ty = v.y;

    return new Vector2(cos * tx - sin * ty, sin * tx + cos * ty);
  }

  public static Vector3 Rotate(this Vector3 v, float degrees) {
    float radians = degrees * Mathf.Deg2Rad;
    float sin = Mathf.Sin(radians);
    float cos = Mathf.Cos(radians);

    float tx = v.x;
    float ty = v.y;

    return new Vector3(cos * tx - sin * ty, sin * tx + cos * ty, 0f);
  }

  public static Vector2 ReflectFromNormal(this Vector2 v, Vector2 normal) {
    return v - 2 * Vector2.Dot(v, normal) * normal;
  }

  public static Toggle GetSelected(this ToggleGroup group) {
    foreach(Toggle t in group.ActiveToggles()) {
      if(t.isOn) {
        return t;
      }
    }
    return null;
  }

  public static int GetSelectedId(this ToggleGroup group) {
    int count = 0;
    foreach(Toggle t in group.ActiveToggles()) {
      if(t.isOn) {
        return count;
      }
      count++;
    }
    return -1;
  }

  public static void ClickedEffect(this Button button) {
    button.onClick?.Invoke();
  }


  // return the angle in a float format: from 0 to 360
  public static float GetAngleRotationZeroToThreeSixty(this Vector2 dist) {
    float angle = Vector2.Angle(dist, Vector2.right);
    if(dist.y < 0f) {
      return 360f - angle;
    }
    return angle;
  }

  public static Vector3 ToVector3(this Vector2 pos) {
    return new Vector3(pos.x, pos.y, 0f);
  }

  public static Vector3Int ToVector3Int(this Vector2Int value) {
    return new Vector3Int(value.x, value.y, 0);
  }

  public static string CleanObjectName(this string value) {
    int index = value.IndexOf("(");
    if(index < 0) {
      return value;
    }
    return value.Substring(0, index).Trim();
  }


  public static int CountNonNull(this GameObject[] array) {
    int count = 0;
    for(int i=0; i<array.Length; i++) {
      if(array[i] != null) count++;
    }
    return 1;
  }

  public static int CountNull(this GameObject[] array) {
    int count = 0;
    for(int i = 0; i < array.Length; i++) {
      if(array[i] == null) count++;
    }
    return 1;
  }

  public static float Abs(this float s) {
    return Mathf.Abs(s);
  }

  public static GameObject FindObject(this GameObject parent, string name) {
    Transform[] trs = parent.GetComponentsInChildren<Transform>(true);
    foreach(Transform t in trs) {
      if(t.name == name) {
        return t.gameObject;
      }
    }
    return null;
  }

  public static bool Contains(this List<Vector3> list, Vector3 pos) {
    foreach(Vector3 v in list) {
      if(Vector3.SqrMagnitude(v - pos) < 0.001f) {
        return true;
      }
    }
    return false;
  }

  public static bool Contains(this string[] array, string text) {
    foreach(string s in array) {
      if(s == text) {
        return true;
      }
    }
    return false;
  }

  public static int FindId(this Material[] array, Material mat) {
    for(int i = 0; i < array.Length; i++) {
      if(array[i] == mat) {
        return i;
      }
    }
    return -1;
  }

  public static int FindId(this Sprite[] array, Sprite sprite) {
    for(int i = 0; i < array.Length; i++) {
      if(array[i] == sprite) {
        return i;
      }
    }
    return -1;
  }


  public static void ClearChildren(this Transform content) {
    int childCount = content.childCount;

    for(int i = 0; i < childCount; i++) {
      GameObject.Destroy(content.GetChild(i).gameObject);
    }
  }

  public static string ToTitleCase(this string name) {
    return name[0].ToString().ToUpper() + name.ToString().Substring(1, name.Length-1).ToLower();
  }

  public static List<Vector2Int> RemoveList(this List<Vector2Int> baseList, List<Vector2Int> listToRemove) {
    foreach(Vector2Int pos in listToRemove) {
      baseList.Remove(pos);
    }
    return baseList;
  }

  public static string PrintableName(this Resolution res) {
    return res.width + "x" + res.height;
  }

  public static List<Vector2> ToListOfPositions(this List<Transform> transformsList) {
    List<Vector2> newList = new List<Vector2>();
    foreach(Transform pos in transformsList) {
      newList.Add(pos.position);
    }
    return newList;
  }

  public static bool Contains(this int[] array, int searchFor) {
    foreach(int i in array) {
      if(i == searchFor) {
        return true;
      }
    }
    return false;
  }
}

public static class TweenAnimations {
  public static void AnimTweenPopAppear(this Transform t) {
    t.gameObject.transform.localScale = Vector3.zero;
    t.transform.DOScale(1.1f, 0.5f).SetUpdate(true)
      .OnComplete(() => {
        t.transform.DOScale(1f, 0.2f).SetUpdate(true);
      });
  }

  public static void SubtlePulseAnimation(this Transform t) {
    DOTween.Kill(t.gameObject.transform);
    t.gameObject.transform.localScale = Vector3.one;
    t.transform.DOScale(1.03f, 0.12f).SetUpdate(true)
      .OnComplete(() => {
        t.transform.DOScale(1f, 0.12f).SetUpdate(true);
      });
  }

  public static void StrongPulseAnimation(this Transform t) {
    DOTween.Kill(t.gameObject.transform);
    t.gameObject.transform.localScale = Vector3.one;
    t.transform.DOScale(1.2f, 0.2f).SetUpdate(true)
      .OnComplete(() => {
        t.transform.DOScale(1f, 0.2f).SetUpdate(true);
      });
  }

  public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dict) {
    return dict.ToDictionary(pair => pair.Key, pair => pair.Value);
  }
  
}
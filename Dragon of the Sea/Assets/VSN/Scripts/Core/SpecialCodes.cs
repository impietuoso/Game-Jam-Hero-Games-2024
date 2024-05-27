using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpecialCodes {

  public static string InterpretStrings(string initialString) {
    string currentString = initialString;

    if(initialString == null) {
      return "";
    }

    if(!initialString.Contains("\\") && !initialString.Contains("<")) {
      return initialString;
    }

    do {
      initialString = currentString;

      currentString = InterpretVariableValue(currentString);

      //currentString = currentString.Replace("<competition >", LolLocalizationController.instance.GetTranslation("relation/competition"));
      //currentString = currentString.Replace("<predation >", LolLocalizationController.instance.GetTranslation("relation/predation"));
      //currentString = currentString.Replace("<prey >", LolLocalizationController.instance.GetTranslation("relation/prey"));
      //currentString = currentString.Replace("<parasitism >", LolLocalizationController.instance.GetTranslation("relation/parasitism"));
      //currentString = currentString.Replace("<mutualism >", LolLocalizationController.instance.GetTranslation("relation/mutualism"));
      //currentString = currentString.Replace("<commensalism >", LolLocalizationController.instance.GetTranslation("relation/commensalism"));
      //currentString = currentString.Replace("<end >", LolLocalizationController.instance.GetTranslation("relation/end_format"));
      currentString = currentString.Replace("\\n", "\n");
      currentString = currentString.Replace("\\q", "\"");
    } while(currentString != initialString);

    return currentString;
  }

  public static string InterpretVariableValue(string initial) {
    int start = initial.IndexOf("\\vsn(");
    if(start == -1) {
      return initial;
    }

    int end = initial.Substring(start, initial.Length-start).IndexOf(")") + start;

    Debug.LogWarning("Initial string: " + initial);

    if(end == -1) {
      return initial;
    }

    Debug.LogWarning("Start pos is: " + start + ", end pos is: " + end);

    string varName = initial.Substring(start + 5, (end - start - 5));
    string varString = GetPrintableVariableValue(varName);

    //Debug.LogWarning("VAR NAME IS: " + varName +", its value is " + varString);

    string final = initial.Substring(0, start);
    final += varString + initial.Substring(end + 1, initial.Length - end - 1);

    //Debug.LogWarning("VARIABLE INTERPRETATION:\nFrom: "+initial+"\nTo: "+final);

    return final;
  }


  static string GetPrintableVariableValue(string varName) {
    int intValue = VsnSaveSystem.GetIntVariable(varName);
    string stringValue = VsnSaveSystem.GetStringVariable(varName);

    if(stringValue != "") {
      return stringValue;
    } else {
      return intValue.ToString();
    }
  }


  public static float InterpretFloat(string keycode) {
    if(!keycode.Contains("#")) {
      return 0f;
    }
    return InterpretSpecialNumber(keycode);
  }

  static float InterpretSpecialNumber(string keycode) {
    if(keycode.Contains("#is_save_slot_used[") && keycode.Contains("]")) {
      return IsSaveSlotUsed(keycode);
    }

    switch(keycode) {
      case "#random100":
        return Random.Range(0, 100);
      //case "#shippedCouples":
      //  if(MatchSegmentController.instance == null) {
      //    return 0;
      //  }
      //  return MatchSegmentController.instance.currentCouples.Count;
      default:
        return 0f;
    }
  }

  static int IsSaveSlotUsed(string keycode) {
    string[] divisors = { "#is_save_slot_used[", "]" };
    string[] parts = keycode.Split(divisors, System.StringSplitOptions.RemoveEmptyEntries);
    int slotId = -1;

    if(parts.Length < 1) {
      return 0;
    }
    int.TryParse(parts[0], out slotId);

    if(slotId == -1) {
      return 0;
    }
    return VsnSaveSystem.IsSaveSlotBusy(slotId) ? 1 : 0;
  }

}

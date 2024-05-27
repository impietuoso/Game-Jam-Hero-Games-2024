using System;
using UnityEngine;

public class VsnString : VsnArgument{

  protected string stringValue;

  public VsnString(string text){
		stringValue = text;
  }

  public string RawString() {
    return stringValue;
  }

  public override string GetStringValue(){
    return SpecialCodes.InterpretStrings(LocalizedString(stringValue));
  }

  public static string LocalizedString(string stringValue) {
    string translated;
    translated = stringValue;

    if(translated != null) {
      return translated;
    } else {
      return stringValue;
    }
  }

  public override VsnArgType GetVsnValueType() {
    return VsnArgType.stringArg;
  }
}


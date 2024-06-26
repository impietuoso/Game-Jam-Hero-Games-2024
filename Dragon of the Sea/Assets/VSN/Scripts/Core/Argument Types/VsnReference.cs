﻿using System;

public class VsnReference : VsnArgument{

  protected string variableReferenceValue;

	public VsnReference(string variableName){
		variableReferenceValue = variableName;
	}

  public override float GetNumberValue(){
    if(variableReferenceValue[0] == '#'){
      return SpecialCodes.InterpretFloat(variableReferenceValue);
    }
    return VsnSaveSystem.GetFloatVariable(variableReferenceValue);
  }

  public override string GetStringValue(){
    return SpecialCodes.InterpretStrings(VsnSaveSystem.GetStringVariable(variableReferenceValue));
  }

  public override string GetReference(){
    return variableReferenceValue;
  }

  public override bool GetBooleanValue() {
    return VsnSaveSystem.GetBoolVariable(variableReferenceValue);
  }

  public override VsnArgType GetVsnValueType() {
    return VsnSaveSystem.GetVsnVariableType(variableReferenceValue);
  }
}

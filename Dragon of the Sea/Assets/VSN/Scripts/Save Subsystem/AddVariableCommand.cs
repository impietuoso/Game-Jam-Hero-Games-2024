using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command {

  [CommandAttribute(CommandString = "add_var")]
  public class AddVariableCommand : VsnCommand {

    public override void Execute() {
      if(args[0].GetVsnValueType() == VsnArgType.stringArg || args[1].GetVsnValueType() == VsnArgType.stringArg) {
        ConcatenateString();
      } else {
        AddFloat();
      }
    }

    public void AddFloat() {
      VsnArgument variableName = args[0];
      VsnArgument valueToAdd = args[1];

      float oldValue = VsnSaveSystem.GetFloatVariable(variableName.GetReference());
      float newValue = oldValue + valueToAdd.GetNumberValue();

      Debug.LogError("AddFloat: " + variableName.GetReference() + ", old: "+ oldValue+", plus " + newValue);

      VsnSaveSystem.SetVariable(variableName.GetReference(), newValue);
    }

    public void ConcatenateString() {
      VsnArgument variableName = args[0];
      VsnArgument valueToConcat = args[1];

      string oldValue = VsnSaveSystem.GetStringVariable(variableName.GetReference());
      string newValue = oldValue + valueToConcat.GetStringValue();

      Debug.LogError("ConcatenateString: " + variableName.GetReference() + ", value: " + valueToConcat.GetStringValue());

      VsnSaveSystem.SetVariable(variableName.GetReference(), newValue);
    }


    public override void AddSupportedSignatures() {
      signatures.Add(new VsnArgType[] {
        VsnArgType.referenceArg,
        VsnArgType.numberArg
      });
      signatures.Add(new VsnArgType[] {
        VsnArgType.referenceArg,
        VsnArgType.stringArg
      });
    }
  }
}
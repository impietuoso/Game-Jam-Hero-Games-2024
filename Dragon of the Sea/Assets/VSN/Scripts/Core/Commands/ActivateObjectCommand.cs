using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command {

  [CommandAttribute(CommandString = "activate_object")]
  public class ActivateObjectCommand : VsnCommand {

    public override void Execute() {
      string name = args[0].GetStringValue();
      bool setValue = true;
      //GameObject targetObject = BoardController.instance.gameObject.FindObject(name);

      if(args.Length > 1) {
        setValue = args[1].GetBooleanValue();
      }
      Debug.LogWarning("Selecting object: "+name);
    }


    public override void AddSupportedSignatures() {
      signatures.Add(new VsnArgType[] {
        VsnArgType.stringArg,
        VsnArgType.booleanArg
      });
      signatures.Add(new VsnArgType[] {
        VsnArgType.stringArg
      });
    }
  }
}
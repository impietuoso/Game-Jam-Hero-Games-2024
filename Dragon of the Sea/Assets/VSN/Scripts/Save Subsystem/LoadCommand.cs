using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command {

  [CommandAttribute(CommandString = "load")]
  public class LoadCommand : VsnCommand {

    public override void Execute() {
      int saveSlot = 0;

      if(args.Length > 0) {
        saveSlot = (int)args[0].GetNumberValue();
      }

      // TODO
      //GlobalData.instance.saveToLoad = saveSlot;
      VsnSaveSystem.Load(saveSlot);
    }


    public override void AddSupportedSignatures() {
      signatures.Add(new VsnArgType[0]);

      signatures.Add(new VsnArgType[] {
        VsnArgType.numberArg
      });
    }
  }
}
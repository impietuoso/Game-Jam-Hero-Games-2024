using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command {

  [CommandAttribute(CommandString = "reset_save")]
  public class ResetSaveCommand : VsnCommand {

    public override void Execute() {
      // TODO
      //GlobalData.instance.saveToLoad = saveSlot;
      VsnSaveSystem.CleanAllData();
    }


    public override void AddSupportedSignatures() {
      signatures.Add(new VsnArgType[0]);
    }
  }
}
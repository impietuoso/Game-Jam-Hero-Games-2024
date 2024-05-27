using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command {

  [CommandAttribute(CommandString = "script_exists")]
  public class ScriptExistsCommand : VsnCommand {

    public override void Execute() {
      StaticExecute(args[0].GetStringValue());
    }

    public static void StaticExecute(string scriptName) {
      TextAsset vsnScript = Resources.Load<TextAsset>("VSN Scripts/" + scriptName);
      if(vsnScript != null) {
        VsnSaveSystem.SetVariable("script_exists_check", true);
      } else {
        VsnSaveSystem.SetVariable("script_exists_check", false);
      }
    }

    public override void AddSupportedSignatures() {
      signatures.Add(new VsnArgType[] {
        VsnArgType.stringArg
      });
    }
  }
}
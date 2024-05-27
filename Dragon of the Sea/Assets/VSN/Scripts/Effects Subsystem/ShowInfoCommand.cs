using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command {

  [CommandAttribute(CommandString = "show_info")]
  public class ShowInfoCommand : VsnCommand {

    public override void Execute() {
      string infoText = args[0].GetStringValue();
      float infoTime = 0f;

      if(args.Length >= 2) {
        infoTime = args[1].GetNumberValue();
      }

      if(args.Length == 1) {
        VsnUIManager.instance.ShowInfo(infoText);
      } else if(args.Length == 2) {
        VsnUIManager.instance.ShowInfo(infoText, infoTime, false);
      } else {
        bool shouldWait = args[2].GetBooleanValue();
        VsnUIManager.instance.ShowInfo(infoText, infoTime, shouldWait);
      }
    }


    public override void AddSupportedSignatures() {
      signatures.Add(new VsnArgType[] {
        VsnArgType.stringArg
      });

      signatures.Add(new VsnArgType[] {
        VsnArgType.stringArg,
        VsnArgType.numberArg
      });

      signatures.Add(new VsnArgType[] {
        VsnArgType.stringArg,
        VsnArgType.numberArg,
        VsnArgType.booleanArg
      });
    }
  }
}
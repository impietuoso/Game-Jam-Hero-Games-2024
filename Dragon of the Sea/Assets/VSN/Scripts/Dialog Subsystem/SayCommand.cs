﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command {

    [CommandAttribute(CommandString = "say")]
    public class SayCommand : VsnCommand {

        public override void Execute() {
            StaticExecute(args);
        }

        public static void StaticExecute(VsnArgument[] args) {
            VsnUIManager.instance.ShowDialogPanel(true);
            VsnController.instance.state = ExecutionState.WAITINGTOUCH;

            string sayContent = args[0].GetStringValue();

            if (args.Length >= 2) {
                VsnUIManager.instance.SetTextTitle(args[0].GetStringValue());
                sayContent = args[1].GetStringValue();
            } else {
            }
            VsnUIManager.instance.ShowText(sayContent);
        }



        public override void AddSupportedSignatures() {
            signatures.Add(new VsnArgType[] {
        VsnArgType.stringArg
      });

            signatures.Add(new VsnArgType[] {
        VsnArgType.stringArg,
        VsnArgType.stringArg
      });
        }
    }
}
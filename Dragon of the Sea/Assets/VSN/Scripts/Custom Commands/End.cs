using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command {

    [CommandAttribute(CommandString = "End")]
    public class End : VsnCommand {

        public override void Execute() {
            Player.instance.FinishInitialCutscene();
        }

        public override void AddSupportedSignatures() {
            signatures.Add(new VsnArgType[] {
        VsnArgType.stringArg
      });
        }
    }
}
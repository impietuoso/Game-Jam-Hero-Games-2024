using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command {
    [CommandAttribute(CommandString = "Begin")]
    public class Begin : VsnCommand {

        public override void Execute() {
            Player.instance.BeginInitialCutscene();
        }

        public override void AddSupportedSignatures() {
            signatures.Add(new VsnArgType[] {
        VsnArgType.stringArg
      });
        }
    }
}
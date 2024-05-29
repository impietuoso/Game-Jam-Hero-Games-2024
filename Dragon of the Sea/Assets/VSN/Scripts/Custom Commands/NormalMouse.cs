using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command {
    [CommandAttribute(CommandString = "Mouse")]
    public class NormalMouse : VsnCommand {

        public override void Execute() {
            ChangeMouseCursor.instance.SetNormalMouse();
        }

        public override void AddSupportedSignatures() {
            signatures.Add(new VsnArgType[] {
        VsnArgType.stringArg
      });
        }
    }
}
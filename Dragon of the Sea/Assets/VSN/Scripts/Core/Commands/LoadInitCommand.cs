using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Command {

  [CommandAttribute(CommandString = "load_init")]
  public class LoadInitCommand : VsnCommand {

    public override void Execute() {
      string scriptFileName = SceneManager.GetActiveScene().name + " init";
      TextAsset textContent = Resources.Load<TextAsset>("VSN Scripts/" + scriptFileName);
      if(textContent != null) {
        VsnController.instance.StartVSN(scriptFileName);
      } else {
        Debug.LogWarning("No script found to load init: " + scriptFileName);
      }
    }

    public override void AddSupportedSignatures() {
      signatures.Add(new VsnArgType[0]);
    }
  }
}

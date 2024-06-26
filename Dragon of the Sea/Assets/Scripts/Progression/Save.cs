using UnityEngine;

[CreateAssetMenu(menuName ="Save")]
public class Save : ScriptableObject {
    public int rescuedVillagers;
    public int maxRescuredVillagers;
    public bool rescueComplete;
    [TextArea(5,10)]
    public string objective;

    public void Rescue() {
        if (rescueComplete) return;
        rescuedVillagers++;
        if(rescuedVillagers == maxRescuredVillagers) rescueComplete = true;
    }

    public void Reset() {
        rescuedVillagers = 0;
        rescueComplete = false;
    }
}
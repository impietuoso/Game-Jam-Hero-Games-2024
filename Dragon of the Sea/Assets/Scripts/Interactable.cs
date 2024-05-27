using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour {
    public UnityEvent unityEvent = new UnityEvent();
    public virtual void Interact() {
        unityEvent.Invoke();
    }
}
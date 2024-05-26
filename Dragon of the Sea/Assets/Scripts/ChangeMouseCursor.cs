using UnityEngine;

public class ChangeMouseCursor : MonoBehaviour {
    public static ChangeMouseCursor instance;
    public Texture2D mormalMouse;
    public Texture2D meleeMouse;
    public Texture2D rangedMouse;
    private CursorMode cursorMode = CursorMode.Auto;
    private Vector2 hotSpot = Vector2.zero;

    private void Awake() { instance = this; }
    public void SetNormalMouse() { Cursor.SetCursor(mormalMouse, hotSpot, cursorMode); }
    public void SetMeleeMouse() { Cursor.SetCursor(meleeMouse, hotSpot, cursorMode); }
    public void SetRangedMouse() { Cursor.SetCursor(rangedMouse, hotSpot, cursorMode); }
}
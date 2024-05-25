using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIFollow : MonoBehaviour
{
    public GameObject target;
    public Canvas canvas;

    private void LateUpdate() {
        var rt = GetComponent<RectTransform>();
        RectTransform parent = (RectTransform)rt.parent;
        var vp = Camera.main.WorldToViewportPoint(target.transform.position);
        var sp = canvas.worldCamera.ViewportToScreenPoint(vp);
        Vector3 worldPoint;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(parent, sp, canvas.worldCamera, out worldPoint);
        rt.position = worldPoint;
    }
}

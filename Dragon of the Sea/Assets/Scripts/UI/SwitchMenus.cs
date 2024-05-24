using UnityEngine;

// SCRIPT USADO EXCLUSIVAMENTE PARA BOTOES QUE ATIVAM A TROCA DE MENUS (CANVAS)
public class SwitchMenus : MonoBehaviour
{
    [SerializeField] private GameObject actualMenu;
    [SerializeField] private GameObject nextMenu;

    public void SwitchMenu()
    {
        actualMenu.GetComponent<CanvasGroup>().blocksRaycasts = false;
        actualMenu.GetComponent<CanvasGroup>().alpha = 0;
        nextMenu.GetComponent<CanvasGroup>().blocksRaycasts = true;
        nextMenu.GetComponent<CanvasGroup>().alpha = 1;
    }
}

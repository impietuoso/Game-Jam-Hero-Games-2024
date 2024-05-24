using UnityEngine;
using UnityEngine.EventSystems;

// SCRIPT USADO EXCLUSIVAMENTE PARA CANVAS DETECTAREM QUAL BOTAO DEVE SER O PRIMEIRO A SER SELECIONADO
public class GetFirstSelectedElement : MonoBehaviour
{
    [SerializeField] private GameObject firstSelectedElement;

    private void OnEnable()
    {
        EventSystem.current.SetSelectedGameObject(firstSelectedElement);
    }
}

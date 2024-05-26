using UnityEngine;

public class TextureOffsetController : MonoBehaviour
{
    public float scrollSpeedX = 0.1f;
    public float scrollSpeedY = 0.1f;

    private Renderer rend;
    private Vector2 savedOffset;

    void Start()
    {
        // Obtem o componente Renderer do GameObject
        rend = GetComponent<Renderer>();

        // Salva o offset original da textura
        savedOffset = rend.material.GetTextureOffset("_MainTex");
    }

    void Update()
    {
        // Calcula o novo offset baseado no tempo e na velocidade de scroll
        float newOffsetX = Mathf.Repeat(Time.time * scrollSpeedX, 1);
        float newOffsetY = Mathf.Repeat(Time.time * scrollSpeedY, 1);
        Vector2 newOffset = new Vector2(newOffsetX, newOffsetY);

        // Aplica o novo offset ao material
        rend.material.SetTextureOffset("_MainTex", newOffset);
    }

    void OnDisable()
    {
        // Restaura o offset original da textura quando o script é desativado
        rend.material.SetTextureOffset("_MainTex", savedOffset);
    }
}

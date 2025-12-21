using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class FitBackgroundToCamera : MonoBehaviour
{
    public Camera cam;
    public float extraMargin = 1.3f; // daj większy zapas, np. 1.3–1.6

    private SpriteRenderer sr;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        if (cam == null) cam = Camera.main;
    }

    void OnPreCull()
    {
        if (cam == null || sr.sprite == null) return;

        // rozmiar widoku kamery w jednostkach świata
        float worldH = cam.orthographicSize * 2f;
        float worldW = worldH * cam.aspect;

        // rozmiar sprite'a BEZ skali (stały)
        Vector2 spriteSize = sr.sprite.bounds.size;

        // ile razy trzeba powiększyć sprite, żeby zakrył ekran
        float scaleX = (worldW / spriteSize.x) * extraMargin;
        float scaleY = (worldH / spriteSize.y) * extraMargin;

        float scale = Mathf.Max(scaleX, scaleY);

        // tło jest dzieckiem kamery → skalujemy lokalnie
        transform.localScale = new Vector3(scale, scale, 1f);
    }
}


using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class BackgroundFollowAndFit : MonoBehaviour
{
    public Transform cam;               // kamera
    public bool followX = true;
    public bool followY = true;

    public float extraCover = 1.1f;     // zapas, ¿eby nie by³o przerw

    private Camera cameraRef;
    private SpriteRenderer sr;

    void Start()
    {
        if (cam == null) cam = Camera.main.transform;
        cameraRef = Camera.main;
        sr = GetComponent<SpriteRenderer>();

        FitToCamera();
        FollowCamera();
    }

    void LateUpdate()
    {
        FitToCamera();
        FollowCamera();
    }

    void FollowCamera()
    {
        Vector3 pos = transform.position;
        if (followX) pos.x = cam.position.x;
        if (followY) pos.y = cam.position.y;
        transform.position = pos;
    }

    void FitToCamera()
    {
        if (cameraRef == null || sr.sprite == null) return;

        float camWorldHeight = cameraRef.orthographicSize * 2f;
        float camWorldWidth = camWorldHeight * cameraRef.aspect;

        // rozmiar sprite’a w jednostkach œwiata (bez skali)
        float spriteW = sr.sprite.bounds.size.x;
        float spriteH = sr.sprite.bounds.size.y;

        float needW = camWorldWidth * extraCover;
        float needH = camWorldHeight * extraCover;

        // zachowaj proporcje (uniform)
        float scale = Mathf.Max(needW / spriteW, needH / spriteH);

        transform.localScale = new Vector3(scale, scale, 1f);
    }
}




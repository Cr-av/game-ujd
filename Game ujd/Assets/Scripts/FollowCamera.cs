using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    [Header("Cel kamery")]
    public Transform target;              // obiekt, za którym kamera ma pod¹¿aæ
    public Rigidbody2D targetRb;           // Rigidbody pojazdu, ¿eby znaæ prêdkoœæ

    [Header("Parametry ruchu kamery")]
    public float smoothSpeed = 0.125f;     // p³ynnoœæ ruchu kamery
    public Vector3 offset = new Vector3(0f, 2f, -10f); // przesuniêcie wzglêdem celu

    [Header("Zoom")]
    public float minZoom = 5f;             // minimalne przybli¿enie
    public float maxZoom = 8f;             // maksymalne oddalenie
    public float zoomSpeed = 2f;           // szybkoœæ zmiany zoomu

    private Camera cam;

    void Start()
    {
        cam = Camera.main;
    }

    void LateUpdate()
    {
        if (target == null || targetRb == null) return;

        // Pozycja kamery (p³ynne pod¹¿anie)
        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;

        // Dynamiczny zoom zale¿ny od prêdkoœci pojazdu
        float speed = targetRb.linearVelocity.magnitude;
        float targetZoom = Mathf.Lerp(minZoom, maxZoom, speed / 10f); // im szybciej, tym wiêkszy zoom
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetZoom, Time.deltaTime * zoomSpeed);
    }
}



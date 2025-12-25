using UnityEngine;
using UnityEngine.U2D;

[ExecuteAlways]
public class RoadGenerator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SpriteShapeController spriteShape;

    [Header("Road length / spacing")]
    [Min(2)] public int pointsCount = 60;
    [Min(0.01f)] public float stepX = 1f;              // odleg³oœæ w osi X miêdzy punktami
    [Min(0.01f)] public float minPointDistance = 0.2f; // zabezpieczenie przed zbyt bliskimi punktami

    [Header("Road shape")]
    public float amplitude = 2f;       // wysokoœæ falowania drogi
    public float noiseScale = 0.08f;   // "rozci¹gniêcie" Perlin noise
    public float baseY = 0f;           // bazowa wysokoœæ drogi
    public float bottomY = -10f;       // gdzie domykamy kszta³t (wype³nienie w dó³)

    [Header("Tangents (smoothness)")]
    [Range(0f, 3f)] public float tangentSize = 1.2f;

    [Header("Regeneration")]
    public int seed = 12345;
    public bool regenerateInEditMode = true;

    private void OnValidate()
    {
        if (!regenerateInEditMode) return;
        if (!spriteShape) spriteShape = GetComponent<SpriteShapeController>();
        if (spriteShape) Generate();
    }

    [ContextMenu("Generate Road")]
    public void Generate()
    {
        if (!spriteShape) return;

        var spline = spriteShape.spline;
        spline.Clear();

        // Seed -> stabilne generowanie (ta sama droga dla tego samego seed)
        Random.InitState(seed);
        float offset = Random.Range(-10000f, 10000f);

        Vector3 last = Vector3.positiveInfinity;

        // Górna krawêdŸ drogi
        int added = 0;
        for (int i = 0; i < pointsCount; i++)
        {
            float x = i * stepX;
            float n = Mathf.PerlinNoise((x + offset) * noiseScale, 0f);
            float y = baseY + (n - 0.5f) * 2f * amplitude;

            Vector3 p = new Vector3(x, y, 0f);

            // zabezpieczenie przed punktami zbyt blisko
            if (added > 0 && Vector3.Distance(last, p) < minPointDistance)
            {
                p.x = last.x + minPointDistance; // wymuœ minimalny odstêp w X
            }

            spline.InsertPointAt(added, p);

            // Tangenty dla p³ynnej drogi (nie dla pierwszego i ostatniego)
            spline.SetTangentMode(added, ShapeTangentMode.Continuous);
            spline.SetLeftTangent(added, Vector3.left * tangentSize);
            spline.SetRightTangent(added, Vector3.right * tangentSize);

            last = p;
            added++;
        }

        // Domkniêcie kszta³tu (¿eby by³ fill)
        // punkt pod koñcem
        spline.InsertPointAt(added, new Vector3(last.x, bottomY, 0f));
        spline.SetTangentMode(added, ShapeTangentMode.Linear);
        added++;

        // punkt pod pocz¹tkiem
        spline.InsertPointAt(added, new Vector3(0f, bottomY, 0f));
        spline.SetTangentMode(added, ShapeTangentMode.Linear);

        spriteShape.RefreshSpriteShape();
    }
}


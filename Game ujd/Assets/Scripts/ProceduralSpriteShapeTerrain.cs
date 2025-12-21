using UnityEngine;
using UnityEngine.U2D;

[RequireComponent(typeof(SpriteShapeController))]
public class ProceduralSpriteShapeTerrain : MonoBehaviour
{
    [Header("Size")]
    [Min(2)] public int points = 80;          // ile punktów w poziomie
    public float segmentWidth = 1.0f;         // odleg³oœæ miêdzy punktami (X)
    public float groundY = 0.0f;              // bazowa wysokoœæ terenu
    public float height = 6.0f;               // amplituda (jak du¿e górki)

    [Header("Noise")]
    public float noiseScale = 0.15f;          // skala szumu (mniejsze = d³u¿sze fale)
    public float noiseOffset = 0f;            // przesuniêcie (mo¿esz animowaæ)

    [Header("Fill Closing")]
    public float bottomY = -20f;              // jak nisko zamkn¹æ kszta³t do do³u (fill)

    [Header("Auto Generate")]
    public bool generateOnStart = true;
    public bool regenerateWithR = true;

    [Header("Collider")]
    public bool updateEdgeCollider = true;

    private SpriteShapeController _ssc;
    private EdgeCollider2D _edge;

    private void Awake()
    {
        _ssc = GetComponent<SpriteShapeController>();
        _edge = GetComponent<EdgeCollider2D>(); // opcjonalnie, jeœli masz na tym samym obiekcie
    }

    private void Start()
    {
        if (generateOnStart)
            Generate();
    }

    private void Update()
    {
        if (regenerateWithR && Input.GetKeyDown(KeyCode.R))
            Generate();
    }

    [ContextMenu("Generate Terrain")]
    public void Generate()
    {
        var spline = _ssc.spline;

        // Czyœcimy splajn
        while (spline.GetPointCount() > 0)
            spline.RemovePointAt(0);

        // Górna krawêdŸ terenu (widoczna, po niej jedzie auto)
        for (int i = 0; i < points; i++)
        {
            float x = i * segmentWidth;
            float n = Mathf.PerlinNoise((x + noiseOffset) * noiseScale, 0f);
            float y = groundY + (n - 0.5f) * 2f * height;

            int idx = spline.GetPointCount();
            spline.InsertPointAt(idx, new Vector3(x, y, 0));

            // Tangenty na auto (³adniejsze zakrêty)
            spline.SetTangentMode(idx, ShapeTangentMode.Continuous);

            // Ustaw d³ugoœæ tangentów (mo¿esz zmieniaæ dla “g³adkoœci”)
            spline.SetLeftTangent(idx, new Vector3(-segmentWidth * 0.5f, 0, 0));
            spline.SetRightTangent(idx, new Vector3(segmentWidth * 0.5f, 0, 0));
        }

        // Zamkniêcie kszta³tu do do³u, ¿eby Fill dzia³a³
        // Punkt na dole po prawej
        int rightBottom = spline.GetPointCount();
        spline.InsertPointAt(rightBottom, new Vector3((points - 1) * segmentWidth, bottomY, 0));
        spline.SetTangentMode(rightBottom, ShapeTangentMode.Linear);

        // Punkt na dole po lewej
        int leftBottom = spline.GetPointCount();
        spline.InsertPointAt(leftBottom, new Vector3(0, bottomY, 0));
        spline.SetTangentMode(leftBottom, ShapeTangentMode.Linear);

        // Zamknij pêtlê
        spline.isOpenEnded = false;

        // Odœwie¿ geometriê
        _ssc.RefreshSpriteShape();

        // Odœwie¿ collider (jeœli u¿ywasz EdgeCollider2D)
        if (updateEdgeCollider && _edge != null)
        {
            // EdgeCollider powinien braæ tylko górn¹ krawêdŸ,
            // wiêc ustawiamy mu punkty 0..points-1
            Vector2[] colPoints = new Vector2[points];
            for (int i = 0; i < points; i++)
            {
                Vector3 p = spline.GetPosition(i);
                colPoints[i] = new Vector2(p.x, p.y);
            }
            _edge.points = colPoints;
        }
    }
}


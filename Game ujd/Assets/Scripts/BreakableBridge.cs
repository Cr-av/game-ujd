using System.Collections.Generic;
using UnityEngine;

public class BreakableBridge : MonoBehaviour
{
    [Header("Elementy mostu")]
    public Rigidbody2D[] bridgePieces;

    [Header("Ustawienia")]
    public float gravityScale = 1.5f;

    [Tooltip("Jak blisko auto musi być elementu, żeby go złamać (w jednostkach Unity).")]
    public float breakDistance = 1.2f;

    [Tooltip("Opcjonalne małe opóźnienie łamania danego elementu (0 = natychmiast).")]
    public float perPieceDelay = 0.03f;

    private Transform vehicle;
    private bool active = false;

    // żeby nie łamać tego samego elementu 100x
    private readonly HashSet<Rigidbody2D> broken = new HashSet<Rigidbody2D>();
    private readonly Dictionary<Rigidbody2D, float> scheduledAt = new Dictionary<Rigidbody2D, float>();

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (active) return;

        if (other.transform.root.CompareTag("Player"))
        {
            vehicle = other.transform.root;
            active = true;
        }
    }

    private void Update()
    {
        if (!active || vehicle == null) return;

        // znajdź najbliższy NIEZŁAMANY element
        Rigidbody2D nearest = null;
        float minDist = float.MaxValue;

        foreach (var rb in bridgePieces)
        {
            if (rb == null) continue;
            if (broken.Contains(rb)) continue;

            float dist = Vector2.Distance(vehicle.position, rb.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = rb;
            }
        }

        // jeśli auto jest wystarczająco blisko – złam element
        if (nearest != null && minDist <= breakDistance)
        {
            if (perPieceDelay <= 0f)
            {
                BreakNow(nearest);
            }
            else
            {
                // zaplanuj złamanie za chwilę (żeby był fajny feeling)
                if (!scheduledAt.ContainsKey(nearest))
                    scheduledAt[nearest] = Time.time + perPieceDelay;
            }
        }

        // obsłuż zaplanowane złamania
        if (scheduledAt.Count > 0)
        {
            // kopiujemy klucze, bo będziemy usuwać
            var keys = new List<Rigidbody2D>(scheduledAt.Keys);
            foreach (var rb in keys)
            {
                if (rb == null) { scheduledAt.Remove(rb); continue; }

                if (Time.time >= scheduledAt[rb])
                {
                    scheduledAt.Remove(rb);
                    if (!broken.Contains(rb))
                        BreakNow(rb);
                }
            }
        }

        // jeśli wszystkie złamane, usuń trigger
        if (broken.Count >= bridgePieces.Length)
        {
            Destroy(gameObject);
        }
    }

    private void BreakNow(Rigidbody2D rb)
    {
        broken.Add(rb);
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = gravityScale;
    }
}







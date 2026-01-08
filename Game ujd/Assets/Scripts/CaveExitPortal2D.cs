using System.Collections;
using UnityEngine;

public class CaveExitPortal2D : MonoBehaviour
{
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private Transform exitTarget;

    [Header("Stabilizacja po teleportacji")]
    [SerializeField] private bool resetVelocities = true;
    [SerializeField] private bool temporarilyDisableSimulation = true;
    [SerializeField] private float disableSeconds = 0.05f; // 0.02–0.08 zwykle ok

    private bool _busy;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_busy) return;
        if (exitTarget == null) return;

        // Szukamy RB pojazdu
        var rb = other.attachedRigidbody;
        if (rb == null) rb = other.GetComponentInParent<Rigidbody2D>();
        if (rb == null) return;

        // Tag sprawdzamy na obiekcie z RB (zwykle Vehicle/root)
        if (!rb.CompareTag(playerTag)) return;

        StartCoroutine(TeleportWholeVehicle(rb));
    }

    private IEnumerator TeleportWholeVehicle(Rigidbody2D anyRbInVehicle)
    {
        _busy = true;

        // Root = ca³y pojazd (Vehicle), razem z ko³ami
        Transform root = anyRbInVehicle.transform.root;

        // Zbierz wszystkie RB w pojeŸdzie (rodzic + ko³a)
        var rbs = root.GetComponentsInChildren<Rigidbody2D>(true);

        // (Opcjonalnie) wy³¹cz symulacjê na moment ¿eby jointy nie dosta³y “kopa”
        if (temporarilyDisableSimulation)
            foreach (var rb in rbs) rb.simulated = false;

        // Przenieœ CA£EGO root-a (nie tylko rb.position)
        Vector3 delta = exitTarget.position - root.position;
        root.position += delta;

        // Poczekaj do nastêpnego kroku fizyki
        yield return new WaitForFixedUpdate();

        if (resetVelocities)
        {
            foreach (var rb in rbs)
            {
                rb.linearVelocity = Vector2.zero;
                rb.angularVelocity = 0f;
            }
        }

        if (temporarilyDisableSimulation)
        {
            // chwila bufora, potem w³¹cz fizykê
            yield return new WaitForSeconds(disableSeconds);
            foreach (var rb in rbs) rb.simulated = true;
        }

        _busy = false;
    }
}



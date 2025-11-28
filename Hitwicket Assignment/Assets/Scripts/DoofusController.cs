using UnityEngine;

public class DoofusController : MonoBehaviour
{
    public float speed = 3f;
    public float rayDistance = 2f;
    public GameManager gm;
    public Pulpit currentPulpit;
    public float multiplier;

    Rigidbody rb;
    Vector3 input;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (gm == null) gm = FindObjectOfType<GameManager>();
    }

    void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        input = new Vector3(h, 0f, v).normalized;

        DetectPulpit();
    }

    void FixedUpdate()
    {
        if (rb == null) return;
        Vector3 vel = input * speed * multiplier;
        rb.linearVelocity = new Vector3(vel.x, rb.linearVelocity.y, vel.z);
    }

    public void SetCurrentPulpit(Pulpit p)
    {
        currentPulpit = p;
    }

    void DetectPulpit()
    {
        Ray ray = new Ray(transform.position, Vector3.down);

        if (Physics.Raycast(ray, out RaycastHit hit, rayDistance))
        {
            Pulpit p = hit.collider.GetComponent<Pulpit>();
            if (p != null && p != currentPulpit)
            {
                currentPulpit = p;
                if (gm != null) gm.OnPulpitStepped(p);
            }
        }
        else
        {
            currentPulpit = null;
            if (gm != null) gm.OnDoofusFell();
        }
    }
}

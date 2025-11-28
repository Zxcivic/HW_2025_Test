using UnityEngine;

public class DoofusController : MonoBehaviour
{
    public float speed = 3f;
    public float rayDistance = 2f;
    public GameManager gm;
    public Pulpit currentPulpit;
    public float multiplier = 1f;
    public float jumpForce = 7f;
    public float groundCheckDistance = 1.5f;

    Rigidbody rb;
    Vector3 input;
    bool isGrounded;

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

        UpdateGrounded();
        DetectPulpit();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isGrounded)
            {
                Jump();
            }
            else
            {
                // Uncomment for debugging:
                // Debug.Log("Tried to jump but not grounded");
            }
        }
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

    void UpdateGrounded()
    {
        Vector3 origin = transform.position + Vector3.up * 0.1f;
        isGrounded = Physics.Raycast(origin, Vector3.down, groundCheckDistance);
    }

    void DetectPulpit()
    {
        if (gm == null) return;

        Ray ray = new Ray(transform.position, Vector3.down);

        if (Physics.Raycast(ray, out RaycastHit hit, rayDistance))
        {
            Pulpit p = hit.collider.GetComponent<Pulpit>();
            if (p != null && p != currentPulpit)
            {
                currentPulpit = p;
                gm.OnPulpitStepped(p);
            }
        }
        else
        {
            currentPulpit = null;
            // IMPORTANT: no OnDoofusFell() here – falling is handled by GameManager via Y position
        }
    }

    void Jump()
    {
        Vector3 v = rb.linearVelocity;
        v.y = jumpForce;
        rb.linearVelocity = v;
    }
}

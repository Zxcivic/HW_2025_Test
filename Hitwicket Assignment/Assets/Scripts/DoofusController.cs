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

    [Header("Audio")]
    public AudioSource walkSource;      // looping walking sound
    public AudioSource sfxSource;       // jump / other SFX
    public AudioClip jumpClip;
    public float walkFadeSpeed = 5f;    // higher = faster fade

    Rigidbody rb;
    Vector3 input;
    bool isGrounded;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (gm == null) gm = FindObjectOfType<GameManager>();

        // Optional safety: try auto-assign if not set
        if (walkSource == null || sfxSource == null)
        {
            var sources = GetComponents<AudioSource>();
            if (sources.Length > 0 && walkSource == null) walkSource = sources[0];
            if (sources.Length > 1 && sfxSource == null) sfxSource = sources[1];
        }
    }

    void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        input = new Vector3(h, 0f, v).normalized;

        UpdateGrounded();
        DetectPulpit();
        HandleWalkAudio();

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump();
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
        }
    }

    void HandleWalkAudio()
    {
        if (walkSource == null) return;

        // Should we be playing walking sound?
        bool shouldWalk =
            isGrounded &&
            input.sqrMagnitude > 0.01f;   // has movement input

        float targetVol = shouldWalk ? 1f : 0f;
        walkSource.volume = Mathf.MoveTowards(
            walkSource.volume,
            targetVol,
            walkFadeSpeed * Time.deltaTime
        );

        if (shouldWalk)
        {
            if (!walkSource.isPlaying)
                walkSource.Play();
        }
        else
        {
            // fully faded out -> pause so it doesn't keep reading
            if (walkSource.volume <= 0.001f && walkSource.isPlaying)
                walkSource.Pause();
        }
    }

    void Jump()
    {
        Vector3 v = rb.linearVelocity;
        v.y = jumpForce;
        rb.linearVelocity = v;

        if (sfxSource != null && jumpClip != null)
        {
            sfxSource.PlayOneShot(jumpClip);
        }
    }
}

using UnityEngine;
using UnityEngine.InputSystem; // 👈 New Input System

public class TennisPlayer : MonoBehaviour
{
    public float moveSpeed = 5f;
    private float currentMoveSpeed;

    private Animator animator;
    private bool isHoldingSpace = false;
    private bool canMove = true;

    public GameObject hitboxPrefab;
    private GameObject currentHitbox;

    [Header("Target Indicator")]
    public GameObject target; // 👈 Drag your Target GameObject here
    public float targetSpinSpeed = 180f; // Degrees per second

    // New Input System Fields
    public PlayerInput playerInput; // 👈 Drag your PlayerInput here
    private InputAction moveAction;
    private InputAction strikeAction;

    private float moveInput;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        currentMoveSpeed = moveSpeed;

        moveAction = playerInput.actions["Move"];
        strikeAction = playerInput.actions["Strike"];
    }


    private void OnEnable()
    {
        strikeAction.started += OnStrikeStarted;
        strikeAction.canceled += OnStrikeCanceled;
    }

    private void OnDisable()
    {
        strikeAction.started -= OnStrikeStarted;
        strikeAction.canceled -= OnStrikeCanceled;
    }

    private void Update()
    {
        moveInput = moveAction.ReadValue<float>();
        transform.Translate(Vector2.right * moveInput * currentMoveSpeed * Time.deltaTime);

        // Clamp to screen bounds
        float clampedX = Mathf.Clamp(transform.position.x, -8f, 8f);
        transform.position = new Vector2(clampedX, transform.position.y);

        // Spin the target if it is active
        if (target != null && target.activeSelf)
        {
            target.transform.Rotate(Vector3.forward, targetSpinSpeed * Time.deltaTime);
        }
    }


    private void OnStrikeStarted(InputAction.CallbackContext context)
    {
        Debug.Log("Space Key Held - Ready to Strike");

        isHoldingSpace = true;
        animator.SetBool("isHoldingSpace", true);

        // 🐢 Slow down movement by half
        currentMoveSpeed = moveSpeed * 0.5f;

        if (target != null)
        {
            target.SetActive(true); // Show the target
        }
    }


    private void OnStrikeCanceled(InputAction.CallbackContext context)
    {
        Debug.Log("Space Key Released - Strike!");

        isHoldingSpace = false;
        animator.SetBool("isHoldingSpace", false);
        animator.SetTrigger("strikeTrigger");

        // 🚀 Restore full movement speed
        currentMoveSpeed = moveSpeed;

        if (target != null)
        {
            target.SetActive(false); // Hide the target
        }

        DetachBall();

        StartCoroutine(SpawnHitbox());
        StartCoroutine(ResetAfterStrike());
    }


    private void DetachBall()
    {
        Ball ball = FindObjectOfType<Ball>();
        if (ball != null && ball.attachedToPaddle)
        {
            ball.attachedToPaddle = false;
            ball.isLaunched = true;
        }
    }


    private System.Collections.IEnumerator SpawnHitbox()
    {
        yield return new WaitForSeconds(0.1f); // Adjust timing

        currentHitbox = Instantiate(hitboxPrefab, transform.position, Quaternion.identity, transform);

        yield return new WaitForSeconds(0.3f); // Active hit window

        Destroy(currentHitbox);
    }

    private System.Collections.IEnumerator ResetAfterStrike()
    {
        // Wait for Strike animation to finish (tune this timing!)
        yield return new WaitForSeconds(0.5f); // or whatever your Strike animation duration is
        canMove = true;
    }
}

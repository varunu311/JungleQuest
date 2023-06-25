using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public float jumpForce = 5f;
    public float moveSpeed = 5f;
    public float gravityScale = 2f;
    public LayerMask groundLayer;
    public float resetHeight = -10f; // Specify the height at which the player should be reset

    private bool isJumping = false;
    private bool isGrounded = false;
    private bool isFacingRight = true;
    private Rigidbody2D rb;
    private Animator animator;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = gravityScale;
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        // Jump input detection
        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            isJumping = true;
            animator.SetTrigger("Jump");
        }

        // Movement input detection
        float moveDirection = Input.GetAxis("Horizontal");
        if (moveDirection < 0 && isFacingRight)
        {
            // Turn left
            Flip();
        }
        else if (moveDirection > 0 && !isFacingRight)
        {
            // Turn right
            Flip();
        }

        // Apply movement velocity
        rb.velocity = new Vector2(moveDirection * moveSpeed, rb.velocity.y);

        // Trigger running animation
        animator.SetFloat("Speed", Mathf.Abs(moveDirection));

        // Check if player falls below the reset height
        if (transform.position.y < resetHeight)
        {
            ResetGame();
        }
    }

    private void FixedUpdate()
    {
        // Apply jump force if the player is jumping
        if (isJumping && isGrounded)
        {
            rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
            isJumping = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if player is grounded
        if (collision.contacts.Length > 0)
        {
            ContactPoint2D contact = collision.contacts[0];
            if (Vector2.Dot(contact.normal, Vector2.up) > 0.7)
            {
                isGrounded = true;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        // Update grounded status when leaving a collision
        isGrounded = false;
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        transform.Rotate(0f, 180f, 0f);
    }

    private void ResetGame()
    {
        // Reset the game by reloading the current scene
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movement : MonoBehaviour
{
    private Rigidbody2D rb;
    private bool isGrounded;
    public float jumpForce = 5.0f;
    public Transform groundCheck;  // To detect if the player is grounded
    public float checkRadius = 0.2f;
    public LayerMask groundLayer;  // Define what counts as "ground"
    public Animator myAnim;
    private Vector3 originalScale;
    void Start()
    {
        originalScale = transform.localScale;
        rb = GetComponent<Rigidbody2D>();
        myAnim = GetComponent<Animator>();
    }

    void Update()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);

        if (isGrounded && Input.GetButtonDown("Jump")) 
        {
            myAnim.Play("jump");
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
        if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.C)) {
        transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y / 2, transform.localScale.z);
        }

        if (Input.GetKeyUp(KeyCode.LeftControl) || Input.GetKeyUp(KeyCode.C)) {
            transform.localScale = originalScale;
        }

    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterController : MonoBehaviour
{
    public PlayerInputActions inputActions;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float horizontalSpeed;
    [SerializeField] private float jumpForce;

    private float moveDirection;

    private void Awake() 
    {
        inputActions = new PlayerInputActions();
        inputActions.Player.Move.performed += ctx => moveDirection = ctx.ReadValue<float>();
        inputActions.Player.Move.canceled += ctx => moveDirection = ctx.ReadValue<float>();
        inputActions.Player.Jump.performed += ctx => Jump();


    }

    private void FixedUpdate()
    {
        MoveHorizontal();
    }

    private void MoveHorizontal()
    {
        Vector2 move = new Vector2(moveDirection * horizontalSpeed * Time.fixedDeltaTime, rb.velocity.y);
        rb.velocity = move;
        //Flip();
    }

    private void Flip()
    {
        if (moveDirection == 0)
            return;

        Vector2 direction = new Vector2(transform.localScale.x * moveDirection, transform.localScale.y);
        transform.localScale = direction;
    }

    private void Jump()
    {
        if (Grounded())
        {
            Vector2 move = new Vector2(rb.velocity.x, jumpForce * Time.fixedDeltaTime);
            rb.velocity = move;
        }
    }

    private bool Grounded()
    {
        Debug.DrawRay(transform.position, Vector2.down, Color.red, 3f, false);
        if (Physics2D.Raycast(transform.position, Vector2.down, 2f, 1 << 8))
        {
            return true;
        }
        else 
        {
            return false;
        }
    }

    private void OnEnable() 
    {
        inputActions.Enable();
    }

    private void OnDisable() 
    {
        inputActions.Disable();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwoDBasicPlayerController: MonoBehaviour
{
    public float playerSpeed;
    public Animator animator;
    // public GameObject playerSprite;

    void FixedUpdate()
    {
        // Movement
        Vector3 movement = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 1.0f);
        transform.position = transform.position + (movement * playerSpeed * Time.deltaTime);

        // basic animation float sets. none of these are implemented as of import
        animator.SetFloat("Horizontal", movement.x);
        animator.SetFloat("Vertical", movement.y);
        animator.SetFloat("Speed", movement.sqrMagnitude);
    }
}

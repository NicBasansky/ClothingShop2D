using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Shop.Control
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] float speed = 2f;
        Rigidbody2D rb;
        Vector2 movement;
        Animator anim;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            anim = GetComponent<Animator>();
        }

        private void Update()
        {
            movement.x = Input.GetAxis("Horizontal");
            movement.y = Input.GetAxis("Vertical");

            UpdateAnimations();
        }

        private void FixedUpdate()
        {
            rb.MovePosition(rb.position + movement * speed * Time.fixedDeltaTime);
        }

        private void UpdateAnimations()
        {
            anim.SetFloat("Horizontal", movement.x);
            anim.SetFloat("Vertical", movement.y);
            anim.SetFloat("Speed", movement.magnitude);
        }

    }
}


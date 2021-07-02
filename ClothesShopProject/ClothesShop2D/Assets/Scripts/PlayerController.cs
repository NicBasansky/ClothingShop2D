using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Shop.Control
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] float speed = 2f;
        public bool shouldFreeze = false;
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
            if (!shouldFreeze)
            {
                movement.x = Input.GetAxis("Horizontal");
                movement.y = Input.GetAxis("Vertical");
            }
            else
            {
                movement = Vector2.zero;
            }

            UpdateAnimations();
        }

        private void FixedUpdate()
        {
            rb.MovePosition(rb.position + movement * speed * Time.fixedDeltaTime);
        }

        private void UpdateAnimations()
        {
            // when the vector is zero the animation doesn't receive an ambiguous direction
            // therefore the last idle animation used is played when stopped
            if (movement != Vector2.zero)
            {
                anim.SetFloat("Horizontal", movement.x);
                anim.SetFloat("Vertical", movement.y);
            }

            anim.SetFloat("Speed", movement.magnitude);
        }

    }
}


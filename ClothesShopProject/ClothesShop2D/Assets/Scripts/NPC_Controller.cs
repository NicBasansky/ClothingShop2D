using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shop.UI;
using System;

namespace Shop.Control
{
    public class NPC_Controller : MonoBehaviour
    {
        Animator animator;

        [SerializeField] PatrolPath patrolPath;
        [SerializeField] float walkingSpeed = 1f;
        [SerializeField] float stoppingDistance = 0.5f;
        [SerializeField] float waitDelay = 2f;

        int currentWaypointIndex = 0;
        float timeSinceArrivedAtPatrolPoint = Mathf.Infinity;
        bool shouldPatrol = true;
        Vector3 nextPosition;
        Rigidbody2D rb;
        DialogueManager dialogueManager;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            rb = GetComponent<Rigidbody2D>();
            dialogueManager = FindObjectOfType<DialogueManager>();
        }

        private void OnEnable() 
        {
            dialogueManager.onDialogueEnd += OnFinishedDialogue;
        }

        private void OnDisable()
        {
            dialogueManager.onDialogueEnd -= OnFinishedDialogue;
        }

        private void Update()
        {
            if (shouldPatrol)
            {
                PatrolBehaviour();

                timeSinceArrivedAtPatrolPoint += Time.deltaTime;
            }
           
        }

        private void PatrolBehaviour()
        {
            if (patrolPath != null)
            {
                if (AtWaypoint())
                {
                    CycleWaypoint();
                }
                nextPosition = GetCurrentWaypoint();

                if (timeSinceArrivedAtPatrolPoint >= waitDelay)
                {
                    MoveTo(nextPosition);
                    animator.SetFloat("Speed", 1); // TODO change to walking speed
                }
                else
                {
                    animator.SetFloat("Speed", 0);
                }
            }
        }

        private void MoveTo(Vector3 position)
        {
            Vector3 movement = (position - transform.position).normalized;
            transform.Translate(movement * walkingSpeed * Time.deltaTime);

            animator.SetFloat("Horizontal", movement.x);
            animator.SetFloat("Vertical", movement.y);
        }

        private Vector3 GetCurrentWaypoint()
        {
            return patrolPath.GetWaypoint(currentWaypointIndex);
        }      

        private bool AtWaypoint()
        {
            if (Vector2.Distance(patrolPath.GetWaypoint(currentWaypointIndex), transform.position) <= stoppingDistance)
            {
                timeSinceArrivedAtPatrolPoint = 0;
                return true;
            }
            return false;
        }

        private void CycleWaypoint()
        {
            currentWaypointIndex = patrolPath.GetNextIndex(currentWaypointIndex);
        }      

        public void FacePlayer(Vector3 playerPos) // todo reset to the starting direction
        {
            if (animator == null) return;

            shouldPatrol = false;

            Vector2 direction = new Vector2(playerPos.x - transform.position.x,
                                                    playerPos.y - transform.position.y);
            Vector2 normalizedDir = direction.normalized;

            animator.SetFloat("Horizontal", normalizedDir.x);
            animator.SetFloat("Vertical", normalizedDir.y);
        }

        private void OnFinishedDialogue()
        {
            shouldPatrol = true;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                if (animator == null) return;
                shouldPatrol = false;
                animator.SetFloat("Speed", 0);
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                if (animator == null) return;
                shouldPatrol = true;
                animator.SetFloat("Speed", 1);
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shop.Control;

public class PurchaseClothesSequencer : MonoBehaviour
{
    [SerializeField] PatrolPath path;
    [SerializeField] PlayerController controller;
    [SerializeField] float walkingSpeed = 1f;
    [SerializeField] int indexToStopAt = 4;
    Animator animator;
    // playable director [SerializeField] 

    int currentWaypointIndex = 0;
    float timeSinceArrivedAtPatrolPoint = Mathf.Infinity;
    bool shouldMove = false;
    bool arrivedAtWaypoint = false;
    Vector3 nextPosition;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update() 
    {
        if (shouldMove && path != null)
        {
            if (currentWaypointIndex == indexToStopAt)
            {
                arrivedAtWaypoint = true;
                // trigger door closing here
                return;
            }

            if (AtWaypoint())
            {
                CycleWaypoint();
            }
            nextPosition = GetCurrentWaypoint();


            MoveTo(nextPosition);
            animator.SetFloat("Speed", 1);

        }
    }

    public void BeginSequence()
    {
        shouldMove = true;
    }


    private bool AtWaypoint()
    {
        if (Vector2.Distance(path.GetWaypoint(currentWaypointIndex), transform.position) <= 0.1f)
        {
            timeSinceArrivedAtPatrolPoint = 0;
            return true;
        }
        return false;
    }

    private void CycleWaypoint()
    {
        currentWaypointIndex = path.GetNextIndex(currentWaypointIndex);
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
        return path.GetWaypoint(currentWaypointIndex);
    }
}

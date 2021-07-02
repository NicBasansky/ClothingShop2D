using UnityEngine;

namespace Shop.Control
{
    public class PatrolPath : MonoBehaviour
    {
        const float waypointRadius = 0.1f;

        public Vector3 GetWaypoint(int index)
        {
            if (index > transform.childCount - 1) 
                return Vector3.zero;
            
            return transform.GetChild(index).position;
        }

        public int GetNextIndex(int i)
        {
            return (i + 1) % transform.childCount;

        }

        private void OnDrawGizmosSelected()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                int j = GetNextIndex(i);
                Gizmos.DrawSphere(transform.GetChild(i).position, waypointRadius);
                Gizmos.DrawLine(GetWaypoint(i), GetWaypoint(j));
            }   
        }
    }
}

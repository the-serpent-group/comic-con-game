using UnityEngine;
using Aoiti.Pathfinding; //A* PATHFINDING



// - ERNEST  (CHANGE IF U WISH JUST COMMENT / - _ * ) _ v _( */ ) - _/      ( +(*)_(*)+ )
public class NPCAnimation : MonoBehaviour
{
    private Animator animator; // ref animator 
    private MovementController2D movementController; // ref movement controller2D component

    void Start()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator component not found on NPC.", this);
            this.enabled = false;
            return;
        }

        movementController = GetComponent<MovementController2D>(); //using movement controller reference for componeent script attached to npc object
        if (movementController == null)
        {
            Debug.LogError("MovementController2D component not found on NPC.", this);
            this.enabled = false;
            return;
        }

        StandManager.OnStandPlaced += HandleStandPlaced;
    }



    void OnDestroy()
    {
        StandManager.OnStandPlaced -= HandleStandPlaced;
    }

    private void HandleStandPlaced(GameObject stand)
    {
        if (stand != null)
        {
            Vector3 standPosition = stand.transform.position;
            Vector2 targetPosition = new Vector2(standPosition.x, standPosition.y); //remove z 

            if (movementController != null)
            {
                movementController.SetTarget(targetPosition);
            }
        }
    }

    private void UpdateAnimator(Vector2 velocity)
    {
        Debug.Log($"Velocity: {velocity}, Magnitude: {velocity.magnitude}");
        animator.SetFloat("Horizontal", velocity.x);
        animator.SetFloat("Vertical", velocity.y);
        animator.SetFloat("Speed", velocity.magnitude);
    }

    void Update()
    {
        if (movementController.IsMoving)
        {
            Vector2 velocity = movementController.CurrentVelocity;
            animator.SetFloat("Horizontal", velocity.x);
            animator.SetFloat("Vertical", velocity.y);
            animator.SetFloat("Speed", velocity.magnitude);
        }
    }
    //draw AI pathfinding 
    private void OnDrawGizmosSelected()
    {
        if (movementController != null && movementController.PathLeftToGo != null)
        {
            Gizmos.color = Color.green;
            var path = movementController.PathLeftToGo;
            for (int i = 0; i < path.Count - 1; i++)
            {
                Gizmos.DrawLine(path[i], path[i + 1]);
            }
        }
    }
}


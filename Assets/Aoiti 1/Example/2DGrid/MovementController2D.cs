using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Aoiti.Pathfinding; //import the pathfinding library 


//public class NavigationTest: MonoBehaviour
//{
//    Pathfinder<Vector3> pathfinder;
//    List<Vector3> path = new List<Vector3>();

//    private void Start()
//    {
//        pathfinder = new Pathfinder<Vector3>(GetDistance, GetNeighbourNodes);
//    }
//    private void Update()
//    {
//        if (Input.GetMouseButtonDown(0)) //check for a new target
//        {
//            Vector3 target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
//            var _path = new Vector3[0];
//            if (pathfinder.GenerateAstarPath(transform.position, target, out _path)) //if there is a path from current position to target position reassign path.
//                path = new List<Vector3>(_path); 
//        }

//        transform.position = path[0]; //go to next node
//        path.RemoveAt(0); //remove the node from path

//    }

//    float GetDistance(Vector3 A, Vector3 B)
//    {
//        return (A - B).sqrMagnitude; 
//    }
//    Dictionary<Vector3, float> GetNeighbourNodes(Vector3 pos)
//    {
//        Dictionary<Vector3, float> neighbours = new Dictionary<Vector3, float>();
//        for (int i = -1; i < 2; i++)
//        {
//            for (int j = -1; j < 2; j++)
//            {
//                for (int k=-1;k<2;k++)
//                {

//                    if (i == 0 && j == 0 && k==0) continue;

//                    Vector3 dir = new Vector3(i, j,k);
//                    if (!Physics2D.Linecast(pos, pos + dir))
//                    {
//                        neighbours.Add(pos + dir, dir.magnitude);
//                    }
//                }
//            }

//        }
//        return neighbours;
//    }

//} 
public class MovementController2D : MonoBehaviour
{
    [Header("Navigator options")]
    [SerializeField] float gridSize = 0.5f; //increase patience or gridSize for larger maps
    [SerializeField] float speed = 0.05f; //increase for faster movement

    Pathfinder<Vector2> pathfinder; //the pathfinder object that stores the methods and patience
    [Tooltip("The layers that the navigator can not pass through.")]
    [SerializeField] LayerMask obstacles;
    [Tooltip("Deactivate to make the navigator move along the grid only, except at the end when it reaches to the target point. This shortens the path but costs extra Physics2D.LineCast")]
    [SerializeField] bool searchShortcut = false;
    [Tooltip("Deactivate to make the navigator to stop at the nearest point on the grid.")]
    [SerializeField] bool snapToGrid = false;
    Vector2 targetNode; //target in 2D space
    List<Vector2> path;
    List<Vector2> pathLeftToGo = new List<Vector2>();
    public List<Vector2> PathLeftToGo { get { return pathLeftToGo; } }
    [SerializeField] bool drawDebugLines;
    public LineRenderer lineRenderer;
    private Vector2 currentVelocity;

    public bool IsMoving => pathLeftToGo.Count > 0;
    public Vector2 CurrentVelocity => currentVelocity;
    private Animator animator;

    void Start()
    {
        pathfinder = new Pathfinder<Vector2>(GetDistance, GetNeighbourNodes, 1000);
        animator = GetComponent<Animator>(); // Get the Animator component

        // Check if the line renderer is already assigned, if not, add it
        if (lineRenderer == null)
        {
            lineRenderer = GetComponent<LineRenderer>(); // Try to get the LineRenderer if it's already attached
            if (lineRenderer == null) // If it's still null, then add it
            {
                lineRenderer = gameObject.AddComponent<LineRenderer>();
                lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
                lineRenderer.widthMultiplier = 0.1f;
                lineRenderer.startColor = Color.green;
                lineRenderer.endColor = Color.green;
            }
        }
    }

    private bool isMoving;

    public void SetTarget(Vector2 target)
    {
        if (pathfinder == null)
        {
            Debug.LogError("Pathfinder is not initialized");
            return;
        }

        Vector2 closestNode = GetClosestNode(transform.position);
        if (pathfinder.GenerateAstarPath(closestNode, GetClosestNode(target), out path))
        {
            pathLeftToGo = new List<Vector2>(path);
            if (!snapToGrid) pathLeftToGo.Add(target);
            isMoving = true;
        }
    }

    // Update is called once per frame



    void GetMoveCommand(Vector2 target)
    {
        Vector2 closestNode = GetClosestNode(transform.position);
        if (pathfinder.GenerateAstarPath(closestNode, GetClosestNode(target), out path)) //Generate path between two points on grid that are close to the transform position and the assigned target.
        {
            if (searchShortcut && path.Count > 0)
                pathLeftToGo = ShortenPath(path);
            else
            {
                pathLeftToGo = new List<Vector2>(path);
                if (!snapToGrid) pathLeftToGo.Add(target);
            }

        }

    }


    /// <summary>
    /// Finds closest point on the grid
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    Vector2 GetClosestNode(Vector2 target)
    {
        return new Vector2(Mathf.Round(target.x / gridSize) * gridSize, Mathf.Round(target.y / gridSize) * gridSize);
    }

    /// <summary>
    /// A distance approximation. 
    /// </summary>
    /// <param name="A"></param>
    /// <param name="B"></param>
    /// <returns></returns>
    float GetDistance(Vector2 A, Vector2 B)
    {
        return (A - B).sqrMagnitude;
    }

    /// <summary>
    /// Finds possible conenctions and the distances to those connections on the grid.
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    Dictionary<Vector2, float> GetNeighbourNodes(Vector2 pos)
    {
        Dictionary<Vector2, float> neighbours = new Dictionary<Vector2, float>();
        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                if (i == 0 && j == 0) continue;

                Vector2 dir = new Vector2(i, j) * gridSize;
                if (!Physics2D.Linecast(pos, pos + dir, obstacles))
                {
                    neighbours.Add(GetClosestNode(pos + dir), dir.magnitude);
                }
            }

        }
        return neighbours;
    }

    List<Vector2> ShortenPath(List<Vector2> path)
    {
        List<Vector2> newPath = new List<Vector2>();

        for (int i = 0; i < path.Count; i++)
        {
            newPath.Add(path[i]);
            for (int j = path.Count - 1; j > i; j--)
            {
                if (!Physics2D.Linecast(path[i], path[j], obstacles))
                {

                    i = j;
                    break;
                }
            }
            newPath.Add(path[i]);
        }
        newPath.Add(path[path.Count - 1]);
        return newPath;
    }
    public bool IsPathfinderInitialized()
    {
        return pathfinder != null;
    }

    void Awake()
    {
        pathfinder = new Pathfinder<Vector2>(GetDistance, GetNeighbourNodes, 1000);
        animator = GetComponent<Animator>();

    }

    void Update()
    {
        if (isMoving && pathLeftToGo.Count > 0)
        {
            Vector2 targetPosition = pathLeftToGo[0];
            currentVelocity = (targetPosition - (Vector2)transform.position).normalized * speed;
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

            if (drawDebugLines) // Check if we should draw debug lines
            {
                for (int i = 0; i < pathLeftToGo.Count - 1; i++)
                {
                    Debug.DrawLine(pathLeftToGo[i], pathLeftToGo[i + 1], Color.green);
                }
            }

            // on reach condition 
            if ((Vector2)transform.position == targetPosition)
            {
                pathLeftToGo.RemoveAt(0);
                if (pathLeftToGo.Count == 0)
                {
                    // bool stop moving now false
                    isMoving = false;
                    // notifing the NPC has reached its target
                    NPCBehavior npcBehavior = GetComponent<NPCBehavior>();
                    if (npcBehavior != null)
                    {
                        npcBehavior.OnReachedTargetStand(); // call unity event. 
                    }
                }
            }

            if (pathLeftToGo.Count > 0)
            {
                Vector2 directionToNextPoint = (pathLeftToGo[0] - (Vector2)transform.position).normalized; 
                currentVelocity = directionToNextPoint * speed;
            }
            else
            {
                currentVelocity = Vector2.zero;
            }
        }
    }
}

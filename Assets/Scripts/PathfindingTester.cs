using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PathfindingTester : MonoBehaviour
{
    // The A* manager.
    private AStarManager AStarManager = new AStarManager(); // Array of possible waypoints.
    private List<GameObject> Waypoints = new List<GameObject>();
    // Array of waypoint map connections. Represents a path.
    private List<Connection> ConnectionArray = new List<Connection>();
    // The start and end nodes.
    [SerializeField]
    private GameObject start;
    [SerializeField] private List<GameObject> ends = new List<GameObject>();
    public int present = 0;

    //private GameObject end;
    // Debug line offset.
    Vector3 OffSet = new Vector3(0, 0.3f, 0);
    // Movement variables.
    public float currentSpeed = 8;
    private int currentTarget = 0;
    private Vector3 currentTargetPos;
    private int moveDirection = 1;
    private bool agentMove = true;

    //time tracking
    [SerializeField] PerformanceTracker performanceTracker;


    //play sound
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip audioClip;

    // Start is called before the first frame update
    void Start()
    {
        if (start == null || ends.Count == 0)
        {
            Debug.Log("No start or end waypoints.");
            return;
        }
        // Find all the waypoints in the level.
        GameObject[] GameObjectsWithWaypointTag;
        GameObjectsWithWaypointTag = GameObject.FindGameObjectsWithTag("Waypoint");
        foreach (GameObject waypoint in GameObjectsWithWaypointTag)
        {
            WaypointCON tmpWaypointCon = waypoint.GetComponent<WaypointCON>();
            if (tmpWaypointCon)
            {
                Waypoints.Add(waypoint);
            }
        }
        // Go through the waypoints and create connections.
        foreach (GameObject waypoint in Waypoints)
        {
            WaypointCON tmpWaypointCon = waypoint.GetComponent<WaypointCON>();
            // Loop through a waypoints connections.
            foreach (GameObject WaypointConNode in tmpWaypointCon.Connections)
            {
                Connection aConnection = new Connection();
                aConnection.FromNode = waypoint;
                aConnection.ToNode = WaypointConNode;
                AStarManager.AddConnection(aConnection);
            }
        }
        // Run A Star...
        // ConnectionArray stores all the connections in the route to the goal / end node.
        ConnectionArray = AStarManager.PathfindAStar(start, ends[present]);
    }
    // Draws debug objects in the editor and during editor play (if option set).
    void OnDrawGizmos()
    {
        // Draw path.
        foreach (Connection aConnection in ConnectionArray)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawLine((aConnection.FromNode.transform.position + OffSet), (aConnection.ToNode.transform.position + OffSet));
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (agentMove)
        {
            // Determine the direction to current target node in the array.
            currentTargetPos = ConnectionArray[currentTarget].ToNode.transform.position;

            // Clear y to avoid up/down movement.
            currentTargetPos.y = transform.position.y;

            Vector3 direction = currentTargetPos - transform.position;

            // Calculate the length of the relative position vector
            float distance = direction.magnitude;

            // Face in the right direction.
            direction.y = 0;
            Quaternion rotation = Quaternion.LookRotation(direction, Vector3.up);
            transform.rotation = rotation;

            // Calculate the normalised direction to the target from a game object.
            Vector3 normDirection = direction / distance;

            // Move the game object.
            transform.position = transform.position + normDirection * currentSpeed * Time.deltaTime;

            // Check if close to current target.
            if (distance < 1)
            {
                // Close to target, so move to the next target in the list (if there is one).
                currentTarget += moveDirection;

                if (currentTarget == ConnectionArray.Count)
                {
                    // Reached end node. Change direction and set target to start node.
                    moveDirection = -1;
                    currentTarget = ConnectionArray.Count - 2;
                }
                else if (currentTarget == -1)
                {
                    if (present == ends.Count - 1)
                    {
                        // Reached start node after going in reverse from the final endpoint. Stop moving.
                        agentMove = false;
                        performanceTracker.StopTracking();
                    }
                    else
                    {
                        // Reached start node after going in reverse. Change direction and set target to first end node.
                        moveDirection = 1;
                        currentTarget = 1;
                        present = (present + 1) % ends.Count; // switch to next end point
                        ConnectionArray = AStarManager.PathfindAStar(start, ends[present]); // calculate new path to next end point
                    }
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Box"))
        {
            audioSource.PlayOneShot(audioClip);
            Debug.Log("Dink");
        }

        if (other.CompareTag("Obstacle"))
        {
            currentSpeed = 0;
            Debug.Log("Dink");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            currentSpeed = 15;
            Debug.Log("Dink");
        }
    }
}

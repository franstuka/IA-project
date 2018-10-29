using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Navegation : MonoBehaviour {

    
    public float angularSpeed = 120f;
    public float maxSpeed = 3.5f;
    public float acceleration = 4f;
    [Range (0f , 1f)] public float stoppingDistanceFactor = 0.5f;
    public float maxCorrectionAcceleration = 15f;

    private Vector2Int thisLastSquarePosition;
    private Vector2Int targetLastSquarePosition;
    private AStarPathfinding Astar;
    private LinkedList<Vector2Int> savedPath;
    private Rigidbody rigidbody;
    private float stoppingDistance;
    private bool stopped = false;
    private float setStoppedValue = 0.05f;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        float minimunGridSize = GridMap.instance.GetGridSizeX() < GridMap.instance.GetGridSizeY()? GridMap.instance.GetGridSizeX() : GridMap.instance.GetGridSizeY();
        stoppingDistance = minimunGridSize * stoppingDistanceFactor;
    }

    private void Update()
    {
        if(!stopped) //if entity is not idle
            UpdateActualPosition();
    }

    public void SetDestination(Vector3 pos)
    {
        Vector2Int thisActualSquarePosition = GridMap.instance.CellCordFromWorldPoint(transform.position); ;
        Vector2Int targetActualSquarePosition = GridMap.instance.CellCordFromWorldPoint(pos); ;

        stopped = false; //activate movement if entity was idle.

        //if()
        switch (Astar.GetUpdateMode())
        {
            case AStarPathfinding.UpdateMode.ONLY_ON_TARGET_MOVE:
                {
                    if(targetActualSquarePosition != targetLastSquarePosition)
                    {
                        savedPath = Astar.GetPath(thisActualSquarePosition, targetActualSquarePosition);
                    }
                    break;
                }
            case AStarPathfinding.UpdateMode.ON_TARGET_OR_ORIGIN_MOVE:
                {
                    if (targetActualSquarePosition != targetLastSquarePosition && thisActualSquarePosition != thisLastSquarePosition)
                    {
                        savedPath = Astar.GetPath(thisActualSquarePosition, targetActualSquarePosition);
                    }
                    break;
                }
            case AStarPathfinding.UpdateMode.ONLY_ON_TARGET_MOVE_WITH_COLLISION_DETECTER:
                {
                    if (targetActualSquarePosition != targetLastSquarePosition )
                    {
                        savedPath = Astar.GetPath(thisActualSquarePosition, targetActualSquarePosition);
                    }
                    break;
                }
            case AStarPathfinding.UpdateMode.EVERY_CELL_CHANGE:
                {
                    if (targetActualSquarePosition != targetLastSquarePosition && thisActualSquarePosition != thisLastSquarePosition)
                    {
                        savedPath = Astar.GetPath(thisActualSquarePosition, targetActualSquarePosition);
                    }
                    break;
                }
            case AStarPathfinding.UpdateMode.ON_TIMER:
                {
                    savedPath = Astar.GetPath(thisActualSquarePosition, targetActualSquarePosition);
                    break;
                }
            default:
                {
                    Debug.LogError("A* has not update mode setted");
                    break;
                }
        }
    }

    public void UpdateActualPosition()
    {
        Vector2Int thisActualSquarePosition = GridMap.instance.CellCordFromWorldPoint(transform.position); 
        if (thisActualSquarePosition == savedPath.First.Value)
        {
            if(savedPath.First.Next == null) //end of path
            {
                Move(GridMap.instance.grid[thisActualSquarePosition.x, thisActualSquarePosition.y].GlobalPosition); // move to square center
            }
            else
            {
                savedPath.RemoveFirst();
                Move(GridMap.instance.grid[savedPath.First.Value.x, savedPath.First.Value.y].GlobalPosition); // move to next point
            }
        }
        else
        {
            Move(GridMap.instance.grid[savedPath.First.Value.x, savedPath.First.Value.y].GlobalPosition); // move normal
        }
    }

    public void Move(Vector3 position)
    {
        float velZ = rigidbody.velocity.z;
        float velX = rigidbody.velocity.x;

        //rotation
        float invertedSpeed = Mathf.Sqrt(Mathf.Pow(maxSpeed, 2) - Mathf.Pow(new Vector2(velX, velZ).magnitude, 2));
        rigidbody.AddTorque(transform.up * angularSpeed * invertedSpeed, ForceMode.Acceleration);

        if (stoppingDistance < Vector3.Distance(transform.position, position) && savedPath.First.Next == null) //stop
        {
            if (Mathf.Abs(velX) < setStoppedValue && Mathf.Abs(velZ) < setStoppedValue)
            {
                rigidbody.velocity.Set(0f, rigidbody.velocity.y, 0f);
                stopped = true;
            }
            else
            {
                float correctionAccelerationX = Mathf.Abs(-velX / Time.deltaTime) > maxCorrectionAcceleration ? maxCorrectionAcceleration : -velX / Time.deltaTime;
                float correctionAccelerationZ = Mathf.Abs(-velZ / Time.deltaTime) > acceleration ? acceleration : -velZ / Time.deltaTime;
                rigidbody.AddForce(new Vector3(correctionAccelerationX, 0f, correctionAccelerationZ), ForceMode.Acceleration);
            }
        }
        else
        {
            //movement
            float correctionAcceleration = Mathf.Abs(-velX / Time.deltaTime) > maxCorrectionAcceleration ? maxCorrectionAcceleration : -velX / Time.deltaTime;
            Vector2 toTarget = new Vector2(position.x - transform.position.x, position.z - transform.position.z).normalized;
            Vector2 toSpeed = new Vector2(velX, velZ).normalized;

            float finalAcceleration;
            float factor = Vector2.Dot(toTarget, toSpeed) * 0.9f;
            if (factor >= 0)
            {
                factor += 0.1f;
            }
            else
            {
                factor -= 0.1f;
            }

            if (velZ < 1 / 2 * maxSpeed)
            {
                finalAcceleration = Mathf.Abs((maxSpeed * factor - velZ) / Time.deltaTime) < acceleration ? Mathf.Max((maxSpeed * factor - velZ) / Time.deltaTime, -acceleration) : acceleration;
                rigidbody.AddForce(new Vector3(correctionAcceleration, 0f, finalAcceleration), ForceMode.Acceleration);
            }
            else if (velZ < 3 / 4 * maxSpeed)
            {
                finalAcceleration = Mathf.Abs((maxSpeed * factor - velZ) / Time.deltaTime) < acceleration / 2 ? (maxSpeed * factor - velZ) / Time.deltaTime : acceleration / 2;
                rigidbody.AddForce(new Vector3(correctionAcceleration, 0f, finalAcceleration), ForceMode.Acceleration);
            }
            else
            {
                finalAcceleration = Mathf.Abs((maxSpeed * factor - velZ) / Time.deltaTime) < acceleration / 4 ? (maxSpeed * factor - velZ) / Time.deltaTime : acceleration / 4;
                rigidbody.AddForce(new Vector3(correctionAcceleration, 0f, finalAcceleration), ForceMode.Acceleration);
            }
        }
    }
}

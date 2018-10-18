using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System.Threading;



public class AStarPathfinding { //By default this is for a quad grid

    public enum TargetDistanceAdvanceDirection { UP_RIGHT, UP_LEFT, DOWN_RIGHT, DOWN_LEFT };
    public enum AStarAlgorithmState { FINISHED , IN_PROCESS , LIMITED_BY_STEPS , NO_AVAILABLE_SOLUTION};

    private LinkedList<Vector2Int> Heap; //position in grid, in x,y
    private AStarAlgorithmState state;
    private const int normalCost = 10;
    private const int diagonalCost = 14;
    private const int enemyInSameCellCost = 6;
    private uint maxSteps = uint.MaxValue;
    private Vector2Int lastStepPos;
    private Vector2Int endNodePos;
    private Vector2Int startNodePos;

    private int maxX;
    private int maxY;

    public AStarPathfinding(Vector2Int startNodePos, Vector2Int endNodePos)
    {
        Heap = new LinkedList<Vector2Int>();
        this.endNodePos = endNodePos;
        this.startNodePos = startNodePos;
        maxX = GridMap.instance.GetGridSizeX();
        maxY = GridMap.instance.GetGridSizeY();
    }

    public AStarPathfinding(Vector2Int startNodePos, Vector2Int endNodePos , uint maxSteps)
    {
        Heap = new LinkedList<Vector2Int>();
        this.endNodePos = endNodePos;
        this.startNodePos = startNodePos;
        maxX = GridMap.instance.GetGridSizeX();
        maxY = GridMap.instance.GetGridSizeY();
        this.maxSteps = maxSteps;
    }

    #region stage 0
    public void CalculateTargetDistance()
    {
        //initial node setup
        GridMap.instance.grid[endNodePos.x, endNodePos.y].Node = new AStarNode(0)
        {
            AvaibleAdjacentNodes = CheckAvaiblesPositions(endNodePos.x, endNodePos.y)
        };

        //set adjacent (non diagonal) cells

        for (int i = 1; i < maxY - endNodePos.y; i++)
        {
            GridMap.instance.grid[endNodePos.x, i + endNodePos.y].Node = new AStarNode(i * normalCost)
            {
                AvaibleAdjacentNodes = CheckAvaiblesPositions(endNodePos.x, i + endNodePos.y)
            };

        }
        for (int i = -1; i > -endNodePos.y - 1; i--)
        {
            GridMap.instance.grid[endNodePos.x, i + endNodePos.y].Node = new AStarNode(Mathf.Abs(i) * normalCost)
            {
                AvaibleAdjacentNodes = CheckAvaiblesPositions(endNodePos.x, i + endNodePos.y)
            };

        }
        for (int i = 1; i < maxX - endNodePos.x; i++)
        {
            GridMap.instance.grid[i + endNodePos.x, endNodePos.y].Node = new AStarNode(i * normalCost)
            {
                AvaibleAdjacentNodes = CheckAvaiblesPositions(i + endNodePos.x, endNodePos.y)
            };

        }
        for (int i = -1; i > -endNodePos.x - 1; i--)
        {
            GridMap.instance.grid[i + endNodePos.x, endNodePos.y].Node = new AStarNode(Mathf.Abs(i) * normalCost)
            {
                AvaibleAdjacentNodes = CheckAvaiblesPositions(i + endNodePos.x, endNodePos.y)
            };

        }
        /////////////////////////////////


        //expand in diagonal nodes
        if (endNodePos.x > 0 && endNodePos.x < maxX - 1 && endNodePos.y > 0 && endNodePos.y < maxY - 1)
        {
            CalculateTargetRecursion(diagonalCost, endNodePos.x - 1, endNodePos.y + 1, TargetDistanceAdvanceDirection.UP_RIGHT);
            CalculateTargetRecursion(diagonalCost, endNodePos.x - 1, endNodePos.y - 1, TargetDistanceAdvanceDirection.UP_LEFT);
            CalculateTargetRecursion(diagonalCost, endNodePos.x + 1, endNodePos.y + 1, TargetDistanceAdvanceDirection.DOWN_RIGHT);
            CalculateTargetRecursion(diagonalCost, endNodePos.x + 1, endNodePos.y - 1, TargetDistanceAdvanceDirection.DOWN_LEFT);
        }
        else if (endNodePos.x == 0 && endNodePos.y == 0)
        {
            CalculateTargetRecursion(diagonalCost, endNodePos.x + 1, endNodePos.y + 1, TargetDistanceAdvanceDirection.DOWN_RIGHT);
        }
        else if (endNodePos.x == 0 && endNodePos.y == maxY - 1)
        {
            CalculateTargetRecursion(diagonalCost, endNodePos.x + 1, endNodePos.y - 1, TargetDistanceAdvanceDirection.DOWN_LEFT);
        }
        else if (endNodePos.x == maxX - 1 && endNodePos.y == 0)
        {
            CalculateTargetRecursion(diagonalCost, endNodePos.x - 1, endNodePos.y + 1, TargetDistanceAdvanceDirection.UP_RIGHT);
        }
        else if (endNodePos.x == maxX - 1 && endNodePos.y == maxY - 1)
        {
            CalculateTargetRecursion(diagonalCost, endNodePos.x - 1, endNodePos.y - 1, TargetDistanceAdvanceDirection.UP_LEFT);
        }
        else if (endNodePos.x == 0)
        {
            CalculateTargetRecursion(diagonalCost, endNodePos.x - 1, endNodePos.y + 1, TargetDistanceAdvanceDirection.UP_RIGHT);
            CalculateTargetRecursion(diagonalCost, endNodePos.x + 1, endNodePos.y + 1, TargetDistanceAdvanceDirection.DOWN_RIGHT);
        }
        else if (endNodePos.y == 0)
        {
            CalculateTargetRecursion(diagonalCost, endNodePos.x + 1, endNodePos.y + 1, TargetDistanceAdvanceDirection.DOWN_RIGHT);
            CalculateTargetRecursion(diagonalCost, endNodePos.x + 1, endNodePos.y - 1, TargetDistanceAdvanceDirection.DOWN_LEFT);
        }
        else if (endNodePos.x == maxX - 1)
        {
            CalculateTargetRecursion(diagonalCost, endNodePos.x - 1, endNodePos.y - 1, TargetDistanceAdvanceDirection.UP_LEFT);
            CalculateTargetRecursion(diagonalCost, endNodePos.x + 1, endNodePos.y - 1, TargetDistanceAdvanceDirection.DOWN_LEFT);
        }
        else if (endNodePos.y == maxY - 1)
        {
            CalculateTargetRecursion(diagonalCost, endNodePos.x - 1, endNodePos.y + 1, TargetDistanceAdvanceDirection.UP_RIGHT);
            CalculateTargetRecursion(diagonalCost, endNodePos.x - 1, endNodePos.y - 1, TargetDistanceAdvanceDirection.UP_LEFT);
        }
        else
            Debug.LogError("No suitable solution");

    }

    private void CalculateTargetRecursion(int baseCost, int x, int y, TargetDistanceAdvanceDirection direction)
    {
        if (direction == TargetDistanceAdvanceDirection.DOWN_RIGHT)
        {

            for (int i = 0; i < maxY - y; i++)
            {
                GridMap.instance.grid[x, i + y].Node = new AStarNode(baseCost + i * normalCost)
                {
                    AvaibleAdjacentNodes = CheckAvaiblesPositions(x, i + y)
                };
            }
            for (int i = 1; i < maxX - x; i++)
            {
                GridMap.instance.grid[i + x, y].Node = new AStarNode(baseCost + i * normalCost)
                {
                    AvaibleAdjacentNodes = CheckAvaiblesPositions(i + x, y)
                };
            }
            if (x < maxX && y < maxY)
                CalculateTargetRecursion(baseCost + diagonalCost, x + 1, y + 1, TargetDistanceAdvanceDirection.DOWN_RIGHT);
        }
        else if (direction == TargetDistanceAdvanceDirection.DOWN_LEFT)
        {

            for (int i = 0; i > -y - 1; i--)
            {
                GridMap.instance.grid[x, i + y].Node = new AStarNode(baseCost + Mathf.Abs(i) * normalCost)
                {
                    AvaibleAdjacentNodes = CheckAvaiblesPositions(x, i + y)
                };
            }
            for (int i = 1; i < maxX - x; i++)
            {
                GridMap.instance.grid[i + x, y].Node = new AStarNode(baseCost + i * normalCost)
                {
                    AvaibleAdjacentNodes = CheckAvaiblesPositions(i + x, y)
                };
            }
            if (x < maxX && y >= 0)
                CalculateTargetRecursion(baseCost + diagonalCost, x + 1, y - 1, TargetDistanceAdvanceDirection.DOWN_LEFT);
        }
        else if (direction == TargetDistanceAdvanceDirection.UP_LEFT)
        {

            for (int i = 0; i < -y - 1; i--)
            {
                GridMap.instance.grid[x, i + y].Node = new AStarNode(baseCost + Mathf.Abs(i) * normalCost)
                {
                    AvaibleAdjacentNodes = CheckAvaiblesPositions(x, i + y)
                };
            }
            for (int i = -1; i < -x - 1; i--)
            {
                GridMap.instance.grid[i + x, y].Node = new AStarNode(baseCost + Mathf.Abs(i) * normalCost)
                {
                    AvaibleAdjacentNodes = CheckAvaiblesPositions(i + x, y)
                };
            }
            if (x >= 0 && y >= 0)
                CalculateTargetRecursion(baseCost + diagonalCost, x - 1, y - 1, TargetDistanceAdvanceDirection.UP_LEFT);
        }
        else if (direction == TargetDistanceAdvanceDirection.UP_RIGHT)
        {

            for (int i = 0; i < maxY - y; i++)
            {
                GridMap.instance.grid[x, i + y].Node = new AStarNode(baseCost + i * normalCost)
                {
                    AvaibleAdjacentNodes = CheckAvaiblesPositions(x, i + y)
                };
            }
            for (int i = -1; i < -x - 1; i--)
            {
                GridMap.instance.grid[i + x, y].Node = new AStarNode(baseCost + Mathf.Abs(i) * normalCost)
                {
                    AvaibleAdjacentNodes = CheckAvaiblesPositions(i + x, y)
                };
            }
            if (x >= 0 && y < maxY)
                CalculateTargetRecursion(baseCost + diagonalCost, x - 1, y + 1, TargetDistanceAdvanceDirection.UP_RIGHT);
        }
        else
            Debug.LogError("This case don't exist");

    }

    private byte CheckAvaiblesPositions(int x, int y)
    {
        byte cellsAvaible = 0;
        if (!AvaibleListPositions(x, y)) //if this is true, cell is inaccesible, so it never will be inserted in heap or visited
        {
            GridMap.instance.grid[x, y].Node.visited = true;
            return 0;
        }
        else if (x > 0 && x < maxX - 1 && y > 0 && y < maxY - 1)
        {
            if (AvaibleListPositions(x - 1, y - 1)) cellsAvaible++;
            if (AvaibleListPositions(x + 1, y + 1)) cellsAvaible++;
            if (AvaibleListPositions(x + 1, y)) cellsAvaible++;
            if (AvaibleListPositions(x, y + 1)) cellsAvaible++;
            if (AvaibleListPositions(x - 1, y)) cellsAvaible++;
            if (AvaibleListPositions(x, y - 1)) cellsAvaible++;
            if (AvaibleListPositions(x + 1, y - 1)) cellsAvaible++;
            if (AvaibleListPositions(x - 1, y + 1)) cellsAvaible++;
        }
        else if (x == 0 && y == 0)
        {
            if (AvaibleListPositions(x + 1, y)) cellsAvaible++;
            if (AvaibleListPositions(x, y + 1)) cellsAvaible++;
            if (AvaibleListPositions(x + 1, y + 1)) cellsAvaible++;
        }
        else if (x == 0 && y == maxY - 1)
        {
            if (AvaibleListPositions(x + 1, y)) cellsAvaible++;
            if (AvaibleListPositions(x + 1, y - 1)) cellsAvaible++;
            if (AvaibleListPositions(x, y - 1)) cellsAvaible++;
        }
        else if (x == maxX - 1 && y == 0)
        {
            if (AvaibleListPositions(x - 1, y)) cellsAvaible++;
            if (AvaibleListPositions(x - 1, y + 1)) cellsAvaible++;
            if (AvaibleListPositions(x, y + 1)) cellsAvaible++;
        }
        else if (x == maxX - 1 && y == maxY - 1)
        {
            if (AvaibleListPositions(x - 1, y)) cellsAvaible++;
            if (AvaibleListPositions(x, y - 1)) cellsAvaible++;
            if (AvaibleListPositions(x - 1, y - 1)) cellsAvaible++;
        }
        else if (x == 0)
        {
            if (AvaibleListPositions(x + 1, y + 1)) cellsAvaible++;
            if (AvaibleListPositions(x + 1, y)) cellsAvaible++;
            if (AvaibleListPositions(x + 1, y - 1)) cellsAvaible++;
            if (AvaibleListPositions(x, y + 1)) cellsAvaible++;
            if (AvaibleListPositions(x, y - 1)) cellsAvaible++;
        }
        else if (y == 0)
        {
            if (AvaibleListPositions(x + 1, y + 1)) cellsAvaible++;
            if (AvaibleListPositions(x + 1, y)) cellsAvaible++;
            if (AvaibleListPositions(x, y + 1)) cellsAvaible++;
            if (AvaibleListPositions(x - 1, y)) cellsAvaible++;
            if (AvaibleListPositions(x - 1, y + 1)) cellsAvaible++;
        }
        else if (x == maxX - 1)
        {
            if (AvaibleListPositions(x - 1, y - 1)) cellsAvaible++;
            if (AvaibleListPositions(x, y + 1)) cellsAvaible++;
            if (AvaibleListPositions(x - 1, y)) cellsAvaible++;
            if (AvaibleListPositions(x, y - 1)) cellsAvaible++;
            if (AvaibleListPositions(x - 1, y + 1)) cellsAvaible++;
        }
        else if (y == maxY - 1)
        {
            if (AvaibleListPositions(x - 1, y - 1)) cellsAvaible++;
            if (AvaibleListPositions(x + 1, y)) cellsAvaible++;
            if (AvaibleListPositions(x - 1, y)) cellsAvaible++;
            if (AvaibleListPositions(x, y - 1)) cellsAvaible++;
            if (AvaibleListPositions(x + 1, y - 1)) cellsAvaible++;
        }
        else
            Debug.LogError("No suitable solution");

        return cellsAvaible;
    }

    private bool AvaibleListPositions(int x, int y) // all allowed positions are setted here
    {
        switch (GridMap.instance.grid[x, y].CellType) //banned positions
        {
            case CellTypes.blocked:
            case CellTypes.chest:
            case CellTypes.exit:
                {
                    return false;
                }
        }
        return true;
    }

    #endregion

    #region stage 1

    private void InitializeHeap()
    {

        GridMap.instance.grid[startNodePos.x, startNodePos.y].Node.FromInitialCost = 0;
        GridMap.instance.grid[startNodePos.x, startNodePos.y].Node.SetFinalCost();
        GridMap.instance.grid[startNodePos.x, startNodePos.y].Node.visited = true;
        GridMap.instance.grid[startNodePos.x, startNodePos.y].Node.stepsUsed = 0;
        UpdateAdjacentAvaibles(startNodePos.x, startNodePos.y);
        Heap.AddFirst(new Vector2Int(startNodePos.x, startNodePos.y)); 
    }

    private void UpdateAdjacentAvaibles(int x, int y)
    {
        if (x > 0 && x < maxX - 1 && y > 0 && y < maxY - 1)
        {
            GridMap.instance.grid[x + 1, y].Node.ReduceAvaiblesNodes();
            GridMap.instance.grid[x - 1, y].Node.ReduceAvaiblesNodes();
            GridMap.instance.grid[x + 1, y + 1].Node.ReduceAvaiblesNodes();
            GridMap.instance.grid[x - 1, y - 1].Node.ReduceAvaiblesNodes();
            GridMap.instance.grid[x + 1, y - 1].Node.ReduceAvaiblesNodes();
            GridMap.instance.grid[x - 1, y + 1].Node.ReduceAvaiblesNodes();
            GridMap.instance.grid[x, y + 1].Node.ReduceAvaiblesNodes();
            GridMap.instance.grid[x, y - 1].Node.ReduceAvaiblesNodes();
        }
        else if (x == 0 && y == 0)
        {
            GridMap.instance.grid[x + 1, y].Node.ReduceAvaiblesNodes();
            GridMap.instance.grid[x + 1, y + 1].Node.ReduceAvaiblesNodes();
            GridMap.instance.grid[x, y + 1].Node.ReduceAvaiblesNodes();
        }
        else if (x == 0 && y == maxY - 1)
        {
            GridMap.instance.grid[x + 1, y].Node.ReduceAvaiblesNodes();
            GridMap.instance.grid[x, y - 1].Node.ReduceAvaiblesNodes();
            GridMap.instance.grid[x + 1, y - 1].Node.ReduceAvaiblesNodes();
        }
        else if (x == maxX - 1 && y == 0)
        {
            GridMap.instance.grid[x - 1, y].Node.ReduceAvaiblesNodes();
            GridMap.instance.grid[x - 1, y + 1].Node.ReduceAvaiblesNodes();
            GridMap.instance.grid[x, y + 1].Node.ReduceAvaiblesNodes();
        }
        else if (x == maxX - 1 && y == maxY - 1)
        {
            GridMap.instance.grid[x - 1, y - 1].Node.ReduceAvaiblesNodes();
            GridMap.instance.grid[x - 1, y].Node.ReduceAvaiblesNodes();
            GridMap.instance.grid[x, y - 1].Node.ReduceAvaiblesNodes();
        }
        else if (x == 0)
        {
            GridMap.instance.grid[x + 1, y].Node.ReduceAvaiblesNodes();
            GridMap.instance.grid[x + 1, y + 1].Node.ReduceAvaiblesNodes();
            GridMap.instance.grid[x, y + 1].Node.ReduceAvaiblesNodes();
            GridMap.instance.grid[x, y - 1].Node.ReduceAvaiblesNodes();
            GridMap.instance.grid[x + 1, y - 1].Node.ReduceAvaiblesNodes();
        }
        else if (y == 0)
        {
            GridMap.instance.grid[x + 1, y].Node.ReduceAvaiblesNodes();
            GridMap.instance.grid[x - 1, y].Node.ReduceAvaiblesNodes();
            GridMap.instance.grid[x + 1, y + 1].Node.ReduceAvaiblesNodes();
            GridMap.instance.grid[x - 1, y + 1].Node.ReduceAvaiblesNodes();
            GridMap.instance.grid[x, y + 1].Node.ReduceAvaiblesNodes();
        }
        else if (x == maxX - 1)
        {
            GridMap.instance.grid[x - 1, y].Node.ReduceAvaiblesNodes();
            GridMap.instance.grid[x - 1, y - 1].Node.ReduceAvaiblesNodes();
            GridMap.instance.grid[x - 1, y + 1].Node.ReduceAvaiblesNodes();
            GridMap.instance.grid[x, y + 1].Node.ReduceAvaiblesNodes();
            GridMap.instance.grid[x, y - 1].Node.ReduceAvaiblesNodes();
        }
        else if (y == maxY - 1)
        {
            GridMap.instance.grid[x + 1, y].Node.ReduceAvaiblesNodes();
            GridMap.instance.grid[x - 1, y].Node.ReduceAvaiblesNodes();
            GridMap.instance.grid[x - 1, y - 1].Node.ReduceAvaiblesNodes();
            GridMap.instance.grid[x + 1, y - 1].Node.ReduceAvaiblesNodes();
            GridMap.instance.grid[x, y - 1].Node.ReduceAvaiblesNodes();
        }
        else
            Debug.LogError("No suitable solution");
    }

    #endregion

    #region stage 2

    private bool SearchMinimun()
    {
        Vector3Int temporalPositionAndMin = new Vector3Int(0,0,int.MaxValue);
        Vector3Int holdedPositionAndMin = new Vector3Int(0, 0, int.MaxValue);
        Vector2Int fromInitialNodePosition = new Vector2Int(0, 0);
        LinkedListNode<Vector2Int> i = Heap.First;

        if(i == null)
        {
            state = AStarAlgorithmState.NO_AVAILABLE_SOLUTION;
            return true;
        }

        while (i != null)
        {
            if (GridMap.instance.grid[i.Value.x, i.Value.y].Node.AvaibleAdjacentNodes == 0)
            {
                if (i.Previous != null)
                {
                    i = i.Previous;
                    Heap.Remove(i.Next);
                    i = i.Next;
                    continue;
                }
                else
                {
                    i = i.Next; //if null, bucle ends
                    Heap.RemoveFirst();
                    continue;
                }
            }
            temporalPositionAndMin = GetMinimumAroundNode(i.Value.x, i.Value.y, ref GridMap.instance.grid[i.Value.x, i.Value.y].Node.FromInitialCost);
            if (temporalPositionAndMin.z == int.MaxValue)
            {
                Debug.LogError("min search arround this node has maximun int value");
            }
            if(temporalPositionAndMin.z < holdedPositionAndMin.z)//new min
            {
                holdedPositionAndMin = temporalPositionAndMin;
                fromInitialNodePosition = new Vector2Int(i.Value.x, i.Value.y);
            }
            i = i.Next;
        }
        
        //Staff to do around the node
        GridMap.instance.grid[holdedPositionAndMin.x, holdedPositionAndMin.y].Node.FromInitialCost = 
            holdedPositionAndMin.z - GridMap.instance.grid[startNodePos.x, startNodePos.y].Node.FromFinalCost; //maybe the cost has changed 
        GridMap.instance.grid[holdedPositionAndMin.x, holdedPositionAndMin.y].Node.SetFinalCost();
        UpdateAdjacentAvaibles(holdedPositionAndMin.x, holdedPositionAndMin.y);
        GridMap.instance.grid[holdedPositionAndMin.x, holdedPositionAndMin.y].Node.visited = true;
        GridMap.instance.grid[holdedPositionAndMin.x, holdedPositionAndMin.y].Node.stepsUsed =
            GridMap.instance.grid[fromInitialNodePosition.x, fromInitialNodePosition.y].Node.stepsUsed + 1;

        //Staff to set nodes relationships
        GridMap.instance.grid[fromInitialNodePosition.x, fromInitialNodePosition.y].Node.AddChill(holdedPositionAndMin.x, holdedPositionAndMin.y);
        GridMap.instance.grid[holdedPositionAndMin.x, holdedPositionAndMin.y].Node.SetParent(fromInitialNodePosition.x, fromInitialNodePosition.y);

        if(holdedPositionAndMin.x == endNodePos.x && holdedPositionAndMin.y == endNodePos.y) //we reach the exit
        {
            state = AStarAlgorithmState.FINISHED;
            return true;
        }
        else if(GridMap.instance.grid[holdedPositionAndMin.x, holdedPositionAndMin.y].Node.stepsUsed <= maxSteps) //continue
        {
            //Add to heap
            Heap.AddFirst(new Vector2Int(holdedPositionAndMin.x, holdedPositionAndMin.y));
            return false;
        }
        else //limited by steps
        {
            lastStepPos = new Vector2Int(holdedPositionAndMin.x, holdedPositionAndMin.y);
            state = AStarAlgorithmState.LIMITED_BY_STEPS;
            return true;
        }
        
    }

    private Vector3Int GetMinimumAroundNode(int x, int y, ref int fromLastInitialNodeCost) //REWORKED FUNCTION, LIKE RYZE
    { 
        #region Stage 1, SetCostAround
        /*
        if (x > 0 && x < maxX - 1 && y > 0 && y < maxY - 1) //Stage 1, SetCostAround
        {
            GridMap.instance.grid[x + 1, y].Node.SetFinalCost(normalCost + fromLastInitialNodeCost);
            GridMap.instance.grid[x - 1, y].Node.SetFinalCost(normalCost + fromLastInitialNodeCost);
            GridMap.instance.grid[x + 1, y + 1].Node.SetFinalCost(diagonalCost + fromLastInitialNodeCost);
            GridMap.instance.grid[x - 1, y - 1].Node.SetFinalCost(diagonalCost + fromLastInitialNodeCost);
            GridMap.instance.grid[x + 1, y - 1].Node.SetFinalCost(diagonalCost + fromLastInitialNodeCost);
            GridMap.instance.grid[x - 1, y + 1].Node.SetFinalCost(diagonalCost + fromLastInitialNodeCost);
            GridMap.instance.grid[x, y + 1].Node.SetFinalCost(normalCost + fromLastInitialNodeCost);
            GridMap.instance.grid[x, y - 1].Node.SetFinalCost(normalCost + fromLastInitialNodeCost);
        }
        else if (x == 0 && y == 0)
        {
            GridMap.instance.grid[x + 1, y].Node.SetFinalCost(normalCost + fromLastInitialNodeCost);
            GridMap.instance.grid[x + 1, y + 1].Node.SetFinalCost(diagonalCost + fromLastInitialNodeCost);
            GridMap.instance.grid[x, y + 1].Node.SetFinalCost(normalCost + fromLastInitialNodeCost);
        }
        else if (x == 0 && y == maxY - 1)
        {
            GridMap.instance.grid[x + 1, y].Node.SetFinalCost(normalCost + fromLastInitialNodeCost);
            GridMap.instance.grid[x, y - 1].Node.SetFinalCost(normalCost + fromLastInitialNodeCost);
            GridMap.instance.grid[x + 1, y - 1].Node.SetFinalCost(diagonalCost + fromLastInitialNodeCost);
        }
        else if (x == maxX - 1 && y == 0)
        {
            GridMap.instance.grid[x - 1, y].Node.SetFinalCost(normalCost + fromLastInitialNodeCost);
            GridMap.instance.grid[x - 1, y + 1].Node.SetFinalCost(diagonalCost + fromLastInitialNodeCost);
            GridMap.instance.grid[x, y + 1].Node.SetFinalCost(normalCost + fromLastInitialNodeCost);
        }
        else if (x == maxX - 1 && y == maxY - 1)
        {
            GridMap.instance.grid[x - 1, y - 1].Node.SetFinalCost(diagonalCost + fromLastInitialNodeCost);
            GridMap.instance.grid[x - 1, y].Node.SetFinalCost(normalCost + fromLastInitialNodeCost);
            GridMap.instance.grid[x, y - 1].Node.SetFinalCost(normalCost + fromLastInitialNodeCost);
        }
        else if (x == 0)
        {
            GridMap.instance.grid[x + 1, y].Node.SetFinalCost(normalCost + fromLastInitialNodeCost);
            GridMap.instance.grid[x + 1, y + 1].Node.SetFinalCost(diagonalCost + fromLastInitialNodeCost);
            GridMap.instance.grid[x, y + 1].Node.SetFinalCost(normalCost + fromLastInitialNodeCost);
            GridMap.instance.grid[x, y - 1].Node.SetFinalCost(normalCost + fromLastInitialNodeCost);
            GridMap.instance.grid[x + 1, y - 1].Node.SetFinalCost(diagonalCost + fromLastInitialNodeCost);
        }
        else if (y == 0)
        {
            GridMap.instance.grid[x + 1, y].Node.SetFinalCost(normalCost + fromLastInitialNodeCost);
            GridMap.instance.grid[x - 1, y].Node.SetFinalCost(normalCost + fromLastInitialNodeCost);
            GridMap.instance.grid[x + 1, y + 1].Node.SetFinalCost(diagonalCost + fromLastInitialNodeCost);
            GridMap.instance.grid[x - 1, y + 1].Node.SetFinalCost(diagonalCost + fromLastInitialNodeCost);
            GridMap.instance.grid[x, y + 1].Node.SetFinalCost(normalCost + fromLastInitialNodeCost);
        }
        else if (x == maxX - 1)
        {
            GridMap.instance.grid[x - 1, y].Node.SetFinalCost(normalCost + fromLastInitialNodeCost);
            GridMap.instance.grid[x - 1, y - 1].Node.SetFinalCost(diagonalCost + fromLastInitialNodeCost);
            GridMap.instance.grid[x - 1, y + 1].Node.SetFinalCost(diagonalCost + fromLastInitialNodeCost);
            GridMap.instance.grid[x, y + 1].Node.SetFinalCost(normalCost + fromLastInitialNodeCost);
            GridMap.instance.grid[x, y - 1].Node.SetFinalCost(normalCost + fromLastInitialNodeCost);
        }
        else if (y == maxY - 1)
        {
            GridMap.instance.grid[x + 1, y].Node.SetFinalCost(normalCost + fromLastInitialNodeCost);
            GridMap.instance.grid[x - 1, y].Node.SetFinalCost(normalCost + fromLastInitialNodeCost);
            GridMap.instance.grid[x - 1, y - 1].Node.SetFinalCost(diagonalCost + fromLastInitialNodeCost);
            GridMap.instance.grid[x + 1, y - 1].Node.SetFinalCost(diagonalCost + fromLastInitialNodeCost);
            GridMap.instance.grid[x, y - 1].Node.SetFinalCost(normalCost + fromLastInitialNodeCost);
        }
        else
        {
            Debug.LogError("No suitable solution");
            return new Vector3Int(0, 0, int.MaxValue);    
        }*/
        #endregion
        #region Stage 2, get minimum and remove cells with 0 adjacents

        Vector3Int positionAndMinimum = new Vector3Int(0,0,int.MaxValue);

        if (x+1 >= 0 && x+1 <= maxX - 1 && y >= 0 && y <= maxY - 1 && GridMap.instance.grid[x + 1, y].Node.visited != true)
        {
            GridMap.instance.grid[x + 1, y].Node.SetFinalCost(normalCost + fromLastInitialNodeCost);
            if(GridMap.instance.grid[x + 1, y].Node.NodeFinalCost < positionAndMinimum.z)
                positionAndMinimum = new Vector3Int(x + 1, y, GridMap.instance.grid[x + 1, y].Node.NodeFinalCost);
        }

        if (x-1 >= 0 && x-1 <= maxX - 1 && y >= 0 && y <= maxY - 1 && GridMap.instance.grid[x - 1, y].Node.visited != true)
        {
            GridMap.instance.grid[x - 1, y].Node.SetFinalCost(normalCost + fromLastInitialNodeCost);
            if(GridMap.instance.grid[x - 1, y].Node.NodeFinalCost < positionAndMinimum.z)
                positionAndMinimum = new Vector3Int(x - 1, y, GridMap.instance.grid[x - 1, y].Node.NodeFinalCost);
        }

        if (x+1 >= 0 && x+1 <= maxX - 1 && y+1 >= 0 && y+1 <= maxY - 1 && GridMap.instance.grid[x + 1, y+1].Node.visited != true)
        {
            GridMap.instance.grid[x + 1, y + 1].Node.SetFinalCost(diagonalCost + fromLastInitialNodeCost);
            if(GridMap.instance.grid[x + 1, y + 1].Node.NodeFinalCost < positionAndMinimum.z)
                positionAndMinimum = new Vector3Int(x + 1, y+1, GridMap.instance.grid[x + 1, y+1].Node.NodeFinalCost);
        }

        if (x-1 >= 0 && x-1 <= maxX - 1 && y-1 >= 0 && y-1 <= maxY - 1 && GridMap.instance.grid[x -1, y-1].Node.visited != true)
        {
            GridMap.instance.grid[x - 1, y - 1].Node.SetFinalCost(diagonalCost + fromLastInitialNodeCost);
            if(GridMap.instance.grid[x - 1, y - 1].Node.NodeFinalCost < positionAndMinimum.z)
                positionAndMinimum = new Vector3Int(x - 1, y-1, GridMap.instance.grid[x -1, y-1].Node.NodeFinalCost);
        }

        if (x+1 >= 0 && x+1 <= maxX - 1 && y-1 >= 0 && y-1 <= maxY - 1 && GridMap.instance.grid[x + 1, y-1].Node.visited != true)
        {
            GridMap.instance.grid[x + 1, y - 1].Node.SetFinalCost(diagonalCost + fromLastInitialNodeCost);
            if(GridMap.instance.grid[x + 1, y - 1].Node.NodeFinalCost < positionAndMinimum.z)
                positionAndMinimum = new Vector3Int(x + 1, y-1, GridMap.instance.grid[x + 1, y-1].Node.NodeFinalCost);
        }

        if (x-1 >= 0 && x-1 <= maxX - 1 && y+1 >= 0 && y+1 <= maxY - 1 && GridMap.instance.grid[x - 1, y +1].Node.visited != true)
        {
            GridMap.instance.grid[x - 1, y + 1].Node.SetFinalCost(diagonalCost + fromLastInitialNodeCost);
            if(GridMap.instance.grid[x - 1, y + 1].Node.NodeFinalCost < positionAndMinimum.z)
                positionAndMinimum = new Vector3Int(x - 1, y + 1, GridMap.instance.grid[x - 1, y +1].Node.NodeFinalCost);
        }

        if (x >= 0 && x <= maxX - 1 && y+1 >= 0 && y+1 <= maxY - 1 && GridMap.instance.grid[x, y+1].Node.visited != true)
        {
            GridMap.instance.grid[x, y + 1].Node.SetFinalCost(normalCost + fromLastInitialNodeCost);
            if(GridMap.instance.grid[x, y + 1].Node.NodeFinalCost < positionAndMinimum.z)
                positionAndMinimum = new Vector3Int(x, y+1, GridMap.instance.grid[x, y+1].Node.NodeFinalCost);
        }

        if (x >= 0 && x <= maxX - 1 && y - 1 >= 0 && y - 1 <= maxY - 1 && GridMap.instance.grid[x, y - 1].Node.visited != true)
        {
            GridMap.instance.grid[x, y - 1].Node.SetFinalCost(normalCost + fromLastInitialNodeCost);
            if(GridMap.instance.grid[x, y - 1].Node.NodeFinalCost < positionAndMinimum.z)
                positionAndMinimum = new Vector3Int(x, y - 1, GridMap.instance.grid[x, y - 1].Node.NodeFinalCost);
        }

        #endregion

        return positionAndMinimum;
    }

    #endregion

    #region stage 3 , return final path

    private LinkedList<Vector2Int> GetFinalPath()
    {
        LinkedList<Vector2Int> path = new LinkedList<Vector2Int>();
        Vector2Int point;

        if(state == AStarAlgorithmState.FINISHED)
        {
            point = endNodePos;
            while (point != startNodePos) //this stops before get the initial point
            {
                path.AddFirst(point);
                point = GridMap.instance.grid[point.x, point.y].Node.GetParent();
            }
            return path;
        }
        else if (state == AStarAlgorithmState.LIMITED_BY_STEPS)
        {
            point = lastStepPos;
            while (point != startNodePos) //this stops before get the initial point
            {
                path.AddFirst(point);
                point = GridMap.instance.grid[point.x, point.y].Node.GetParent();
            }
            return path;
        }
        else // stay in position
        {
            path.AddFirst(startNodePos);
            return path;
        }
    }

    #endregion

    public LinkedList<Vector2Int> GetPath()
    {
        bool ended = false;
        state = AStarAlgorithmState.IN_PROCESS;
        CalculateTargetDistance();   //stage 0
        InitializeHeap();            //stage 1
        while(!ended)
        {
            ended = SearchMinimun();             //stage 2
        }

        return GetFinalPath();
    }

}

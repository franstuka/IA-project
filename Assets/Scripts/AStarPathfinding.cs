using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System.Threading;



public class AStarPathfinding { //By default this is for a quad grid

    public enum TargetDistanceAdvanceDirection { UP_RIGHT, UP_LEFT, DOWN_RIGHT, DOWN_LEFT};

    private LinkedList<Vector2Int> Heap; //position in grid, in x,y
    
    private const int normalCost = 10;
    private const int diagonalCost = 14;
    private const int enemyInSameCellCost = 6;

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
        for (int i = -1; i > - endNodePos.y -1; i--)
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
        for (int i = -1; i > - endNodePos.x -1; i--)
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
            CalculateTargetRecursion(diagonalCost, endNodePos.x-1, endNodePos.y+1, TargetDistanceAdvanceDirection.UP_RIGHT);
            CalculateTargetRecursion(diagonalCost, endNodePos.x-1, endNodePos.y-1, TargetDistanceAdvanceDirection.UP_LEFT);
            CalculateTargetRecursion(diagonalCost, endNodePos.x+1, endNodePos.y+1, TargetDistanceAdvanceDirection.DOWN_RIGHT);
            CalculateTargetRecursion(diagonalCost, endNodePos.x+1, endNodePos.y-1, TargetDistanceAdvanceDirection.DOWN_LEFT);
        }
        else if (endNodePos.x == 0 && endNodePos.y == 0)
        {
            CalculateTargetRecursion(diagonalCost, endNodePos.x+1, endNodePos.y+1, TargetDistanceAdvanceDirection.DOWN_RIGHT);
        }
        else if (endNodePos.x == 0 && endNodePos.y == maxY - 1)
        {
            CalculateTargetRecursion(diagonalCost, endNodePos.x+1, endNodePos.y-1, TargetDistanceAdvanceDirection.DOWN_LEFT);
        }
        else if (endNodePos.x == maxX - 1 && endNodePos.y == 0)
        {
            CalculateTargetRecursion(diagonalCost, endNodePos.x-1, endNodePos.y+1, TargetDistanceAdvanceDirection.UP_RIGHT);
        }
        else if (endNodePos.x == maxX - 1 && endNodePos.y == maxY - 1)
        {
            CalculateTargetRecursion(diagonalCost, endNodePos.x-1, endNodePos.y-1, TargetDistanceAdvanceDirection.UP_LEFT);
        }
        else if (endNodePos.x == 0)
        {
            CalculateTargetRecursion(diagonalCost, endNodePos.x-1, endNodePos.y+1, TargetDistanceAdvanceDirection.UP_RIGHT);
            CalculateTargetRecursion(diagonalCost, endNodePos.x+1, endNodePos.y+1, TargetDistanceAdvanceDirection.DOWN_RIGHT);
        }
        else if (endNodePos.y == 0)
        {
            CalculateTargetRecursion(diagonalCost, endNodePos.x+1, endNodePos.y+1, TargetDistanceAdvanceDirection.DOWN_RIGHT);
            CalculateTargetRecursion(diagonalCost, endNodePos.x+1, endNodePos.y-1, TargetDistanceAdvanceDirection.DOWN_LEFT);
        }
        else if (endNodePos.x == maxX - 1)
        {
            CalculateTargetRecursion(diagonalCost, endNodePos.x-1, endNodePos.y-1, TargetDistanceAdvanceDirection.UP_LEFT);
            CalculateTargetRecursion(diagonalCost, endNodePos.x+1, endNodePos.y-1, TargetDistanceAdvanceDirection.DOWN_LEFT);
        }
        else if (endNodePos.y == maxY - 1)
        {
            CalculateTargetRecursion(diagonalCost, endNodePos.x-1, endNodePos.y+1, TargetDistanceAdvanceDirection.UP_RIGHT);
            CalculateTargetRecursion(diagonalCost, endNodePos.x-1, endNodePos.y-1, TargetDistanceAdvanceDirection.UP_LEFT);
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
            
            for (int i = 0; i > - y - 1; i--)
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
            
            for (int i = 0; i < - y -1; i--)
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
            for (int i = -1; i < -x -1; i--)
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
        if(!AvaibleListPositions(x,y)) //if this is true, cell is inaccesible, so it never will be inserted in heap or visited
        {
            GridMap.instance.grid[x, y].Node.visited = true;
            return 0;
        }
        else if (x > 0 && x < maxX - 1 && y > 0 && y < maxY - 1)
        {
            if (AvaibleListPositions(x-1, y-1)) cellsAvaible++;
            if (AvaibleListPositions(x+1, y+1)) cellsAvaible++;
            if (AvaibleListPositions(x+1, y)) cellsAvaible++;
            if (AvaibleListPositions(x, y+1)) cellsAvaible++;
            if (AvaibleListPositions(x-1, y)) cellsAvaible++;
            if (AvaibleListPositions(x, y-1)) cellsAvaible++;
            if (AvaibleListPositions(x+1, y-1)) cellsAvaible++;
            if (AvaibleListPositions(x-1, y+1)) cellsAvaible++;
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
        switch(GridMap.instance.grid[x,y].CellType) //banned positions
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
        GridMap.instance.grid[startNodePos.x, startNodePos.y].Node.visited = true;
        UpdateAdjacentAvaibles(startNodePos.x, startNodePos.y);

    }

    private void SearchAndInsertOnHeap()
    {

    }

    private void UpdateAdjacentAvaibles(int x, int y)
    {
        if (x > 0 && x < maxX - 1 && y > 0 && y < maxY - 1)
        {

        }
        else if (x == 0 && y == 0)
        {

        }
        else if (x == 0 && y == maxY - 1)
        {

        }
        else if (x == maxX - 1 && y == 0)
        {

        }
        else if (x == maxX - 1 && y == maxY - 1)
        {

        }
        else if (x == 0)
        {

        }
        else if (y == 0)
        {

        }
        else if (x == maxX - 1)
        {

        }
        else if (y == maxY - 1)
        {

        }
        else
            Debug.LogError("No suitable solution");
    }

    #endregion
}

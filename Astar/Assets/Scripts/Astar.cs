using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Astar
{
    /// <summary>
    /// TODO: Implement this function so that it returns a list of Vector2Int positions which describes a path
    /// Note that you will probably need to add some helper functions
    /// from the startPos to the endPos
    /// </summary>
    /// <param name="startPos"></param>
    /// <param name="endPos"></param>
    /// <param name="grid"></param>
    /// <returns></returns>
    /// 

    public Dictionary<Vector2Int, Node> nodeDictionary = new Dictionary<Vector2Int, Node>();

    public List<Vector2Int> FindPathToTarget(Vector2Int startPos, Vector2Int endPos, Cell[,] grid)
    {
        Node startNode = new Node(startPos, null, 0, Mathf.RoundToInt(Vector2Int.Distance(endPos, startPos)));
        Node targetNode = new Node(endPos, null,int.MaxValue, 0);

        List<Node> openList = new List<Node>();
        HashSet<Node> closedList = new HashSet<Node>();

        openList.Add(startNode);

        while(openList.Count > 0)
        {
            Node currentNode = openList.OrderBy((x) => x.FScore).First();

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            if(currentNode.position == endPos)
            {
                List<Vector2Int> FinalPath = new List<Vector2Int>();

                while(currentNode != startNode)
                {
                    FinalPath.Add(currentNode.position);
                    currentNode = currentNode.parent;
                }

                FinalPath.Add(startNode.position);
                FinalPath.Reverse();
                return FinalPath;
            }

            List<Node> neighbours = GetNeighbours(currentNode, grid.GetLength(0), grid.GetLength(1));

            foreach(Node neighbourNode in neighbours)
            {
                Cell currentCell = grid[currentNode.position.x, currentNode.position.y];
                Cell neighbourCell = grid[neighbourNode.position.x , neighbourNode.position.y];

                if (closedList.Contains(neighbourNode))
                {
                    continue;
                }

                float newGScore = currentNode.GScore + GetDistance(currentNode, neighbourNode);

                if(newGScore < neighbourNode.GScore || !openList.Contains(neighbourNode))
                {
                    neighbourNode.GScore = newGScore;
                    neighbourNode.HScore = Vector2.Distance(neighbourNode.position, endPos);
                    neighbourNode.parent = currentNode;

                    if (!openList.Contains(neighbourNode) && IsTraversable(currentCell, neighbourCell))
                    {
                        openList.Add(neighbourNode);
                    }
                }
            }
        }

       
        return null; 
    }

    List<Node> GetNeighbours(Node node, int width, int height)
    {
        List<Node> result = new List<Node>();
        for (int x = -1; x < 2; x++)
        {
            for (int y = -1; y < 2; y++)
            {
                int nodeX = node.position.x + x;
                int nodeY = node.position.y + y;
                if (nodeX < 0 || nodeX >= width || nodeY < 0 || nodeY >= height || Mathf.Abs(x) == Mathf.Abs(y))
                {
                    continue;
                }

                Vector2Int neigboursNodePosition = new Vector2Int(nodeX, nodeY);
                if (nodeDictionary.ContainsKey(neigboursNodePosition))
                {
                    result.Add(nodeDictionary[neigboursNodePosition]);
                }
                else
                {
                    Node neighbour = new Node(neigboursNodePosition, node, Mathf.RoundToInt(node.GScore + Vector2Int.Distance(node.position, neigboursNodePosition)), 0);
                    nodeDictionary.Add(neighbour.position, neighbour);
                    result.Add(neighbour);
                }
            }
        }

        return result;
    }


    bool IsTraversable(Cell startCell, Cell neighbourCell)
    {
        if (neighbourCell.gridPosition.x > startCell.gridPosition.x && neighbourCell.HasWall(Wall.LEFT))
        {
            return false;
        }
        else if (neighbourCell.gridPosition.x < startCell.gridPosition.x && neighbourCell.HasWall(Wall.RIGHT))
        {
            return false;
        }
        else if (neighbourCell.gridPosition.y < startCell.gridPosition.y && neighbourCell.HasWall(Wall.UP))
        {
            return false;
        }
        else if (neighbourCell.gridPosition.y > startCell.gridPosition.y && neighbourCell.HasWall(Wall.DOWN))
        {
            return false;
        }
        return true;
    }

    int GetDistance(Node nodeA, Node nodeB)
    {
        int distanceX = Mathf.Abs(nodeA.position.x - nodeB.position.x);
        int distanceY = Mathf.Abs(nodeA.position.y - nodeB.position.y);

        if(distanceX > distanceY)
        {
            int distance = Mathf.Abs(distanceX - distanceY);
            return 14 * distanceY + 10 *(distanceX-distanceY);
        }
        else
        {
            return 14 * distanceX + 10 * (distanceY - distanceX);
        }



    }


    /// <summary>
    /// This is the Node class you can use this class to store calculated FScores for the cells of the grid, you can leave this as it is
    /// </summary>
    public class Node
    {
        public Vector2Int position; //Position on the grid
        public Node parent; //Parent Node of this node

        public float FScore { //GScore + HScore
            get { return GScore + HScore; }
        }
        public float GScore; //Current Travelled Distance
        public float HScore; //Distance estimated based on Heuristic

        public Node() { }
        public Node(Vector2Int position, Node parent, int GScore, int HScore)
        {
            this.position = position;
            this.parent = parent;
            this.GScore = GScore;
            this.HScore = HScore;
        }
    }
}

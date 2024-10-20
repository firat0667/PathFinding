﻿using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public bool DisplayGridGizmos;
    public LayerMask unWalkableMask;
    public Vector2 gridWorldSize;
    public float nodeRadius;
    public int obstacleProximityPenalty = 10;
    public TerrainType[] walkableRegions;
    private LayerMask _walkableMask;
    Dictionary<int, int> walkableRegionsDictionary = new Dictionary<int, int>();
    Node[,] grid;



    float nodeDiameter;
    int gridSizeX, gridSizeY;


    int penaltymin= int.MaxValue;
    int penaltyMax = int.MinValue;

    private void Awake()
    {
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        foreach (TerrainType region in walkableRegions)
        {
            _walkableMask.value |=  region.TerrainLayerMask.value;
            walkableRegionsDictionary.Add((int)Mathf.Log(region.TerrainLayerMask.value, 2f), region.terraniPenalty);
        }
        CreateGrid();
    }

    public int MaxSize
    {
        get
        {
            return gridSizeX * gridSizeY;
        }
    }

    void CreateGrid()
    {

        grid = new Node[gridSizeX, gridSizeY];
        Vector3 worldButtomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPoint = worldButtomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
                bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius, unWalkableMask));
                int movementPenalty = 0;
              
                    Ray ray = new Ray(worldPoint + Vector3.up * 50, Vector3.down);
                    RaycastHit hit;
                    if(Physics.Raycast(ray,out hit, 100, _walkableMask))
                    {
                        walkableRegionsDictionary.TryGetValue(hit.collider.gameObject.layer, out movementPenalty);
                    }
                if (!walkable)
                {
                    movementPenalty += obstacleProximityPenalty;
                }

                grid[x, y] = new Node(walkable, worldPoint, x, y, movementPenalty);
            }
        }
        BlurPenaltyMap(3);
    }
    void BlurPenaltyMap(int blurSize)
    {
        int kernelSize = blurSize * 2 + 1;
        int kernelExtents = (kernelSize - 1) / 2;

        int[,] penaltiesHorizontalPass = new int[gridSizeX, gridSizeY];
        int[,] penaltiesVerticalPass = new int[gridSizeX, gridSizeY];

        // Yatay geçiş için dış döngü y üzerinden yapılıyor
        for (int y = 0; y < gridSizeY; y++)
        {
            // İlk satırda kernel genişliği kadar bir pencere hesaplanıyor
            for (int x = -kernelExtents; x <= kernelExtents; x++)
            {
                int sampleX = Mathf.Clamp(x, 0, gridSizeX - 1);
                penaltiesHorizontalPass[0, y] += grid[sampleX, y].MovementPenalty;
            }

            // Diğer satırlarda pencere kaydırılarak hesaplamalar yapılıyor
            for (int x = 1; x < gridSizeX; x++)
            {
                int removeIndex = Mathf.Clamp(x - kernelExtents - 1, 0, gridSizeX - 1);
                int addIndex = Mathf.Clamp(x + kernelExtents, 0, gridSizeX - 1);
                penaltiesHorizontalPass[x, y] = penaltiesHorizontalPass[x - 1, y]
                    - grid[removeIndex, y].MovementPenalty
                    + grid[addIndex, y].MovementPenalty;
            }
        }

        // Dikey geçiş için dış döngü x üzerinden yapılıyor
        for (int x = 0; x < gridSizeX; x++)
        {
            // İlk sütun için kernel genişliği kadar pencere hesaplanıyor
            for (int y = -kernelExtents; y <= kernelExtents; y++)
            {
                int sampleY = Mathf.Clamp(y, 0, gridSizeY - 1);
                penaltiesVerticalPass[x, 0] += penaltiesHorizontalPass[x, sampleY];
            }
            int blurredPenalty = Mathf.RoundToInt((float)penaltiesVerticalPass[x, 0] / (kernelSize * kernelSize));
            grid[x, 0].MovementPenalty = blurredPenalty;
            // Diğer sütunlarda pencere kaydırılarak hesaplamalar yapılıyor
            for (int y = 1; y < gridSizeY; y++)
            {
                int removeIndex = Mathf.Clamp(y - kernelExtents - 1, 0, gridSizeY - 1);
                int addIndex = Mathf.Clamp(y + kernelExtents, 0, gridSizeY - 1);
                penaltiesVerticalPass[x, y] = penaltiesVerticalPass[x, y - 1]
                    - penaltiesHorizontalPass[x, removeIndex]
                    + penaltiesHorizontalPass[x, addIndex];

                 blurredPenalty =Mathf.RoundToInt((float)penaltiesVerticalPass[x, y] / (kernelSize * kernelSize));
                grid[x, y].MovementPenalty = blurredPenalty;
                if (blurredPenalty > penaltyMax)
                {
                    penaltyMax = blurredPenalty;
                }
                if(blurredPenalty < penaltymin)
                {
                    penaltymin = blurredPenalty;
                }
            }
        }
    }
    public List<Node> Getneighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                    continue;

                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                {
                    neighbours.Add(grid[checkX, checkY]);
                }
            }
        }
        return neighbours;
    }
    public Node NodeFromWorldPoint(Vector3 worldPosition)
    {
        float percentX = (worldPosition.x + gridSizeX / 2) / gridWorldSize.x;

        float percentY = (worldPosition.z + gridSizeY / 2) / gridWorldSize.y;

        percentX = Mathf.Clamp01(percentX);

        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);

        return grid[x, y];
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));

                if (grid != null && DisplayGridGizmos)
                {
                    foreach (Node n in grid)
                    {
                      Gizmos.color = Color.Lerp(Color.white, Color.black, Mathf.InverseLerp(penaltymin, penaltyMax, n.MovementPenalty));
                        Gizmos.color = (n.Walkable) ? Gizmos.color : Color.red;
                        Gizmos.DrawCube(n.WorldPosition, Vector3.one * (nodeDiameter - 0.1f));
                    }
                }
    }
 
}
[System.Serializable]
public class TerrainType
{
    public LayerMask TerrainLayerMask;
    public int terraniPenalty;
}
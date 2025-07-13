using UnityEngine;

[RequireComponent(typeof(Terrain))]
public class GridOnTerrainManager : MonoBehaviour
{
    [Header("Grid Settings")]
    public int columns = 18;
    public int rows = 32;
    public GameObject cellPrefab;
    public Color color1 = Color.white;
    public Color color2 = Color.gray;

    [Header("King Tower")]
    public GameObject kingTowerPrefab;

    [Header("Enemy Tower")]
    public GameObject enemyTowerPrefab;

    private Terrain terrain;
    private float cellWorldSize;
    private MaterialPropertyBlock propBlock;
    private GameObject[,] cellGrid;

    public static GridOnTerrainManager Instance;

    void Start()
    {
        terrain = GetComponent<Terrain>();
        if (terrain == null)
        {
            Debug.LogError("Этот скрипт должен быть на объекте с Terrain!");
            return;
        }

        if (cellPrefab == null)
        {
            Debug.LogError("Не указан prefab ячейки!");
            return;
        }

        propBlock = new MaterialPropertyBlock();
        cellGrid = new GameObject[columns, rows];

        GenerateGrid();

        // Спавн башен
        SpawnKingTower(7, 1, 10, 4);
        SpawnEnemyTower(7, 1, 10, 4);
    }

    void Awake()
    {
        Instance = this;
    }

    void GenerateGrid()
    {
        Vector3 terrainSize = terrain.terrainData.size;
        float cellWidth = terrainSize.x / columns;
        float cellDepth = terrainSize.z / rows;
        cellWorldSize = Mathf.Min(cellWidth, cellDepth);

        Vector3 origin = terrain.transform.position + new Vector3(
            (terrainSize.x - cellWorldSize * columns) / 2f,
            0,
            (terrainSize.z - cellWorldSize * rows) / 2f
        );

        for (int z = 0; z < rows; z++)
        {
            for (int x = 0; x < columns; x++)
            {
                float posX = origin.x + cellWorldSize * (x + 0.5f);
                float posZ = origin.z + cellWorldSize * (z + 0.5f);
                float posY = terrain.SampleHeight(new Vector3(posX, 0, posZ)) + terrain.transform.position.y;

                Vector3 pos = new Vector3(posX, posY + 0.05f, posZ);
                GameObject cell = Instantiate(cellPrefab, pos, Quaternion.identity, transform);
                cell.name = $"Cell_{x}_{z}";

                float prefabSize = GetPrefabPhysicalSize(cellPrefab);
                float scaleFactor = cellWorldSize / prefabSize;
                cell.transform.localScale = new Vector3(scaleFactor, 1f, scaleFactor);

                Renderer renderer = cell.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.GetPropertyBlock(propBlock);
                    bool isAltColor = (x + z) % 2 == 0;
                    propBlock.SetColor("_BaseColor", isAltColor ? color1 : color2);
                    renderer.SetPropertyBlock(propBlock);
                }

                cellGrid[x, z] = cell;
            }
        }
    }

    float GetPrefabPhysicalSize(GameObject prefab)
    {
        MeshRenderer mr = prefab.GetComponent<MeshRenderer>();
        if (mr != null)
        {
            Vector3 size = mr.bounds.size;
            return size.x != 0 ? size.x : 1f;
        }
        return 1f;
    }

    public void SpawnKingTower(int xMin, int zMin, int xMax, int zMax)
    {
        if (kingTowerPrefab == null)
        {
            Debug.LogError("KingTowerPrefab не назначен!");
            return;
        }

        if (!IsInBounds(xMin, zMin) || !IsInBounds(xMax, zMax))
        {
            Debug.LogError("Координаты башни вне пределов сетки!");
            return;
        }

        Vector3 bottomLeft = cellGrid[xMin, zMin].transform.position;
        Vector3 topRight = cellGrid[xMax, zMax].transform.position;
        Vector3 center = (bottomLeft + topRight) / 2f;

        GameObject tower = Instantiate(kingTowerPrefab, center, Quaternion.identity);
        tower.name = "KingTower";

        Vector3 scale = tower.transform.localScale;
        //float towerWidth = (xMax - xMin + 1) * cellWorldSize;
        //float towerDepth = (zMax - zMin + 1) * cellWorldSize;

        //tower.transform.localScale = new Vector3(towerWidth, scale.y, towerDepth);
    }

    public void SpawnEnemyTower(int xMin, int zMin, int xMax, int zMax)
    {
        if (enemyTowerPrefab == null)
        {
            Debug.LogError("EnemyTowerPrefab не назначен!");
            return;
        }

        int zMinMirror = rows - 1 - zMax;
        int zMaxMirror = rows - 1 - zMin;

        if (!IsInBounds(xMin, zMinMirror) || !IsInBounds(xMax, zMaxMirror))
        {
            Debug.LogError("Координаты вражеской башни вне пределов сетки!");
            return;
        }

        Vector3 bottomLeft = cellGrid[xMin, zMinMirror].transform.position;
        Vector3 topRight = cellGrid[xMax, zMaxMirror].transform.position;
        Vector3 center = (bottomLeft + topRight) / 2f;

        GameObject tower = Instantiate(enemyTowerPrefab, center, Quaternion.identity);
        tower.name = "EnemyTower";

        Vector3 scale = tower.transform.localScale;
        tower.transform.Rotate(0f, 180f, 0f);
        //float towerWidth = (xMax - xMin + 1) * cellWorldSize;
        //float towerDepth = (zMax - zMin + 1) * cellWorldSize;

        //tower.transform.localScale = new Vector3(towerWidth, scale.y, towerDepth);
    }

    bool IsInBounds(int x, int z)
    {
        return x >= 0 && x < columns && z >= 0 && z < rows;
    }

    public GameObject GetCell(int x, int z)
    {
        if (x >= 0 && x < columns && z >= 0 && z < rows)
            return cellGrid[x, z];
        return null;
    }

    public Vector3 GetWorldPosition(int x, int z)
    {
        GameObject cell = GetCell(x, z);
        return cell != null ? cell.transform.position : Vector3.zero;
    }

    public Vector2Int GetGridCoordinates(Vector3 worldPos)
    {
        Vector3 terrainPos = worldPos - terrain.transform.position;

        float terrainWidth = terrain.terrainData.size.x;
        float terrainDepth = terrain.terrainData.size.z;

        float usableWidth = columns * cellWorldSize;
        float usableDepth = rows * cellWorldSize;

        float startX = (terrainWidth - usableWidth) / 2f;
        float startZ = (terrainDepth - usableDepth) / 2f;

        int x = Mathf.FloorToInt((terrainPos.x - startX) / cellWorldSize);
        int z = Mathf.FloorToInt((terrainPos.z - startZ) / cellWorldSize);

        return new Vector2Int(x, z);
    }

    public float GetCellWorldSize()
    {
        return cellWorldSize;
    }
}

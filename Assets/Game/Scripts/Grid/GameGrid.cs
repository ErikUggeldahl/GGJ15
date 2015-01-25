using UnityEngine;
using System.Collections;

public struct GridPosition
{
    public int X;
    public int Y;

    public GridPosition(int aX, int aY)
    {
        X = aX;
        Y = aY;
    }

    public static GridPosition operator +(GridPosition x, GridPosition y)
    {
        return new GridPosition(x.Y + y.Y, x.X + y.X);
    }
}

public class GameGrid : MonoBehaviour 
{
    static GameGrid instance;
    public static GameGrid Instance
    {
        get
        {
            return instance;
        }
    }

    public float clearingRadius = 20.0f;

    public float gridScale = 1;

    public int gridSizeX = 100;
    public int gridSizeY = 100;
	private Vector3 gridWorldCenter;

    GridSquare[,] gridSquares;

    public Mesh gridQuad;
    public Material gridMaterial;

    public Tree TreePrefab;
    public int TreeClusters = 20;
    public int MaxTreeClusterCount = 20;
    public int MinTreeClusterCount = 10;
    public float TreeClusterRadius = 10.0f;
	public int NewTreeSpawnRadius = 15;
	public int VotesPerTree = 10;
	private Transform treeParent;

    public Rock RockPrefab;
    public int RockCount = 5;

    void Awake()
    {
        instance = this;
        CreateGrid();
        CreateGridCollider();
        PopulateResources();
        Shader.SetGlobalFloat("_ClearingRadius", clearingRadius);
    }

	void Start()
	{
		NetPoller.GetInstance().Polled += OnPollResult;
	}

	void OnPollResult(int good, int bad)
	{
		int halfGridX = gridSizeX / 2;
		int halfGridY = gridSizeY / 2;

		int treesToSpawn = good / VotesPerTree;

		for (int i = 0; i < treesToSpawn; i++)
			PopulateTreeCluster(halfGridX - NewTreeSpawnRadius, halfGridX + NewTreeSpawnRadius, halfGridY - NewTreeSpawnRadius, halfGridY + NewTreeSpawnRadius, 1, 1, true);
	}

    public GridSquare GetGridSquare(GridPosition aPosition)
    {
        if (aPosition.X < gridSizeX && aPosition.Y < gridSizeY && aPosition.X >= 0 && aPosition.Y >= 0)
            return gridSquares[aPosition.X, aPosition.Y];
        else
            return null;
    }

    void CreateGrid()
    {
        var groundLayer = LayerMask.NameToLayer("Ground");
        GridSquare.GroundLayer = groundLayer;

        gridSquares = new GridSquare[gridSizeX,gridSizeY];

        for (int y = 0; y < gridSizeY; y++)
        {
            for (int x = 0; x < gridSizeX; x++)
            {
                GameObject newSquareObj = new GameObject("GridSquare x" + x + " y" + y );
                newSquareObj.transform.SetParent(this.transform);

                GridSquare newSquare = newSquareObj.AddComponent<GridSquare>();
                newSquare.Init(x, y);
                gridSquares[x, y] = newSquare;
            }
        }

		gridWorldCenter = GridToWorldSpace(new GridPosition(gridSizeX / 2, gridSizeY / 2));
    }

    void CreateGridCollider()
    {
        GameObject gridCollider = new GameObject("Grid collider");
        var groundLayer = LayerMask.NameToLayer("Ground");
        gridCollider.layer = groundLayer;
        gridCollider.transform.SetParent(this.transform);

        var boxCollider = gridCollider.AddComponent<BoxCollider>();
        boxCollider.size = new Vector3(1000f, 1f, 1000f);
        boxCollider.center = new Vector3(-0.5f, -0.5f, -0.5f);
    }

    void PopulateResources()
    {
        PopulateRocks();
        PopulateTrees();
    }

    bool IsInClearing(GridPosition aPosition)
    {
        Vector3 worldSpace = GridToWorldSpace(aPosition);
        float distanceToCenter = (worldSpace - gridWorldCenter).magnitude;
        return distanceToCenter < clearingRadius;
    }

    void PopulateRocks()
    {
        var rockParent = new GameObject("Rock parent").transform;

        for (int i = 0; i < RockCount; i++)
        {
            GridPosition newPosition;
            do
            {
                newPosition = new GridPosition(Random.Range(0, gridSizeX), Random.Range(0, gridSizeY));
            }
            while (IsInClearing(newPosition) || GetGridSquare(newPosition) == null || GetGridSquare(newPosition).ResidingObject != null);

            var rockGO = (Instantiate(RockPrefab, GridToWorldSpace(newPosition), Quaternion.identity) as Rock).gameObject;
            rockGO.transform.parent = rockParent;
            GetGridSquare(newPosition).ResidingObject = rockGO;
        }
    }

    void PopulateTrees()
    {
        treeParent = new GameObject("Tree parent").transform;

		for (int i = 0; i < TreeClusters; i++)
			PopulateTreeCluster(0, gridSizeX, 0, gridSizeY, MinTreeClusterCount, MaxTreeClusterCount);
    }

	void PopulateTreeCluster(int minX, int maxX, int minY, int maxY, int minTreeCount, int maxTreeCount, bool isAnimated = false)
	{
		GridPosition clusterPosition;
		do
		{
			clusterPosition = new GridPosition(Random.Range(minX, maxX), Random.Range(minY, maxY));
		}
		while (IsInClearing(clusterPosition) || GetGridSquare(clusterPosition).ResidingObject != null);

		int clusterSize = Random.Range(minTreeCount, maxTreeCount);

		for (int c = 0; c < clusterSize; c++)
		{
			var spawnPosition = FindSpawnPosition(clusterPosition);
			if (spawnPosition == null)
				continue;

			var tree = CreateTree(spawnPosition.Value);
			if (isAnimated)
				AnimateTreeGrowth(tree);
		}
	}

	GridPosition? FindSpawnPosition(GridPosition clusterPosition)
	{
		GridPosition spawnPosition;
		int halfRadius = (int)TreeClusterRadius / 2;

		// Just in case, have an escape plan.
		int maxLoops = 200;
		int loops = 0;
		do
		{
			loops++;
			spawnPosition = new GridPosition(Random.Range(-halfRadius, halfRadius), Random.Range(-halfRadius, halfRadius));
			spawnPosition += clusterPosition;
		}
		while ((IsInClearing(spawnPosition) || GetGridSquare(spawnPosition) == null || GetGridSquare(spawnPosition).ResidingObject != null) && loops < maxLoops);

		if (loops >= maxLoops)
			return null;

		return spawnPosition;
	}

	Tree CreateTree(GridPosition spawnPosition)
	{
		Tree newTree = Instantiate(TreePrefab, GridToWorldSpace(spawnPosition), Quaternion.identity) as Tree;
		newTree.transform.parent = treeParent;
		GetGridSquare(spawnPosition).ResidingObject = newTree.gameObject;
		return newTree;
	}

	void AnimateTreeGrowth(Tree tree)
	{
		var growth = tree.gameObject.AddComponent<GrowthAnimation>();
		growth.StartGrowth(2f);
	}

    public Vector3 GridToWorldSpace(GridPosition aPosition)
    {
        return new Vector3(aPosition.X - (gridSizeX / 2), 0, aPosition.Y - (gridSizeY / 2));
    }

    public GridPosition WorldToGridPosition(Vector3 aPosition)
    {
        return new GridPosition(Mathf.RoundToInt(aPosition.x) + (gridSizeX/2), Mathf.RoundToInt(aPosition.z) + (gridSizeY/2));
    }
}

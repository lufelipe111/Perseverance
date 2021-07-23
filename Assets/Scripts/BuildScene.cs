using System.Collections.Generic;
using UnityEngine;

public class BuildScene : MonoBehaviour
{
    public GameObject rover, prefab, terrain;
    public int numRocks = 50;
    public float radius = 1;
    public Vector3 regionSize;
    public int rejectionSamples = 30;

    private float rockSize;
    private Vector3 offset;
    
        List<Vector3> points;

    // Start is called before the first frame update
    void Start()
    {
        rockSize = prefab.transform.localScale.y;

        offset = new Vector3(-125, rockSize / 2, -125);
        //points = PoissonDiscSampling.GeneratePoints(radius, regionSize, numRocks, rejectionSamples);

        points = GeneratePoints();

        InstantiateRocks();
    }

    List<Vector3> GeneratePoints()
    {
        List<Vector3> points = new List<Vector3>();

        Terrain terrain = GameObject.Find("Terrain").GetComponent<Terrain>();
        Vector3 terrainSize = terrain.terrainData.size;

        for (int i = 0; i < numRocks; i++)
        {
            int x = (int)Random.Range(-terrainSize.x / 2, terrainSize.x / 2);
            int z = (int)Random.Range(-terrainSize.z / 2, terrainSize.z / 2);

            Vector3 point = new Vector3(x, rockSize / 2, z);

            points.Add(point);
        }

        return points;
    }

    void InstantiateRocks()
    {
        int n = 0;
        foreach(Vector3 point in points)
        {
            GameObject obj = Instantiate<GameObject>(prefab, point, Quaternion.identity, transform);
            obj.name = "Rock" + n;
            n++;
        }
    }
}

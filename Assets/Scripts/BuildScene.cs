using System.Collections.Generic;
using UnityEngine;

public class BuildScene : MonoBehaviour
{
    public GameObject rover, rockPrefab, lifePrefab, terrain;
    public int numRocks = 50;
    public int numLifeUps = 20;
    public float radius = 1;
    public Vector3 regionSize;
    public int rejectionSamples = 30;
    private List<Vector3> points;

    // Start is called before the first frame update
    void Start()
    {
        InstantiateRocks();
        InstantiateLifes();
    }

    List<Vector3> GeneratePoints(Vector3 area, GameObject prefab, int n)
    {
        points = new List<Vector3>();

        //int i = 0;
        //while(i < n)
        for(int i = 0; i < n; i++)
        {
            int x = (int) Random.Range(-area.x / 2, area.x / 2);
            int z = (int) Random.Range(-area.z / 2, area.z / 2);

            Vector3 point = new Vector3(x, prefab.transform.position.y, z);

            points.Add(point);
        }

        return points;
    }

    void InstantiateRocks()
    {
        int n = 0;

        Terrain terrainObject = terrain.GetComponent<Terrain>();
        var scl = 0.60f;
        Vector3 terrainSize = Vector3.Scale(terrainObject.terrainData.size, new Vector3(scl, scl, scl));

        var rockPoints = GeneratePoints(terrainSize, rockPrefab, numRocks);

        foreach (Vector3 point in rockPoints)
        {
            GameObject obj = Instantiate<GameObject>(rockPrefab, point, Quaternion.identity, transform);
            obj.name = "Rock" + n;
            n++;
        }
    }

    void InstantiateLifes()
    {
        int n = 0;

        Terrain terrainObject = terrain.GetComponent<Terrain>();
        var scl = 0.60f;
        Vector3 terrainSize = Vector3.Scale(terrainObject.terrainData.size, new Vector3(scl, scl, scl));

        var lifePoints = GeneratePoints(terrainSize, lifePrefab, numLifeUps);

        foreach (Vector3 point in lifePoints)
        {
            GameObject obj = Instantiate<GameObject>(lifePrefab, point, Quaternion.identity, transform);
            obj.name = "Life";
            n++;
        }
    }
}

using System.Collections.Generic;
using UnityEngine;

public class BuildScene : MonoBehaviour
{
    public GameObject rover, prefab, terrain;
    public int numRocks = 50;
    public float radius = 1;
    public Vector3 regionSize;
    public int rejectionSamples = 30;

    // Start is called before the first frame update
    void Start()
    {
        InstantiateRocks();
    }

    List<Vector3> GeneratePoints(Vector3 area, GameObject prefab)
    {
        List<Vector3> points = new List<Vector3>();

        for (int i = 0; i < numRocks; i++)
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
        Vector3 terrainSize = terrainObject.terrainData.size;

        var points = GeneratePoints(terrainSize, prefab);

        foreach (Vector3 point in points)
        {
            GameObject obj = Instantiate<GameObject>(prefab, point, Quaternion.identity, transform);
            obj.name = "Rock" + n;
            n++;
        }
    }
}

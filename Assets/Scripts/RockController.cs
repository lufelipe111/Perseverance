using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RockController : MonoBehaviour
{
    public List<ResourcesModel> resources = new List<ResourcesModel>();
    // Start is called before the first frame update
    void Start()
    {
        int quantity = Mathf.FloorToInt(Random.Range(1, 4));

        for (int i= 0; i < quantity; i++)
        {
            var rsrc = new ResourcesModel()
            {
                idResource = Mathf.FloorToInt(Random.Range(0, 3))
            };

            resources.Add(rsrc);
        }
    }
}

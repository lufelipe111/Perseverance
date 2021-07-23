using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrecisionTaskController : MonoBehaviour
{
    public GameObject rover;
    Vector3 offset;
    // Start is called before the first frame update
    void Start()
    {
        offset = transform.position - rover.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = rover.transform.position + offset;
    }
}

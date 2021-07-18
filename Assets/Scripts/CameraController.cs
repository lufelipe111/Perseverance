using UnityEngine;

public class CameraController : MonoBehaviour
{
    Vector3 offset;
    public GameObject rover;

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

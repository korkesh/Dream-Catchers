using UnityEngine;
using System.Collections;

public class RotateCollectible : MonoBehaviour
{
    public Vector3 upDir;
    public float rotationSpeed;
    void Update()
    {
        transform.Rotate(upDir.z * rotationSpeed, upDir.x * rotationSpeed, upDir.y * rotationSpeed);
    }


}

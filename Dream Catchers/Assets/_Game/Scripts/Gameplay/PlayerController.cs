using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    //===================================
    // Fields
    //===================================

    public Vector3 pOrigin; // center of collider

    public Vector3 camForward; // forward vector relative to camera

    public float hAxis;
    public float vAxis;

    public CapsuleCollider pCollider;
   

	// Use this for initialization
	void Start ()
    {
        pCollider = GetComponent<CapsuleCollider>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        CheckInput();

        camForward = Camera.main.transform.forward;
        camForward.y = 0;
        camForward.Normalize();
	}


    public void CheckInput()
    {
        hAxis = Input.GetAxis("Horizontal");
        vAxis = Input.GetAxis("Vertical");
    }


    //==================================
    // Collisions
    //==================================
    // TODO: move to a physics workspace

    public void AABB_AABB_Intersect()
    {
        GameObject[] floors = GameObject.FindGameObjectsWithTag("floor");
        BoxCollider playerCollider = GetComponent<BoxCollider>();

        foreach (GameObject floor in floors)
        {
            BoxCollider collider = floor.GetComponent<BoxCollider>();

            Vector3 oOrigin = collider.transform.position + collider.center;

            // AABB / AABB collision condition
            if ((pOrigin.x - playerCollider.bounds.extents.x) <= (oOrigin.x + collider.bounds.extents.x) &&
                (pOrigin.x + playerCollider.bounds.extents.x) >= (oOrigin.x - collider.bounds.extents.x) &&
                (pOrigin.y - playerCollider.bounds.extents.y) <= (oOrigin.y + collider.bounds.extents.y) &&
                (pOrigin.y + playerCollider.bounds.extents.y) >= (oOrigin.y - collider.bounds.extents.y) &&
                (pOrigin.z - playerCollider.bounds.extents.z) <= (oOrigin.z + collider.bounds.extents.z) &&
                (pOrigin.z + playerCollider.bounds.extents.z) >= (oOrigin.z - collider.bounds.extents.z))
            {
                Collide(collider);
            }
        }
    }


    public bool Point_Sphere_Intersect(Vector3 point, SphereCollider sphere)
    {
        float distance = Mathf.Sqrt((point.x - sphere.center.x) * (point.x - sphere.center.x) +
                                    (point.y - sphere.center.y) * (point.y - sphere.center.y) +
                                    (point.z - sphere.center.z) * (point.z - sphere.center.z));

        return distance < sphere.radius;
    }


    // SPHERE/SPHERE COLLISION:
    public bool Sphere_Sphere_Intersect(SphereCollider sphere, SphereCollider other)
    {
        float distance = Mathf.Sqrt((sphere.center.x - other.center.x) * (sphere.center.x + other.center.x) +
                                    (sphere.center.y - other.center.y) * (sphere.center.y - other.center.y) +
                                    (sphere.center.z - other.center.z) * (sphere.center.z - other.center.z));

        return distance < (sphere.radius + other.radius);
    }


    // SPHERE / AABB COLLISION:
    public bool Sphere_AABB_Intersect(SphereCollider sphere, BoxCollider box)
    {
        // get point nearest to sphere
        float x = Mathf.Max(box.center.x - box.bounds.extents.x, Mathf.Min(sphere.center.x, box.center.x + box.bounds.extents.x));
        float y = Mathf.Max(box.center.y - box.bounds.extents.y, Mathf.Min(sphere.center.y, box.center.y + box.bounds.extents.y));
        float z = Mathf.Max(box.center.z - box.bounds.extents.z, Mathf.Min(sphere.center.z, box.center.z + box.bounds.extents.z));

        // now have a point and a sphere, call PointSphereIntersect
        return Point_Sphere_Intersect(new Vector3(x, y, z), sphere);
    }


    // TODO: FIX
    // CAPSULE / AABB COLLISION:
    public bool Capsule_AABB_Intersect(CapsuleCollider capsule, BoxCollider box)
    {
        // get point on AABB nearest to capsule
        float x = Mathf.Max(box.center.x - box.bounds.extents.x, Mathf.Min(capsule.center.x, box.center.x + box.bounds.extents.x));
        float y = Mathf.Max(box.center.y - box.bounds.extents.y, Mathf.Min(capsule.center.y, box.center.y + box.bounds.extents.y));
        float z = Mathf.Max(box.center.z - box.bounds.extents.z, Mathf.Min(capsule.center.z, box.center.z + box.bounds.extents.z));

        //return Point_Capsule_Intersect(new Vector3(x, y, z), capsule);
        return false;
    }


    public void Collide(Collider other)
    {
        
    }
}

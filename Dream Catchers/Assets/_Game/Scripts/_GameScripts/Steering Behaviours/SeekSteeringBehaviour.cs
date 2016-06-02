using UnityEngine;
using System.Collections;

public class SeekSteeringBehaviour : SteeringBehaviourBase
{
    public float SlowDownDistance = 3.0f;
    public int Deceleration = 2;
    public float StopDistance = 0.01f;

    public override Vector3 calculateForce()
    {
        Vector3 toTarget = (steeringComponent.Target - gameObject.transform.position).normalized;

        Vector3 DesiredVelocity = toTarget * steeringComponent.MaxSpeed;

        return DesiredVelocity - steeringComponent.Velocity;
    }
}

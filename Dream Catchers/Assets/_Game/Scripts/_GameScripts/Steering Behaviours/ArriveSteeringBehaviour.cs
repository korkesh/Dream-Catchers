using UnityEngine;
using System.Collections;

public class ArriveSteeringBehaviour : SteeringBehaviourBase
{
    
	float SlowDownDistance = 3.0f;
	public int Deceleration = 2;
	public float StopDistance = 0.01f;

	public override Vector3 calculateForce()
	{
		Vector3 toTarget = steeringComponent.Target - gameObject.transform.position;
		float distToTarget = toTarget.magnitude;

		//steeringComponent.ReachedGoal = false;
		if (distToTarget > SlowDownDistance)
		{
			return calculateSeekForce();
		}
		else if (distToTarget <= SlowDownDistance && distToTarget > StopDistance)
		{
			toTarget.Normalize();

			//because Deceleration is enumerated as an int, this value is required
			//to provide fine tweaking of the deceleration.
			const float DecelerationTweaker = 1.25f;

			//calculate the speed required to reach the target given the desired
			//deceleration
			float speed = distToTarget / ((float)Deceleration * DecelerationTweaker);

			//make sure the velocity does not exceed the max
			speed = (((speed) < (steeringComponent.MaxSpeed)) ? (speed) : (steeringComponent.MaxSpeed));

			//from here proceed just like Seek except we don't need to normalize
			//the ToTarget vector because we have already gone to the trouble
			//of calculating its length: dist.
			float _value = (speed / distToTarget);
			Vector3 DesiredVelocity = toTarget * _value;
			return (DesiredVelocity - steeringComponent.Velocity);
		}
		else
		{
			//steeringComponent.ReachedGoal = true;
		}

		return Vector3.zero;
	}

    public Vector3 calculateSeekForce()
    {
        Vector3 toTarget = steeringComponent.Target - gameObject.transform.position;
        float distToTarget = toTarget.magnitude;

        //steeringComponent.ReachedGoal = false;

        if (distToTarget <= SlowDownDistance && distToTarget > StopDistance)
        {
            toTarget.Normalize();

            //because Deceleration is enumerated as an int, this value is required
            //to provide fine tweaking of the deceleration.
            const float DecelerationTweaker = 1.25f;

            //calculate the speed required to reach the target given the desired
            //deceleration
            float speed = distToTarget / ((float)Deceleration * DecelerationTweaker);

            //make sure the velocity does not exceed the max
            speed = (((speed) < (steeringComponent.MaxSpeed)) ? (speed) : (steeringComponent.MaxSpeed));

            //from here proceed just like Seek except we don't need to normalize
            //the ToTarget vector because we have already gone to the trouble
            //of calculating its length: dist.
            float _value = (speed / distToTarget);
            Vector3 DesiredVelocity = toTarget * _value;
            return (DesiredVelocity - steeringComponent.Velocity);
        }
        else
        {
            //steeringComponent.ReachedGoal = true;
        }

        return Vector3.zero;
    }
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SteeringBehaviourComponent : MonoBehaviour 
{
	public enum ESummingMethod
	{
		WeightedAverage,
		Prioritized,
		Dithered,
	};

	public ESummingMethod SummingMethod = ESummingMethod.WeightedAverage;
	public float Mass = 1.0f;
	public float MaxSpeed = 1.0f;
	public float MaxForce = 10.0f;
	public float SlowRadius = 25.0f;
	public Vector3 Velocity = Vector3.zero;
	public Vector3 Target = Vector3.zero;
	private Vector3 Heading = Vector3.zero;
	private Vector3 Side = Vector3.zero;
	public float BoundingRadius = 25.0f;

	private float stopDistance = 0.01f;

	public bool UseMouseInput = false;
	public bool ChangeRotation = false;
	public bool CheckScreenWrap = false;

	private List<SteeringBehaviourBase> SteeringBehaviours = new List<SteeringBehaviourBase>();
	private Vector3 SteeringForce;

	// Use this for initialization
	void Start () 
	{
		SteeringBehaviours.AddRange(GetComponentsInParent<SteeringBehaviourBase>());
		foreach (SteeringBehaviourBase behaviour in SteeringBehaviours)
		{
			behaviour.steeringComponent = this;
		}
	}
	

	// Update is called once per frame
	void Update () 
	{
		checkMouseInput();

		// Calculate Steering Force
		SteeringForce = calculateSteeringForce();

		//// Remove this code if we want to keep a velocity from before!
		//// calculate the distance to the target position
		Vector3 toTarget = Target - gameObject.transform.position;
		float distToTarget = toTarget.magnitude;

		if (distToTarget < stopDistance)
		{
			Velocity = Vector3.zero;
		}
		else
		{
			// Get acceleration
			Vector3 acceleration = SteeringForce * (1.0f / Mass);

			////update velocity
			Velocity = Velocity + (acceleration * Time.deltaTime);

			// Ensure Velocity does not exceed max speed
			Velocity = Vector3.ClampMagnitude(Velocity, MaxSpeed);

			// Update Position
			Vector3 _location = gameObject.transform.position + (Velocity * Time.deltaTime);
			gameObject.transform.position = _location;
		}

		if (Velocity.magnitude > 0 && ChangeRotation == true)
		{
			var angle = Mathf.Atan2(Velocity.y, Velocity.x) * Mathf.Rad2Deg;
			gameObject.transform.rotation = Quaternion.Euler(0, 0, angle);
		}

		if (CheckScreenWrap == true)
		{
			ScreenWrap();
		}
	}

	private Vector3 calculateSteeringForce()
	{
		Vector3 totalForce = Vector3.zero;

		foreach (SteeringBehaviourBase behaviour in SteeringBehaviours)
		{
			if (behaviour.Enabled == true)
			{
				switch (SummingMethod)
				{
					case ESummingMethod.WeightedAverage:
						{
							totalForce = totalForce + (behaviour.calculateForce() * behaviour.Weight);
							totalForce = Vector3.ClampMagnitude(totalForce, MaxForce);
							break;
						}

					case ESummingMethod.Prioritized:
						{
							Vector3 steeringForce = (behaviour.calculateForce() * behaviour.Weight);
							if (!AccumulateForce(totalForce, steeringForce))
							{
								return totalForce;
							}
							break;
						}

					case ESummingMethod.Dithered:
						// Uses a Random number to determine if it should use this. Not going to implement
						// But cover in class
						break;
				}
			}
		}

		return totalForce;
	}

	bool AccumulateForce(Vector3 RunningTot, Vector3 ForceToAdd)
	{
		//calculate how much steering force the vehicle has used so far
		float MagnitudeSoFar = RunningTot.magnitude;

		//calculate how much steering force remains to be used by this vehicle
		float MagnitudeRemaining = MaxForce - MagnitudeSoFar;

		//return false if there is no more force left to use
		if (MagnitudeRemaining <= 0.0)
		{
			return false;
		}

		//calculate the magnitude of the force we want to add
		float MagnitudeToAdd = ForceToAdd.magnitude;
  
		//if the magnitude of the sum of ForceToAdd and the running total
		//does not exceed the maximum force available to this vehicle, just
		//add together. Otherwise add as much of the ForceToAdd vector is
		//possible without going over the max.
		if (MagnitudeToAdd < MagnitudeRemaining)
		{
			RunningTot = RunningTot + ForceToAdd;
		}
		else
		{
			//add it to the steering force
			RunningTot = RunningTot + (ForceToAdd.normalized * MagnitudeRemaining); 
		}

		return true;
	}

	private void checkMouseInput()
	{
		if (Input.GetMouseButtonDown(0) && UseMouseInput == true)
		{
			Target = Input.mousePosition;
			Target = Camera.main.ScreenToWorldPoint(Target);
			Target.z = 0.0f;
		}
	}

	private void ScreenWrap()
	{
		bool isWrappingX = false;
		bool isWrappingY = false;

		var viewportPosition = Camera.main.WorldToViewportPoint(gameObject.transform.position);
		var newPosition = transform.position;

		if (!isWrappingX && (viewportPosition.x > 1 || viewportPosition.x < 0))
		{
			newPosition.x = -newPosition.x;
			isWrappingX = true;
		}

		if (!isWrappingY && (viewportPosition.y > 1 || viewportPosition.y < 0))
		{
			newPosition.y = -newPosition.y;
			isWrappingY = true;
		}

		gameObject.transform.position = newPosition;
	}
}

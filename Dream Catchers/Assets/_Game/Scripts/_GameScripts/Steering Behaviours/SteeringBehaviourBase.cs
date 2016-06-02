using UnityEngine;
using System.Collections;

public abstract class SteeringBehaviourBase : MonoBehaviour 
{
	public bool Enabled = true;

	public float Weight = 1.0f;

	[HideInInspector]
	public SteeringBehaviourComponent steeringComponent;

	public abstract Vector3 calculateForce();
}

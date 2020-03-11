using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleFollowing : MonoBehaviour
{
    public Path path;
    public float speed = 20.0f;
    public float mass = 5.0f;
    public bool isLooping = true;

    //Actual speed of the vehicle
    private float curSpeed;

    private int curPathIndex;
    private float pathLength;
    private Vector3 targetPoint;

    Vector3 velocity;

    // Start is called before the first frame update
    void Start()
    {
        pathLength = path.Length;
        curPathIndex = 0;

        //get the current velocity of the vehicle
        velocity = transform.forward;
    }

    // Update is called once per frame
    void Update()
    {
        //Unifty the speed
        curSpeed = speed * Time.deltaTime;
        targetPoint = path.GetPoint(curPathIndex);

        //if reach the radius within the path then move to next poitn in the path
        if(Vector3.Distance(transform.position, targetPoint) < path.Radius)
        {
            //Don't move the vehicle if path is finished
            if (curPathIndex < pathLength - 1)
                curPathIndex++;
            else if (isLooping)
                curPathIndex = 0;
            else
                return;
        }

        //Move the vehicle unti lthe end point is reached in the path
        if (curPathIndex >= pathLength)
            return;

        //Calculate the next velocity toward the path
        if (curPathIndex >= pathLength - 1 && !isLooping)
            velocity += Steer(targetPoint, true);
        else
            velocity += Steer(targetPoint);

        transform.position += velocity; // Move the vehicle according to the velocity
        transform.rotation = Quaternion.LookRotation(velocity); //Rotate the vehicle towards the desired velocity
        
    }

    //Steering algorithm to steer the vector towards the target
    public Vector3 Steer(Vector3 target, bool bFinalPoint = false)
    {
        //Calculate the directional vector from the current position towards the target point
        Vector3 desiredVelocity = (target - transform.position);
        float dist = desiredVelocity.magnitude;

        //Normalize the desired Velocity
        desiredVelocity.Normalize();

        //Calculate the velocity accordign to the speed
        if (bFinalPoint && dist < 10.0f)
            desiredVelocity *= (curSpeed * (dist / 10.0f));
        else
            desiredVelocity *= curSpeed;
        //Calculate the force vector
        Vector3 steeringForce = desiredVelocity - velocity;
        Vector3 acceleration = steeringForce / mass;

        return acceleration;
    }
}

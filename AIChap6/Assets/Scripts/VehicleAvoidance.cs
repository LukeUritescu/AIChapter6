using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleAvoidance : MonoBehaviour
{
    public float speed = 20.0f;
    public float mass = 5.0f;
    public float force = 50.0f;
    public float minimumDistToAvoid = 20.0f;

    //Actual speed othe vehicle
    private float curSpeed;
    private Vector3 targetPoint;

    
    // Start is called before the first frame update
    void Start()
    {
        mass = 5.0f;
        targetPoint = Vector3.zero;
    }

    private void OnGUI()
    {
        GUILayout.Label("CLick anywhere to move the vehicle to the clicked point");
    }

    // Update is called once per frame
    void Update()
    {
        //Vehicle move by mouse click
        RaycastHit hit;
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if(Input.GetMouseButtonDown(0) && Physics.Raycast(ray, out hit, 100.0f))
        {
            targetPoint = hit.point;
        }

        //Directional vector to the target position
        Vector3 dir = (targetPoint - transform.position);
        dir.Normalize();

        //Apply obstacle avoidance
        AvoidObstacles(ref dir);

        //Don't move the vehiclewhen the target point is reached
        if (Vector3.Distance(targetPoint, transform.position) < 3.0f)
            return;
        //Assign the speed with delta time
        curSpeed = speed * Time.deltaTime;

        //Rortate the vehicle to its target directional vector 
        var rot = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, 5.0f * Time.deltaTime);

        //Move the vehicle towards
        transform.position += transform.forward * curSpeed;
    }

    public void AvoidObstacles(ref Vector3 dir)
    {
        RaycastHit hit;
        //Only detect layer 8 (Obstacles)
        int layerMask = 1 << 8;

        //Check that the vehicle hit with obstacles within it's minimum distance to avoid
        if(Physics.Raycast(transform.position, transform.forward, out hit, minimumDistToAvoid, layerMask)){
            //Get the normal of the hit point to calculate the new direction
            Vector3 hitNormal = hit.normal;
            hitNormal.y = 0.0f; //DOn't want to move in Y-space

            //Get the new directional vector by addign force to vehicle's current forward vector
            dir = transform.forward + hitNormal * force;
        }
    }
}

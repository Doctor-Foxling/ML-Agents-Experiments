using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class RedCellAgent : Agent
{
    private Vector3 localPosition;
    public bool isHeuristic = false;

    [SerializeField] private Material winMaterial;
    [SerializeField] private Material loseMaterial;
    [SerializeField] private MeshRenderer cellMesh;

    private void Start()
    {
        localPosition = transform.localPosition;
        
    }
    public override void OnEpisodeBegin()
    {
        transform.localPosition = new Vector3(Random.Range(-7f, 7f), Random.Range(-7f, 7f), Random.Range(-7f, 7f));
        
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        
    }
    public override void OnActionReceived(ActionBuffers actions)
    {
        Vector2 rotation = Vector2.zero;
        float forwardSpeed = 10f, strafeSpeed = 4f, turnStrafeSpeed = 40f, turnHoverSpeed = 16f;

        float moveForward = actions.ContinuousActions[0] * forwardSpeed;
        float moveStrafe = actions.ContinuousActions[1] * strafeSpeed;
        float turnHover = actions.ContinuousActions[2] * turnHoverSpeed;
        float turnStrafe = actions.ContinuousActions[3] * turnStrafeSpeed;

        //float moveX = actions.ContinuousActions[0];
        //float moveZ = actions.ContinuousActions[1];
        //rotation.y += actions.ContinuousActions[2];
        //rotation.x += actions.ContinuousActions[3];

        //float moveSpeed = 5f;
        //float rotSpeed = 20f;
        transform.localPosition += transform.forward * moveForward * Time.deltaTime;
        transform.localPosition += transform.right * moveStrafe * Time.deltaTime;
        transform.Rotate(-turnHover, turnStrafe, 0);

        Debug.DrawLine(transform.forward, transform.forward*5);
        //transform.localPosition += transform.up * moveHover * Time.deltaTime;ww
    }

    //Here we can essentially modify the actions that would then be recieved by the OnActionRecieved function
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        if (isHeuristic)
        {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxisRaw("Vertical");
        continuousActions[1] = Input.GetAxisRaw("Horizontal");
        //Debug.Log("Mouse X: " + Input.GetAxisRaw("Mouse X"));
        continuousActions[2] = Input.GetAxisRaw("Mouse Y");
        //Debug.Log("Mouse Y: " + Input.GetAxisRaw("Mouse Y"));
        continuousActions[3] = Input.GetAxisRaw("Mouse X");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collided With: "+collision.collider.name);
        

        
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Triggered By: " + other.name);

        if (other.TryGetComponent<RedCellAgent>(out RedCellAgent goal))
        {
            SetReward(+0.5f);
            cellMesh.material = winMaterial;
            //The Episode should end when the agent recieves the final reward, or loses
        }

        if (other.TryGetComponent<WorldBoundary>(out WorldBoundary boundary))
        {
            SetReward(-1f);
            cellMesh.material = loseMaterial;
            EndEpisode();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent<RedCellAgent>(out RedCellAgent goal))
        {
            SetReward(+1f);
            //The Episode should end when the agent recieves the final reward, or loses
        }
    }
}

// https://www.immersivelimit.com/tutorials/rayperceptionsensorcomponent-tutorial

// Interesting Project (Space X)
//https://github.com/ParkJH1/SpaceX-Landing/blob/c84cdbb9f33d98edbd6ebc5661d593d430bf522e/Assets/Final/Scripts/AgentControllerFinal.cs

// Very Interesting (Quad Copter)
// https://github.com/Jamesscn/Quadcopter/tree/0a1e1f61d1a41c34c7d5e30bbfc5672e5895d003
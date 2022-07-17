using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using System;

public class HallwayAgent : Agent
{
    public event EventHandler OnEpisodeBeginEvent;
    public Transform targetX;
    public Transform targetO;
    public ObjectiveSpawner objective;
    bool hasPassedMiddle = false;
    public override void OnEpisodeBegin()
    {
        float x = UnityEngine.Random.Range(-9.5f, -1.0f);
        float z = UnityEngine.Random.Range(-3.5f, 3.5f);
        transform.localPosition = new Vector3(x, -0.5f, z);
        OnEpisodeBeginEvent?.Invoke(this, EventArgs.Empty);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        Vector3 dirToX = (targetX.position - transform.position).normalized;
        Vector3 dirToO = (targetO.position - transform.position).normalized;
        sensor.AddObservation(dirToX);
        sensor.AddObservation(dirToO);
        sensor.AddObservation(objective.GetObjective().CompareTag("X") ? 1 : 0);
       
    }
    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveSpeed = 5f;
        float moveZ = -actions.ContinuousActions[0];
        float moveX = actions.ContinuousActions[1];
        transform.position += new Vector3(moveX, 0, moveZ) * Time.deltaTime * moveSpeed;
        if (hasPassedMiddle==false && transform.position.x > 0)
        {
            hasPassedMiddle = true;
            SetReward(0.5f);

        }
        else if(hasPassedMiddle==true && transform.position.x < 0)
        {
            SetReward(-1f);
            hasPassedMiddle = false;
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> ca = actionsOut.ContinuousActions;
        ca[0] = Input.GetAxisRaw("Horizontal")*0.01f;
        ca[1] = Input.GetAxisRaw("Vertical")*0.01f;
    }

    public void OnCollisionEnter(Collision collision)
    {
        /*if (collision.gameObject.CompareTag("X") || collision.gameObject.CompareTag("O"))
        {
            SetReward(-0.5f);
            //Debug.Log("hello");
        }*/
        if (collision.gameObject.CompareTag("Wall"))
        {
            SetReward(-0.5f);
            EndEpisode();
        }
        else if((collision.gameObject.CompareTag("X_Target")&&objective.GetObjective().CompareTag("O")) || (collision.gameObject.CompareTag("O_Target") && objective.GetObjective().CompareTag("X"))){
            SetReward(-2f);
           
            EndEpisode();
        }
        else if((collision.gameObject.CompareTag("X_Target") &&objective.GetObjective().CompareTag("X"))|| (collision.gameObject.CompareTag("O_Target") && objective.GetObjective().CompareTag("O"))){
            SetReward(2f);
          
            EndEpisode();

        }
        
    }
}

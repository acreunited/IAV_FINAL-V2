using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class Presa : Agent {

    private int i = 0;
    //private bool colision = false;
    public Transform predador;

    public override void OnEpisodeBegin() {

       // if (colision) {
            transform.localPosition = new Vector3(Random.Range(-8f, 8f), 0.5f, Random.Range(-8f, 8f));
            //colision = false;
        //}
       
    }

    /*public override void CollectObservations(VectorSensor sensor) {
        base.CollectObservations(sensor);
    }*/

    public override void OnActionReceived(ActionBuffers actions) {

        float moveSpeed = 30f;
        float rotateSpeed = 300f;
        float move = actions.ContinuousActions[0];
        float rotate = actions.ContinuousActions[1];
        transform.Rotate(new Vector3(0, rotate * Time.fixedDeltaTime * rotateSpeed, 0));
        transform.localPosition += transform.forward * move * Time.fixedDeltaTime * moveSpeed;

        float distance = Vector3.Distance(this.transform.position, predador.transform.position);


        AddReward(0.01f);


    }
    public override void Heuristic(in ActionBuffers actionsOut) {
        ActionSegment<float> ca = actionsOut.ContinuousActions;
        ca[0] = Input.GetAxis("Vertical");
        ca[1] = Input.GetAxis("Horizontal");
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Predador")) {
            AddReward(-1f);
            EndEpisode();
           //colision = true;
        }
        if (other.gameObject.CompareTag("Wall")) {
            AddReward(-1f);
            // colision = true;
            EndEpisode();
        }
        

    }

    // Update is called once per frame
    private void FixedUpdate() {
        if (i++ % 5 == 0) {
            RequestDecision();
        }
    }
}
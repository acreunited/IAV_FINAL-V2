using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class Predador : Agent {

    private int i = 0;
    public Transform presa;

    public override void OnEpisodeBegin() {

            transform.localPosition = new Vector3(Random.Range(-8f, 8f), 0.5f, Random.Range(-8f, 8f));
            //presa.localPosition = new Vector3(Random.Range(-8f, 8f), 0.5f, Random.Range(-8f, 8f));

    }

   /* public override void CollectObservations(VectorSensor sensor) {
        base.CollectObservations(sensor);
    }*/

    public override void OnActionReceived(ActionBuffers actions) {

        float moveSpeed = 30f;
        float rotateSpeed = 300f;
        float move = actions.ContinuousActions[0];
        float rotate = actions.ContinuousActions[1];
        transform.Rotate(new Vector3(0, rotate * Time.fixedDeltaTime * rotateSpeed, 0));
        transform.localPosition += transform.forward * move * Time.fixedDeltaTime * moveSpeed;

        AddReward(-0.0001f);


    }
    public override void Heuristic(in ActionBuffers actionsOut) {
        ActionSegment<float> ca = actionsOut.ContinuousActions;
        ca[0] = Input.GetAxis("Vertical");
        ca[1] = Input.GetAxis("Horizontal");
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Presa")) {           
            AddReward(1f);
            EndEpisode();
            //colision = true;
        }
        if (other.gameObject.CompareTag("Wall")) {
            AddReward(-1f);
            EndEpisode();
            // colision = true;
        }
       

    }

    // Update is called once per frame
    private void FixedUpdate() {
        if (i++ % 5 == 0) {
            RequestDecision();
        }
    }
}
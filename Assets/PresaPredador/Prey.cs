using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class Prey : Agent {
    private Rigidbody rigidBody;
    private readonly float strength = 5f;
    private float rotateSpeed = 90f;

    private void Awake() {
        rigidBody = GetComponent<Rigidbody>();
    }

    public override void OnEpisodeBegin() {
        // come�a o agente numa posi��o aleat�ria(menos o 4� quadrante)
        transform.localPosition = new Vector3(UnityEngine.Random.Range(-9f, 9f), 0.5f, UnityEngine.Random.Range(8f, 9f));
        transform.rotation = Quaternion.identity;
        transform.Rotate(0, 180, 0);

    }

    public override void CollectObservations(VectorSensor sensor) {
        base.CollectObservations(sensor);
    }

    public override void OnActionReceived(ActionBuffers actions) {
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];
        float rotation = actions.ContinuousActions[2];
        transform.Rotate(new Vector3(0, rotation * Time.fixedDeltaTime * rotateSpeed, 0));


        // quando salta � 1 sen�o 0
        Vector3 move = new Vector3(moveX, 0, moveZ);


        // aplicado com for�a da fisica
        rigidBody.velocity = move * strength;


        AddReward(0.001f);
    }

    public override void Heuristic(in ActionBuffers actionsOut) {

        ActionSegment<float> ca = actionsOut.ContinuousActions;
        ca[0] = Input.GetAxis("Horizontal");
        ca[1] = Input.GetAxis("Vertical");
    }

    private void OnCollisionEnter(Collision collision) {

        

        if (collision.gameObject.CompareTag("Predador")) {
            AddReward(-3f);
            EndEpisode();
        }

        if (collision.gameObject.CompareTag("Wall")) {
            AddReward(-1f);
            EndEpisode();
        }

    }
}
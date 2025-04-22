using System;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class Jumper : Agent
{
    [SerializeField]
    float _jumpForce = 5f;
    private Rigidbody rb;

    void Start()
    {
        Initialize();
    }
    public override void Initialize()
    {
        rb = GetComponent<Rigidbody>();
    }

    public override void OnEpisodeBegin()
    {
        resetPosition();
    }

    private void resetPosition()
    {
        this.transform.localPosition = new Vector3(-9, 1f, 0);
        rb.linearVelocity = Vector3.zero;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
       
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        RaycastHit hit;
        bool isGrounded = Physics.Raycast(transform.position, Vector3.down, out hit, 1.01f) && hit.collider.CompareTag("Ground");
        if (Mathf.FloorToInt(actions.DiscreteActions[0]) == 1 && isGrounded)
        {
            Debug.Log("Jumping");
            rb.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
        }
    }
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActions = actionsOut.DiscreteActions;
        discreteActions[0] = Input.GetKey(KeyCode.Space) ? 1 : 0;
    }
}

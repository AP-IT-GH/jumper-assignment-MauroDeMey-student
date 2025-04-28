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
    private bool wasAboveCar = false;
    [SerializeField]
    private float episodeTimeLimit = 60f; // Time limit in seconds
    private float elapsedTime = 0f;
    [SerializeField]
    private Spawner spawner; // Reference to the Spawner script

    void Start()
    {
        Initialize();
    }

    void Update()
    {
        elapsedTime += Time.deltaTime; // Increment elapsed time
        if (elapsedTime >= episodeTimeLimit)
        {
            EndEpisode(); // End the episode when time limit is reached
            elapsedTime = 0f; // Reset elapsed time for the next episode
        }
    }
    public override void Initialize()
    {
        rb = GetComponent<Rigidbody>();
    }

    public override void OnEpisodeBegin()
    {
        resetPosition();
        spawner.ResetSpawner(); // Reset the spawner to spawn new objects
        wasAboveCar = false;
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
        if (Mathf.FloorToInt(actions.DiscreteActions[0]) == 1 && isGrounded) //Dit lijkt backwards coded. Negative penalty zal altijd applied zijn als de agent jumped.
        {
            rb.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
            AddReward(-0.1f); // Negative reward for jumping
        }
        else
        {
            AddReward(-0.01f); // Small penalty for doing nothing
        }

    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActions = actionsOut.DiscreteActions;
        discreteActions[0] = Input.GetKey(KeyCode.Space) ? 1 : 0;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Coin"))
        {
            Debug.Log("Coin collected");
            AddReward(1.0f); // Positive reward for collecting a coin
            Destroy(collision.gameObject); // Remove the coin
        }
        else if (collision.collider.CompareTag("Car"))
        {
            Debug.Log("got hit by car!");
            AddReward(-1.0f); // Large negative reward for hitting the car
            EndEpisode(); // End the episode
        }
        else if (collision.collider.CompareTag("Ground") && wasAboveCar)
        {
            Debug.Log("good jump!");
            AddReward(2.0f); // Big positive reward for landing after being above the car
            wasAboveCar = false; // Reset the flag
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Car"))
        {
            AddReward(0.5f); // Small positive reward for being above the car
            wasAboveCar = true; // Set the flag
        }
    }
}

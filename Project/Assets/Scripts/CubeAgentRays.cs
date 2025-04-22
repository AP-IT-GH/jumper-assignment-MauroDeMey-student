using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Unity.VisualScripting;
using UnityEngine;

public class CubeAgentRays : Agent
{
    //TODO
    // STOP ROTATION SPAMMING
    // STOP BALL HUGGING (+- done?)

    [SerializeField]
    Transform _target = null;
    [SerializeField]
    float _speed = 1f;
    [SerializeField]
    float _rotationSpeed = 1f;
    [SerializeField]
    private bool collectedBall = false;

    private Rigidbody rb;
    public override void OnEpisodeBegin()
    {
        resetPosition();
        rb = GetComponent<Rigidbody>();
        if (this.transform.localPosition.y < 0)
        {
            resetPosition();
            AddReward(-0.2f);
        }

        _target.GetComponent<MeshRenderer>().material.color = Color.gray;
        this._target.localPosition = new Vector3(
            Random.Range(-5f, 5f),
            .5f,
            Random.Range(-5f, 5f)
        );
        collectedBall = false;
    }

    private void resetPosition()
    {
        this.transform.localPosition = new Vector3(0, 0.5f, 0);
        this.transform.rotation = Quaternion.identity;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        //3 observables want vector3 (X/Y/Z)
        sensor.AddObservation(this.transform.localPosition);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        Vector3 moveController = Vector3.zero;
        Vector3 rotateController = Vector3.zero;

        moveController.z = actions.ContinuousActions[1];
        rotateController.y = actions.ContinuousActions[0];
        transform.Rotate(rotateController * this._rotationSpeed);
        transform.Translate(moveController * this._speed);

        if (transform.localPosition.y < 0)
        {
            EndEpisode();
            AddReward(-0.2f);
            return;
        }

    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.name == "goodzone" && collectedBall)
        {
            SetReward(0.9f);
            Debug.Log("IN ZONE AT RIGHT TIME");
            EndEpisode();
        }
        else if(other.gameObject.name == "goodzone" && !collectedBall)
        {
            SetReward(-0.1f);
            Debug.Log("IN ZONE AT WRONG TIME");
            EndEpisode();
        }

        if (other.gameObject.name == "Sphere" && !collectedBall)
        {
            Debug.Log("BALL PICKED UP!");
            _target.GetComponent<MeshRenderer>().material.color = Color.green;
            collectedBall = true;
            SetReward(0.5f);
        }
    }


    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxis("Horizontal");
        continuousActions[1] = Input.GetAxis("Vertical");
    }
}

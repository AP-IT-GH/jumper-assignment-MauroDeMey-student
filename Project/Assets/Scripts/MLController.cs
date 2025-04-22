using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class MLController : Agent
{
    [SerializeField]
    Transform _target = null;
    [SerializeField]
    float _speed = 1f;
    public override void OnEpisodeBegin()
    {
        if(this.transform.localPosition.y < 0)
        {
            resetPosition();
        }

        this._target.localPosition = new Vector3(
            Random.Range(-5f, 5f),
            .5f,
            Random.Range(-5f, 5f)
        );
    }

    private void resetPosition()
    {
        this.transform.localPosition = new Vector3(0, 0.5f, 0);
        this.transform.rotation = Quaternion.identity;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        //6 observables want 2x vector3 (X/Y/Z)
        sensor.AddObservation(this._target.localPosition); 
        sensor.AddObservation(this.transform.localPosition);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        Vector3 controller = Vector3.zero;

        controller.x = actions.ContinuousActions[0];
        controller.z = actions.ContinuousActions[1];

        transform.Translate(controller * this._speed);

        if (transform.localPosition.y < 0)
        {
            EndEpisode();
            return;
        }

        var distToTarget = Vector3.Distance(this.transform.localPosition, this._target.localPosition);

        if (distToTarget < 1.5)
        {
            SetReward(1);
            EndEpisode();
            return;
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxis("Horizontal");
        continuousActions[1] = Input.GetAxis("Vertical");
    }
}

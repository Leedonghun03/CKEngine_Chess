
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class NetworkTransform : MonoBehaviour
{
    public float positionCorrectionSpeed = 10f;
    public float maxPositionError = 1.0f;
    public float maxExtrapolationTime = 0.5f;

    private Vector3 serverSidePosition;
    private Quaternion serverSideRotation;
    private Vector3 velocity;
    private Vector3 acceleration;

    protected bool hasServerData = false;
    private float age = 0f;

    private Rigidbody rigidbody3d;


    public bool IsCanMoveRigidbody => rigidbody3d != null && !rigidbody3d.isKinematic;

    void Awake()
    {
        rigidbody3d = GetComponent<Rigidbody>();
        rigidbody3d.interpolation = RigidbodyInterpolation.Interpolate;
    }

    void Update()
    {
        if (!hasServerData) return;

        age += Time.deltaTime;

        Vector3 predictedVelocity = velocity + (acceleration * age);
        Vector3 predictedPosition = transform.position + predictedVelocity * Time.deltaTime;

        ApplyMove(predictedPosition);
        ApplyRotation(serverSideRotation);

        float distanceError = Vector3.Distance(transform.position, serverSidePosition);
        if (distanceError > maxPositionError)
        {
            Vector3 correctedPosition = Vector3.Lerp(transform.position, serverSidePosition, Time.deltaTime * positionCorrectionSpeed);
            ApplyMove(correctedPosition);
        }
    }

    private void ApplyMove(Vector3 position)
    {
        if (IsCanMoveRigidbody)
        {
            rigidbody3d.MovePosition(position);
        }
        else
        {
            transform.position = position;
        }
    }

    private void ApplyRotation(Quaternion rotation)
    {
        if (IsCanMoveRigidbody)
        {
            Quaternion newRotation = Quaternion.Slerp(rigidbody3d.rotation, rotation, Time.deltaTime * positionCorrectionSpeed);
            rigidbody3d.MoveRotation(newRotation);
        }
        else
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * positionCorrectionSpeed);
        }
    }

    public void Apply(Vector3 position, Quaternion rotation, Vector3 velocity, Vector3 acceleration)
    {
        this.serverSidePosition = position;
        this.serverSideRotation = rotation;
        this.velocity = velocity;
        this.acceleration = acceleration;

        this.hasServerData = true;
        this.age = 0f;
    }
}
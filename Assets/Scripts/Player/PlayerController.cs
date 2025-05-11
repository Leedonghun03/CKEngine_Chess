using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(PlayerInputHandler))]
public class PlayerController : MonoBehaviour
{
    private PlayerInputHandler input;
    public float moveSpeed = 5.0f;

    public Transform holdPoint;
    public GameObject heldPice;

    // Ray
    public float rayDistance;
    Ray ray;

    private void Awake()
    {
        input = GetComponent<PlayerInputHandler>();
    }

    private void Update()
    {
        HandleMovement();
        HandleInteraction();

        Debug.DrawRay(transform.position + Vector3.up, transform.forward, Color.red);
    }

    private void HandleMovement()
    {
        Vector2 move = input.moveVector;
        Vector3 moveDir = new Vector3(move.x, 0.0f, move.y);

        transform.Translate(moveDir * (moveSpeed * Time.deltaTime), Space.World);

        if (moveDir.magnitude > 0.0f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(moveDir, transform.up), 10.0f * Time.deltaTime);
        }
    }

    private void HandleInteraction()
    {
        if (!input.pickTriggerd)
        {
            return;
        }

        // 이미 들고 있다면 놓기!
        if (heldPice != null)
        {
            // 일단 return
            return;
        }

        ShootRay();
    }

    private void ShootRay()
    {
        var origin = transform.position + Vector3.up;
        var forward = transform.forward;

        if (Physics.Raycast(origin, forward, out RaycastHit hit, rayDistance))
        {
            var hitObject = hit.collider.gameObject;
            Debug.Log($"Hit Object : {hitObject.name}");

            if(hitObject.CompareTag("ChessPiece"))
            {
                hitObject.transform.SetParent(holdPoint, false);
                hitObject.transform.localPosition = Vector3.zero;

                heldPice = hitObject;
            }
        }
    }
}

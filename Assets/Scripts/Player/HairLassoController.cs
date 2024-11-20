using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HairLassoController : MonoBehaviour
{
    [SerializeField] private Rigidbody2D playerRb;
    [SerializeField] private PlayerController playerController;
    [SerializeField] public DistanceJoint2D distanceJoint;

    public LineRenderer lineRenderer;
    private GameObject currentSwingable;
    private bool isLassoActive = false;

    private void Awake()
    {
        distanceJoint.enabled = false; // Start disabled
        lineRenderer.enabled = false;
        Debug.Log("Distance joint is: " + distanceJoint.name);
        Debug.Log("Line Renderer is: " + lineRenderer.name);
    }

    private void Update()
    {
        if (isLassoActive)
        {
            UpdateLassoPosition();
            UpdatePlayerRotation();
        }
    }

    private void UpdateLassoPosition()
    {
        if (currentSwingable != null)
        {
            Vector3 playerPosition = playerRb.transform.position;
            Vector3 swingablePosition = currentSwingable.transform.position;
            lineRenderer.SetPosition(0, playerPosition);
            lineRenderer.SetPosition(1, swingablePosition);
        }
    }

    private void UpdatePlayerRotation()
    {
        if (currentSwingable != null)
        {
            Vector2 direction = (currentSwingable.transform.position - playerRb.transform.position).normalized;

            float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90;

            float angleDifference = Quaternion.Angle(playerRb.transform.rotation, Quaternion.Euler(0, 0, targetAngle));
            if (angleDifference > 45f) // angle threshold for rotating
            {
                Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);
                float rotationSpeed = 5f;
                playerRb.transform.rotation = Quaternion.Lerp(playerRb.transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
            }
        }
    }

    public void SetSwingableObject(GameObject swingable)
    {
        Debug.Log("Distance joint is: " + distanceJoint);
        Debug.Log("Line Renderer is: " + lineRenderer);

        currentSwingable = swingable;
    }

    public void ClearSwingableObject()
    {
        currentSwingable = null;
        isLassoActive = false;
        distanceJoint.enabled = false;
        lineRenderer.enabled = false;
        playerRb.transform.rotation = Quaternion.identity;
    }

    public bool TryAttachLasso()
    {
        if (currentSwingable != null)
        {
            Rigidbody2D swingableRb = currentSwingable.GetComponent<Rigidbody2D>();

            // Configure DistanceJoint2D to connect player to swingable
            distanceJoint.connectedBody = swingableRb;
            distanceJoint.autoConfigureConnectedAnchor = false;

            // Set anchor point on the player to the top (where the rope visually attaches)
            distanceJoint.anchor = Vector2.zero;

            // Set the connected anchor point on the swingable object to its center
            distanceJoint.connectedAnchor = swingableRb.transform.InverseTransformPoint(swingableRb.transform.position);

            // Set the distance of the joint to the length of the rope
            float ropeLength = Vector2.Distance(playerRb.transform.position, currentSwingable.transform.position);
            distanceJoint.distance = ropeLength;

            // Allow some flexibility by enabling limits if necessary
            distanceJoint.enableCollision = false;

            // Enable the DistanceJoint2D
            distanceJoint.enabled = true;

            // Enable the lasso visual
            lineRenderer.enabled = true;
            isLassoActive = true;

            // Set player to rope swinging state
            playerController.IsRopeSwinging = true;

            // Enable gravity for natural swinging effect
            playerRb.gravityScale = 1; // Keeps the player tethered but allows for swinging motion

            // Optional: Apply an initial impulse to get the swing started
            playerRb.AddForce(Vector2.right * 5f, ForceMode2D.Impulse);  // Adjust force as needed for initial motion

            return true;
        }
        return false;
    }

    public void ReleaseLasso()
    {
        // Detach the lasso
        distanceJoint.enabled = false;
        lineRenderer.enabled = false;
        isLassoActive = false;
        playerController.IsRopeSwinging = false;
        ClearSwingableObject();
    }
}

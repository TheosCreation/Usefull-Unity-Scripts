using Runtime;
using System;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class MovementController : MonoBehaviour
{
    private Rigidbody rb;

    [Tab("Settings")]
    [Header("Velocity Safety Measures")]
    [SerializeField] private float horizontalMaxVelocity = 25f;
    [SerializeField] private float velocityThreshold = 0.01f;

    [Header("Friction")]
    [SerializeField] private bool useFriction = true;
    [SerializeField, Range(0f, 1f)] private float friction = 0.9f;
    [SerializeField, Range(0f, 1f)] private float airFriction = 1f;

    [Header("Ground Check")]
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float maxGroundDistance = 1.0f;
    [SerializeField] private Transform groundCheckPosition;
    [SerializeField] private float groundCheckBoxWidth = 1.0f;
    public bool isGrounded = false;

    [Header("Slope Handling")]
    [SerializeField] private float maxSlopeAngle = 45.0f;
    private RaycastHit slopeHit;
    public bool isOnSlope = false;

    [Header("Step Climb")]
    [SerializeField] private Transform stepRayUpper;
    [SerializeField] private Transform stepRayLower;
    [SerializeField] float stepHeight = 0.3f;
    [SerializeField] float stepSmooth = 0.1f;

    [Header("Debugging")]
    [SerializeField] private bool debug = false;

    private bool movement = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        stepRayUpper.position = new Vector3(stepRayUpper.position.x, stepHeight, stepRayUpper.position.z);
    }

    private void FixedUpdate()
    {
        isGrounded = CheckGrounded();
        //StepClimb();

        if(isGrounded)
        {
            SetGravity(false);
            isOnSlope = CheckOnSlope();
            if(!movement && useFriction)
            {
                ApplyFriction(friction);
            }
        }
        else
        {
            isOnSlope = false;
            SetGravity(true);
            if (!movement && useFriction)
            {
                ApplyFriction(airFriction);
            }
        }

        if (rb.linearVelocity.magnitude < velocityThreshold)
        {
            rb.linearVelocity = Vector3.zero;
        }

    }

    private void LateUpdate()
    {
        ClampVelocity();
    }

    private void OnDrawGizmos()
    {
        if (!debug) return;

        // Define the positions of the corners relative to groundCheckPosition.position
        Vector3[] cornerOffsets = new Vector3[]
        {
            new Vector3(-groundCheckBoxWidth, 0, 0), // Left
            new Vector3(groundCheckBoxWidth, 0, 0),  // Right
            new Vector3(0, 0, groundCheckBoxWidth),  // Front
            new Vector3(0, 0, -groundCheckBoxWidth)  // Back
        };
    
        // Set the Gizmos color for the box
        Gizmos.color = Color.red;
    
        // Draw lines between the corners to form the 2D box
        Gizmos.DrawLine(groundCheckPosition.position + cornerOffsets[0], groundCheckPosition.position + cornerOffsets[2]);
        Gizmos.DrawLine(groundCheckPosition.position + cornerOffsets[2], groundCheckPosition.position + cornerOffsets[1]);
        Gizmos.DrawLine(groundCheckPosition.position + cornerOffsets[1], groundCheckPosition.position + cornerOffsets[3]);
        Gizmos.DrawLine(groundCheckPosition.position + cornerOffsets[3], groundCheckPosition.position + cornerOffsets[0]);

        // Draw the ground check rays
        Gizmos.color = Color.blue;
        foreach (Vector3 offset in cornerOffsets)
        {
            Gizmos.DrawRay(groundCheckPosition.position + offset, Vector3.down * maxGroundDistance);
        }

        // Draw the step climb rays
        Gizmos.color = Color.green;

        // Forward rays
        Gizmos.DrawRay(stepRayLower.position, transform.TransformDirection(Vector3.forward) * 0.1f);
        Gizmos.DrawRay(stepRayUpper.position, transform.TransformDirection(Vector3.forward) * 0.2f);

        // Forward 45 degrees rays
        Gizmos.DrawRay(stepRayLower.position, transform.TransformDirection(1.5f, 0, 1) * 0.1f);
        Gizmos.DrawRay(stepRayUpper.position, transform.TransformDirection(1.5f, 0, 1) * 0.2f);

        // Forward -45 degrees rays
        Gizmos.DrawRay(stepRayLower.position, transform.TransformDirection(-1.5f, 0, 1) * 0.1f);
        Gizmos.DrawRay(stepRayUpper.position, transform.TransformDirection(-1.5f, 0, 1) * 0.2f);

    }

    public void Rotate(Transform transform)
    {
        rb.rotation = transform.rotation;
    }

    public void AddForce(Vector3 directionalForce)
    {
        rb.AddForce(directionalForce, ForceMode.VelocityChange);
    }

    public void AddForce(Vector3 direction, float force)
    {
        Vector3 directionalForce = direction * force;
        AddForce(directionalForce);
    }

    public void MoveLocal(Vector3 movementVector, float maxSpeed, float acceleration)
    {
        // Transform the movement vector to world space and normalize it
        movementVector = transform.TransformDirection(movementVector);

        MoveWorld(movementVector, maxSpeed, acceleration);
    }

    public void MoveWorld(Vector3 directionVector, float maxSpeed, float acceleration)
    {
        if (directionVector.sqrMagnitude <= 0)
        {
            movement = false;
            return;
        }

        // Get the current velocity of the rigidbody
        Vector3 currentVelocity = rb.linearVelocity;

        // Calculate the desired velocity based on the direction vector and target speed
        Vector3 desiredVelocity = directionVector * maxSpeed;

        // Maintain the current vertical velocity
        desiredVelocity.y = currentVelocity.y;

        Vector3 velocityDifference = desiredVelocity - rb.linearVelocity;

        if (new Vector2(currentVelocity.x, currentVelocity.z).sqrMagnitude > maxSpeed * maxSpeed)
        {
            movement = false;
            return;
        }

        if(velocityDifference.sqrMagnitude > Mathf.Epsilon * Mathf.Epsilon)
        {
            movement = true;
            
            if(isOnSlope) velocityDifference = Vector3.ProjectOnPlane(velocityDifference, slopeHit.normal);

            // Apply the calculated velocity to the rigidbody
            rb.linearVelocity = new Vector3(currentVelocity.x + (velocityDifference.x * acceleration * Time.fixedDeltaTime),
                                            currentVelocity.y,
                                            currentVelocity.z + (velocityDifference.z * acceleration * Time.fixedDeltaTime));
        }

    }

    public void StopMovement()
    {
        rb.linearVelocity = Vector3.zero;
    }

    private void ClampVelocity()
    {
        Vector3 velClamped = Vector3.ClampMagnitude(rb.linearVelocity, horizontalMaxVelocity);
        rb.linearVelocity = new Vector3(velClamped.x, rb.linearVelocity.y, velClamped.z);
    }

    public void ResetVerticalVelocity()
    {
        Vector3 currentVelocity = rb.linearVelocity;
        rb.linearVelocity = new Vector3(currentVelocity.x, 0, currentVelocity.z);
    }
    
    public void ResetHorizontalVelocity()
    {
        Vector3 currentVelocity = rb.linearVelocity;
        rb.linearVelocity = new Vector3(0, currentVelocity.y, 0);
    }

    public void SetGravity(bool useGravity)
    {
        rb.useGravity = useGravity;
    }

    public float GetLinearVelocityMagnitude()
    {
        return rb.linearVelocity.magnitude;
    }

    private bool CheckGrounded()
    {
        // Define the positions of the corners relative to feetTransform
        Vector3[] cornerOffsets = new Vector3[]
        {
            new Vector3(-groundCheckBoxWidth, 0, 0), // Left
            new Vector3(groundCheckBoxWidth, 0, 0),  // Right
            new Vector3(0, 0, groundCheckBoxWidth),  // Front
            new Vector3(0, 0, -groundCheckBoxWidth)  // Back
        };
    
        // Perform raycasts from each corner
        foreach (Vector3 offset in cornerOffsets)
        {
            if (Physics.Raycast(groundCheckPosition.position + offset, Vector3.down, maxGroundDistance, groundMask))
            {
                return true;
            }
        }
    
        // If none of the raycasts hit the ground, return false
        return false;
    }

    private bool CheckOnSlope()
    {
        // Define the positions of the corners relative to feetTransform
        Vector3[] cornerOffsets = new Vector3[]
        {
            new Vector3(-groundCheckBoxWidth, 0, 0), // Left
            new Vector3(groundCheckBoxWidth, 0, 0),  // Right
            new Vector3(0, 0, groundCheckBoxWidth),  // Front
            new Vector3(0, 0, -groundCheckBoxWidth)  // Back
        };

        // Perform raycasts from each corner
        foreach (Vector3 offset in cornerOffsets)
        {
            if (Physics.Raycast(groundCheckPosition.position + offset, Vector3.down, out slopeHit, maxGroundDistance, groundMask))
            {
                float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
                if (angle < maxSlopeAngle && angle != 0)
                {
                    return true;
                }
            }
        }

        // If none of the raycasts hit the ground, return false
        return false;
    }
    
    public void SetFriction(bool useFriction)
    {
        this.useFriction = useFriction;
    }    

    private void ApplyFriction(float friction)
    {
        // Get the current velocity of the rigidbody
        Vector3 currentVelocity = rb.linearVelocity;

        // Apply friction only to the horizontal components (x and z)
        Vector3 horizontalVelocity = new Vector3(currentVelocity.x, 0, currentVelocity.z);

        // Apply the friction coefficient to the horizontal velocity over time
        horizontalVelocity = horizontalVelocity * friction;

        // Update the rigidbody's velocity with the new horizontal velocity and keep the vertical component unchanged
        rb.linearVelocity = new Vector3(horizontalVelocity.x, currentVelocity.y, horizontalVelocity.z);
    }

    private void StepClimb()
    {
        RaycastHit hitLower;
        if(Physics.Raycast(stepRayLower.position, transform.TransformDirection(Vector3.forward), out hitLower, 0.1f, groundMask))
        {
            RaycastHit hitUpper;
            if (!Physics.Raycast(stepRayUpper.position, transform.TransformDirection(Vector3.forward), out hitUpper, 0.2f))
            {
                rb.position -= new Vector3(0f, -stepSmooth, 0f);
            }
        }
        
        RaycastHit hitLower45;
        if(Physics.Raycast(stepRayLower.position, transform.TransformDirection(1.5f, 0, 1), out hitLower45, 0.1f, groundMask))
        {
            RaycastHit hitUpper45;
            if (!Physics.Raycast(stepRayUpper.position, transform.TransformDirection(1.5f, 0, 1), out hitUpper45, 0.2f))
            {
                rb.position -= new Vector3(0f, -stepSmooth, 0f);
            }
        }
        
        RaycastHit hitLowerNegative45;
        if(Physics.Raycast(stepRayLower.position, transform.TransformDirection(-1.5f, 0, 1), out hitLowerNegative45, 0.1f, groundMask))
        {
            RaycastHit hitUpperNegative45;
            if (!Physics.Raycast(stepRayUpper.position, transform.TransformDirection(-1.5f, 0, 1), out hitUpperNegative45, 0.2f))
            {
                rb.position -= new Vector3(0f, -stepSmooth, 0f);
            }
        }
    }

}

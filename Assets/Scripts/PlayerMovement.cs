using UnityEngine;
using UnityEngine.Animations;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float turnSpeed = 360;
    Animator animator;
    private Vector3 inputDirection;

    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }
    void Update()
    {
        GatherInputForIsometricPlane();
        Look();
        Animate();
    }

    void FixedUpdate()
    {
        Move();
    }

    void GatherInputForIsometricPlane()
    {
        
        inputDirection = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        
    }

    void Look()
    {
        if(inputDirection != Vector3.zero)
        {
            var matrix = Matrix4x4.Rotate(Quaternion.Euler(0, 45, 0));
            var skewedInput = matrix.MultiplyPoint3x4(inputDirection);

            var relative = (transform.position + skewedInput) - transform.position;
            var rotation = Quaternion.LookRotation(relative, Vector3.up);

            transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, turnSpeed * Time.deltaTime);
        }
    }

    void Move()
    {
        rb.MovePosition(transform.position + (transform.forward * inputDirection.magnitude) * moveSpeed * Time.deltaTime);
    }

    void Animate()
    {
        if(animator != null)
        {
            animator.SetFloat("Speed", inputDirection.magnitude);
        }
    }
}

using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float Speed = 1f;
    public Rigidbody2D Rb;
    public float Deadzone = 0.1f;

    private Vector2 _inputVec;

    void Start()
    {
        Rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        var xInput = Input.GetAxis("Horizontal");
        var yInput = Input.GetAxis("Vertical");
        _inputVec = new Vector2(xInput, yInput).normalized;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(GameController.ENEMY_TAG))
        {
            GameController.Current.GameOver();
            Destroy(this);
        }
    }

    private void FixedUpdate()
    {
        if (_inputVec.magnitude > Deadzone)
        {
            Vector3 deltaPosition = (Vector3)_inputVec * Speed * Time.fixedDeltaTime;
            Vector3 newPosition = transform.position + deltaPosition;
            Rb.MovePosition(newPosition);
        }
    }
}

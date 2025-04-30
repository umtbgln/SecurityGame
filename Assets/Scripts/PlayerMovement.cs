using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    public Slider staminaBar;
    public Transform orientation;
    public Transform groundCheck;
    public Transform camHolder;
    public float moveSpeed = 5f;
    public float jumpForce = 7f;
    public float groundDrag = 5f;
    public float groundDistance = 0.3f;
    public float crouchCamY = -0.25f;
    public float standCamY = 0f;
    public float runSpeed = 10f;
    public float slowdownSpeed = 1f;
    public float speedRegenRate = 0.5f;
    public float stamina = 100f;
    public float maxStamina = 100f;
    public float staminaDecreaseRate = 20f;
    public float staminaRegenRate = 10f;

    Rigidbody rb;
    Vector3 moveDirection;
    bool isGrounded;
    bool isCrouching;
    float horizontalInput;
    float verticalInput;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    private void Update()
    {
        isGrounded = Physics.Raycast(groundCheck.position, Vector3.down, groundDistance);
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        if (isGrounded)
        {
            rb.drag = groundDrag;

            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (isCrouching)
                {
                    StandUp();
                }
                Jump();
            }

            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                Crouch();
            }

            if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                StandUp();
            }
        }
        else
            rb.drag = 0f;

        if (moveSpeed < 5f)
        {
            SlowDown();
        }

        if (Input.GetKey(KeyCode.F) && stamina > 0f)
        {
            Run();
        }
        else
        {
            if (stamina == 0f)
            {
                Invoke(nameof(StoreStamina), 5f); // Stamina tamamen bittikten sonra tekrar dolmaya ba�larken ko�maya ba�lad���m�zda stamina hem dolmaya �al���yor hem bo�almaya �al���yor. ��nk� "Invoke" kullan�yoruz.
                moveSpeed = slowdownSpeed;
            }
            else
                StoreStamina();
        }

        if (Input.GetKeyUp(KeyCode.F))
        {
            moveSpeed = 5f;
        }

        staminaBar.value = stamina / maxStamina;
    }

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + moveSpeed * Time.fixedDeltaTime * moveDirection.normalized);
    }

    void Jump() // Z�plama Fonksiyonu
    {
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    void Crouch() // E�ilme Fonksiyonu
    {
        isCrouching = true;
        camHolder.localPosition = new Vector3(0, crouchCamY, 0);
    }

    void StandUp() // Aya�a Kalkma Fonksiyonu
    {
        isCrouching = false;
        camHolder.localPosition = new Vector3(0, standCamY, 0);
    }

    void Run() // Ko�ma Fonksiyonu
    {
        moveSpeed = runSpeed;
        stamina -= staminaDecreaseRate * Time.deltaTime;
        if (stamina < 0f) stamina = 0f;
    }

    void SlowDown() // Yava�lama Fonksiyonu
    {
        moveSpeed += speedRegenRate * Time.deltaTime;
        if (moveSpeed > 5f) moveSpeed = 5f;
    }

    void StoreStamina() // Dayan�kl�l�k Depolama Fonksiyonu
    {
        if (stamina < maxStamina)
        {
            stamina += staminaRegenRate * Time.deltaTime;
            if (stamina > maxStamina) stamina = maxStamina;
        }
    }
}

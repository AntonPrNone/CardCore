using UnityEngine;
using UnityEngine.InputSystem;


public class CameraRigScript : MonoBehaviour
{
   [Header("Speed Settings")]
    public float moveSpeed = 20f;
    public float zoomSpeed = 500f;
    
    [Header("Edge Scrolling")]
    public float edgeSize = 10f;
    
    [Header("Zoom Limits")]
    public float minY = 10f;
    public float maxY = 100f;

    private InputSystem_Actions controls;
    private Vector2 moveInput = Vector2.zero;
    private float zoomInput = 0f;
    private Vector2 mousePos = Vector2.zero;

    private void Awake()
    {
        controls = new InputSystem_Actions();

        // Подписки на действия
        controls.Camera.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Camera.Move.canceled += ctx => moveInput = Vector2.zero;

        controls.Camera.Zoom.performed += ctx => zoomInput = ctx.ReadValue<float>();
        controls.Camera.Zoom.canceled += ctx => zoomInput = 0f;

        controls.Camera.MousePos.performed += ctx => mousePos = ctx.ReadValue<Vector2>();
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    private void Update()
    {
        Vector3 pos = transform.position;

        // Перемещение по клавишам
        Vector3 inputDir = new Vector3(moveInput.x, 0f, moveInput.y);
        pos += inputDir * moveSpeed * Time.deltaTime;

        // Перемещение по краям экрана
        if (mousePos.x >= Screen.width - edgeSize)
            pos += Vector3.right * moveSpeed * Time.deltaTime;
        if (mousePos.x <= edgeSize)
            pos += Vector3.left * moveSpeed * Time.deltaTime;
        if (mousePos.y >= Screen.height - edgeSize)
            pos += Vector3.forward * moveSpeed * Time.deltaTime;
        if (mousePos.y <= edgeSize)
            pos += Vector3.back * moveSpeed * Time.deltaTime;

        // Зум
        pos.y -= zoomInput * zoomSpeed * Time.deltaTime;
        pos.y = Mathf.Clamp(pos.y, minY, maxY);

        transform.position = pos;
    }

}

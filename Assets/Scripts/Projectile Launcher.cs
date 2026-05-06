using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class ProjectileLauncher : MonoBehaviour
{
    [Header("SingleShot Setup")]
    [SerializeField] private Transform centerPoint;
    [SerializeField] private Transform leftForkTip;
    [SerializeField] private Transform rightForkTip;
    [SerializeField] private GameObject Ball;
    private Vector2 ball_initial_pos;

    [SerializeField] private LineRenderer trajectoryLine;
    [SerializeField] private float timeStep = 0.05f;
    [SerializeField] private int PointCount = 30;

    Rigidbody2D ball_rb;
    


    [Header("Rubber band")]
    [SerializeField] private LineRenderer leftBand;
    [SerializeField] private LineRenderer rightBand;

    [Header("Settings")]
    [SerializeField] private float maxPullDistance;
    [SerializeField] private float launchForceMultiplier;
    private Vector2 pullPosition;

    private Camera cam;
    private bool isDragging = false;

    Vector2 GetPointerPosition()
    {
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
            return Touchscreen.current.position.ReadValue();

        return Mouse.current.position.ReadValue();
    }

    bool PointerPressedThisFrame()
    {
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
            return true;
        if (Mouse.current != null)
            return Mouse.current.press.wasPressedThisFrame;

        return false;
    }

    bool PointerReleasedThisFrame()
    {
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasReleasedThisFrame)
            return true;
        if (Mouse.current != null)
            return Mouse.current.press.wasReleasedThisFrame;

        return false;
    }

    bool PointerHeld()
    {
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
            return true;
        if (Mouse.current != null)
            return Mouse.current.press.isPressed;

        return false;
    }

    private void OnEnable()
    {
        GameManager.instance.OnResetBallPos += Reset;
    }
    private void OnDisable()
    {
        GameManager.instance.OnResetBallPos -= Reset;
    }

    private void Start()
    {
        cam = Camera.main;
        ball_rb = Ball.GetComponent<Rigidbody2D>();

        ball_initial_pos = Ball.transform.position;

        trajectoryLine.useWorldSpace = true;
        trajectoryLine.gameObject.SetActive(false);

        leftBand.gameObject.SetActive(false);
        rightBand.gameObject.SetActive(false);
        leftBand.useWorldSpace = true;
        rightBand.useWorldSpace = true;

        leftBand.SetPosition(0, leftForkTip.position);
        leftBand.SetPosition(1, centerPoint.position);
        rightBand.SetPosition(0, rightForkTip.position);
        rightBand.SetPosition(1, centerPoint.position);
    }
    private void Update()
    {
        if (PointerPressedThisFrame())
        {
            Vector2 MousePos = cam.ScreenToWorldPoint(GetPointerPosition());
            RaycastHit2D hit = Physics2D.Raycast(MousePos , Vector2.zero);
            if(hit.collider != null)
            {
                if(hit.transform == Ball.transform)
                {
                    isDragging = true;

                    leftBand.gameObject.SetActive(true);
                    rightBand.gameObject.SetActive(true);
                    ball_rb.bodyType = RigidbodyType2D.Static;

                }
            }
        }

        if (PointerHeld() && isDragging)
        {
            Vector2 MousePos = cam.ScreenToWorldPoint(GetPointerPosition());

            Vector2 pullDir = MousePos - (Vector2)centerPoint.position;
            if(pullDir.magnitude > maxPullDistance)
            {
                pullDir = pullDir.normalized * maxPullDistance;
            }

            pullPosition = (Vector2)centerPoint.position + pullDir;

            Vector2 launchDir = (Vector2)centerPoint.position - pullPosition;
            float force = launchDir.magnitude * launchForceMultiplier;
            Vector2 predictVelocity = launchDir.normalized * force / ball_rb.mass;

            ShowTrajectory(pullPosition, predictVelocity);


            float stretchAmount = pullDir.magnitude / maxPullDistance;
            float squishX = 0.2f + stretchAmount * 0.02f;
            float squishY = 0.2f - stretchAmount * 0.01f;

            Ball.transform.localScale = new Vector2(squishX, squishY);
            Ball.transform.position = pullPosition;


            leftBand.SetPosition(1, pullPosition);
            rightBand.SetPosition(1, pullPosition);
        }

        if (PointerReleasedThisFrame() && isDragging)
        {
            HideTrajectory();

            Ball.GetComponent<TrailRenderer>().enabled = true;

            Vector2 launchDir = (Vector2)centerPoint.position - pullPosition;
            float force = launchDir.magnitude * launchForceMultiplier;

            ball_rb.bodyType = RigidbodyType2D.Dynamic;
            ball_rb.AddForce(launchDir.normalized * force, ForceMode2D.Impulse);

            Ball.transform.localScale = new Vector2(0.2f , 0.2f);
            

            leftBand.gameObject.SetActive(false);
            rightBand.gameObject.SetActive(false);

            isDragging = false;
        }
    }

    void ShowTrajectory(Vector2 startPos , Vector2 velocity)
    {
        trajectoryLine.gameObject.SetActive(true);
        trajectoryLine.positionCount = PointCount;

        for(int i = 0; i < PointCount; i++)
        {
            float t = i * timeStep;

            Vector2 point = startPos + velocity * t + 0.5f * Physics2D.gravity * t * t;

            trajectoryLine.SetPosition(i, point);
        }
    }

    void HideTrajectory()
    {
        trajectoryLine.gameObject.SetActive(false);
    }

    private void Reset()
    {
        Ball.GetComponent<TrailRenderer>().enabled = false;
        Ball.transform.position = ball_initial_pos;
        ball_rb.bodyType = RigidbodyType2D.Static;
    }

}

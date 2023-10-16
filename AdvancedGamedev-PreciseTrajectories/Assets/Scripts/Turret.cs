
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class Turret : MonoBehaviour
{

    #region Public Variables

    public float shootingCooldown = 1.0f;
    public float turnSpeed = 3.0f;
    public float cannonUpDownSpeed = 5.0f;
    public float targetHeight = 0.0f;
    public float samplingRate = 0.2f;
    public float cannonballDrag = 0.2f;
    public float crosshairOffset = 0.01f;

    public float shootVelocity = 15.0f;

    public GameObject cannonBallPrefab;

    public GameObject turnTable;
    public GameObject cannon;
    public GameObject crosshair;

    public LineRenderer lineRenderer;

    public Transform cannonBallTransform;

    public Vector2 cannonUpDownRange;


    #endregion

    #region Private Variables

    private bool isShooting = false;

    private float currentCannonAngle = 0.0f;

    #endregion

    void Start()
    {
        this.currentCannonAngle = 0.0f;
    }

    private IEnumerator Shoot()
    {
        this.isShooting = true;

        var cannonball = GameObject.Instantiate(this.cannonBallPrefab);
        cannonball.transform.position = this.cannonBallTransform.position;
        var rb = cannonball.GetComponentInChildren<Rigidbody>();

        
        //No negative drag ^^
        rb.drag = Mathf.Clamp(this.cannonballDrag, 0.0f, float.PositiveInfinity);
        rb.AddForce(this.shootVelocity * this.cannonBallTransform.forward, ForceMode.VelocityChange);

        yield return new WaitForSeconds(this.shootingCooldown);

        this.isShooting = false;
    }

    private void HandleInput()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        this.transform.Rotate(Vector3.up, horizontal * this.turnSpeed * Time.deltaTime);

        this.currentCannonAngle += vertical * this.cannonUpDownSpeed * Time.deltaTime;
        this.currentCannonAngle = Mathf.Clamp(this.currentCannonAngle, this.cannonUpDownRange.y, this.cannonUpDownRange.x);

        this.cannon.transform.localEulerAngles = new Vector3(this.currentCannonAngle, 0.0f, 0.0f);

        if(!this.isShooting && Input.GetKeyDown(KeyCode.Space))
        {
            this.StartCoroutine(this.Shoot());
        }
    }

    private void CalculateTrajectoryLine()
    {
        List<Vector3> lrPositions = ListPool<Vector3>.Get();

        Vector3 currentPosition = this.cannonBallTransform.position;
        float currentTime = 0.0f;
        while (currentPosition.y > this.targetHeight)
        {
            currentPosition = Trajectory.GetPosition(this.cannonBallTransform.position, this.shootVelocity * this.cannonBallTransform.forward, this.cannonballDrag, currentTime);
            currentTime += this.samplingRate;
            lrPositions.Add(currentPosition);
        }

        this.lineRenderer.positionCount = lrPositions.Count;
        this.lineRenderer.SetPositions(lrPositions.ToArray());

        ListPool<Vector3>.Release(lrPositions);
    }

    private void PlaceCrosshair()
    {

        float timeToHitGround = Trajectory.GetTimeForReachingYOnTheWayDown(this.cannonBallTransform.position,
            this.shootVelocity * this.cannonBallTransform.forward,
            this.cannonballDrag,
            this.targetHeight);

        this.crosshair.transform.position = Trajectory.GetPosition(this.cannonBallTransform.position,
            this.shootVelocity * this.cannonBallTransform.forward,
            this.cannonballDrag,
            timeToHitGround);

        this.crosshair.transform.position += Vector3.up * this.crosshairOffset;
    }

    void Update()
    {
        if (this.samplingRate > 0.0f)
        {
            this.CalculateTrajectoryLine();
        }

        this.HandleInput();

        this.PlaceCrosshair();
    }
}

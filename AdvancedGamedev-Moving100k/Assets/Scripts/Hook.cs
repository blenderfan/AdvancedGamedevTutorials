using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hook : MonoBehaviour
{

    public Camera cam = null;

    public float sideSpeed = 2.0f;
    public float forwardSpeed = 2.0f;
    public float upDownSpeed = 2.0f;

    // Update is called once per frame
    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        float horSpeed = horizontal * this.sideSpeed * Time.deltaTime;
        float verSpeed = vertical * this.forwardSpeed * Time.deltaTime;

        float upDown = 0.0f;
        if (Input.GetKey(KeyCode.Q))
        {
            upDown = this.upDownSpeed * Time.deltaTime;
        } else if(Input.GetKey(KeyCode.E))
        {
            upDown = -this.upDownSpeed * Time.deltaTime;
        }

        var position = this.transform.position;
        var position2D = new Vector3(position.x, 0.0f, position.z);
        var camPos2D = new Vector3(this.cam.transform.position.x, 0.0f, this.cam.transform.position.z);
        var dist = Vector3.Distance(position2D, camPos2D);
        dist = Mathf.Clamp(dist, 1.0f, float.PositiveInfinity);
        var viewForward = position2D - camPos2D;
        var newForward = Quaternion.AngleAxis(horSpeed, Vector3.up) * viewForward.normalized;
        position = camPos2D + newForward.normalized * (dist + verSpeed);
        position.y = this.transform.position.y + upDown;
        position.y = Mathf.Clamp(position.y, 0, float.PositiveInfinity);


        this.transform.position = position;
    }
}

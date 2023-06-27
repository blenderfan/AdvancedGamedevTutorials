using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hook : MonoBehaviour
{
    public float speed = 2.0f;


    // Update is called once per frame
    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        float horSpeed = horizontal * this.speed * Time.deltaTime;
        float verSpeed = vertical * this.speed * Time.deltaTime;

        float upDown = 0.0f;
        if (Input.GetKey(KeyCode.Q))
        {
            upDown = this.speed * Time.deltaTime;
        } else if(Input.GetKey(KeyCode.E))
        {
            upDown = -this.speed * Time.deltaTime;
        }

        this.transform.position += new Vector3(horSpeed, upDown, verSpeed);
        float y = this.transform.position.y;
        y = Mathf.Clamp(y, 0, float.PositiveInfinity);
        this.transform.position = new Vector3(this.transform.position.x, y, this.transform.position.z);
    }
}

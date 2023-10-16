using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretCamera : MonoBehaviour
{

    public Turret turret;

    private Vector3 offset;

    void Start()
    {
        this.offset = this.turret.transform.position - this.transform.position;
    }

    void Update()
    {
        var turretDir = this.turret.transform.TransformDirection(this.offset);
        this.transform.position = Vector3.Lerp(this.transform.position, this.turret.transform.position - turretDir, Time.deltaTime);
        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.LookRotation(turretDir), Time.deltaTime);
    }
}

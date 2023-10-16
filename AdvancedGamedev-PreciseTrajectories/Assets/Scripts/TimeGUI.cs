using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeGUI : MonoBehaviour
{
    public Turret turret;

    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(0, 0, 300, 400));

        GUI.BeginGroup(new Rect(0, 0, 300, 400));

        GUILayout.Label("Flight Time");

        var cannonTransform = this.turret.cannonBallTransform;
        float shootVelocity = this.turret.shootVelocity;
        float drag = this.turret.cannonballDrag;
        float targetHeight = this.turret.targetHeight;

        float timeToHitGround = Trajectory.GetTimeForReachingYOnTheWayDown(cannonTransform.position,
            shootVelocity * cannonTransform.forward,
            drag,
            targetHeight);

        GUILayout.Label($"{timeToHitGround.ToString("0.00")}s");

        GUI.EndGroup();

        GUILayout.EndArea();
    }
}

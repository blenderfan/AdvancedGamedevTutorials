using System;
using UnityEngine;

public static class Trajectory
{

    public static float GetTimeForReachingYOnTheWayDown(Vector3 startPosition, Vector3 startVelocity, float drag, float targetY,
        float startTime = 1.0f, int iterations = 16, float epsilon = 0.01f)
    {
        float time = startTime;
        Vector3 tangent = GetTangent(startVelocity, drag, time);

        for (int i = 0; i < iterations; i++)
        {
            //On the way up
            if (tangent.y >= 0.0f)
            {
                time *= 2.0f;
                tangent = GetTangent(startVelocity, drag, time);
            }
            else
            {
                //Using Newton's method
                var position = GetPosition(startPosition, startVelocity, drag, time);
                tangent = GetTangent(startVelocity, drag, time);

                if (Mathf.Abs(position.y - targetY) < epsilon) return time;

                time = time - ((position.y - targetY) / tangent.y);
            }
        }

        return time;
    }

    public static float GetTimeForReachingYOnTheWayDown(Rigidbody rigidBody, Vector3 startVelocity, float targetY, 
        float startTime = 1.0f, int iterations = 16, float epsilon = 0.01f)
    {
        return GetTimeForReachingYOnTheWayDown(rigidBody.transform.position, startVelocity, rigidBody.drag, targetY,
            startTime, iterations, epsilon);
    }

    //Untested
    /*
    public static float GetTimeForShootingAngleToHitTarget(Rigidbody rigidBody, Vector3 targetPosition, float targetAngle, out Vector3 startVelocity,
        float startTime = 1.0f, int iterations = 16, float epsilon = 0.01f)
    {
        startVelocity = GetStartVelocity(rigidBody, targetPosition, startTime);

        float prevTime = startTime * 0.5f;
        float time = startTime;

        for (int i = 0; i < iterations; i++)
        {

            //Using the Secant Method here
            startVelocity = GetStartVelocity(rigidBody, targetPosition, time);
            var prevVelocity = GetStartVelocity(rigidBody, targetPosition, prevTime);

            var flatStartVelocity = new Vector3(startVelocity.x, 0.0f, startVelocity.z);
            var flatPrevVelocity = new Vector3(prevVelocity.x, 0.0f, prevVelocity.z);

            float angle = Vector3.Angle(startVelocity.normalized, flatStartVelocity.normalized);
            float prevAngle = Vector3.Angle(prevVelocity.normalized, flatPrevVelocity.normalized);

            if (Mathf.Abs(angle - prevAngle) < epsilon) return time;

            float currentTime = time;
            time = time - ((angle - targetAngle) * (time - prevTime)) / (angle - prevAngle);
            prevTime = currentTime;
        }

        return time;
    }
    */

    public static Vector3 GetTangent(Vector3 startVelocity, float drag, float time)
    {
        if (drag > 0.0f)
        {

            float frameDrag = 1.0f - drag * Time.fixedDeltaTime;
            float frames = time / Time.fixedDeltaTime;

            float frameDragPower = Mathf.Pow(frameDrag, frames);

            Vector3 tangent = startVelocity * frameDragPower;

            Vector3 accTangent = (Physics.gravity / drag) * (drag * Time.fixedDeltaTime * frameDragPower - frameDragPower + 1.0f);
            accTangent -= Physics.gravity * Time.fixedDeltaTime;

            return (tangent + accTangent);
        }
        else
        {
            //Derrivative of the classic mechanics formula
            return (startVelocity + Physics.gravity * time);
        }
    }

    public static Vector3 GetTangent(Rigidbody rigidBody, Vector3 startVelocity, float time)
    {
        return GetTangent(startVelocity, rigidBody.drag, time);
    }

    public static Vector3 GetPosition(Vector3 startPosition, Vector3 startVelocity, float drag, float time)
    {
        Vector3 acceleration = Physics.gravity;
        Vector3 position = startPosition;

        if (drag > 0.0f)
        {

            float frameDrag = 1.0f - drag * Time.fixedDeltaTime;
            float frames = time / Time.fixedDeltaTime;

            float frameDragLog = (float)Math.Log(frameDrag);
            float frameDragPower = Mathf.Pow(frameDrag, frames);

            position += startVelocity * ((frameDragPower - 1.0f) / frameDragLog) * Time.fixedDeltaTime;

            float accDragFactor = (drag * Time.fixedDeltaTime - 1);
            accDragFactor *= (frameDragPower - frames * frameDragLog - 1.0f);
            accDragFactor /= (drag * frameDragLog);

            position += acceleration * accDragFactor * Time.fixedDeltaTime;

            return position;
        }
        else
        {
            //Newton Mechanics
            return position + startVelocity * time + 0.5f * acceleration * time * time;
        }
    }

    public static Vector3 GetPosition(Rigidbody rigidBody, Vector3 startVelocity, float time)
    {
        return GetPosition(rigidBody.transform.position, startVelocity, rigidBody.drag, time);
    }

    public static Vector3 GetStartVelocity(Vector3 startPosition, Vector3 targetPosition, float drag, float time)
    {
        Vector3 startVelocity;
        Vector3 positionDiff = targetPosition - startPosition;

        if (drag > 0.0f)
        {

            float frameDrag = 1.0f - drag * Time.fixedDeltaTime;
            float frames = time / Time.fixedDeltaTime;

            float frameDragLog = (float)Math.Log(frameDrag);
            float frameDragPower = Mathf.Pow(frameDrag, frames);

            float accDragFactor = (drag * Time.fixedDeltaTime - 1);
            accDragFactor *= (frameDragPower - frames * frameDragLog - 1.0f);
            accDragFactor /= (drag * frameDragLog);

            startVelocity = positionDiff - Physics.gravity * accDragFactor * Time.fixedDeltaTime;
            startVelocity /= ((frameDragPower - 1.0f) / frameDragLog) * Time.fixedDeltaTime;

            return startVelocity;
        }
        else
        {
            return (positionDiff - 0.5f * Physics.gravity * time * time) / time;
        }
    }

    public static Vector3 GetStartVelocity(Rigidbody rigidBody, Vector3 targetPosition, float time)
    {
        return GetStartVelocity(rigidBody.transform.position, targetPosition, rigidBody.drag, time);
    }

}

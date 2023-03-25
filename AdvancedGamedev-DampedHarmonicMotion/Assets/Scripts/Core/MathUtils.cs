using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MathUtils 
{

    /// <summary>
    /// Applies damped harmonic motion between two positions
    /// </summary>
    /// <param name="start">Start position</param>
    /// <param name="target">Target position</param>
    /// <param name="t">Time</param>
    /// <param name="freq">Frequency</param>
    /// <param name="epsilon">Error rate as maximal distance from target</param>
    /// <returns></returns>
    public static Vector3 DHM(Vector3 start, Vector3 target, float t, float freq = 3.0f, float epsilon = 0.01f)
    {
        t = Mathf.Clamp01(t);

        var dir = start - target;
        float distance = dir.magnitude;

        //See PDF
        float epsilonPercent = Mathf.Clamp01(epsilon / distance);
        float expDecay = Mathf.Pow(epsilonPercent, t);

        Vector3 shm = Mathf.Cos(Mathf.PI * 2.0f * freq * t) * dir;

        return target + shm * expDecay;
    }

}

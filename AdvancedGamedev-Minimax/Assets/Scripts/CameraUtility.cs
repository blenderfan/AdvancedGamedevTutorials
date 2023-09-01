using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CameraUtility 
{


    public static Ray CreateCameraRay(Camera camera, Vector2 screenPos)
    {
        if (camera != null)
        {
            var zPos = new Vector3(screenPos.x, screenPos.y, camera.nearClipPlane);

            //Unsure wether this issue only occurs in special editor cases, so I leave it here for safety
            if (float.IsFinite(zPos.x) && float.IsFinite(zPos.y) && float.IsFinite(zPos.z))
            {

                var worldPos = camera.ScreenToWorldPoint(zPos);
                var dir = worldPos - camera.transform.position;

                var ray = new Ray()
                {
                    origin = camera.transform.position,
                    direction = dir
                };
                return ray;
            }
        }
        return default;
    }

}

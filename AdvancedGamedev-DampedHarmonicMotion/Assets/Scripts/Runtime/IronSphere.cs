using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IronSphere : MonoBehaviour
{

    #region Public Variables

    public bool dampedHarmonicMotion = false;

    public Collider sphereCollider = null;

    public float chainSegmentLength = 0.1f;
    public float snapbackTime = 1.0f;
    public float dhmFrequency = 3.0f;

    public LineRenderer lr = null;

    #endregion

    #region Private Variables

    private bool interpolationInProgress = false;
    private bool dragStarted = false;

    private Camera mainCamera = null;

    private Coroutine interpolationCoroutine = null;

    private float dragDepth = 0.0f;

    private Vector3 dragMouseStart;
    private Vector3 dragStart;

    private Vector3 anchor;

    #endregion

    void Start()
    {
        this.mainCamera = FindObjectOfType<Camera>();
        this.dragStart = this.transform.position;

        this.anchor = this.transform.position;
    }

    private IEnumerator SnapBackToAnchorLerp()
    {
        this.interpolationInProgress = true;

        float timer = 0.0f;

        Vector3 startPos = this.transform.position;
        Vector3 endPos = this.anchor;

        while(timer < this.snapbackTime)
        {
            float percent = timer / this.snapbackTime;
            this.transform.position = Vector3.Lerp(startPos, endPos, percent);

            yield return null;

            timer += Time.deltaTime;
        }

        this.transform.position = this.anchor;
        this.interpolationInProgress = false;
    }

    private IEnumerator SnapBackToAnchorDHM()
    {

        this.interpolationInProgress = true;

        float timer = 0.0f;

        Vector3 startPos = this.transform.position;
        Vector3 endPos = this.anchor;

        while (timer < this.snapbackTime)
        {
            float percent = timer / this.snapbackTime;
            this.transform.position = MathUtils.DHM(startPos, endPos, percent, this.dhmFrequency);

            yield return null;

            timer += Time.deltaTime;
        }

        this.transform.position = this.anchor;
        this.interpolationInProgress = false;
    }

    void Update()
    {



        if(Input.GetMouseButtonDown(0))
        {
            var ray = this.mainCamera.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(ray, out var hitInfo, 1000.0f))
            {
                if(hitInfo.collider == this.sphereCollider)
                {
                    if (this.interpolationCoroutine != null && this.interpolationInProgress)
                    {
                        this.StopCoroutine(this.interpolationCoroutine);
                        this.interpolationInProgress = false;
                    }

                    this.dragDepth = hitInfo.distance;
                    this.dragMouseStart = new Vector3(Input.mousePosition.x, Input.mousePosition.y, this.dragDepth);
                    this.dragStart = this.transform.position;
                    this.dragStarted = true;
                }
            }
        }


        if (this.dragStarted)
        {
            if (Input.GetMouseButton(0))
            {
                var offset = this.mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, this.dragDepth)) - this.mainCamera.ScreenToWorldPoint(this.dragMouseStart);

                this.transform.position = this.dragStart + new Vector3(offset.x, offset.y, 0.0f);
            }
        }

        if(Input.GetMouseButtonUp(0) && !this.interpolationInProgress)
        {
            if (this.dampedHarmonicMotion)
            {
                this.interpolationCoroutine = this.StartCoroutine(this.SnapBackToAnchorDHM());
            } else {
                this.interpolationCoroutine = this.StartCoroutine(this.SnapBackToAnchorLerp());
            }
            this.dragStarted = false;
        }

        var dir = this.transform.position - this.anchor;
        var distance = dir.magnitude;
        int additionalPoints = (int)(distance / this.chainSegmentLength);
        var lrPositions = new Vector3[2 + additionalPoints];
        lrPositions[0] = this.anchor;
        for(int i = 0; i < additionalPoints; i++)
        {
            float percent = ((i + 1) / (float)(additionalPoints + 1));
            lrPositions[i + 1] = this.anchor + dir * percent;
        }
        lrPositions[lrPositions.Length - 1] = this.transform.position;

        this.lr.positionCount = 2 + additionalPoints;
        this.lr.SetPositions(lrPositions);
        
    }
}

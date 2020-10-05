using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeVisual : MonoBehaviour
{
    LineRenderer lr;

    List<RopeSegment> segments = new List<RopeSegment>();
    public float segmentLength = 0.25f;
    public int numSegments = 10;
    public float ropeWidth = 0.1f;
    public Color ropeColor = Color.white;

    public Vector2 gravity = new Vector2(0f, -1f);

    public Vector3 endPos = new Vector3();

    // Start is called before the first frame update
    void Start()
    {
        lr = GetComponent<LineRenderer>();
        //lr.useWorldSpace = false;

        Vector3 ropeStartPos = transform.position;

        for(int i = 0; i < numSegments; ++i)
        {
            segments.Add(new RopeSegment(ropeStartPos));
            ropeStartPos.y -= segmentLength;
        }
    }

    // Update is called once per frame
    void Update()
    {
        DrawRope();
    }

    void FixedUpdate()
    {
        Simulate();
    }

    void Simulate()
    {
        // SIMULATION
        for(int i = 0; i < numSegments; ++i)
        {
            RopeSegment a = segments[i];
            Vector2 velocity = a.posNow - a.posOld;
            a.posOld = a.posNow;
            a.posNow += velocity;
            a.posNow += gravity * Time.deltaTime;

            segments[i] = a;
        }

        // CONSTRAINTS
        for (int i = 0; i < 50; ++i)
        {
            ApplyConstraint();
        }
    }

    void ApplyConstraint()
    {
        // First point follows transform
        RopeSegment firstSegment = segments[0];
        firstSegment.posNow = transform.position;
        segments[0] = firstSegment;

        // Last point follows endPos
        RopeSegment lastSegment = segments[numSegments - 1];
        lastSegment.posNow = endPos;
        segments[numSegments - 1] = lastSegment;

        for (int i = 0; i < numSegments - 1; ++i)
        {
            RopeSegment a = segments[i];
            RopeSegment b = segments[i + 1];

            float distance = (a.posNow - b.posNow).magnitude;
            float error = distance - segmentLength;

            Vector2 changeDirection = (a.posNow - b.posNow).normalized;
            Vector2 changeAmount = changeDirection * error;

            if(i != 0)
            {
                a.posNow -= changeAmount * 0.5f;
                b.posNow += changeAmount * 0.5f;
            } else
            {
                b.posNow += changeAmount;
            }

            segments[i] = a;
            segments[i + 1] = b;
        }
    }

    void DrawRope()
    {
        lr.startWidth = ropeWidth;
        lr.endWidth = ropeWidth;

        lr.startColor = ropeColor;
        lr.endColor = ropeColor;

        lr.loop = false;

        Vector3[] ropePositions = new Vector3[numSegments];

        for(int i = 0; i < numSegments; ++i)
        {
            ropePositions[i] = segments[i].posNow;
        }

        lr.positionCount = numSegments;
        lr.SetPositions(ropePositions);


    }

    public struct RopeSegment
    {
        public Vector2 posNow;
        public Vector2 posOld;

        public RopeSegment(Vector2 pos)
        {
            this.posNow = pos;
            this.posOld = pos;
        }
    }
}

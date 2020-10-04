using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class AtomVisualController : MonoBehaviour
{
    LineRenderer lr;
    public float numSegmentsFactor = 64;
    private int numSegments = 8;
    public float pulseAmp = 0.25f;
    public float pulseFrequency = 1f;
    public float pulseDiff = 1f;
    public float iScale = 1f;

    List<float> wave = new List<float>();
    List<Vector3> circle = new List<Vector3>();
    List<Vector3> positions = new List<Vector3>();

    public bool updateParams = false;

    public float rotateSpeed = 1f;

    // Start is called before the first frame update
    void Start()
    {
        lr = GetComponent<LineRenderer>();

        if(lr == null)
        {
            Debug.LogError("Requires line renderer.");
        }

        CreateCircle(Time.time);
    }

    float FncA(float x)
    {
        return Mathf.Abs((x++ % 6) - 3) - 3;
    }

    void CreateCircle(float time)
    {
        positions = new List<Vector3>();

        numSegments = Mathf.RoundToInt(transform.localScale.x * 2f * Mathf.PI * numSegmentsFactor);

        if(numSegments < 0)
        {
            numSegments = 1;
        }

        if (circle.Count != numSegments + 1)
        {
            circle = new List<Vector3>();

            // Create circle
            for (int i = 0; i < numSegments + 1; ++i)
            {
                float rad = Mathf.Deg2Rad * (i * 360f / numSegments);

                circle.Add(new Vector3(Mathf.Sin(rad), Mathf.Cos(rad), 0));
            }
        }

        //if(wave.Count != numSegments + 1)
        {
            wave = new List<float>();

            // Create wave
            for (int i = 0; i < numSegments + 1; ++i)
            {
                wave.Add((Mathf.PerlinNoise(time * pulseFrequency, i * iScale) - 0.5f) * pulseAmp / transform.localScale.x + pulseDiff);
            }
        }

        List<Vector3> segmentPos = new List<Vector3>();

        // Create Line
        for(int i = 0; i < numSegments + 1; ++i)
        {
            segmentPos.Add(wave[i] * circle[i]);
        }

        lr.positionCount = numSegments + 1;
        lr.SetPositions(segmentPos.ToArray());
        lr.useWorldSpace = false;
        lr.loop = true;
    }

    // Update is called once per frame
    void Update()
    {
        CreateCircle(Time.time);
        updateParams = false;

        transform.Rotate(Vector3.forward, rotateSpeed * Time.deltaTime);
    }
}

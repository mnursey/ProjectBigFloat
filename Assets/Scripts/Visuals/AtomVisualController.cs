using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class AtomVisualController : MonoBehaviour
{
    LineRenderer lr;

    public int numSegments = 8;
    public float pulseAmp = 0.25f;
    public float pulseFrequency = 1f;
    public float pulseDiff = 1f;
    public float iScale = 1f;
    public float randomScale = 1f;

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

        List<float> segmentRadius = new List<float>();

        // Create Line
        for(int i = 0; i < numSegments + 1; ++i)
        {
            segmentRadius.Add(FncA(time * pulseFrequency * Mathf.Deg2Rad) * pulseAmp * FncA(i * iScale) + pulseDiff);
            segmentRadius[i] += Random.Range(-randomScale, randomScale);
        }

        // Create cirlce
        for(int i = 0; i < numSegments + 1; ++i)
        {
            float rad = Mathf.Deg2Rad * (i * 360f / numSegments);

            positions.Add(new Vector3(Mathf.Sin(rad) * segmentRadius[i], Mathf.Cos(rad) * segmentRadius[i], 0));
        }

        lr.positionCount = numSegments + 1;
        lr.SetPositions(positions.ToArray());
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

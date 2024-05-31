using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Splines;

[System.Serializable]
public class SplinePathData
{
    public SliceData[] slices;
}

[System.Serializable]
public class SliceData
{
    public int splineIndex;
    public SplineRange range;

    public bool isEnabled = true;
    public float sliceLength;
    public float distanceFromStart;
}

public class PlayerFollow : MonoBehaviour
{
    [SerializeField] SplineContainer container;
    [SerializeField] float speed = 0.04f;

    [SerializeField] SplinePathData pathData;

    SplinePath path;

    float progressRatio;
    float progress;
    float totalLength;

    void Start()
    {
        path = new SplinePath(CalculatePath());

        StartCoroutine(FollowCoroutine());
    }

    List<SplineSlice<Spline>> CalculatePath()
    {
        var localToWorldMatrix = container.transform.localToWorldMatrix;

        var enabledSlices = pathData.slices.Where(slice => slice.isEnabled).ToList();

        var slices = new List<SplineSlice<Spline>>();

        totalLength = 0f;
        foreach (var sliceData in enabledSlices)
        {
            var spline = container.Splines[sliceData.splineIndex];
            var slice = new SplineSlice<Spline>(spline, sliceData.range, localToWorldMatrix);
            slices.Add(slice);

            sliceData.distanceFromStart = totalLength;
            sliceData.sliceLength = slice.GetLength();
            totalLength += sliceData.sliceLength;
        }

        return slices;
    }

    IEnumerator FollowCoroutine()
    {
        // Loop forever
        for (var n = 0; ; ++n)
        {
            progressRatio = 0f;

            while (progressRatio <= 1f)
            {
                var pos = path.EvaluatePosition(progressRatio);
                var direction = path.EvaluateTangent(progressRatio);

                transform.position = pos;
                transform.LookAt(pos + direction);

                // 진행 비율 증가
                progressRatio += speed * Time.deltaTime;

                // 현재 이동한 거리 계산
                progress = progressRatio * totalLength;
                yield return null;
            }

            foreach (var sliceData in pathData.slices)
            {
                sliceData.isEnabled = true;
            }

            // 새로운 path 계산 
            path = new SplinePath(CalculatePath());
        }
    }
}

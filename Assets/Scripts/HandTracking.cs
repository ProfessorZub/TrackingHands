using UnityEngine;
using System.Collections;
using System;

public class HandTracking : MonoBehaviour
{
    private OVRHand hand;
    private OVRSkeleton handSkeleton;
    private OVRSkeleton.SkeletonType handType;

    private bool calibrationActive = false;

    private int[] tips =
    {
        (int)OVRSkeleton.BoneId.Hand_PinkyTip,
        (int)OVRSkeleton.BoneId.Hand_RingTip,
        (int)OVRSkeleton.BoneId.Hand_MiddleTip,
        (int)OVRSkeleton.BoneId.Hand_IndexTip,
        (int)OVRSkeleton.BoneId.Hand_ThumbTip
    };

    private float[] tipThresholds = new float[5];
    private float[] minFlexions = new float[5];
    private float[] maxFlexions = new float[5];
    bool allFingersCalibrated = false;
    private bool[] isFingerExtended = new bool[5];

    private int[] cycles = new int[5];

    private int handSum;
    public int HandSum
    {
        get { return handSum; }
    }


    void Start()
    {
        hand = GetComponent<OVRHand>();
        handSkeleton = GetComponent<OVRSkeleton>();

        // Read whether this is a right hand or left hand
        handType = handSkeleton.GetSkeletonType();
    }

    void Update()
    {
        if (!hand.IsTracked)
        {
            LogHUD("Not Tracking ...");
            return;
        }

        if (hand.IsTracked && !calibrationActive)
        {
            calibrationActive = true;
            StartCoroutine(CalibrateFingers());
        }


        foreach (int tip in tips)
        {
            int index = Array.IndexOf<int>(tips, tip);

            if (calibrationActive)
            {
                // During calibration, the GetFingerExtension method is responsible for adjusting the minFlexions and maxFlexions
                GetFingerExtension(index);
            }
            else
            {
                // Not in calibration, use tipThresholds to determine if finger is extended or not
                float extension = GetFingerExtension(index);
                isFingerExtended[index] = extension >= tipThresholds[index];
            }
        }

        if (!calibrationActive && allFingersCalibrated)
        {
            // Calculate the sum of the fingers
            handSum = 0;
            foreach (int tip in tips)
            {
                int index = Array.IndexOf(tips, tip);

                if (isFingerExtended[index])
                {
                    handSum += (int)Mathf.Pow(2, index);
                }
            }
            LogHUD("Sum of fingers: " + handSum);
        }

    }

    float GetFingerExtension(int _fingerIndex)
    {
        OVRBone bone = handSkeleton.Bones[(int)tips[_fingerIndex]];
        Vector3 distance = handSkeleton.transform.InverseTransformPoint(bone.Transform.position);
        Vector3 distAbs = Abs(distance);
        return bone.Id == OVRSkeleton.BoneId.Hand_ThumbTip ? distAbs.z : distAbs.x;
    }

    Vector3 Abs(Vector3 v)
    {
        return new Vector3(Mathf.Abs(v.x), Mathf.Abs(v.y), Mathf.Abs(v.z));
    }

    IEnumerator CalibrateFingers()
    {
        LogHUD("Calibrating...");

        for (int i = 0; i < 5; i++)
        {
            minFlexions[i] = float.MaxValue;
            maxFlexions[i] = float.MinValue;
            cycles[i] = 0;
        }
        float[] prevExtension = new float[5];

        while (!allFingersCalibrated)
        {
            allFingersCalibrated = true;
            for (int i = 0; i < 5; i++)
            {
                float extension = GetFingerExtension(i);
                minFlexions[i] = Mathf.Min(minFlexions[i], extension);
                maxFlexions[i] = Mathf.Max(maxFlexions[i], extension);
                tipThresholds[i] = (minFlexions[i] + maxFlexions[i]) / 2f;
                if (prevExtension[i] < tipThresholds[i] && extension >= tipThresholds[i])
                {
                    cycles[i]++;
                }
                else if (prevExtension[i] >= tipThresholds[i] && extension < tipThresholds[i])
                {
                    cycles[i]++;
                }
                prevExtension[i] = extension;
                if (cycles[i] < 6)
                {
                    allFingersCalibrated = false;
                }
            }
            yield return null;
        }
        calibrationActive = false;
    }

    void LogHUD(string message)
    {
        LogManager.Instance.Log(this.GetType() + ":" + handType + ": " + message);
    }
}

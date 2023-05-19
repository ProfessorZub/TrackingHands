using UnityEngine;
using System.Collections;
using System;

public class HandTracking : MonoBehaviour
{
    // Oculus Integration SDK objects for tracking hands.
    private OVRHand hand;
    private OVRSkeleton handSkeleton;
    private OVRSkeleton.SkeletonType handType;

    // ID's to identify the tip bone for each finger.
    private int[] tips =
    {
        (int)OVRSkeleton.BoneId.Hand_PinkyTip,
        (int)OVRSkeleton.BoneId.Hand_RingTip,
        (int)OVRSkeleton.BoneId.Hand_MiddleTip,
        (int)OVRSkeleton.BoneId.Hand_IndexTip,
        (int)OVRSkeleton.BoneId.Hand_ThumbTip
    };

    // Arrays to store calibration data for each finger's tip threshold, minimum and maximum flexion, and the number of calibration cycles.
    private float[] tipThresholds = new float[5];
    private float[] minFlexions = new float[5];
    private float[] maxFlexions = new float[5];
    private int[] cycles = new int[5];

    // Boolean flags to track the state of the calibration process
    private bool calibrationActive = false;
    private bool allFingersCalibrated = false;

    // Store the sum of all fingers interpreted as bits: finger extended -> 1  ; finger flexed -> 0.
    // Thumb is the most significant bit and pinky the least.
    private bool[] isFingerExtended = new bool[5];
    private int _handSum;
    public int HandSum
    {
        get { return _handSum; }
    }

    void Start()
    {
        // Get these components from the same game object this script is attached to.
        hand = GetComponent<OVRHand>();
        handSkeleton = GetComponent<OVRSkeleton>();

        // Read whether this is a right hand or left hand.
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
            // Get the OVRSkeleton id for the tip bone of the current finger.
            int id = Array.IndexOf<int>(tips, tip);

            if (calibrationActive)
            {
                // During calibration, the GetFingerExtension method is responsible for adjusting the minFlexions and maxFlexions.
                GetFingerExtension(id);
            }
            else
            {
                // Not in calibration, use tipThresholds to determine if finger is extended or not.
                float extension = GetFingerExtension(id);

                // store the corresponding booleans.
                isFingerExtended[id] = extension >= tipThresholds[id];
            }
        }

        if (!calibrationActive && allFingersCalibrated)
        {
            // Calculate the sum of the fingers.
            _handSum = 0;
            foreach (int tip in tips)
            {
                int id = Array.IndexOf(tips, tip);

                if (isFingerExtended[id])
                {
                    _handSum += (int)Mathf.Pow(2, id);
                }
            }
            LogHUD("Sum of fingers: " + _handSum);
        }

    }
    
    IEnumerator CalibrateFingers()
    {
        // Set up base flexion values per finger and initialize counter.
        for (int i = 0; i < 5; i++)
        {
            // min starts at max value, max starts at min and they evolve towards each other to find the mid-point for each finger.
            minFlexions[i] = float.MaxValue;
            maxFlexions[i] = float.MinValue;
            cycles[i] = 0;
        }

        
        float[] prevExtension = new float[5];

        while (!allFingersCalibrated)
        {
            // Each finger will get a chance to contradict that all fingers are calibrated.
            allFingersCalibrated = true;
            LogHUD("Calibrating...");

            for (int i = 0; i < 5; i++)
            {
                // Set new min and max flexions if current finger extension is smaller or greater than the current min and max.
                float extension = GetFingerExtension(i);
                minFlexions[i] = Mathf.Min(minFlexions[i], extension);
                maxFlexions[i] = Mathf.Max(maxFlexions[i], extension);

                // Threshold is the average of min and max flexion.
                tipThresholds[i] = (minFlexions[i] + maxFlexions[i]) / 2f;

                // Detect if the current extension crosses over the current thresholds and count that as a completed calibration cycle.
                if (prevExtension[i] < tipThresholds[i] && extension >= tipThresholds[i])
                {
                    cycles[i]++;
                }
                // Same detection for crossing under current threshold.
                else if (prevExtension[i] >= tipThresholds[i] && extension < tipThresholds[i])
                {
                    cycles[i]++;
                }

                // Update previous extensions.
                prevExtension[i] = extension;
                
                // Each finger can deny calibration complete and keep loop running.
                if (cycles[i] < 6)
                {
                    allFingersCalibrated = false;
                }
            }
            yield return null;
        }
        // All fingers calibrated at this point.
        calibrationActive = false;
    }


    float GetFingerExtension(int _fingerId)
    {
        // Get tip bone for current finger.
        OVRBone bone = handSkeleton.Bones[(int)tips[_fingerId]];

        // Calculate the position of tip bone relative to the handSkeleton object (i.e, the root bone of the hand, near the wrist).
        Vector3 distance = handSkeleton.transform.InverseTransformPoint(bone.Transform.position);

        Vector3 distAbs = Abs(distance);

        // The thumb flexes on a different plane than the rest of the fingers so we look at a different coordinate. 
        return bone.Id == OVRSkeleton.BoneId.Hand_ThumbTip ? distAbs.z : distAbs.x;
    }

    Vector3 Abs(Vector3 v)
    {
        return new Vector3(Mathf.Abs(v.x), Mathf.Abs(v.y), Mathf.Abs(v.z));
    }

    void LogHUD(string message)
    {
        LogManager.Instance.Log(this.GetType() + ":" + handType + ": " + message);
    }
}

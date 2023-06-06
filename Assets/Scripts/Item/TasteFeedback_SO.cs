using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Taste_Feedback_SO", menuName = "BlindDemo/Taste/Taste_Feedback_SO")]
public class TasteFeedback_SO : ScriptableObject
{
    public List<FeedbackRange> feedbackRanges;

    public string GetFeedbackByRange(uint valueIn)
    {
        foreach (FeedbackRange range in feedbackRanges)
        {
            if (valueIn < range.rangeCeiling)
            {
                return range.feedback;
            }
        }
        return "no feedback";
    }
}

[System.Serializable]
public class FeedbackRange
{
    public uint rangeCeiling;
    public string feedback;
}

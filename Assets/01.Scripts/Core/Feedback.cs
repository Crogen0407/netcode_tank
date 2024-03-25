using UnityEngine;

public abstract class Feedback : MonoBehaviour
{
    public abstract void CreateFeedback();
    public abstract void CompleteFeedback();

    protected void OnDestroy()
    {
        CompleteFeedback();
    }
}

using System.Collections;
using System.Collections.Generic;
#if UNITY_ANDROID
using Google.Play.Review;
#endif

using UnityEngine;
#if UNITY_IOS
using UnityEngine.iOS;
#endif

public class InAppReviewController : Singleton<InAppReviewController>
{
    public void ReviewRequest()
    {
#if UNITY_IOS
        ReviewRequestForiOS();

#elif UNITY_ANDROID
    StartCoroutine(AnroidReviewRequest());
#endif
    }

    void ReviewRequestForiOS()
    {
#if UNITY_IOS
        Device.RequestStoreReview();
#endif
    }

    IEnumerator AnroidReviewRequest()
    {
        yield return null;

#if UNITY_ANDROID

        ReviewManager reviewManager = new ReviewManager();

        var requestFlowOperation = reviewManager.RequestReviewFlow();
        yield return requestFlowOperation;
        if (requestFlowOperation.Error != ReviewErrorCode.NoError)
        {
            // Log error. For example, using requestFlowOperation.Error.ToString().
            yield break;
        }
        var _playReviewInfo = requestFlowOperation.GetResult();

        var launchFlowOperation = reviewManager.LaunchReviewFlow(_playReviewInfo);
        yield return launchFlowOperation;
        _playReviewInfo = null; // Reset the object
        if (launchFlowOperation.Error != ReviewErrorCode.NoError)
        {
            // Log error. For example, using requestFlowOperation.Error.ToString().
            yield break;
        }
        // The flow has finished. The API does not indicate whether the user
        // reviewed or not, or even whether the review dialog was shown. Thus, no
        // matter the result, we continue our app flow.
#endif

    }
}

using System.Collections;
using UnityEngine;

public delegate bool delegateGetFlag();

public delegate void delegateLoadScene(string sceneName);
public delegate void delegateActiveFlag();

public delegate void delegatePurchaseResult(EPurchaseErrorType ePurchaseErrorType);

public class CMDelegate
{
    public static IEnumerator CoroutineThreadWait(delegateGetFlag delegateGetFlag)
    {
        const float kMaxThreadWaitTime = 15.0f;
        float threadWaitTime = 0.0f;
        while (delegateGetFlag() == false)
        {
            threadWaitTime += Time.deltaTime;
            yield return null;

            if (threadWaitTime >= kMaxThreadWaitTime)
            {
                yield break;
            }
        }
    }
}

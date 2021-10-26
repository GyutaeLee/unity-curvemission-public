using UnityEngine;
using System.Collections;

using Services.Enum.Shop;
using Services.Delegate;

namespace Services.Thread
{
    public static class Waiter
    {
        private static class Constants
        {
            public const float MaxThreadWaitTime = 15.0f;
        }

        private static bool thread_wait_isCompleted;
        private static bool thread_wait_isPurchaseResultCompleted;

        public static PurchaseResultType Thread_PurchaseResultType { get; private set; }

        public static void ActiveThreadWait()
        {
            thread_wait_isCompleted = true;
        }

        public static void InActiveThreadWait()
        {
            thread_wait_isCompleted = false;
        }

        public static bool GetThreadWaitIsCompleted()
        {
            return thread_wait_isCompleted;
        }

        public static void ActiveThreadWaitPurchaseResult(PurchaseResultType purchaseResultType)
        {
            thread_wait_isPurchaseResultCompleted = true;
            Thread_PurchaseResultType = purchaseResultType;
        }

        public static void InActiveThreadWaitPurchaseResult()
        {
            thread_wait_isPurchaseResultCompleted = false;
            Thread_PurchaseResultType = PurchaseResultType.None;
        }

        public static bool GetThreadWaitIsPurchaseResultCompleted()
        {
            return thread_wait_isPurchaseResultCompleted;
        }

        public static IEnumerator CoroutineThreadWait(delegateGetFlag delegateGetFlag)
        {
            float threadWaitTime = 0.0f;
            while (delegateGetFlag() == false)
            {
                threadWaitTime += Time.deltaTime;
                yield return null;

                if (threadWaitTime >= Constants.MaxThreadWaitTime)
                {
                    yield break;
                }
            }
        }
    }
}
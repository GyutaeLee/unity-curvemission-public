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
        private static bool thread_wait_isServerRequestResultCompleted;

        public static PurchaseResultType Thread_PurchaseResultType { get; private set; }
        public static Enum.RequestResult.Server Thread_ServerRequestResult { get; private set; }

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

        public static void ActiveThreadWaitServerRequestResult(Enum.RequestResult.Server serverRequestResult)
        {
            thread_wait_isServerRequestResultCompleted = true;
            Thread_ServerRequestResult = serverRequestResult;
        }

        public static void InActiveThreadWaitServerRequestResult()
        {
            thread_wait_isServerRequestResultCompleted = false;
            Thread_ServerRequestResult = Enum.RequestResult.Server.None;
        }

        public static bool GetThreadWaitIsPurchaseResultCompleted()
        {
            return thread_wait_isPurchaseResultCompleted;
        }

        public static bool GetThreadWaitIsServerRequestResultCompleted()
        {
            return thread_wait_isServerRequestResultCompleted;
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
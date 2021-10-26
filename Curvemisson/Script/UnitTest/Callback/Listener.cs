using NUnit.Framework;

using UnityEngine;
using UnityEditor.TestTools.TestRunner.Api;

namespace UnitTest.Callback
{
    public class Listener : ICallbacks
    {
        public void RunFinished(ITestResultAdaptor result)
        {
            Debug.Log("RunFinished");
        }

        public void RunStarted(ITestAdaptor testsToRun)
        {
            Debug.Log("RunStarted");
        }

        public void TestFinished(ITestResultAdaptor result)
        {
            Debug.Log("TestFinished");
        }

        public void TestStarted(ITestAdaptor test)
        {
            Debug.Log("TestStarted");
        }
    }
}
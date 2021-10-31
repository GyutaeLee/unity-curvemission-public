using NUnit.Framework;

using UnityEngine;
using UnityEditor.TestTools.TestRunner.Api;

namespace TestAutomation.Callback
{
    public class Registrant
    {
        [Test]
        public void SetupListener()
        {
            Debug.Log("Start Register Listener");

            var api = ScriptableObject.CreateInstance<TestRunnerApi>();
            api.RegisterCallbacks(new Listener());

            Debug.Log("Register Listener Success");
        }
    }
}
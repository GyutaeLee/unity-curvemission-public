using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace UnitTest
{
    public class AllTest
    {
        public static bool IsAllTest = false;

        [Test]
        public void StartTest()
        {
            Debug.Log("Start All Test");

            IsAllTest = true;
            SceneManager.LoadScene(Services.Constants.SceneName.Logo);
        }
    }
}

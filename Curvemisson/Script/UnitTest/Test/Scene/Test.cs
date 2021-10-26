using System.Collections;
using NUnit.Framework;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace UnitTest.Scene
{    
    public enum SceneState
    {
        Ready = 0,
        Active = 1,
        InActive = 2
    }

    public abstract class Test
    {
        /*
         * 플랫폼 제어 코드
         * [UnityPlatform(RuntimePlatform.WindowsEditor)]
         */

        /*
         * 테스트 코드
         * Assert.AreEqual("MyGameObject", go.name);
         * Assert.AreNotEqual(originalPosition, go.transform.position.y);
         * Assert.IsTrue(utility.isSuccess);
         * LogAssert.Expect(LogType.Log, ("Log Message")); Debug.Log("Log Message");
         */

        protected SceneState SceneState;

        [Test]
        public void StartTestLogic()
        {
            if (AllTest.IsAllTest == true)
                return;

            AwakeInUnitTest();
        }

        public abstract void AwakeInUnitTest();
    }
}
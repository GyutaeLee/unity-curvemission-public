using System.Collections;

using UnityEngine;
using UnityEngine.SceneManagement;

namespace TestAutomation.Scene
{
    public abstract class AbstractClass
    {
        private GameObject _canvas;
        protected GameObject canvas
        {
            get
            {
                if (this._canvas == null)
                {
                    this._canvas = GameObject.Find("Canvas");
                }
                return this._canvas;
            }
        }

        private GameObject _script;
        protected GameObject script
        {
            get
            {
                if (this._script == null)
                {
                    this._script = GameObject.Find("Script");
                }
                return this._script;
            }
        }

        protected IEnumerator Preparation(string sceneName)
        {
            yield return Test.PreparationForTest();

            SceneManager.LoadScene(sceneName);
            yield return Test.WaitingScene(sceneName);
        }

        public IEnumerator SceneTest(string sceneName)
        {
            yield return Preparation(sceneName);
            yield return FullTest();
        }

        public abstract IEnumerator FullTest();
    }
}
using System;
using System.Collections;
using System.Reflection;
using System.Text.RegularExpressions;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace FlowState.Tests.PlayMode
{
    public class StageGoalIntegrationTests
    {
        private const string SceneName = "SampleScene";

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            AsyncOperation loadOperation = SceneManager.LoadSceneAsync(
                SceneName,
                LoadSceneMode.Single);

            while (!loadOperation.isDone)
            {
                yield return null;
            }

            yield return null;
            yield return new WaitForFixedUpdate();
        }

        [UnityTest]
        public IEnumerator PlayerEntersGoal_ClearsStageAndEndsGameOnce()
        {
            GameObject player = FindSceneGameObject("Player");
            GameObject goal = FindSceneGameObject("Goal");
            Rigidbody playerRigidbody = player.GetComponent<Rigidbody>();
            MonoBehaviour gameSystem = FindRequiredBehaviour(
                "GameSystem",
                "GameSystem");
            MonoBehaviour stageSystem = FindRequiredBehaviour(
                "StageSystem",
                "StageSystem");
            MonoBehaviour resultSystem = FindRequiredBehaviour(
                "ResultSystem",
                "ResultSystem");
            MonoBehaviour clearTimeText = FindRequiredBehaviour(
                "Clear Time Text",
                "TextMeshProUGUI");
            GameObject stageHud = FindSceneGameObject("StageHUD");
            GameObject resultPanel = FindSceneGameObject("ResultPanel");
            int clearCount = 0;
            int endCount = 0;

            AddListener(
                stageSystem,
                "AddStageClearedListener",
                () => clearCount++);
            AddListener(
                stageSystem,
                "AddStageEndedListener",
                () => endCount++);

            yield return new WaitForSeconds(0.02f);

            player.transform.position = goal.transform.position;
            playerRigidbody.linearVelocity = new Vector3(8.0f, 4.0f, 0.0f);
            Physics.SyncTransforms();

            yield return new WaitForFixedUpdate();
            yield return null;

            Assert.That(
                GetProperty<bool>(stageSystem, "IsPlaying"),
                Is.False);
            Assert.That(
                GetProperty<bool>(stageSystem, "IsCleared"),
                Is.True);
            Assert.That(
                GetProperty<bool>(stageSystem, "HasEnded"),
                Is.True);
            Assert.That(clearCount, Is.EqualTo(1));
            Assert.That(endCount, Is.EqualTo(1));
            Assert.That(
                GetProperty<bool>(resultSystem, "HasResultData"),
                Is.True);
            object resultData =
                GetProperty<object>(resultSystem, "CurrentResultData");
            Assert.That(resultData, Is.Not.Null);
            Assert.That(
                GetObjectProperty<double>(resultData, "ClearTime"),
                Is.GreaterThan(0.0));
            string resultText =
                GetProperty<string>(clearTimeText, "text");
            Assert.That(
                Regex.IsMatch(
                    resultText,
                    @"^Clear Time: \d+\.\d{3} s$"),
                Is.True);
            Assert.That(
                GetProperty<object>(gameSystem, "CurrentGameState").ToString(),
                Is.EqualTo("Ended"));
            Assert.That(stageHud.activeSelf, Is.False);
            Assert.That(resultPanel.activeSelf, Is.True);
            Assert.That(playerRigidbody.linearVelocity, Is.EqualTo(Vector3.zero));
            Assert.That(playerRigidbody.angularVelocity, Is.EqualTo(Vector3.zero));

            yield return new WaitForSeconds(0.1f);

            Assert.That(
                GetProperty<string>(clearTimeText, "text"),
                Is.EqualTo(resultText));
        }

        [UnityTest]
        public IEnumerator StartGame_AfterGoalClear_RestoresNewStagePlay()
        {
            GameObject player = FindSceneGameObject("Player");
            GameObject goal = FindSceneGameObject("Goal");
            GameObject startPoint = FindSceneGameObject("StartPoint");
            Rigidbody playerRigidbody = player.GetComponent<Rigidbody>();
            MonoBehaviour gameSystem = FindRequiredBehaviour(
                "GameSystem",
                "GameSystem");
            MonoBehaviour stageSystem = FindRequiredBehaviour(
                "StageSystem",
                "StageSystem");
            MonoBehaviour resultSystem = FindRequiredBehaviour(
                "ResultSystem",
                "ResultSystem");

            player.transform.position = goal.transform.position;
            Physics.SyncTransforms();
            yield return new WaitForFixedUpdate();
            yield return null;

            InvokePublicMethod(gameSystem, "StartGame");
            yield return null;
            yield return new WaitForFixedUpdate();

            Assert.That(
                GetProperty<object>(gameSystem, "CurrentGameState").ToString(),
                Is.EqualTo("Playing"));
            Assert.That(
                GetProperty<bool>(stageSystem, "IsPlaying"),
                Is.True);
            Assert.That(
                GetProperty<bool>(stageSystem, "IsCleared"),
                Is.False);
            Assert.That(
                GetProperty<bool>(stageSystem, "HasEnded"),
                Is.False);
            Assert.That(
                GetProperty<bool>(resultSystem, "HasResultData"),
                Is.False);
            Assert.That(
                player.transform.position,
                Is.EqualTo(startPoint.transform.position));
            Assert.That(playerRigidbody.linearVelocity, Is.EqualTo(Vector3.zero));
        }

        private GameObject FindSceneGameObject(string gameObjectName)
        {
            GameObject[] gameObjects =
                Resources.FindObjectsOfTypeAll<GameObject>();

            foreach (GameObject gameObject in gameObjects)
            {
                if (gameObject.name == gameObjectName &&
                    gameObject.scene.IsValid() &&
                    gameObject.scene.isLoaded)
                {
                    return gameObject;
                }
            }

            Assert.Fail($"{gameObjectName} was not found in the loaded Scene.");
            return null;
        }

        private MonoBehaviour FindRequiredBehaviour(
            string gameObjectName,
            string typeName)
        {
            GameObject targetObject = FindSceneGameObject(gameObjectName);

            foreach (MonoBehaviour behaviour in
                     targetObject.GetComponents<MonoBehaviour>())
            {
                if (behaviour != null && behaviour.GetType().Name == typeName)
                {
                    return behaviour;
                }
            }

            Assert.Fail(
                $"{typeName} was not found on {gameObjectName}.");
            return null;
        }

        private void AddListener(
            MonoBehaviour targetBehaviour,
            string methodName,
            Action listener)
        {
            MethodInfo method = targetBehaviour.GetType().GetMethod(
                methodName,
                BindingFlags.Instance | BindingFlags.Public);

            Assert.That(method, Is.Not.Null);
            method.Invoke(targetBehaviour, new object[] { listener });
        }

        private T GetProperty<T>(
            MonoBehaviour targetBehaviour,
            string propertyName)
        {
            PropertyInfo property = targetBehaviour.GetType().GetProperty(
                propertyName,
                BindingFlags.Instance | BindingFlags.Public);

            Assert.That(property, Is.Not.Null);
            return (T)property.GetValue(targetBehaviour);
        }

        private T GetObjectProperty<T>(
            object target,
            string propertyName)
        {
            PropertyInfo property = target.GetType().GetProperty(
                propertyName,
                BindingFlags.Instance | BindingFlags.Public);

            Assert.That(property, Is.Not.Null);
            return (T)property.GetValue(target);
        }

        private void InvokePublicMethod(
            MonoBehaviour targetBehaviour,
            string methodName)
        {
            MethodInfo method = targetBehaviour.GetType().GetMethod(
                methodName,
                BindingFlags.Instance | BindingFlags.Public);

            Assert.That(method, Is.Not.Null);
            method.Invoke(targetBehaviour, null);
        }
    }
}

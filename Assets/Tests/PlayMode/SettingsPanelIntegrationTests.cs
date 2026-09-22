using System.Collections;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace FlowState.Tests.PlayMode
{
    public class SettingsPanelIntegrationTests
    {
        [UnityTest]
        public IEnumerator SettingsPanel_ConfirmationAndRebindButtonsAreWired()
        {
            yield return SceneManager.LoadSceneAsync("SampleScene", LoadSceneMode.Single);
            yield return null;

            MonoBehaviour controller = FindBehaviour("SettingsWindow", "SettingsUIController");
            Assert.That(controller, Is.Not.Null);

            GameObject window = Find("SettingsWindow");
            GameObject confirmation = GetPrivateField<GameObject>(
                controller,
                "_restoreDefaultsConfirmationPanel");
            Button restore = GetPrivateField<Button>(
                controller,
                "_restoreDefaultsButton");
            Button confirm = GetPrivateField<Button>(
                controller,
                "_restoreDefaultsConfirmButton");
            Assert.That(confirmation, Is.Not.Null);
            Assert.That(restore, Is.Not.Null);
            Assert.That(confirm, Is.Not.Null);
            Assert.That(confirmation.transform.parent, Is.EqualTo(window.transform));
            Assert.That(confirmation.activeSelf, Is.False);

            restore.onClick.Invoke();
            Assert.That(confirmation.activeSelf, Is.True);

            Button cancel = FindUniqueOtherButton(confirmation, confirm);
            cancel.onClick.Invoke();
            Assert.That(confirmation.activeSelf, Is.False);

            Button[] rebindButtons = GetPrivateField<Button[]>(
                controller,
                "_rebindButtons");
            Assert.That(rebindButtons, Has.Length.EqualTo(6));

            foreach (Button rebindButton in rebindButtons)
            {
                Assert.That(rebindButton, Is.Not.Null);
                Assert.That(rebindButton.transform.IsChildOf(window.transform), Is.True);
                Assert.That(rebindButton.onClick.GetPersistentEventCount(), Is.EqualTo(1));
            }
        }

        private static GameObject Find(string name)
        {
            foreach (GameObject gameObject in Resources.FindObjectsOfTypeAll<GameObject>())
                if (gameObject.name == name && gameObject.scene.isLoaded) return gameObject;
            Assert.Fail($"{name} was not found.");
            return null;
        }

        private static Button FindUniqueOtherButton(
            GameObject root,
            Button excludedButton)
        {
            Button match = null;
            foreach (Button button in root.GetComponentsInChildren<Button>(true))
            {
                if (button == excludedButton)
                {
                    continue;
                }

                Assert.That(match, Is.Null,
                    $"{root.name} must have exactly one cancel Button.");
                match = button;
            }

            Assert.That(match, Is.Not.Null,
                $"{root.name} must have one cancel Button.");
            return match;
        }

        private static T GetPrivateField<T>(MonoBehaviour component, string fieldName)
        {
            FieldInfo field = component.GetType().GetField(
                fieldName,
                BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.That(field, Is.Not.Null);
            return (T)field.GetValue(component);
        }

        private static MonoBehaviour FindBehaviour(string objectName, string typeName)
        {
            foreach (MonoBehaviour behaviour in Find(objectName).GetComponents<MonoBehaviour>())
                if (behaviour != null && behaviour.GetType().Name == typeName) return behaviour;
            Assert.Fail($"{typeName} was not found on {objectName}.");
            return null;
        }
    }
}

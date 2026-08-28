using System;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;

namespace FlowState.Tests.PlayMode
{
    public class UIInputSystemTests
    {
        private GameObject _systemObject;
        private MonoBehaviour _uiInputSystem;

        [SetUp]
        public void SetUp()
        {
            Type systemType = FindType("FlowState.Runtime.Systems.UIInputSystem");
            _systemObject = new GameObject("UIInputSystemTest");
            _uiInputSystem = (MonoBehaviour)_systemObject.AddComponent(systemType);
        }

        [TearDown]
        public void TearDown()
        {
            if (_systemObject != null)
            {
                UnityEngine.Object.DestroyImmediate(_systemObject);
            }
        }

        [Test]
        public void Initialize_LeavesUIActionMapDisabled()
        {
            InvokePublicMethod("Initialize");

            Assert.That(GetIsUIActionMapEnabled(), Is.False);
        }

        [Test]
        public void EnableAndDisableUIActionMap_ChangesEnabledState()
        {
            InvokePublicMethod("EnableUIActionMap");

            Assert.That(GetIsUIActionMapEnabled(), Is.True);

            InvokePublicMethod("DisableUIActionMap");

            Assert.That(GetIsUIActionMapEnabled(), Is.False);
        }

        [Test]
        public void RepeatedInitializeAndEnable_DoesNotChangeExpectedState()
        {
            InvokePublicMethod("Initialize");
            InvokePublicMethod("Initialize");
            InvokePublicMethod("EnableUIActionMap");
            InvokePublicMethod("EnableUIActionMap");

            Assert.That(GetIsUIActionMapEnabled(), Is.True);
        }

        private bool GetIsUIActionMapEnabled()
        {
            PropertyInfo property = _uiInputSystem.GetType().GetProperty(
                "IsUIActionMapEnabled",
                BindingFlags.Instance | BindingFlags.Public);

            Assert.That(property, Is.Not.Null);
            return (bool)property.GetValue(_uiInputSystem);
        }

        private void InvokePublicMethod(string methodName)
        {
            MethodInfo method = _uiInputSystem.GetType().GetMethod(
                methodName,
                BindingFlags.Instance | BindingFlags.Public);

            Assert.That(method, Is.Not.Null);
            method.Invoke(_uiInputSystem, null);
        }

        private Type FindType(string fullTypeName)
        {
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                Type type = assembly.GetType(fullTypeName);

                if (type != null)
                {
                    return type;
                }
            }

            Assert.Fail($"{fullTypeName} was not found.");
            return null;
        }
    }
}

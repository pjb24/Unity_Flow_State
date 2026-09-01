using System;
using System.Reflection;
using FlowState.Runtime.Core;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace FlowState.Tests.EditMode
{
    public class ResultSystemTests
    {
        private GameObject _systemObject;
        private MonoBehaviour _resultSystem;

        [SetUp]
        public void SetUp()
        {
            _systemObject = new GameObject("ResultSystemTests.System");
            Type resultSystemType = FindType(
                "FlowState.Runtime.Systems.ResultSystem");
            Assert.That(resultSystemType, Is.Not.Null);
            _resultSystem =
                (MonoBehaviour)_systemObject.AddComponent(resultSystemType);
            Invoke("Initialize");
        }

        [TearDown]
        public void TearDown()
        {
            UnityEngine.Object.DestroyImmediate(_systemObject);
        }

        [Test]
        public void CreateStageThenInfiniteResult_SecondModeIsRejected()
        {
            Assert.That(
                InvokeBool("CreateResultData", true, 12.5),
                Is.True);
            LogAssert.Expect(
                LogType.Warning,
                "[ResultSystem] Infinite Result Data was not created.");

            bool didCreateInfinite = InvokeBool(
                "CreateInfiniteResultData",
                E_GameMode.Infinite,
                true,
                true,
                100.0f,
                1000);

            Assert.That(didCreateInfinite, Is.False);
            Assert.That(GetResultData().GameMode, Is.EqualTo(E_GameMode.Stage));
        }

        [Test]
        public void CreateInfiniteThenStageResult_SecondModeIsRejected()
        {
            Assert.That(
                InvokeBool(
                    "CreateInfiniteResultData",
                    E_GameMode.Infinite,
                    true,
                    true,
                    100.0f,
                    1000),
                Is.True);
            LogAssert.Expect(
                LogType.Warning,
                "[ResultSystem] Result Data was not created.");

            bool didCreateStage = InvokeBool(
                "CreateResultData",
                true,
                12.5);

            Assert.That(didCreateStage, Is.False);
            Assert.That(
                GetResultData().GameMode,
                Is.EqualTo(E_GameMode.Infinite));
        }

        [Test]
        public void Initialize_ExistingInfiniteResult_ClearsBothRecordPaths()
        {
            Assert.That(
                InvokeBool(
                    "CreateInfiniteResultData",
                    E_GameMode.Infinite,
                    true,
                    true,
                    100.0f,
                    1000),
                Is.True);

            Invoke("Initialize");

            Assert.That(GetBoolProperty("HasResultData"), Is.False);
            Assert.That(GetProperty("CurrentResultData"), Is.Null);
            Assert.That(
                InvokeBool("CreateResultData", true, 12.5),
                Is.True);
        }

        private Type FindType(string fullName)
        {
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                Type type = assembly.GetType(fullName);

                if (type != null)
                {
                    return type;
                }
            }

            return null;
        }

        private ResultData GetResultData()
        {
            return (ResultData)GetProperty("CurrentResultData");
        }

        private bool GetBoolProperty(string propertyName)
        {
            return (bool)GetProperty(propertyName);
        }

        private object GetProperty(string propertyName)
        {
            PropertyInfo property = _resultSystem.GetType().GetProperty(
                propertyName,
                BindingFlags.Instance | BindingFlags.Public);
            Assert.That(property, Is.Not.Null);
            return property.GetValue(_resultSystem);
        }

        private bool InvokeBool(string methodName, params object[] arguments)
        {
            return (bool)Invoke(methodName, arguments);
        }

        private object Invoke(string methodName, params object[] arguments)
        {
            MethodInfo method = _resultSystem.GetType().GetMethod(
                methodName,
                BindingFlags.Instance | BindingFlags.Public);
            Assert.That(method, Is.Not.Null);
            return method.Invoke(_resultSystem, arguments);
        }
    }
}

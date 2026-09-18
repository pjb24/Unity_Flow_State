using System;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;

namespace FlowState.Tests.EditMode
{
    public class CameraWorldRebaseTests
    {
        private const BindingFlags InstanceMembers =
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        private GameObject _cameraSystemObject;
        private GameObject _cameraRigObject;
        private MonoBehaviour _cameraSystem;
        private Transform _followTarget;
        private Behaviour _cinemachineCamera;

        [SetUp]
        public void SetUp()
        {
            _cameraSystemObject = new GameObject("CameraSystem");
            _cameraRigObject = new GameObject("CameraRig");
            _cameraRigObject.transform.position =
                new Vector3(30.0f, 4.0f, -2.0f);
            _followTarget = new GameObject("CameraFollowTarget").transform;
            _followTarget.SetParent(_cameraRigObject.transform, false);
            _followTarget.localPosition = new Vector3(0.0f, 2.0f, 0.0f);
            _cinemachineCamera = AddCinemachineCamera();
            _cinemachineCamera.transform.SetParent(_cameraRigObject.transform, false);
            _cameraSystem = AddSystem("CameraSystem");
            SetField(_cameraSystem, "_cinemachineCamera", _cinemachineCamera);
            SetField(_cameraSystem, "_followTarget", _followTarget);
            Assert.That(Invoke(_cameraSystem, "Initialize"), Is.True);
        }

        [TearDown]
        public void TearDown()
        {
            UnityEngine.Object.DestroyImmediate(_cameraSystemObject);
            UnityEngine.Object.DestroyImmediate(_cameraRigObject);
        }

        [Test]
        public void ApplyWorldRebaseOffset_MovesCameraRigAndPreservesCameraState()
        {
            Vector3 rigPosition = _cameraRigObject.transform.position;
            Vector3 followLocalPosition = _followTarget.localPosition;
            bool isEnabled = _cinemachineCamera.enabled;

            Assert.That(Invoke(
                _cameraSystem, "TryApplyWorldRebaseOffset", -880.0f), Is.True);
            Assert.That(Invoke(
                _cameraSystem,
                "TryNotifyWorldRebase",
                new Vector3(-880.0f, 0.0f, 0.0f)), Is.True);

            Assert.That(_cameraRigObject.transform.position.x,
                Is.EqualTo(rigPosition.x - 880.0f));
            Assert.That(_cameraRigObject.transform.position.y,
                Is.EqualTo(rigPosition.y));
            Assert.That(_cameraRigObject.transform.position.z,
                Is.EqualTo(rigPosition.z));
            Assert.That(_followTarget.localPosition, Is.EqualTo(followLocalPosition));
            Assert.That(_cinemachineCamera.enabled, Is.EqualTo(isEnabled));
        }

        [TestCase(0.0f)]
        [TestCase(1.0f)]
        [TestCase(float.NaN)]
        [TestCase(float.PositiveInfinity)]
        public void ApplyWorldRebaseOffset_InvalidOffsetLeavesCameraRigUnchanged(
            float worldXOffset)
        {
            Vector3 rigPosition = _cameraRigObject.transform.position;

            Assert.That(Invoke(
                _cameraSystem,
                "TryApplyWorldRebaseOffset",
                worldXOffset), Is.False);
            Assert.That(_cameraRigObject.transform.position,
                Is.EqualTo(rigPosition));
        }

        private static void SetField(object target, string fieldName, object value)
        {
            FieldInfo field = target.GetType().GetField(fieldName, InstanceMembers);
            Assert.That(field, Is.Not.Null);
            field.SetValue(target, value);
        }

        private MonoBehaviour AddSystem(string typeName)
        {
            string fullName = "FlowState.Runtime.Systems." + typeName;

            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                Type type = assembly.GetType(fullName);

                if (type != null)
                {
                    return (MonoBehaviour)_cameraSystemObject.AddComponent(type);
                }
            }

            Assert.Fail("Production System type was not found: " + fullName);
            return null;
        }

        private Behaviour AddCinemachineCamera()
        {
            const string typeName = "Unity.Cinemachine.CinemachineCamera";

            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                Type type = assembly.GetType(typeName);

                if (type != null)
                {
                    GameObject cameraObject = new GameObject("CinemachineCamera");
                    return (Behaviour)cameraObject.AddComponent(type);
                }
            }

            Assert.Fail("CinemachineCamera type was not found.");
            return null;
        }

        private static object Invoke(
            object target, string methodName, params object[] arguments)
        {
            Type[] argumentTypes = new Type[arguments.Length];

            for (int index = 0; index < arguments.Length; index++)
            {
                argumentTypes[index] = arguments[index].GetType();
            }

            MethodInfo method = target.GetType().GetMethod(
                methodName, InstanceMembers, null, argumentTypes, null);
            Assert.That(method, Is.Not.Null);
            return method.Invoke(target, arguments);
        }
    }
}

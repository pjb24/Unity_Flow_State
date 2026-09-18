using System;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;

namespace FlowState.Tests.EditMode
{
    public class PlayerWorldRebaseTests
    {
        private const BindingFlags InstanceMembers =
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        private GameObject _playerObject;
        private GameObject _startPointObject;
        private Rigidbody _rigidbody;
        private MonoBehaviour _system;

        [SetUp]
        public void SetUp()
        {
            _playerObject = new GameObject("Player");
            _startPointObject = new GameObject("StartPoint");
            _startPointObject.transform.position =
                new Vector3(10.0f, 2.0f, 3.0f);
            _rigidbody = _playerObject.AddComponent<Rigidbody>();
            _system = AddSystem("PlayerControllerSystem");
            SetField(_system, "_playerRigidbody", _rigidbody);
            SetField(_system, "_startPoint", _startPointObject.transform);
            Assert.That(Invoke(_system, "Initialize"), Is.True);
        }

        [TearDown]
        public void TearDown()
        {
            UnityEngine.Object.DestroyImmediate(_playerObject);
            UnityEngine.Object.DestroyImmediate(_startPointObject);
        }

        [Test]
        public void ApplyWorldRebaseOffset_ChangesOnlyXAndPreservesPhysicsState()
        {
            _rigidbody.linearVelocity = new Vector3(7.0f, -4.0f, 0.0f);
            _rigidbody.angularVelocity = new Vector3(1.0f, 2.0f, 3.0f);
            _rigidbody.rotation = Quaternion.Euler(10.0f, 20.0f, 30.0f);
            RigidbodyConstraints constraints = _rigidbody.constraints;
            Vector3 originalPosition = _rigidbody.position;
            Vector3 velocity = _rigidbody.linearVelocity;
            Vector3 angularVelocity = _rigidbody.angularVelocity;
            Quaternion rotation = _rigidbody.rotation;

            Assert.That(Invoke(
                _system, "TryApplyWorldRebaseOffset", -880.0f), Is.True);

            Assert.That(_rigidbody.position.x,
                Is.EqualTo(originalPosition.x - 880.0f));
            Assert.That(_rigidbody.position.y, Is.EqualTo(originalPosition.y));
            Assert.That(_rigidbody.position.z, Is.EqualTo(originalPosition.z));
            Assert.That(_rigidbody.linearVelocity, Is.EqualTo(velocity));
            Assert.That(_rigidbody.angularVelocity, Is.EqualTo(angularVelocity));
            Assert.That(
                Mathf.Abs(Quaternion.Dot(_rigidbody.rotation, rotation)),
                Is.EqualTo(1.0f).Within(0.00001f));
            Assert.That(_rigidbody.constraints, Is.EqualTo(constraints));
        }

        [TestCase(0.0f)]
        [TestCase(1.0f)]
        [TestCase(float.NaN)]
        [TestCase(float.PositiveInfinity)]
        public void ApplyWorldRebaseOffset_InvalidOffsetLeavesPlayerUnchanged(
            float worldXOffset)
        {
            Vector3 position = _rigidbody.position;

            Assert.That(Invoke(
                _system, "TryApplyWorldRebaseOffset", worldXOffset), Is.False);
            Assert.That(_rigidbody.position, Is.EqualTo(position));
        }

        [Test]
        public void ApplyWorldRebaseOffset_PausedSystemIsRejected()
        {
            Assert.That(Invoke(_system, "PausePhysics"), Is.True);
            Vector3 position = _rigidbody.position;

            Assert.That(Invoke(
                _system, "TryApplyWorldRebaseOffset", -880.0f), Is.False);
            Assert.That(_rigidbody.position, Is.EqualTo(position));
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
                    return (MonoBehaviour)_playerObject.AddComponent(type);
                }
            }

            Assert.Fail("Production System type was not found: " + fullName);
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

using System.Reflection;
using FlowState.Runtime.Features;
using NUnit.Framework;
using UnityEngine;

namespace FlowState.Tests.EditMode
{
    public class CameraFollowTests
    {
        private const float Tolerance = 0.0001f;

        private GameObject _playerObject;
        private GameObject _targetObject;
        private GameObject _featureObject;
        private CameraFollow _cameraFollow;

        [SetUp]
        public void SetUp()
        {
            _playerObject = new GameObject("Player");
            _targetObject = new GameObject("CameraFollowTarget");
            _featureObject = new GameObject(nameof(CameraFollowTests));
            _cameraFollow = _featureObject.AddComponent<CameraFollow>();

            _targetObject.transform.position = new Vector3(0.0f, 2.0f, 0.0f);
            SetPrivateField("_player", _playerObject.transform);
            SetPrivateField("_followTarget", _targetObject.transform);
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_featureObject);
            Object.DestroyImmediate(_targetObject);
            Object.DestroyImmediate(_playerObject);
        }

        [Test]
        public void LateUpdate_Following_TracksOnlyPlayerX()
        {
            _cameraFollow.StartFollowing();
            _playerObject.transform.position = new Vector3(5.0f, 8.0f, 3.0f);

            InvokeLateUpdate();

            Vector3 targetPosition = _targetObject.transform.position;
            Assert.That(targetPosition.x, Is.EqualTo(5.0f).Within(Tolerance));
            Assert.That(targetPosition.y, Is.EqualTo(2.0f).Within(Tolerance));
            Assert.That(targetPosition.z, Is.EqualTo(0.0f).Within(Tolerance));
        }

        [Test]
        public void LateUpdate_AfterStopFollowing_DoesNotMoveTarget()
        {
            _cameraFollow.StartFollowing();
            _cameraFollow.StopFollowing();
            _playerObject.transform.position = new Vector3(5.0f, 8.0f, 3.0f);

            InvokeLateUpdate();

            Assert.That(
                _targetObject.transform.position,
                Is.EqualTo(new Vector3(0.0f, 2.0f, 0.0f)));
        }

        private void InvokeLateUpdate()
        {
            MethodInfo lateUpdate = typeof(CameraFollow).GetMethod(
                "LateUpdate",
                BindingFlags.Instance | BindingFlags.NonPublic);

            Assert.That(lateUpdate, Is.Not.Null);
            lateUpdate.Invoke(_cameraFollow, null);
        }

        private void SetPrivateField(string fieldName, object value)
        {
            FieldInfo field = typeof(CameraFollow).GetField(
                fieldName,
                BindingFlags.Instance | BindingFlags.NonPublic);

            Assert.That(field, Is.Not.Null);
            field.SetValue(_cameraFollow, value);
        }
    }
}

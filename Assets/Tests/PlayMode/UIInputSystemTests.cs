using System;
using System.Reflection;
using FlowState.Runtime.Core;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

namespace FlowState.Tests.PlayMode
{
    public class UIInputSystemTests
    {
        private GameObject _systemObject;
        private GameObject _eventSystemObject;
        private InputActionAsset _inputActionsAsset;
        private InputSystemUIInputModule _inputModule;
        private MonoBehaviour _uiInputSystem;
        private Keyboard _keyboard;
        private Gamepad _gamepad;
        private Mouse _mouse;

        [SetUp]
        public void SetUp()
        {
            _eventSystemObject = new GameObject("UIInputModuleTest");
            _eventSystemObject.SetActive(false);
            _inputModule = _eventSystemObject.AddComponent<InputSystemUIInputModule>();
            _inputActionsAsset = CreateUIInputActionsAsset();
            _inputModule.actionsAsset = _inputActionsAsset;

            Type systemType = FindType("FlowState.Runtime.Systems.UIInputSystem");
            _systemObject = new GameObject("UIInputSystemTest");
            _uiInputSystem = (MonoBehaviour)_systemObject.AddComponent(systemType);
            SetPrivateField(_uiInputSystem, "_inputModule", _inputModule);
        }

        [TearDown]
        public void TearDown()
        {
            if (_systemObject != null)
            {
                UnityEngine.Object.DestroyImmediate(_systemObject);
            }

            if (_eventSystemObject != null)
            {
                UnityEngine.Object.DestroyImmediate(_eventSystemObject);
            }

            if (_inputActionsAsset != null)
            {
                UnityEngine.Object.DestroyImmediate(_inputActionsAsset);
            }

            RemoveTestDevice(_mouse);
            RemoveTestDevice(_gamepad);
            RemoveTestDevice(_keyboard);
            _inputModule = null;
            _keyboard = null;
            _gamepad = null;
            _mouse = null;
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

        [Test]
        public void InputDevices_UpdateDisplayDevice()
        {
            _keyboard = InputSystem.AddDevice<Keyboard>();
            _gamepad = InputSystem.AddDevice<Gamepad>();
            _mouse = InputSystem.AddDevice<Mouse>();

            InvokePrivateMethod("RecordInputDevice", _keyboard);
            Assert.That(
                GetLastInputDisplayDevice(),
                Is.EqualTo(E_InputDisplayDevice.KeyboardMouse));

            InvokePrivateMethod("RecordInputDevice", _gamepad);
            Assert.That(
                GetLastInputDisplayDevice(),
                Is.EqualTo(E_InputDisplayDevice.Gamepad));

            InvokePrivateMethod("RecordInputDevice", _mouse);
            Assert.That(
                GetLastInputDisplayDevice(),
                Is.EqualTo(E_InputDisplayDevice.KeyboardMouse));
        }

        private bool GetIsUIActionMapEnabled()
        {
            PropertyInfo property = _uiInputSystem.GetType().GetProperty(
                "IsUIActionMapEnabled",
                BindingFlags.Instance | BindingFlags.Public);

            Assert.That(property, Is.Not.Null);
            return (bool)property.GetValue(_uiInputSystem);
        }

        private E_InputDisplayDevice GetLastInputDisplayDevice()
        {
            PropertyInfo property = _uiInputSystem.GetType().GetProperty(
                "LastInputDisplayDevice",
                BindingFlags.Instance | BindingFlags.Public);

            Assert.That(property, Is.Not.Null);
            return (E_InputDisplayDevice)property.GetValue(_uiInputSystem);
        }

        private void InvokePublicMethod(string methodName)
        {
            MethodInfo method = _uiInputSystem.GetType().GetMethod(
                methodName,
                BindingFlags.Instance | BindingFlags.Public);

            Assert.That(method, Is.Not.Null);
            method.Invoke(_uiInputSystem, null);
        }

        private void InvokePrivateMethod(string methodName, InputDevice device)
        {
            MethodInfo method = _uiInputSystem.GetType().GetMethod(
                methodName,
                BindingFlags.Instance | BindingFlags.NonPublic,
                null,
                new[] { typeof(InputDevice) },
                null);

            Assert.That(method, Is.Not.Null);
            method.Invoke(_uiInputSystem, new object[] { device });
        }

        private static void SetPrivateField(object target, string fieldName, object value)
        {
            FieldInfo field = target.GetType().GetField(
                fieldName,
                BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.That(field, Is.Not.Null);
            field.SetValue(target, value);
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

        private static InputActionAsset CreateUIInputActionsAsset()
        {
            InputActionAsset asset = ScriptableObject.CreateInstance<InputActionAsset>();
            InputActionMap map = asset.AddActionMap("UI");
            map.AddAction("Navigate", InputActionType.PassThrough);
            map.AddAction("Submit", InputActionType.Button);
            map.AddAction("Cancel", InputActionType.Button);
            map.AddAction("Point", InputActionType.PassThrough);
            map.AddAction("Click", InputActionType.PassThrough);
            return asset;
        }

        private static void RemoveTestDevice(InputDevice device)
        {
            if (device != null && device.added)
            {
                InputSystem.RemoveDevice(device);
            }
        }
    }
}

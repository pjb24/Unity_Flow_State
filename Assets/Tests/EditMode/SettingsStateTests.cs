using System;
using FlowState.Runtime.Core;
using NUnit.Framework;

namespace FlowState.Tests.EditMode
{
    public class SettingsStateTests
    {
        private static readonly SettingsBindingTarget PlayerJumpTarget =
            CreateTarget(
                E_SettingsActionMap.Player,
                E_SettingsDeviceGroup.KeyboardAndMouse,
                "10000000-0000-0000-0000-000000000001",
                "20000000-0000-0000-0000-000000000001",
                string.Empty);

        private static readonly SettingsBindingTarget PlayerMomentumTarget =
            CreateTarget(
                E_SettingsActionMap.Player,
                E_SettingsDeviceGroup.KeyboardAndMouse,
                "10000000-0000-0000-0000-000000000002",
                "20000000-0000-0000-0000-000000000002",
                string.Empty);

        private static readonly SettingsBindingTarget UiNavigateUpTarget =
            CreateTarget(
                E_SettingsActionMap.UI,
                E_SettingsDeviceGroup.KeyboardAndMouse,
                "30000000-0000-0000-0000-000000000001",
                "40000000-0000-0000-0000-000000000001",
                "up");

        private static readonly SettingsBindingTarget UiNavigateDownTarget =
            CreateTarget(
                E_SettingsActionMap.UI,
                E_SettingsDeviceGroup.KeyboardAndMouse,
                "30000000-0000-0000-0000-000000000001",
                "40000000-0000-0000-0000-000000000002",
                "down");

        private SettingsApplicationSpy _application;
        private SettingsState _state;

        [SetUp]
        public void SetUp()
        {
            _application = new SettingsApplicationSpy();
            _state = CreateState(true);
        }

        [Test]
        public void Initialize_UsesRuntimeDefaultsWithoutApplyingPlatformState()
        {
            Assert.That(_state.MasterVolume, Is.EqualTo(SettingsState.DefaultMasterVolume));
            Assert.That(_state.IsFullscreen, Is.True);
            Assert.That(_state.RebindingState, Is.EqualTo(E_SettingsRebindingState.None));
            Assert.That(_application.MasterVolumeApplyCount, Is.Zero);
            Assert.That(_application.FullscreenApplyCount, Is.Zero);
        }

        [TestCase(-1, SettingsState.MinimumMasterVolume)]
        [TestCase(45, 45)]
        public void SetMasterVolume_ClampsAndAppliesChangedValueOnce(
            int requestedVolume,
            int expectedVolume)
        {
            Assert.That(_state.TrySetMasterVolume(requestedVolume), Is.True);
            Assert.That(_state.MasterVolume, Is.EqualTo(expectedVolume));
            Assert.That(_application.MasterVolumeApplyCount, Is.EqualTo(1));
            Assert.That(_application.LastMasterVolume, Is.EqualTo(expectedVolume));
        }

        [Test]
        public void SetMasterVolume_SameEffectiveValue_DoesNotApplyAgain()
        {
            Assert.That(_state.TrySetMasterVolume(45), Is.True);

            Assert.That(_state.TrySetMasterVolume(45), Is.False);
            Assert.That(_application.MasterVolumeApplyCount, Is.EqualTo(1));
        }

        [Test]
        public void SetMasterVolume_ClampedDefaultValue_DoesNotApply()
        {
            Assert.That(_state.TrySetMasterVolume(101), Is.False);
            Assert.That(_state.MasterVolume,
                Is.EqualTo(SettingsState.MaximumMasterVolume));
            Assert.That(_application.MasterVolumeApplyCount, Is.Zero);
        }

        [Test]
        public void SetFullscreen_ChangedValue_AppliesOnce()
        {
            Assert.That(_state.TrySetFullscreen(false), Is.True);
            Assert.That(_state.IsFullscreen, Is.False);
            Assert.That(_application.FullscreenApplyCount, Is.EqualTo(1));
            Assert.That(_application.LastFullscreen, Is.False);

            Assert.That(_state.TrySetFullscreen(false), Is.False);
            Assert.That(_application.FullscreenApplyCount, Is.EqualTo(1));
        }

        [Test]
        public void BeginRebind_UnknownTarget_IsRejectedWithoutMutation()
        {
            SettingsBindingTarget unknownTarget = CreateTarget(
                E_SettingsActionMap.UI,
                E_SettingsDeviceGroup.KeyboardAndMouse,
                "50000000-0000-0000-0000-000000000001",
                "60000000-0000-0000-0000-000000000001",
                string.Empty);

            Assert.That(_state.TryBeginRebind(unknownTarget), Is.False);
            Assert.That(_state.HasPendingRebind, Is.False);
            Assert.That(_state.BindingOverrideCount, Is.Zero);
        }

        [Test]
        public void BeginRebind_DuplicateRequest_IsRejectedWhileWaiting()
        {
            Assert.That(_state.TryBeginRebind(PlayerJumpTarget), Is.True);

            Assert.That(_state.TryBeginRebind(PlayerMomentumTarget), Is.False);
            Assert.That(_state.PendingTarget, Is.EqualTo(PlayerJumpTarget));
        }

        [Test]
        public void CompleteRebind_ValidTarget_ReplacesEffectiveControlPath()
        {
            Assert.That(_state.TryBeginRebind(PlayerJumpTarget), Is.True);

            Assert.That(_state.TryCompleteRebind("<Keyboard>/j"), Is.True);
            Assert.That(_state.HasPendingRebind, Is.False);
            Assert.That(_state.BindingOverrideCount, Is.EqualTo(1));
            Assert.That(
                _state.TryGetEffectiveControlPath(PlayerJumpTarget, out string controlPath),
                Is.True);
            Assert.That(controlPath, Is.EqualTo("<Keyboard>/j"));
        }

        [Test]
        public void CompleteRebind_EmptyControlPath_IsRejectedAndKeepsWaiting()
        {
            Assert.That(_state.TryBeginRebind(PlayerJumpTarget), Is.True);

            Assert.That(_state.TryCompleteRebind(string.Empty), Is.False);
            Assert.That(_state.HasPendingRebind, Is.True);
            Assert.That(_state.BindingOverrideCount, Is.Zero);
        }

        [Test]
        public void CancelRebind_ExistingOverride_PreservesOverrideAndClearsWaiting()
        {
            Assert.That(_state.TryBeginRebind(PlayerJumpTarget), Is.True);
            Assert.That(_state.TryCompleteRebind("<Keyboard>/j"), Is.True);
            Assert.That(_state.TryBeginRebind(PlayerJumpTarget), Is.True);

            Assert.That(_state.TryCancelRebind(), Is.True);
            Assert.That(_state.HasPendingRebind, Is.False);
            Assert.That(
                _state.TryGetEffectiveControlPath(PlayerJumpTarget, out string controlPath),
                Is.True);
            Assert.That(controlPath, Is.EqualTo("<Keyboard>/j"));
        }

        [Test]
        public void CompleteRebind_SameMapAndDeviceConflict_IsRejectedAtomically()
        {
            Assert.That(_state.TryBeginRebind(PlayerJumpTarget), Is.True);

            Assert.That(_state.TryCompleteRebind("<Keyboard>/leftShift"), Is.False);
            Assert.That(_state.HasPendingRebind, Is.True);
            Assert.That(_state.BindingOverrideCount, Is.Zero);
            Assert.That(
                _state.TryGetEffectiveControlPath(PlayerJumpTarget, out string controlPath),
                Is.True);
            Assert.That(controlPath, Is.EqualTo("<Keyboard>/space"));
        }

        [Test]
        public void CompleteRebind_DifferentActionMapSameDevice_AllowsSharedControl()
        {
            Assert.That(_state.TryBeginRebind(UiNavigateUpTarget), Is.True);

            Assert.That(_state.TryCompleteRebind("<Keyboard>/space"), Is.True);
            Assert.That(_state.BindingOverrideCount, Is.EqualTo(1));
        }

        [Test]
        public void CompleteRebind_CompositePartConflict_IsRejected()
        {
            Assert.That(_state.TryBeginRebind(UiNavigateDownTarget), Is.True);

            Assert.That(_state.TryCompleteRebind("<Keyboard>/w"), Is.False);
            Assert.That(_state.HasPendingRebind, Is.True);
            Assert.That(_state.BindingOverrideCount, Is.Zero);
        }

        [Test]
        public void CompleteRebind_CompositePartConflictWithDifferentPathCasing_IsRejected()
        {
            Assert.That(_state.TryBeginRebind(UiNavigateDownTarget), Is.True);

            Assert.That(_state.TryCompleteRebind("<Keyboard>/W"), Is.False);
            Assert.That(_state.HasPendingRebind, Is.True);
            Assert.That(_state.BindingOverrideCount, Is.Zero);
        }

        [Test]
        public void RestoreDefaults_ChangedState_RemovesOverridesAndAppliesChangedPlatformValues()
        {
            Assert.That(_state.TrySetMasterVolume(25), Is.True);
            Assert.That(_state.TrySetFullscreen(false), Is.True);
            Assert.That(_state.TryBeginRebind(PlayerJumpTarget), Is.True);
            Assert.That(_state.TryCompleteRebind("<Keyboard>/j"), Is.True);
            Assert.That(_state.TryBeginRebind(UiNavigateUpTarget), Is.True);

            Assert.That(_state.TryRestoreDefaults(), Is.True);
            Assert.That(_state.MasterVolume, Is.EqualTo(SettingsState.DefaultMasterVolume));
            Assert.That(_state.IsFullscreen, Is.True);
            Assert.That(_state.BindingOverrideCount, Is.Zero);
            Assert.That(_state.HasPendingRebind, Is.False);
            Assert.That(_application.MasterVolumeApplyCount, Is.EqualTo(2));
            Assert.That(_application.FullscreenApplyCount, Is.EqualTo(2));
            Assert.That(
                _state.TryGetEffectiveControlPath(PlayerJumpTarget, out string controlPath),
                Is.True);
            Assert.That(controlPath, Is.EqualTo("<Keyboard>/space"));
        }

        [Test]
        public void RestoreDefaults_UnchangedState_IsRejectedWithoutPlatformApply()
        {
            Assert.That(_state.TryRestoreDefaults(), Is.False);
            Assert.That(_application.MasterVolumeApplyCount, Is.Zero);
            Assert.That(_application.FullscreenApplyCount, Is.Zero);
        }

        [Test]
        public void Constructor_DuplicateBindingTarget_IsRejected()
        {
            SettingsBindingDefinition[] definitions =
            {
                new SettingsBindingDefinition(PlayerJumpTarget, "<Keyboard>/space"),
                new SettingsBindingDefinition(PlayerJumpTarget, "<Keyboard>/j")
            };

            Assert.That(
                () => new SettingsState(true, _application, definitions),
                Throws.ArgumentException);
        }

        private SettingsState CreateState(bool defaultFullscreen)
        {
            SettingsBindingDefinition[] definitions =
            {
                new SettingsBindingDefinition(PlayerJumpTarget, "<Keyboard>/space"),
                new SettingsBindingDefinition(
                    PlayerMomentumTarget,
                    "<Keyboard>/leftShift"),
                new SettingsBindingDefinition(UiNavigateUpTarget, "<Keyboard>/w"),
                new SettingsBindingDefinition(UiNavigateDownTarget, "<Keyboard>/s")
            };

            return new SettingsState(
                defaultFullscreen,
                _application,
                definitions);
        }

        private static SettingsBindingTarget CreateTarget(
            E_SettingsActionMap actionMap,
            E_SettingsDeviceGroup deviceGroup,
            string actionId,
            string bindingId,
            string compositePartName)
        {
            return new SettingsBindingTarget(
                actionMap,
                deviceGroup,
                new Guid(actionId),
                new Guid(bindingId),
                compositePartName);
        }
    }
}

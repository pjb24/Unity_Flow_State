using System;

namespace FlowState.Runtime.Core
{
    public readonly struct SettingsBindingTarget : IEquatable<SettingsBindingTarget>
    {
        public E_SettingsActionMap ActionMap { get; }

        public E_SettingsDeviceGroup DeviceGroup { get; }

        public Guid ActionId { get; }

        public Guid BindingId { get; }

        public string CompositePartName { get; }

        public SettingsBindingTarget(
            E_SettingsActionMap actionMap,
            E_SettingsDeviceGroup deviceGroup,
            Guid actionId,
            Guid bindingId,
            string compositePartName)
        {
            if (actionId == Guid.Empty)
            {
                throw new ArgumentException("Action ID is required.", nameof(actionId));
            }

            if (bindingId == Guid.Empty)
            {
                throw new ArgumentException("Binding ID is required.", nameof(bindingId));
            }

            ActionMap = actionMap;
            DeviceGroup = deviceGroup;
            ActionId = actionId;
            BindingId = bindingId;
            CompositePartName = compositePartName ?? string.Empty;
        }

        public bool Equals(SettingsBindingTarget other)
        {
            return ActionMap == other.ActionMap &&
                   DeviceGroup == other.DeviceGroup &&
                   ActionId == other.ActionId &&
                   BindingId == other.BindingId &&
                   CompositePartName == other.CompositePartName;
        }

        public override bool Equals(object obj)
        {
            return obj is SettingsBindingTarget other && Equals(other);
        }

        public override int GetHashCode()
        {
            int hashCode = (int)ActionMap;
            hashCode = (hashCode * 397) ^ (int)DeviceGroup;
            hashCode = (hashCode * 397) ^ ActionId.GetHashCode();
            hashCode = (hashCode * 397) ^ BindingId.GetHashCode();
            return (hashCode * 397) ^ CompositePartName.GetHashCode();
        }
    }
}

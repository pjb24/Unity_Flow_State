using System;
using System.Globalization;
using UnityEngine;

namespace FlowState.Runtime.Features
{
    public readonly struct MomentumHudPresentation
    {
        public MomentumHudPresentation(string multiplierText, float fillAmount)
        {
            MultiplierText = multiplierText;
            FillAmount = fillAmount;
        }

        public string MultiplierText { get; }

        public float FillAmount { get; }
    }

    public static class MomentumHudPresenter
    {
        private const string MultiplierFormat = "x{0:F2}";
        private const string MultiplierPlaceholder = "x--";
        private const float ColorTolerance = 0.0001f;

        private static readonly GradientColorKey[] ApprovedColorKeys =
        {
            new GradientColorKey(Color.red, 0.0f),
            new GradientColorKey(Color.cyan, 1.0f)
        };

        private static readonly GradientAlphaKey[] ApprovedAlphaKeys =
        {
            new GradientAlphaKey(1.0f, 0.0f),
            new GradientAlphaKey(1.0f, 1.0f)
        };

        public static MomentumHudPresentation Create(
            double multiplier,
            double remainingRatio)
        {
            if (!IsFinite(multiplier) || multiplier < 1.0 || multiplier > 3.0 ||
                !IsFinite(remainingRatio))
            {
                return new MomentumHudPresentation(MultiplierPlaceholder, 0.0f);
            }

            string text = string.Format(
                CultureInfo.InvariantCulture,
                MultiplierFormat,
                multiplier);
            float fillAmount = multiplier <= 1.0
                ? 0.0f
                : Mathf.Clamp01((float)remainingRatio);
            return new MomentumHudPresentation(text, fillAmount);
        }

        public static Gradient CreateApprovedGradient()
        {
            Gradient gradient = new Gradient();
            gradient.SetKeys(ApprovedColorKeys, ApprovedAlphaKeys);
            gradient.mode = GradientMode.Blend;
            return gradient;
        }

        public static bool IsApprovedGradient(Gradient gradient)
        {
            return GetGradientValidationError(gradient) == null;
        }

        public static string GetGradientValidationError(Gradient gradient)
        {
            if (gradient == null)
            {
                return "Gradient is null.";
            }

            if (gradient.mode != GradientMode.Blend)
            {
                return $"Mode is {gradient.mode}; expected Blend.";
            }

            GradientColorKey[] colorKeys = gradient.colorKeys;
            GradientAlphaKey[] alphaKeys = gradient.alphaKeys;

            if (colorKeys.Length != ApprovedColorKeys.Length ||
                alphaKeys.Length != ApprovedAlphaKeys.Length)
            {
                return $"Key count is Color {colorKeys.Length}, Alpha {alphaKeys.Length}; expected Color {ApprovedColorKeys.Length}, Alpha {ApprovedAlphaKeys.Length}.";
            }

            for (int index = 0; index < colorKeys.Length; index++)
            {
                if (!Approximately(
                        colorKeys[index].time,
                        ApprovedColorKeys[index].time))
                {
                    return $"Color Key {index} time is {colorKeys[index].time}; expected {ApprovedColorKeys[index].time}.";
                }

                if (!ApproximatelyContractColor(
                        colorKeys[index].color,
                        ApprovedColorKeys[index].color))
                {
                    return $"Color Key {index} is {colorKeys[index].color}; expected {ApprovedColorKeys[index].color} (Gamma or Linear).";
                }
            }

            for (int index = 0; index < alphaKeys.Length; index++)
            {
                if (!Approximately(
                        alphaKeys[index].time,
                        ApprovedAlphaKeys[index].time))
                {
                    return $"Alpha Key {index} time is {alphaKeys[index].time}; expected {ApprovedAlphaKeys[index].time}.";
                }

                if (!Approximately(
                        alphaKeys[index].alpha,
                        ApprovedAlphaKeys[index].alpha))
                {
                    return $"Alpha Key {index} is {alphaKeys[index].alpha}; expected {ApprovedAlphaKeys[index].alpha}.";
                }
            }

            return null;
        }

        private static bool IsFinite(double value)
        {
            return !double.IsNaN(value) && !double.IsInfinity(value);
        }

        private static bool Approximately(float first, float second)
        {
            return Math.Abs(first - second) <= ColorTolerance;
        }

        private static bool ApproximatelyContractColor(
            Color actual,
            Color expected)
        {
            return ApproximatelyRgb(actual, expected) ||
                   ApproximatelyRgb(actual, expected.linear) ||
                   ApproximatelyRgb(actual, expected.gamma);
        }

        private static bool ApproximatelyRgb(Color first, Color second)
        {
            return Approximately(first.r, second.r) &&
                   Approximately(first.g, second.g) &&
                   Approximately(first.b, second.b);
        }
    }
}

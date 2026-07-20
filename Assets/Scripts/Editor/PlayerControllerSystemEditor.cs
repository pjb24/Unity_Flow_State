using FlowState.Runtime.Systems;
using UnityEditor;
using UnityEngine;

namespace FlowState.EditorTools
{
    [CustomEditor(typeof(PlayerControllerSystem))]
    public class PlayerControllerSystemEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            PlayerControllerSystem playerController =
                (PlayerControllerSystem)target;

            EditorGUILayout.Space();
            EditorGUILayout.LabelField(
                "Runtime Movement Debug",
                EditorStyles.boldLabel);

            using (new EditorGUI.DisabledScope(true))
            {
                EditorGUILayout.Vector3Field(
                    "Current Velocity",
                    playerController.CurrentVelocity);
                EditorGUILayout.FloatField(
                    "Horizontal Acceleration (Signed)",
                    playerController.CurrentHorizontalAcceleration);
            }

            if (!Application.isPlaying)
            {
                EditorGUILayout.HelpBox(
                    "Runtime movement values are updated in Play Mode.",
                    MessageType.Info);
            }
        }

        public override bool RequiresConstantRepaint()
        {
            return Application.isPlaying;
        }
    }
}

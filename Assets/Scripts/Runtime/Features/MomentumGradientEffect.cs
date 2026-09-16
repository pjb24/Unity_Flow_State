using UnityEngine;
using UnityEngine.UI;

namespace FlowState.Runtime.Features
{
    [RequireComponent(typeof(Graphic))]
    public class MomentumGradientEffect : BaseMeshEffect
    {
        private Gradient _gradient;

        public bool HasGradient => _gradient != null;

        public void SetGradient(Gradient gradient)
        {
            _gradient = gradient;

            if (graphic != null)
            {
                graphic.SetVerticesDirty();
            }
        }

        public override void ModifyMesh(VertexHelper vertexHelper)
        {
            if (!IsActive() || _gradient == null || vertexHelper == null)
            {
                return;
            }

            Rect rect = graphic.rectTransform.rect;
            float width = rect.width;

            if (width <= 0.0f)
            {
                return;
            }

            UIVertex vertex = new UIVertex();
            for (int index = 0; index < vertexHelper.currentVertCount; index++)
            {
                vertexHelper.PopulateUIVertex(ref vertex, index);
                float ratio = Mathf.InverseLerp(
                    rect.xMin,
                    rect.xMax,
                    vertex.position.x);
                Color inspectorTint = vertex.color;
                vertex.color = inspectorTint * _gradient.Evaluate(ratio);
                vertexHelper.SetUIVertex(vertex, index);
            }
        }
    }
}

using UnityEngine;
using UnityEngine.UI;

namespace Toolkit.ComponentExtend
{
    [AddComponentMenu("UI/Custom/EmptyGraph")]
    public class EmptyGraph : Image
    {
        protected override void OnPopulateMesh(VertexHelper vh)
        {
            // base.OnPopulateMesh(toFill);
            vh.Clear();
        }
    }
}
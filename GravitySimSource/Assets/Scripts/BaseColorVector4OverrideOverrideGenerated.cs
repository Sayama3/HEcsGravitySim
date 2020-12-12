using Unity.Entities;
using Unity.Mathematics;

namespace Unity.Rendering
{
    [MaterialProperty("_BaseColor", MaterialPropertyFormat.Float4)]
    struct BaseColorVector4Override : IComponentData
    {
        public float4 Value;
    }
}

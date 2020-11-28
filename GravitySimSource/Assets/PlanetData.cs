using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct PlanetData : IComponentData
{
    public float Mass;
    public float3 Center;
    public float3 Size;
    public bool IgnoreY;

    public bool CompareBorder(int direction, float3 position)
    {
        float3 center = Center;
        switch (direction)
        {
            case 0 :
                center.x += Size.x/2;
                return position.x > center.x;
            case 1 :
                center.x -= Size.x/2;
                return position.x < center.x;
            case 2 :
                center.y += Size.y/2;
                return position.y > center.y;
            case 3 :
                center.y -= Size.y/2;
                return position.y < center.y;
            case 4 :
                center.z += Size.z/2;
                return position.z > center.z;
            case 5 :
                center.z -= Size.z/2;
                return position.z < center.z;
            default:
                return default;
        }
    }
    public float3 MoveCompareBorder(int direction, float3 position)
    {
        float3 center = Center;
        switch (direction)
        {
            case 0 :
                center.x -= Size.x/2;
                position.x = center.x;
                return position;
            case 1 :
                center.x += Size.x/2;
                position.x = center.x;
                return position;
            case 2 :
                center.y -= Size.y/2;
                position.y = center.y;
                return position;
            case 3 :
                center.y += Size.y/2;
                position.y = center.y;
                return position;
            case 4 :
                center.z -= Size.z/2;
                position.z = center.z;
                return position;
            case 5 :
                center.z += Size.z/2;
                position.z = center.z;
                return position;
            default:
                return default;
        }
    }    
    public float3 GetDirection(int direction)
    {
        switch (direction)
        {
            case 0 :
                return Vector3.right;
            case 1 :
                return Vector3.left;
            case 2 :
                return Vector3.up;
            case 3 :
                return Vector3.down;
            case 4 :
                return Vector3.forward;
            case 5 :
                return Vector3.back;
            default:
                return default;
        }
    }
}


using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

public partial class GravitySystem
{
    [BurstCompile]
    private struct ForceCalcJob : IJobForEach<PlanetData, Translation, Scale, PhysicsVelocity>
    {
        private const float Gravity = 1.5f;
        [ReadOnly] public NativeArray<Translation> PlanetsTranslation;
        [ReadOnly] public NativeArray<PlanetData> PlanetsMasses;
        [ReadOnly] public NativeArray<Scale> PlanetsScales;

        //TODO: Add size
        private static float3 GetForce(Vector3 myPosition, float myMass, float myRadius, Vector3 targetPosition, float targetMass,float targetRadius,bool ignoreY)
        {
            if (ignoreY)
            {
                myPosition.y = 0;
                targetPosition.y = 0;
            }
            var direction = targetPosition - myPosition;
            var distanceSqr = direction.sqrMagnitude;
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            ;
            if (distanceSqr < 1 /*|| distanceSqr - myRadius + targetRadius <= 0 */) return float3.zero;
            
            return direction.normalized * (Gravity * (myMass * targetMass) / distanceSqr);
        }

        public void Execute([ReadOnly] ref PlanetData planetData, [ReadOnly] ref Translation translation, [ReadOnly] ref Scale scale, ref PhysicsVelocity physicsVelocity)
        {
            for (var i = 0; i < PlanetsTranslation.Length; i++)
            {
                physicsVelocity.Linear += GetForce(translation.Value, planetData.Mass, scale.Value/2, PlanetsTranslation[i].Value, PlanetsMasses[i].Mass, PlanetsScales[i].Value/2, planetData.IgnoreY)/(planetData.Mass*.9f);
                physicsVelocity.Angular = float3.zero;
            }

        }
    }
}
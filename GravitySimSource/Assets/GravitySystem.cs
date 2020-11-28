using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
using Unity.Transforms;

public partial class GravitySystem : JobComponentSystem
{
    private ForceCalcJob _forceCalcJob;
    private CheckBorder _checkBorderJob;
    private JobHandle _jobHandle;
    private EntityQuery _worldPlanetsForceCalc;
    private EntityQuery _worldPlanetsCheckBorder;

    protected override void OnCreate()
    {
        _worldPlanetsForceCalc = GetEntityQuery(ComponentType.ReadOnly<PlanetData>(), ComponentType.ReadOnly<Translation>(), ComponentType.ReadOnly<Scale>(), ComponentType.ReadWrite<PhysicsVelocity>());
        
        _worldPlanetsCheckBorder = GetEntityQuery(ComponentType.ReadOnly<PlanetData>(), ComponentType.ReadOnly<Scale>(), ComponentType.ReadOnly<Translation>(), ComponentType.ReadWrite<PhysicsVelocity>());
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        if(UnityEngine.Time.timeScale >0)
        {
            _forceCalcJob.PlanetsTranslation = _worldPlanetsForceCalc.ToComponentDataArray<Translation>(Allocator.TempJob);
            _forceCalcJob.PlanetsMasses = _worldPlanetsForceCalc.ToComponentDataArray<PlanetData>(Allocator.TempJob);
            _forceCalcJob.PlanetsScales = _worldPlanetsForceCalc.ToComponentDataArray<Scale>(Allocator.TempJob);
            _jobHandle = _forceCalcJob.Schedule(_worldPlanetsForceCalc, inputDeps);
            _jobHandle = _checkBorderJob.Schedule(_worldPlanetsCheckBorder, _jobHandle);
            _jobHandle = _forceCalcJob.PlanetsTranslation.Dispose(_jobHandle);
            _jobHandle = _forceCalcJob.PlanetsMasses.Dispose(_jobHandle);
            _jobHandle = _forceCalcJob.PlanetsScales.Dispose(_jobHandle);
            return _jobHandle;
        }

        return default;
    }
}
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
public partial class GravitySystem
{
	[BurstCompile]
	private struct CheckBorder : IJobForEach<PlanetData, Scale, Translation, PhysicsVelocity>
	{
		public void Execute([ReadOnly]ref PlanetData planetData,[ReadOnly]ref Scale planetScale, [ReadOnly] ref Translation translation, ref PhysicsVelocity physicsVelocity)
		{
			for (int i = 0; i < 6; i++)
			{
				var direction = planetData.GetDirection(i);
				if (planetData.CompareBorder(i,translation.Value + direction * planetScale.Value/2) && math.dot(direction,math.normalize(physicsVelocity.Linear)) > 0)
				{
					physicsVelocity.Linear *= .75f;
					physicsVelocity.Linear = math.reflect(physicsVelocity.Linear,direction);
					physicsVelocity.Angular = float3.zero;
					//translation.Value = planetData.MoveCompareBorder(i, translation.Value);
				}
			}
		}
	}
}

using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
/*
public partial class GravitySystem
{
	//TODO: add explosion above a certain threshold ?
	//TODO: can only absorb if planet inferior to itself
	[BurstCompile]
	private class SecondFusion : SystemBase
	{
		protected override void OnUpdate()
		{
			Entities.ForEach((ref PlanetData myPlanetData, ref Scale myPlanetScale, ref PhysicsVelocity myPhysicsVelocity, ref Translation myTranslation) =>
			{
				if (!myPlanetData.Destroy)
				{
					float3 myPosition = myTranslation.Value;

					float mySize = myPlanetScale.Value;
					float myMass = myPlanetData.Mass;
					float3 myVelocity = myPhysicsVelocity.Linear;

					var myId = myPlanetData.uniqueID;

					Entities.ForEach((ref PlanetData targetPlanetData, in Scale targetPlanetScale, in PhysicsVelocity targetPhysicsVelocity, in Translation targetTranslation) =>
					{
						float3 targetPosition = targetTranslation.Value;
						float targetSize = targetPlanetScale.Value;
						float3 targetVelocity = targetPhysicsVelocity.Linear;

						if (targetPlanetData.uniqueID != myId)
						{
							if (math.distance(targetPosition, myPosition) - ((mySize / 2f) + (targetSize / 2f)) < 0)
							{
								float targetMass = targetPlanetData.Mass;
								myVelocity += targetVelocity * (targetMass / myMass);
								float t = myMass / (myMass + targetMass);
								myPosition = math.lerp(targetPosition,myPosition,t);
								myMass += targetPlanetData.Mass*.5f;
								mySize += targetSize*.5f;
								targetPlanetData.Destroy = true;
							}
						}
					}).Run();


					myPlanetScale.Value = mySize;
					myPlanetData.Mass = myMass;
					myPhysicsVelocity.Linear = myVelocity;
					myTranslation.Value = myPosition;
				}

			}).WithoutBurst().Run();
			
			Entities.WithStructuralChanges().ForEach((in PlanetData myPlanetData, in Entity entity) =>
			{
				if(myPlanetData.Destroy)
					EntityManager.DestroyEntity(entity);
			}).WithoutBurst().Run();
		}
	}
}
*/
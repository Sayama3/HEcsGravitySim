using System;
using Sirenix.OdinInspector;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using Collider = Unity.Physics.Collider;
using Material = UnityEngine.Material;
using Random = UnityEngine.Random;

public class RandomGenerator : MonoBehaviour
{
    private int UniqueID = 0;
    private Entity entity;

    [SerializeField] private int howMany = 100;
    [SerializeField,Range(0,2)] private float defaultTimeScale = 1f;

    // ReSharper disable once RedundantDefaultMemberInitializer
    [SerializeField] private GameObject prefab = null;

    [SerializeField] private bool yIsZero = false;
    [SerializeField] private bool advancedParameter;
    
    [SerializeField, Min(0), HideIf("advancedParameter")] private float lenght;
    [SerializeField, HideIf("advancedParameter")] private float3 center = float3.zero;
    private float _divideLenght => lenght / 2f;
    
    [SerializeField,Sirenix.OdinInspector.ShowIf("advancedParameter")] private float2x3 borders;
    
    [SerializeField] private Vector2 mass = Vector2.one, size = Vector2.one;
    [SerializeField] private float2 minMaxMouse = new float2(100,1000);

    private BlobAssetStore blobAssetStore;
    private EntityManager entityManager;
    private Entity ePrefab;
    // Start is called before the first frame update
    private void Start()
    {
        blobAssetStore = new BlobAssetStore();
        ePrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(prefab,GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld,blobAssetStore));
        
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        for (var i = 0; i < howMany; i++)
        {
            var masse = Random.Range(mass.x, mass.y);
            float3 position = advancedParameter ? new float3 {x = Random.Range(borders.c0.x, borders.c0.y), y = yIsZero ? (borders.c1.x+borders.c1.y)/2 : Random.Range(borders.c1.x, borders.c1.y), z = Random.Range(borders.c2.x, borders.c2.y)} : new float3 {x = Random.Range(-_divideLenght-center.x, _divideLenght-center.x), y = yIsZero ? -center.y : Random.Range(-_divideLenght-center.y, _divideLenght-center.y), z = Random.Range(-_divideLenght-center.z, _divideLenght-center.z)};
            InstantiateEntity(position,masse,float3.zero);
        }


        camera = Camera.main;
        Time.timeScale = defaultTimeScale;
    }

    private void InstantiateEntity(float3 instantiationPosition, float instantiationMass, float3 initialVelocity)
    {
        entity = entityManager.Instantiate(ePrefab);


        entityManager.SetComponentData(entity, new Translation
        {
            Value = instantiationPosition + center
        });

        
        float3 newCenter = advancedParameter? new float3((borders.c0.x+borders.c0.y)/2,(borders.c1.x+borders.c1.y)/2,(borders.c2.x+borders.c2.y)/2) : center;
        float3 newSize = advancedParameter?new float3(math.distance(borders.c0.x,borders.c0.y),yIsZero ? 0 : math.distance(borders.c1.x,borders.c1.y),math.distance(borders.c2.x,borders.c2.y)) : new float3(lenght,yIsZero ? 0:lenght,lenght);

        entityManager.AddComponentData(entity, new PlanetData {Mass = instantiationMass, Center = newCenter, Size = newSize, IgnoreY = yIsZero,Destroy = false,uniqueID = UniqueID});
        
        ++UniqueID;
        
        //Add scale and calcul of the size depending on the mass

        float m = instantiationMass;
        float offset = -mass.x;
        float x = m + offset;
        float a = (1 / (mass.y - mass.x)) * (size.y - size.x);
        float b = size.x;
        var scale = a * x + b;
        entityManager.AddComponentData(entity, new Scale {Value = scale});
        entityManager.SetComponentData(entity, new RenderBounds {Value = new AABB {Extents = scale}});
        entityManager.SetComponentData(entity, new PhysicsVelocity{Linear= initialVelocity, Angular = initialVelocity});
        //entityManager.SetComponentData(entity, new PhysicsMass{InverseMass = 1/instantiationMass});
    }

    private Vector3 initialPosMouse;
    private GameObject sphere;
    private float masse;
    private new Camera camera;

    [SerializeField] private Material mat;
    private Camera Camera
    {
        get
        {
            if (camera == null)
            {
                camera = Camera.main;
            }
            if (camera == null)
            {
                camera = Camera.current;
            }
            if (camera == null)
            {
                camera = FindObjectOfType<Camera>();
            }

            return camera;
        }
    }

    private bool addVelocity = false;
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!addVelocity)
            {
                Time.timeScale = 0;
                initialPosMouse = Camera.ScreenToWorldPoint(Input.mousePosition);
                initialPosMouse.y = 0f;
                sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                sphere.transform.position = initialPosMouse;
                sphere.transform.localScale = Vector3.one;
                sphere.GetComponent<MeshRenderer>().material = mat;
            }
        }
        if (Input.GetMouseButton(0))
        {
            if (!addVelocity)
            {
                var mousePosition = Camera.ScreenToWorldPoint(Input.mousePosition);
                mousePosition.y = 0f;
                var mousePositionDelta = Vector3.Distance(mousePosition, initialPosMouse);
                float t = (mousePositionDelta - minMaxMouse.x) / minMaxMouse.y;

                float m = math.lerp(mass.x, mass.y, math.clamp(t, 0f, 1f));

                float d = -mass.x;
                float a = (1 / (mass.y - mass.x)) * (size.y - size.x);
                float b = size.x;
                sphere.transform.localScale = Vector3.one * ((m + d) * a + b);
                sphere.SetActive(t >= 0);
                //Debug.Log("mousePositionDelta : " + mousePositionDelta);
                Debug.DrawLine(initialPosMouse, mousePosition);
            }
            else
            {
                
                var mousePosition = Camera.ScreenToWorldPoint(Input.mousePosition);
                mousePosition.y = 0f;
                var mousePositionDelta = Vector3.Distance(mousePosition, initialPosMouse);
                float3 direction = math.normalize(mousePosition - initialPosMouse);
                Debug.DrawRay(initialPosMouse, direction*mousePositionDelta,Color.red);
            }

        }
        if (Input.GetMouseButtonUp(0))
        {
            if (addVelocity)
            {
                Time.timeScale = defaultTimeScale;
                
                var mousePosition = Camera.ScreenToWorldPoint(Input.mousePosition);
                mousePosition.y = 0f;
                var mousePositionDelta = Vector3.Distance(mousePosition, initialPosMouse);
                float3 direction = math.normalize(mousePosition - initialPosMouse);
                Debug.DrawRay(initialPosMouse, direction*mousePositionDelta,Color.red);
                
                if(masse > mass.x) InstantiateEntity(initialPosMouse,masse,direction*mousePositionDelta);
                Destroy(sphere);
            }
            else
            {
                var mousePosition = Camera.ScreenToWorldPoint(Input.mousePosition);
                mousePosition.y = 0f;
                var mousePositionDelta = Vector3.Distance(mousePosition, initialPosMouse);
                float t = (mousePositionDelta - minMaxMouse.x) / minMaxMouse.y;
                masse = math.lerp(mass.x, mass.y, math.clamp(t,0f,1f));
            }
            
            addVelocity = !addVelocity;
            
        }
    }

    private void OnDestroy()
    {
        blobAssetStore?.Dispose();
    }

    private void OnDrawGizmosSelected()
    {
        float c = .5f;
        Gizmos.color = new Color(c,c,c,c);
        //float2x3 borders = new float2x3 {c0 = x, c1 = yIsZero ? 0 : y, c2 = z};
        
        float3 newCenter = advancedParameter? new float3((borders.c0.x+borders.c0.y)/2,(borders.c1.x+borders.c1.y)/2,(borders.c2.x+borders.c2.y)/2) : center;
        float3 newSize = advancedParameter?new float3(math.distance(borders.c0.x,borders.c0.y),yIsZero ? 0 : math.distance(borders.c1.x,borders.c1.y),math.distance(borders.c2.x,borders.c2.y)) : new float3(lenght,yIsZero ? 0:lenght,lenght);
        Gizmos.DrawCube(newCenter, newSize);
    }
}
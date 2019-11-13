using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Collections;
using Unity.Rendering;
using Unity.Mathematics;

public class Testing : MonoBehaviour
{
    [SerializeField] private Mesh mesh;
    [SerializeField] private Material material;


    EntityArchetype starArchtype;

    EntityArchetype planetArchtype;

    EntityArchetype satelliteArchtype;

    EntityManager em ;

    public int numPlanets;

    // Start is called before the first frame update
    void Start()
    {

        em = World.Active.GetOrCreateManager<EntityManager>();

        starArchtype = em.CreateArchetype(
           typeof(Translation),
           typeof(Scale),
           typeof(RenderMesh),
           typeof(LocalToWorld),
           typeof(StarClassComponent)
        );
        planetArchtype = em.CreateArchetype(
            typeof(Translation),
            typeof(Scale),
            typeof(RenderMesh),
            typeof(LocalToWorld),
            typeof(VelocityComponent),
            typeof(PlanetClassComponent)
        );
        satelliteArchtype = em.CreateArchetype(
            typeof(Translation),
            typeof(Scale),
            typeof(RenderMesh),
            typeof(LocalToWorld),
            typeof(VelocityComponent),
            typeof(SatelliteClassComponent)
        );


        em = World.Active.GetOrCreateManager<EntityManager>();

        var star = em.CreateEntity(starArchtype);

        numPlanets = 9;
        NativeArray<Entity> planetArray = new NativeArray<Entity>(numPlanets, Allocator.Temp);
        em.CreateEntity(planetArchtype, planetArray);

        NativeArray<Entity> satelliteArray = new NativeArray<Entity>(1000, Allocator.Temp);
        em.CreateEntity(satelliteArchtype, satelliteArray);


        em.SetComponentData(star, new Translation { Value = new float3(0, 0, 0) });
        em.SetComponentData(star, new Scale       { Value = 4 });
        em.SetSharedComponentData(star, new RenderMesh
        {
            mesh = mesh,
            material = material,
        });

        foreach (var planet in planetArray)
        {
            var highInitialVel = 2;
            var position = new float3(UnityEngine.Random.Range(3f, 8f), 0, 0);
            var distanceFromSun = math.sqrt(math.pow(position.x, 2) + math.pow(position.y, 2));
            em.SetComponentData(planet, new Translation { Value = position });
            em.SetComponentData(planet, new Scale { Value = math.max(distanceFromSun * .1f, .4f) });
            em.SetComponentData(planet, new VelocityComponent { v = new float2(0, UnityEngine.Random.Range(highInitialVel / 2, highInitialVel)), kinematic=false });
            em.SetComponentData(planet, new PlanetClassComponent { controlledByUser = false });
            em.SetSharedComponentData(planet, new RenderMesh
            {
                mesh = mesh,
                material = material,
            });
        }


        foreach (var satellite in satelliteArray)
        {
            em.SetComponentData(satellite, new Translation { Value = new float3(UnityEngine.Random.Range(-8f, 8f), UnityEngine.Random.Range(-5f, 5f), 0) });
            em.SetComponentData(satellite, new Scale { Value = .1f });
            em.SetComponentData(satellite, new VelocityComponent { v = new float2(0, 0), kinematic = false });
            em.SetSharedComponentData(satellite, new RenderMesh
            {
                mesh = mesh,
                material = material,
            });
        }

        planetArray.Dispose();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            numPlanets++;
            var planet = em.CreateEntity(planetArchtype);
            var highInitialVel = 2;
            var position = new float3(UnityEngine.Random.Range(3f, 8f), 0, 0);
            var distanceFromSun = math.sqrt(math.pow(position.x, 2) + math.pow(position.y, 2));
            em.SetComponentData(planet, new Translation { Value = position });
            em.SetComponentData(planet, new Scale { Value = math.max(distanceFromSun * .1f, .4f) });
            em.SetComponentData(planet, new VelocityComponent { v = new float2(0, UnityEngine.Random.Range(highInitialVel / 2, highInitialVel)), kinematic = true });
            em.SetComponentData(planet, new PlanetClassComponent { controlledByUser = true });
            em.SetSharedComponentData(planet, new RenderMesh
            {
                mesh = mesh,
                material = material,
            });

        }
    }
}

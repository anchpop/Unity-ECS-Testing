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

    // Start is called before the first frame update
    void Start()
    {
        var em = World.Active.GetOrCreateManager<EntityManager>();

        var arch = em.CreateArchetype(
            typeof(LevelComponent),
            typeof(Translation),
            typeof(MovespeedComponent),
            typeof(RenderMesh),
            typeof(LocalToWorld)
            );
        NativeArray<Entity> eArray = new NativeArray<Entity>(200, Allocator.Temp);
        em.CreateEntity(arch, eArray);
        foreach (var e in eArray)
        {
            em.SetComponentData(e, new LevelComponent { level = UnityEngine.Random.Range(10, 20) });
            em.SetComponentData(e, new MovespeedComponent { moveSpeed = UnityEngine.Random.Range(1, 5) });
            em.SetComponentData(e, new Translation { Value = new float3(UnityEngine.Random.Range(-8f, 8f), UnityEngine.Random.Range(-3f, 3f), 0) });
            em.SetSharedComponentData(e, new RenderMesh
            {
                mesh = mesh,
                material = material,
            });
        }

        eArray.Dispose();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

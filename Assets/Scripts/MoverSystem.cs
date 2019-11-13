using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Collections;

public class Moversystem1 : ComponentSystem
{
    float sunGravity = 4;
    float planetGravity = 1;

    Testing test;

    protected override void OnUpdate()
    {
        test = GameObject.FindGameObjectWithTag("Player").GetComponent<Testing>();
        NativeArray<Entity> satelliteArray = new NativeArray<Entity>(1000, Allocator.Temp);
        NativeArray<Translation> planetArray = new NativeArray<Translation>(test.numPlanets, Allocator.Temp);

        int i = 0;
        Entities.ForEach((ref PlanetClassComponent _, ref Translation translation) =>
        {
            planetArray[i] = translation;
            i++;
        });

        Entities.ForEach((ref PlanetClassComponent planet, ref Translation translation, ref VelocityComponent vel) =>
        {
            if (!planet.controlledByUser)
            {
                var magnitude = math.max(getMagnitude(translation.Value), .1f);
                var normalized = normalize(new float2(translation.Value.x, translation.Value.y));
                vel.v += (sunGravity * Time.deltaTime * -normalized) / math.pow(magnitude, 2);
            }
            else
            {
                var v3 = Input.mousePosition;
                v3.z = 10.0f;
                v3 = Camera.main.ScreenToWorldPoint(v3);
                var diff = new float3(v3.x, v3.y, 0) - translation.Value;
                var velocity = diff / Time.deltaTime;
                vel.v = new float2(velocity.x, velocity.y);
                translation.Value = new float3(v3.x, v3.y, 0);
                if (Input.GetMouseButtonUp(0))
                {
                    vel.kinematic = false;
                    planet.controlledByUser = false;
                }
            }

        });

        Entities.ForEach((ref SatelliteClassComponent _, ref Translation translation, ref VelocityComponent vel) =>
        {
            // do sun gravity
            //var magnitude = math.max(getMagnitude(translation.Value.x), .1f);
            //var normalized = normalize(new float2(translation.Value.x, translation.Value.y));
            //vel.v += (sunGravity * Time.deltaTime * -normalized) / math.pow(magnitude, 2);

            // do planet gravity
            var closestPlanet = planetArray[0];
            var closestPlanetDiff = translation.Value - closestPlanet.Value;
            var closestPlanetDist = getMagnitude(closestPlanetDiff);
            foreach (var t in planetArray)
            {
                var diff = translation.Value - t.Value;
                var dist = getMagnitude(diff);
                if (dist < closestPlanetDist)
                {
                    closestPlanet = t;
                    closestPlanetDist = dist;
                    closestPlanetDiff = diff;
                }
            }

            var magnitude = math.max(closestPlanetDist, .1f);
            var normalized = normalize(new float2(closestPlanetDiff.x, closestPlanetDiff.y));
            vel.v += (planetGravity * Time.deltaTime * -normalized) / math.pow(magnitude, 2);
        });

        Entities.ForEach((ref Translation translation, ref VelocityComponent vel) =>
        {
            if (!vel.kinematic)
            {
                var newVel = vel.v * Time.deltaTime;
                translation.Value += new float3(newVel.x, newVel.y, 0);
            }
        });
    }


    float getMagnitude(float3 i)
    {
        var magnitude = math.sqrt(math.pow(i.x, 2) + math.pow(i.y, 2));
        return magnitude;
    }

    float2 normalize(float2 i)
    {
        var magnitude = getMagnitude(new float3(i.x, i.y, 0));
        var normalized = new float2(i.x / magnitude, i.y / magnitude);
        return normalized;
    }


}

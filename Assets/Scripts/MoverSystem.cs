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
  

    protected override void OnUpdate()
    {
        // First we get the current number of planets from the Tester gameobject
        int numPlanets = GameObject.FindGameObjectWithTag("Player").GetComponent<Testing>().numPlanets;

        // Allocate an array to hold all the planet positions
        NativeArray<Translation> planetArray = new NativeArray<Translation>(numPlanets, Allocator.Temp);

        // Iterate over all the planets and put their positions in our array
        int i = 0;
        Entities.ForEach((ref PlanetClassComponent _, ref Translation translation) =>
        {
            planetArray[i] = translation;
            i++;
        });

        // Move all the planets towads the sun (which is always at 0,0 so it's eary)
        Entities.ForEach((ref PlanetClassComponent planet, ref Translation translation, ref VelocityComponent vel) =>
        {
            if (!planet.controlledByUser)
            {
                // to move towards the center, we just calculate the force of gravity and add it to the velocity
                var magnitude = math.max(getMagnitude(translation.Value), .1f);
                var normalized = normalize(new float2(translation.Value.x, translation.Value.y));
                vel.v += (sunGravity * Time.deltaTime * -normalized) / math.pow(magnitude, 2);
            }
            else
            {
                // If the user is controlling us, set the position to be the mousepositioe and 
                // the velocity to be the rate at which the user is movint the mouse
                var v3 = Input.mousePosition;
                v3.z = 10.0f;
                v3 = Camera.main.ScreenToWorldPoint(v3);
                var diff = new float3(v3.x, v3.y, 0) - translation.Value;
                var velocity = diff / Time.deltaTime;
                vel.v = new float2(velocity.x, velocity.y);
                translation.Value = new float3(v3.x, v3.y, 0);

                // If they leg go of the mouse this frame, move and be free
                if (Input.GetMouseButtonUp(0))
                {
                    vel.kinematic = false;
                    planet.controlledByUser = false;
                }
            }

        });

        Entities.ForEach((ref SatelliteClassComponent _, ref Translation translation, ref VelocityComponent vel) =>
        {
            // All of this is to get the closest planet as efficiently as possible
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
            // Then we simply move towards the planet that's closet
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

        planetArray.Dispose();
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

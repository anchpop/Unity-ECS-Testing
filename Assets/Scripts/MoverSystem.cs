using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

public class Moversystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((ref Translation translation, ref MovespeedComponent movespeedComponent) =>
        {
            if (math.abs(translation.Value.y) > 5)
            {
                movespeedComponent.moveSpeed = -movespeedComponent.moveSpeed * .9f;
                translation.Value.y += math.sign(movespeedComponent.moveSpeed) * .2f;
            }
            translation.Value.y += movespeedComponent.moveSpeed * Time.deltaTime;
        });
    }
}

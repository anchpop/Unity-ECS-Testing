﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

public struct VelocityComponent : IComponentData
{
    public float2 v;
    public bool kinematic;
}

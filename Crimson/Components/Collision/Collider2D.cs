﻿using System;
using System.Collections.Generic;
using Crimson.Physics;
using Crimson.Spatial;
using Microsoft.Xna.Framework;

namespace Crimson
{
    /// <summary>
    /// Parent class for collider types used with 2D gameplay
    /// </summary>
    public abstract class Collider2D : Component
    {
        public Collider2D() : base(true, false) { }
    }
}
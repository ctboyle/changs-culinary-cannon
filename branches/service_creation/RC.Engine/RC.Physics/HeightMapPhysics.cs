using System;
using System.Collections.Generic;
using System.Text;
using RC.Engine.Rendering;
using Microsoft.Xna.Framework;
using JigLibX.Collision;
using JigLibX.Geometry;
using JigLibX.Utils;
using JigLibX.Physics;
using RC.Content.Heightmap;

namespace RC.Physics
{
    public class RCHeightMapPhysics : RCPhysicsObject
    {
        public RCHeightMapPhysics(RCHeightMap heightMap)
            : base(Vector3.Zero)
        {
            CollisionSkin collision = new CollisionSkin(null);

            float[,] heights = heightMap.Mapping;
            Array2D field = new Array2D(heights.GetUpperBound(0), heights.GetUpperBound(1));

            for (int x = 0; x < heights.GetUpperBound(0); x++)
            {
                for (int z = 0; z < heights.GetUpperBound(1); z++)
                {
                    field.SetAt(x, z, heights[x, z]);
                }
            }

            collision.AddPrimitive(
                new Heightmap(field, 0, 0, 1, 1), 
                (int)MaterialTable.MaterialID.UserDefined, 
                new MaterialProperties(0.7f, 0.7f, 0.6f)
            );

            PhysicsSystem.CurrentPhysicsSystem.CollisionSystem.AddCollisionSkin(collision);
        }
    }
}

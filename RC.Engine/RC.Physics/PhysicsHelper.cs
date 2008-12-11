using System;
using System.Collections.Generic;
using System.Text;
using RC.Content.Heightmap;
using RC.Engine.GraphicsManagement;
using JigLibX.Physics;
using Microsoft.Xna.Framework;

namespace RC.Physics
{
    public class JibLibXPhysicsHelper
    {
        public static JibLibXPhysicsObject CreateObject(RCSpatial spatial)
        {
            JibLibXPhysicsObject physicsObject = new JibLibXPhysicsObject();
            physicsObject.AddCollisionSkin();
            physicsObject.SetChildNode(spatial);



            return physicsObject;
        }

        public static JibLibXPhysicsObject CreateHeightmap(RCHeightMap heightMap)
        {
            JibLibXPhysicsObject physicsHeightMap = new JibLibXPhysicsObject();

            physicsHeightMap.Body.Immovable = true;
            physicsHeightMap.AddCollisionSkin();

            JigLibX.Utils.Array2D heights = new JigLibX.Utils.Array2D(
                heightMap.Mapping.GetUpperBound(0),
                heightMap.Mapping.GetUpperBound(1)
            );

            for (int x = 0; x < heightMap.Mapping.GetUpperBound(0); x++)
            {
                for (int z = 0; z < heightMap.Mapping.GetUpperBound(1); z++)
                {
                    heights.SetAt(x, z, heightMap.Mapping[x, z] + heightMap.LocalTrans.Translation.Y);
                }
            }

            //physicsHeightMap.Body.CollisionSkin.AddPrimitive(
            //    new JigLibX.Geometry.Heightmap(heights, 0, 0, 1, 1),
            //    (int)JigLibX.Collision.MaterialTable.MaterialID.UserDefined,
            //    new JigLibX.Collision.MaterialProperties(0.0f, 0.7f, 0.6f)
            //);


            physicsHeightMap.Body.CollisionSkin.AddPrimitive(
                new JigLibX.Geometry.Plane(Vector3.Up, 0),
                (int)JigLibX.Collision.MaterialTable.MaterialID.UserDefined,
                new JigLibX.Collision.MaterialProperties(0.0f, 0.7f, 0.6f)
            );

            physicsHeightMap.SetChildNode(heightMap);

            return physicsHeightMap;
        }
    }
}

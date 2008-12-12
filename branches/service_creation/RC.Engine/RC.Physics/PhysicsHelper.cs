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
        public static JibLibXObject CreateObject(RCSpatial spatial)
        {
            JibLibXObject physicsObject = new JibLibXObject();
            physicsObject.AddCollisionSkin();
            physicsObject.SetChildNode(spatial);
            return physicsObject;
        }

        public static JibLibXObject CreateHeightmap(RCHeightMap heightMap)
        {
            JibLibXObject physicsHeightMap = new JibLibXObject();

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
                    heights.SetAt(x, z, heightMap.Mapping[x, z]);
                }
            }

            Vector3 position = heightMap.WorldTrans.Translation;

            physicsHeightMap.Body.CollisionSkin.AddPrimitive(
                new JigLibX.Geometry.Heightmap(heights, position.X, position.Z, 0.004f * heightMap.Scaling, 0.004f * heightMap.Scaling),
                (int)JigLibX.Collision.MaterialTable.MaterialID.UserDefined,
                new JigLibX.Collision.MaterialProperties(0.7f, 0.7f, 0.6f)
            );


            physicsHeightMap.Body.CollisionSkin.AddPrimitive(
    new JigLibX.Geometry.Plane(Vector3.Zero, 0),
    (int)JigLibX.Collision.MaterialTable.MaterialID.UserDefined,
    new JigLibX.Collision.MaterialProperties(0.7f, 0.7f, 0.6f)
);

            physicsHeightMap.SetChildNode(heightMap);

            return physicsHeightMap;
        }
    }
}

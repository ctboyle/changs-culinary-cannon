using System;
using System.Collections.Generic;
using System.Text;
using RC.Engine.GraphicsManagement;
using Microsoft.Xna.Framework.Graphics;
using RC.Engine.Rendering;

namespace RC.Engine.Test
{
    class MeshCreator
    {


        public MeshCreator()
        {
        }

        static public RCGeometry CreateObject()
        {
            short[] iBuffer = new short[] { 0, 2, 3, 0, 1, 2 }; 

            float[] vertices = new float[]
            {   -1, -1, 0,
                1,  -1, 0,
                1,  1,  0,
                -1, 1,  0};


            RCVertexAttributes vAttrib = new RCVertexAttributes();
            vAttrib.SetElementChannels(ElementType.Position, RCVertexAttributes.ChannelCount.Three);

            
            RCVertexBuffer vBuffer = new RCVertexBuffer(vAttrib, 4);
            vBuffer.SetData(ElementType.Position, vertices);



            RCGeometry geometry = new RCGeometry(iBuffer, vBuffer);

            RCMaterialState material = new RCMaterialState();

            material.Ambient = Color.Red;
            material.Diffuse = Color.Red;
            material.Specular = Color.White;
            material.Shininess = 10.0f;

            geometry.GlobalStates.Add(material);

            return geometry;

            
        }
    }
}

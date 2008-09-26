using System;
using System.Collections.Generic;
using System.Text;
using RC.Engine.GraphicsManagement;
using Microsoft.Xna.Framework.Graphics;
using RC.Engine.Rendering;
using RC.Engine.SceneEffects;

namespace RC.Engine.Test
{
    class MeshCreator
    {


        public MeshCreator()
        {
        }

        static public RCGeometry CreateObject()
        {
            short[] indicies = new short[] { 0, 2, 3, 0, 1, 2 }; 

            float[] vertices = new float[]
            {   -1, -1, 0,
                 1, -1, 0,
                 1,  1, 0,
                -1,  1, 0
            };

            float[] texCoords = new float[]
            {   
                0, 1,
                1, 1, 
                1, 0,
                0, 0
                
            };

            float[] normals = new float[]
            {
                0, 0, 1,
                0, 0, 1,
                0, 0, 1,
                0, 0, 1
            };


            RCVertexAttributes vAttrib = new RCVertexAttributes();
            vAttrib.SetElementChannels(ElementType.Position, RCVertexAttributes.ChannelCount.Three);
            vAttrib.SetElementChannels(ElementType.Texture, RCVertexAttributes.ChannelCount.Two);
            vAttrib.SetElementChannels(ElementType.Normal, RCVertexAttributes.ChannelCount.Three);

            
            RCVertexBuffer vBuffer = new RCVertexBuffer(vAttrib, 4);
            vBuffer.SetData(ElementType.Position, vertices);
            vBuffer.SetData(ElementType.Texture, texCoords);
            vBuffer.SetData(ElementType.Normal, normals);

            RCIndexBuffer iBuffer = new RCIndexBuffer(6);
            iBuffer.SetData(indicies);



            RCGeometry geometry = new RCGeometry(iBuffer, vBuffer);

            RCMaterialState material = new RCMaterialState();

            material.Ambient = new Color(255, 0, 0, 255);
            material.Diffuse = new Color(255, 0, 0, 255);
            material.Specular = new Color(255, 255, 255, 255);
            material.Shininess = 15.0f;
            material.Alpha = 0.0f;

            geometry.AddEffect(new RCTextureEffect("Content\\Textures\\seattle"));
            geometry.AddEffect(new RCTextureEffect("Content\\Textures\\smiley"));

            geometry.GlobalStates.Add(material);

            return geometry;

            
        }
    }
}

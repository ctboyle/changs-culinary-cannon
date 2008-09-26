using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace RC.Engine.Rendering
{
    public class RCVertexBuffer
    {
        private RCVertexAttributes _attributes;
        private int _numVertices;
        private int _numChannelsPerVertex;
        private int _channelQuantity;
        private float[] _data;
        private VertexBuffer _vertexBuffer;
        private VertexDeclaration _vertexDeclaration;

        public RCVertexAttributes Attributes
        {
            get { return _attributes; }
        }

        public int VertexSize
        {
            get { return _numChannelsPerVertex * sizeof(float);}
        }

        public int NumVertices
        {
            get { return _numVertices; }
        }

        public int SizeInBytes
        {
            get { return _channelQuantity * sizeof(float); }
        }

        public VertexBuffer VertexBuffer
        {
            get { return _vertexBuffer; }
        }

        public VertexDeclaration VertexDeclaration
        {
            get { return _vertexDeclaration; }
        }
           

        public RCVertexBuffer(RCVertexAttributes attributes, int numVertices)
        {
            _attributes = attributes;
            _numVertices = numVertices;
            _numChannelsPerVertex = attributes.GetChannelQuantity();
            _channelQuantity = _numChannelsPerVertex * _numVertices;
            _data = new float[_channelQuantity];
        }

        public void Load(GraphicsDevice device)
        {
            _vertexBuffer = CreateVertexBuffer(device);
            _vertexDeclaration = Attributes.CreateVertexDeclaration(device);
        }

        public void SetData(ElementType type, float[] data)
        {
            int numChannelsForType = (int)_attributes.GetElementChannels(type);
            int offset = _attributes.GetElementOffset(type);

            // Check to see if there are exactly the right number of data elements in the array.
            if ((int)numChannelsForType * _numVertices != data.Length)
            {
                throw new InvalidOperationException("SetData: data array contains incorrect number of elements");
            }

            for (int iVertex = 0; iVertex < _numVertices; iVertex++)
            {
                for (int iChannel = 0; iChannel < numChannelsForType; iChannel++)
                {
                    _data[_numChannelsPerVertex * iVertex + iChannel + offset] = data[numChannelsForType * iVertex + iChannel];
                }
            } 
        }

        public VertexBuffer CreateVertexBuffer(GraphicsDevice device)
        {
            VertexBuffer buffer = new VertexBuffer(device, SizeInBytes, BufferUsage.WriteOnly);
            buffer.SetData<float>(_data);
            return buffer;
        }

        internal void UnLoad()
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }
}

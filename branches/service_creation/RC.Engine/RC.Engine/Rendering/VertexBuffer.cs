using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

using RC.Engine.ContentManagement;

namespace RC.Engine.Rendering
{
    /// <summary>
    /// A class for creating single vertex buffer.
    /// </summary>
    public class RCVertexBuffer : RCDeviceResource
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
            : base()
        {
            _attributes = attributes;
            _numVertices = numVertices;
            _numChannelsPerVertex = attributes.GetChannelQuantity();
            _channelQuantity = _numChannelsPerVertex * _numVertices;
            _data = new float[_channelQuantity];
        }

        public void SetData(ElementType type, int vertexIndex, int channelIndex, float data)
        {
            if (channelIndex >= (int)_attributes.GetElementChannels(type))
            {
                throw new InvalidOperationException(
                    "Channel Index Out of bounds for specifed type");
            }
            int offset = _attributes.GetElementOffset(type);
            _data[_numChannelsPerVertex * vertexIndex + channelIndex + 0 + offset] = data;
        }
        public void SetData(ElementType type, int index, Vector2 data)
        {
            SetData(type, index, 0, data.X);
            SetData(type, index, 1, data.Y);
        }
        public void SetData(ElementType type, int index, Vector3 data)
        {
            SetData(type, index, 0, data.X);
            SetData(type, index, 1, data.Y);
            SetData(type, index, 2, data.Z);
        }
        public void SetData(ElementType type, int index, Vector4 data)
        {
            SetData(type, index, 0, data.X);
            SetData(type, index, 1, data.Y);
            SetData(type, index, 2, data.Z);
            SetData(type, index, 3, data.W);
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

        protected override bool IsOnDevice
        {
            get { return (_vertexBuffer != null); }
        }

        protected override void SetOnDevice()
        {
            _vertexBuffer = new VertexBuffer(Graphics.GraphicsDevice, SizeInBytes, BufferUsage.WriteOnly);
            _vertexBuffer.SetData<float>(_data);
            _vertexDeclaration = Attributes.CreateVertexDeclaration(Graphics.GraphicsDevice);
        }

        protected override void RemoveFromDevice()
        {
            if (_vertexDeclaration != null)
            {
                _vertexDeclaration.Dispose();
                _vertexDeclaration = null;
            }

            if (_vertexBuffer != null)
            {
                _vertexBuffer.Dispose();
                _vertexBuffer = null;
            }
        }
    }
}
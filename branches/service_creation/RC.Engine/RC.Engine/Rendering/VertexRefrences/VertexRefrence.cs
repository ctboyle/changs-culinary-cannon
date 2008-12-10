using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace RC.Engine.Rendering
{
    public class RCVertexRefrence
    {
        private IndexBuffer _iBuffer;
        private VertexBuffer _vBuffer;
        private VertexDeclaration _vertexDeclaration;
        private int _streamOffset;
        private int _startIndex;
        private int _stride;
        private int _baseVertex;
        private int _numVertices;
        private int _numPrimitives;

        public IndexBuffer IndexBuffer
        {
            get { return _iBuffer; }
        }
        public VertexBuffer VertexBuffer
        {
            get { return _vBuffer; }
        }

        public VertexDeclaration VertexDeclaration
        {
            get { return _vertexDeclaration; }
            set { _vertexDeclaration = value; }
        }
        public int StreamOffset
        {
            get { return _streamOffset; }
            set { _streamOffset = value; }
        }

        public int Stride
        {
            get { return _stride; }
            set { _stride = value; }
        }
        
        public int StartIndex
        {
            get { return _startIndex; }
            set { _startIndex = value; }
        }
        public int BaseVertex
        {
            get { return _baseVertex; }
            set { _baseVertex = value; }
        }
        public int NumVertices
        {
            get { return _numVertices; }
            set { _numVertices = value; }
        }
        public int NumPrimitives
        {
            get { return _numPrimitives; }
        }

        public RCVertexRefrence(
            IndexBuffer indexBuffer,
            VertexBuffer vertexBuffer,
            VertexDeclaration vertexDeclaration,
            int vertexStride,
            int streamOffset,
            int startIndex,
            int baseVertex,
            int numVertices,
            int numPrimitives
            )
        {
            _iBuffer = indexBuffer;
            _vBuffer = vertexBuffer;
            _vertexDeclaration = vertexDeclaration;

            _stride = vertexStride;
            _streamOffset = streamOffset;
            _startIndex = startIndex;
            _baseVertex = baseVertex;
            _numVertices = numVertices;
            _numPrimitives = numPrimitives;
        }

        public RCVertexRefrence(
            GraphicsDevice graphicsDevice,
            IndexBuffer indexBuffer,
            VertexBuffer vertexBuffer,
            RCVertexAttributes attributes,
            int numVertices,
            int baseVertex,
            int startIndex
            )
        {
            _iBuffer = indexBuffer;
            _vBuffer = vertexBuffer;

            _vertexDeclaration = attributes.CreateVertexDeclaration(graphicsDevice);

            _stride = VertexDeclaration.GetVertexStrideSize(attributes.VertexElements(), 0);

            _streamOffset = 0;
            _startIndex = startIndex;
            _baseVertex = baseVertex;
            _numVertices = numVertices;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using RC.Engine.GraphicsManagement;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using RC.Engine.Rendering;

namespace RC.ModelProcessor
{
    public class RCGeometryContent : RCSpatial
    {
        private VertexElement[] _vertexElements;
        private VertexBufferContent _vertexBuffer;
        private IndexCollection _indexBuffer;
        private int _vertexCount;
        private int _triangleCount;

        public int VertexCount
        {
            get { return _vertexCount; }
            set { _vertexCount = value; }
        }
        
        public int TriangleCount
        {
            get { return _triangleCount; }
            set { _triangleCount = value; }
        }
        
        
        public IndexCollection IndexBuffer
        {
            get { return _indexBuffer; }
            set { _indexBuffer = value; }
        }        
        
        
        public VertexBufferContent VertexBuffer
        {
            get { return _vertexBuffer; }
            set { _vertexBuffer = value; }
        }        
          
        public VertexElement[] VertexElements
        {
            get { return _vertexElements; }
            set { _vertexElements = value; }
        }

        public RCGeometryContent(
            VertexElement[] vertexElements,
            VertexBufferContent vertexBuffer,
            IndexCollection indexBuffer,
            int vertexCount,
            int triangleCount)

        {
            _vertexElements = vertexElements;
            _vertexBuffer = vertexBuffer;
            _indexBuffer = indexBuffer;
            _vertexCount = vertexCount;
            _triangleCount = triangleCount;
        }

        protected override void UpdateState(RCRenderStateStack stateStack, Stack<RC.Engine.SceneEffects.RCLight> lightStack)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override void Draw(IRCRenderManager render, RC.Engine.ContentManagement.IRCContentRequester contentRqst)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        protected override void UpdateWorldBound()
        {
            throw new Exception("The method or operation is not implemented.");
        }

    }
}

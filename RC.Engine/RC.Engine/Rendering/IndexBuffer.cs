using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using RC.Engine.ContentManagement;

using TPrimitive = System.Int32;

namespace RC.Engine.Rendering
{
    /// <summary>
    /// A class for help creating an index buffer.
    /// </summary>
    public class RCIndexBuffer : RCDeviceResource
    {
        private int _numIndicies;
        private TPrimitive[] _data;
        private IndexBuffer _indexBuffer;

        public int NumPrimitives
        {
            get { return _numIndicies / 3; }
        }

        public int NumIndicies
        {
            get {return _numIndicies;}
        }

        public int SizeInBytes
        {
            get { return _numIndicies * sizeof(TPrimitive); }
        }

        public IndexBuffer IndexBuffer
        {
            get { return _indexBuffer; }
        }

        public RCIndexBuffer( int numIndicies)
            : base()
        {
            _numIndicies = numIndicies;
            _data = new TPrimitive[_numIndicies];
        }

        public void SetData(TPrimitive[] data)
        {

            // Check to see if there are exactly the right number of data elements in the array.
            if (_numIndicies != data.Length)
            {
                throw new InvalidOperationException("SetData: data array contains incorrect number of elements");
            }

            data.CopyTo(_data, 0);
        }

        protected override bool IsOnDevice
        {
            get { return (_indexBuffer != null); }
        }

        protected override void SetOnDevice()
        {
            _indexBuffer = new IndexBuffer(Graphics.GraphicsDevice, SizeInBytes, BufferUsage.None, IndexElementSize.ThirtyTwoBits);
            _indexBuffer.SetData<TPrimitive>(_data);
        }

        protected override void RemoveFromDevice()
        {
            if (_indexBuffer == null) return;
            _indexBuffer.Dispose();
            _indexBuffer = null;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

using RC.Engine.ContentManagement;

namespace RC.Engine.Rendering
{
    public class RCIndexBuffer : RCDeviceResource
    {
        private int _numIndicies;
        private short[] _data;
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
            get { return _numIndicies * sizeof(short); }
        }

        public IndexBuffer IndexBuffer
        {
            get { return _indexBuffer; }
        }

        public RCIndexBuffer(IGraphicsDeviceService graphics, int numIndicies)
            : base(graphics)
        {

            _numIndicies = numIndicies;
            _data = new short[_numIndicies];
        }

        public void SetData(short[] data)
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
            _indexBuffer = new IndexBuffer(Graphics.GraphicsDevice, SizeInBytes, BufferUsage.None, IndexElementSize.SixteenBits);
            _indexBuffer.SetData<short>(_data);
        }

        protected override void RemoveFromDevice()
        {
            if (_indexBuffer == null) return;
            _indexBuffer.Dispose();
            _indexBuffer = null;
        }
    }
}
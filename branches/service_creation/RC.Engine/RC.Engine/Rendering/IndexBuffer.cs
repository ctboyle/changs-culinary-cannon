using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace RC.Engine.Rendering
{
    public class RCIndexBuffer
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


        public RCIndexBuffer(int numIndicies)
        {

            _numIndicies = numIndicies;
            _data = new short[_numIndicies];
        }

        public void Load(GraphicsDevice device)
        {
            _indexBuffer = CreateIndexBuffer(device);
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

        public IndexBuffer CreateIndexBuffer(GraphicsDevice device)
        {
            IndexBuffer buffer = new IndexBuffer(device, SizeInBytes, BufferUsage.None, IndexElementSize.SixteenBits);
            buffer.SetData<short>(_data);
            return buffer;
        }

        internal void UnLoad()
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }
}

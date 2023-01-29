using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NostalgiaEngine.Core
{
    public delegate bool NEDepthCmpFunc(float newValue, float currentValue);

    public class NEDepthBuffer
    {
        public float[] DATA { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }

        private float m_InitialVal;

        public NEDepthBuffer(int width, int height, float initalDepthVal = float.PositiveInfinity)
        {
            Width = width;
            Height = height;
            m_InitialVal = initalDepthVal;
            DATA = new float[width * height];
            for(int i = 0; i < DATA.Length; ++i)
            {
                DATA[i] = initalDepthVal;
            }
        }

        public float Sample(int x, int y)
        {
            return DATA[XY2I(x, y)];
        }

        public bool TestLess(int x, int y, float val)
        {
            int i = XY2I(x, y);
            if (val <= DATA[i]) return true;
            return false;
        }

        /// <summary>
        /// Resets entire buffer to initial value. Usually this will be infinity, unless specified differently when buffer was created.
        /// </summary>
        public void ResetBuffer()
        {
            for (int i = 0; i < DATA.Length; ++i)
            {
                DATA[i] = m_InitialVal;
            }
        }

        /// <summary>
        /// Resets entire bufer to requested value.
        /// </summary>
        /// <param name="val">Default value</param>
        public void ResetBuffer(float val)
        {
            for (int i = 0; i < DATA.Length; ++i)
            {
                DATA[i] = val;
            }
        }
        /// <summary>
        /// Resets fragment at position (x, y) to the initial value. Usually this will be infinity, unless specified differently when buffer was created.
        /// </summary>
        /// <param name="x">Fragment coord X</param>
        /// <param name="y">Fragment coord Y</param>
        public void ResetFragment(int x, int y)
        {
            DATA[XY2I(x, y)] = m_InitialVal;
        }

        /// <summary>
        /// Resets fragment to requested value
        /// </summary>
        /// <param name="x">Fragment coord X</param>
        /// <param name="y">Fragment coord Y</param>
        /// <param name="val">Default  value</param>
        public void ResetFragment(int x, int y, float val)
        {
            DATA[XY2I(x, y)] = val;
        }

        /// <summary>
        /// Upates depth buffer fragment value at position (x, y), when provided cmpFunc evaluates to true.
        /// </summary>
        /// <param name="x">Fragment coord X</param>
        /// <param name="y">Fragment coord Y</param>
        /// <param name="val">New depth value to evaluate</param>
        /// <param name="cmpFunc">Comparison function to use for new value eveluation</param>
        /// <returns>true if the buffer was updated, false otherwise</returns>
        public bool TryUpdate(int x, int y, float val, NEDepthCmpFunc cmpFunc)
        {
            int i = XY2I(x, y);
            if (cmpFunc(val,DATA[i]))
            {
                DATA[i] = val;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Upates depth buffer fragment value at position (x, y) only if new value is less than, or equal to a current value.
        /// </summary>
        /// <param name="x">Fragment coord X</param>
        /// <param name="y">Fragment coord Y</param>
        /// <param name="val">New depth value to evaluate</param>
        /// <returns>true if buffer was updated, false otherwise</returns>
        public bool TryUpdate(int x, int y, float val)
        {
            int i = XY2I(x, y);
            if (val <= DATA[i])
            {
                DATA[i] = val;
                return true;
            }
            return false;
        }

        private int XY2I(int x, int y)
        {
            return (Width * y + x);
        }
    }
}

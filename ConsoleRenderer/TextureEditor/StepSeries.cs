using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleRenderer.TextureEditor
{
    class StepSeries<T>
    {
        T[] m_Items;
        int m_MaxSteps = 10;
        int m_CurrentIndex;
        int m_TotalStepCount;
        int m_AvailableUndoCount;


        public StepSeries(int maxSteps, T initalData)
        {
            if (maxSteps < 3) maxSteps = 3;
            m_Items = new T[maxSteps];
            m_MaxSteps = maxSteps;
            m_CurrentIndex = 0;
            m_Items[m_CurrentIndex] = initalData;
            m_TotalStepCount = 1;
            m_AvailableUndoCount = 1;
        }

        public void AddStep(T data)
        {
            m_TotalStepCount++;

            m_AvailableUndoCount++;
            if (m_AvailableUndoCount > m_MaxSteps) m_AvailableUndoCount = m_MaxSteps;

            m_CurrentIndex++;
            m_CurrentIndex %= m_MaxSteps;
            m_Items[m_CurrentIndex] = data;
        }

        public void UndoStep()
        {
 
            m_AvailableUndoCount--;
            if (m_AvailableUndoCount <= 0)
            {
                m_AvailableUndoCount = 0;
                return;
            }

            m_CurrentIndex--;
            if (m_CurrentIndex < 0)
            {
                //if we can't wrap around
                if (m_TotalStepCount<m_MaxSteps)
                {
                    m_CurrentIndex = 0;
                }
                else // if we can wrap around
                {
                    if(m_AvailableUndoCount > 0)
                    {
                        m_CurrentIndex = m_MaxSteps - 1;
                    }

                }
            }
        }

        public T Data { get { return m_Items[m_CurrentIndex]; } }

       
    }
}

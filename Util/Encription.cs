using UnityEngine;
using System.Collections.Generic;

public class Encription
{
    public struct Int
    {
        private int key;
        private ulong m_value;

        public double Self
        {
            get
            {
                if (this.key == 0
                 || this.m_value == 0
                 || this.m_value % (ulong)this.key > 0)
                {
                    return 0;
                }
                else
                {

                    return (this.m_value / (ulong)this.key);
                }
                //return _value;
            }

            set
            {
            
                this.key = Random.Range(1, 255);
                this.m_value = (ulong)value * (ulong)this.key;
                
            }
        }

		public Int(double _Value)
		{
			key = 0;
            m_value = 0;
			Self = _Value;
		}

        public override string ToString()
        {
            return Self.ToString();
        }
    }

    public struct IntList
    {
        private List<Int> list;

        public List<Int> Self
        {
            get
            {
                return list;
            }

            set
            {
                this.list = value;
            }
        }

        public void Add(double value)
        {
            if (list == null)
            {
                list = new List<Int>();
            }

            Int node = new Int();
            node.Self = value;

            list.Add(node);
        }

        public void RemoveAt(int num)
        {
            if (list != null)
            {
                list.RemoveAt(num);
            }
        }

        public void Clear()
        {
            if (list != null)
            {
                list.Clear();
            }
        }
    }
}

using System.Collections.Generic;

namespace DbReader.DynamicArguments
{
    internal class DynamicMemberInfoArrayEqualityComparer : IEqualityComparer<DynamicMemberInfo[]>
    {
        public bool Equals(DynamicMemberInfo[] x, DynamicMemberInfo[] y)
        {
            if (x.Length != y.Length)
            {
                return false;
            }
            for (int i = 0; i < x.Length; i++)
            {
                if (!x[i].Equals(y[i]))
                {
                    return false;
                }
            }
            return true;
        }

        public int GetHashCode(DynamicMemberInfo[] obj)
        {
            int result = 17;
            for (int i = 0; i < obj.Length; i++)
            {
                unchecked
                {
                    result = result * 23 + obj[i].GetHashCode();
                }
            }
            return result;
        }
    }
}

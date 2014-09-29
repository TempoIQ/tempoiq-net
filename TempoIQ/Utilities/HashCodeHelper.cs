using System.Diagnostics.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TempoIQ.Utilities
{
    internal static class HashCodeHelper
    {
        private const int HashCodeMultiplier = 37;
        private const int HashCodeInitializer = 17;

        internal static int Initialize()
        {
            return HashCodeInitializer;
        }

        internal static int Hash<T>(int code, T value)
        {
            int hash = 0;
            if (value != null)
            {
                hash = value.GetHashCode();
            }
            return MakeHash(code, hash);
        }

        [SuppressMessage("Microsoft.Usage", "CA2233:OperationsShouldNotOverflow", Justification = "Deliberately overflowing.")]
        private static int MakeHash(int code, int value)
        {
            unchecked
            {
                code = (code * HashCodeMultiplier) + value;
            }
            return code;
        }
    }
}
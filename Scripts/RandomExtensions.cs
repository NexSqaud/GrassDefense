using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrassDefense.Scripts
{
    public static class RandomExtensions
    {

        public static bool NextBoolean(this Random random)
        {
            return random.Next(0, 2) == 0;
        }

    }
}

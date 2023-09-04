using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaiyanLib.Core.Helpers
{
    public static class SaiyanHelpers
    {
        public static class KiHelpers
        {
            /// <summary>
            /// Returns how Ki is actually ran per-frame. 
            /// </summary>
            /// <param name="valueInASecond">The amount of Ki per <b>SECOND</b> that is being asked for.</param>
            /// <returns>The amount of ki per <b>FRAME</b>.</returns>
            public static float KiPerFrame(float valueInASecond) { return valueInASecond / 60f;}

        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Data_Object
{

    public class ObjectPos
    {
        public Single x, y, z;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ObjectPos()
        {
            x = 0;
            y = 0;
            z = 0;
        }
        public ObjectPos(Single _x, Single _y, Single _z)
        {
            x = _x;
            y = _y;
            z = _z;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using Data_Object;
using System.Runtime.InteropServices;

namespace Data_Player
{
    public enum WeakAngle
    {
        UP,
        RIGHT,
        DOWN,
        LEFT
    }
    
    public struct S_DataPlayer
    {
        public Single pos_x;
        public Single pos_y;
        public Single pos_z;
        public Single look_a;
        public Single move_a;
        public Int16 ammos;
    }

    public struct S_WrapData
    {
        public Int32 num;
        public S_DataPlayer[] s_Data;
    }

    public class DataPlayer
    {
        public ObjectPos pos;
        public Single look_angle;
        public Single move_angle;
        public Int16 ammos_left;
        //public WeakAngle weak_angle;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public DataPlayer()
        {
            pos = new ObjectPos();
            look_angle = 0;
            move_angle = 0;
            ammos_left = 5;
        }
        public DataPlayer(string _data)
        {
            pos = new ObjectPos();
            look_angle = 0;
            move_angle = 0;
            ammos_left = 5;
            //weak_angle = WeakAngle.UP;
        }
    }
}

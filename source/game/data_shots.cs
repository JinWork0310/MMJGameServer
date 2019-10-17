using System;
using System.Collections.Generic;
using System.Text;
using Data_Object;

namespace Data_Shots
{

    public struct S_DataShots
    {
        public Single pos_x;
        public Single pos_y;
        public Single pos_z;
        public Single angle;
    }

    public class DataShots : ObjectPos
    {
        /// <summary>
        /// 始点・跳弾点・終点
        /// </summary>
        public ObjectPos startPos;
        //ObjectPos[] bouncePos;
        //ObjectPos endPos;

        public Single angle;

        public Int16 lifetime;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public DataShots()
        {
            startPos = new ObjectPos();
            //bouncePos = new ObjectPos[_bouncenum];
            //for (int i = 0; i < _bouncenum; i++)
            //{
            //    bouncePos[i] = new ObjectPos();
            //}
            //endPos = new ObjectPos();

            angle = 0;
            lifetime = 0;
        }
        public DataShots(int _bouncenum, string _data)
        {
            startPos = new ObjectPos();
            //bouncePos = new ObjectPos[_bouncenum];
            //for(int i = 0; i < _bouncenum; i++)
            //{
            //    bouncePos[i] = new ObjectPos();
            //}
            //endPos = new ObjectPos();

            angle = 0;
            lifetime = 0;
        }
    }
}

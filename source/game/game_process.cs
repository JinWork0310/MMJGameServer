using System;
using System.Collections.Generic;
using System.Text;
using DataStructures;
using System.Runtime.InteropServices;
using MMJGameServer;

namespace Game_Process
{

    public class GameProcess
    {
        int MAXPLAYER = 4;

        S_DataPlayer[] player;
        S_DataShots[] shot;

        S_DataProfile[] profile;

        S_StartingData starting;

        Int16 stage_num = 0;

        Int32 countFlame = 0;

        Int32 MAX_SHOTS = 256;

        Int32 DataSize = 0;

        Int32 shotIndex;

        Random rSeed = new Random(256);

        int[] spawnarea;

        // クラスの追加
        Timer timer;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public GameProcess(){
            Int32 num = 4;
            player = new S_DataPlayer[num];
            profile = new S_DataProfile[num];
            shot = new S_DataShots[MAX_SHOTS];
            for (int i = 0; i < num; i++)
            {
                player[i] = new S_DataPlayer();
                profile[i] = new S_DataProfile();
                profile[i].player_id = -1;
            }
            for (int i = 0; i < MAX_SHOTS; i++)
            {
                shot[i] = new S_DataShots();
            }
            spawnarea = new int[4] { -1, -1, -1, -1 };
        }

        // ゲーム処理の初期化
        public void Initialize()
        {

            // スポーンエリアの抽選
            for (int i = 0; i < MAXPLAYER; i++)
            {
                while (true)
                {
                    spawnarea[i] = rSeed.Next(MAXPLAYER);
                    bool double_check = false;
                    for (int j = 0; j < i; j++)
                    {
                        if (spawnarea[j] == spawnarea[i]) double_check = true;
                    }
                    if (!double_check) break;
                }
            }
            //spawnarea = new int[4] { 0,1,2,3};
            Console.WriteLine("SPAWN AREA : {0}", String.Join(", ", spawnarea));


            shotIndex = 0;

            countFlame = 0;

            timer = new Timer();

            Console.WriteLine(" INIT GAME! ");
        }

        public void UpdateGame()
        {
            if (countFlame >= 500)
            {
                Console.WriteLine(" keep running game_process ");
                countFlame = 0;
            }
            countFlame++;

            //for (int i = 0; i < MAX_SHOTS; i++)
            //{
            //    if (shot[i].lifetime == 1) Console.WriteLine(" SHOT No.{0} Died",i);
            //    if (shot[i].lifetime > 0) shot[i].lifetime--;
            //}

        }
        public void CloseGame()
        {
            timer = null;
        }

        public void setPlayerData(int _num, IntPtr _data)
        {
            //Console.WriteLine(string.Format("setPlayerData : {0}",_data));

            S_DataPlayer getdata = (S_DataPlayer)Marshal.PtrToStructure(_data,typeof(S_DataPlayer));
            player[_num].id = (uint)_num;
            player[_num].x = getdata.x;
            player[_num].y = getdata.y;
            player[_num].dead = getdata.dead;
            
            Console.WriteLine("Successed PlayerNumber: {0} pos_x:{1} pos_y:{2} look:{3}",
                _num, player[_num].x, player[_num].y, player[_num].angle);

        }

        public void setShotData(IntPtr _data)
        {
            //Console.WriteLine(string.Format("setShotData : {0}", _data)); Console.WriteLine(string.Format("setPlayerData : {0}", _data));

            S_DataShots getdata = (S_DataShots)Marshal.PtrToStructure(_data, typeof(S_DataShots));


            shot[shotIndex].x = getdata.x;
            shot[shotIndex].y = getdata.y;

            shot[shotIndex].angle = getdata.angle;
            shot[shotIndex].id = shotIndex;

            Console.WriteLine("Successed pos_x:{0} pos_y:{1} angle:{2} ID:{3}",
                shot[shotIndex].x, shot[shotIndex].y, shot[shotIndex].angle, shotIndex);

            shotIndex++;
            if (shotIndex >= 100) shotIndex = 0;
        }

        public IntPtr getGameData(int _num)
        {
            string data = "wow";
            IntPtr result = (IntPtr)1;
            return result;
        }

        // プレイヤー情報の格納
        public IntPtr setProfile(IntPtr _data, int _num)
        {
            //S_DataProfile getdata = (S_DataProfile)Marshal.PtrToStructure(_data, typeof(S_DataProfile));

            //getdata.name = Marshal.PtrToStringUTF8(_data, 20);
            //byte[] name = System.Text.Encoding.UTF8.GetBytes(getdata.name);

            //profile[_num].name = getdata.name;
            profile[_num].player_id = _num;

            Mrs.MRS_LOG_DEBUG("setProfile ID:{0}", profile[_num].player_id);

            IntPtr p_data = Marshal.AllocHGlobal(Marshal.SizeOf(profile[_num]));
            Marshal.StructureToPtr(profile[_num], p_data, false);

            return p_data;
        }


        public S_DataProfile getProfile(int _num)
        {
            return profile[_num];
        }

        public int getProfileSize(int _num)
        {
            return Marshal.SizeOf(profile[_num]);
        }

        public IntPtr getStartData(int _sumplayers)
        {
            starting.spawnid = spawnarea;
            starting.sumplayer = (uint)_sumplayers;

            Mrs.MRS_LOG_DEBUG("getStartData Spawn:{0} countPlayer:{1}", String.Join(", ",starting.spawnid), starting.sumplayer);
            
            IntPtr data = Marshal.AllocHGlobal(Marshal.SizeOf(starting));
            Marshal.StructureToPtr(starting, data, false);

            return data;
        }

        public IntPtr getStartData(double _time, int _sumplayers)
        {
            starting.spawnid = spawnarea;
            starting.sumplayer = (uint)_sumplayers;

            Mrs.MRS_LOG_DEBUG("getStartData Spawn:{0} countPlayer:{1}", String.Join(", ", starting.spawnid), starting.sumplayer);

            IntPtr data = Marshal.AllocHGlobal(Marshal.SizeOf(starting));
            Marshal.StructureToPtr(starting, data, false);

            return data;
        }

        public int getStartDataSize()
        {
            return Marshal.SizeOf(starting);
        }

        //public IntPtr getDataForSend(Int32 _num)
        //{
        //    S_WrapData s_Wrap = new S_WrapData();
        //    s_Wrap.s_Data = new S_DataPlayer[_num];
        //    s_Wrap.num = _num;
        //    DataSize = 0;
        //    for(int i = 0; i < _num; i++)
        //    {
        //        s_Wrap.s_Data[i] = new S_DataPlayer();
        //        s_Wrap.s_Data[i].pos_x = player[i].pos.x;
        //        s_Wrap.s_Data[i].pos_y = player[i].pos.y;
        //        s_Wrap.s_Data[i].pos_z = player[i].pos.z;
        //        s_Wrap.s_Data[i].look_a = player[i].look_angle;
        //        s_Wrap.s_Data[i].move_a = player[i].move_angle;
        //        s_Wrap.s_Data[i].ammos = player[i].ammos_left;
        //        DataSize += Marshal.SizeOf(s_Wrap.s_Data[i]);
        //    }
        //    IntPtr result = Marshal.AllocHGlobal(Marshal.SizeOf(s_Wrap.s_Data[0]));
        //    Marshal.StructureToPtr(s_Wrap.s_Data[0], result, false);
        //    return result;
        //}

        public Int32 getWrapSize() { return DataSize; }

    }
}

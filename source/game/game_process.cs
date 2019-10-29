using System;
using System.Collections.Generic;
using System.Text;
using DataStructures;
using System.Runtime.InteropServices;
using MMJGameServer;
using game_server;

namespace Game_Process
{

    public class GameProcess
    {
        DateTime time;
        int MAXPLAYER = 4;


        S_DataPlayer[] player;
        S_DataShots[] shot;

        S_DataProfile[] profile;

        S_StartingData starting;

        S_DataPlayerPackage sendPack;

        Int32 stage_num = 1;

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
            sendPack = new S_DataPlayerPackage();

            sendPack.data0 = new S_DataPlayer();
            sendPack.data1 = new S_DataPlayer();
            sendPack.data2 = new S_DataPlayer();
            sendPack.data3 = new S_DataPlayer();

            for (int i = 0; i < num; i++)
            {
                player[i] = new S_DataPlayer();
                profile[i] = new S_DataProfile();
                profile[i].player_id = i;
                profile[i].name = "";
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
            player[_num].angle = getdata.angle;
            player[_num].dead = getdata.dead;
            
            //Console.WriteLine("Successed PlayerNumber: {0} pos_x:{1} pos_y:{2} look:{3}",
                _num, player[_num].x, player[_num].y, player[_num].angle);

        }

        public IntPtr setShotData(IntPtr _data)
        {
            S_DataShots getdata = (S_DataShots)Marshal.PtrToStructure(_data, typeof(S_DataShots));

            shot[shotIndex].x = getdata.x;
            shot[shotIndex].y = getdata.y;

            shot[shotIndex].angle = getdata.angle;
            shot[shotIndex].bullet_id = shotIndex;
            shot[shotIndex].whos_shot = getdata.whos_shot;


            IntPtr p_data = Marshal.AllocHGlobal(Marshal.SizeOf(shot[shotIndex]));
            Marshal.StructureToPtr(shot[shotIndex], p_data, false);

            shotIndex++;
            if (shotIndex >= 100) shotIndex = 0;

            return p_data;
        }

        // プレイヤー情報の格納
        public IntPtr setProfile(IntPtr _data, int _num)
        {
            S_DataProfile getdata = (S_DataProfile)Marshal.PtrToStructure(_data, typeof(S_DataProfile));

            profile[_num].name = getdata.name;
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

        public IntPtr getProfileData(int _num)
        {
            IntPtr result = Marshal.AllocHGlobal(Marshal.SizeOf(profile[_num]));
            Marshal.StructureToPtr(profile[_num], result, false);

            return result;
        }

        public int getProfileSize(int _num)
        {
            return Marshal.SizeOf(profile[_num]);
        }


        public void setStageId(int _num)
        {
            stage_num = _num;
        }


        public IntPtr getStartData(int _sumplayers)
        {
            starting.spawnid = spawnarea;
            starting.sumplayer = (uint)_sumplayers;
            starting.stageid = stage_num;

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

        public IntPtr getDataForSend()
        {
            sendPack.data0 = player[0];
            sendPack.data1 = player[1];
            sendPack.data2 = player[2];
            sendPack.data3 = player[3];

            IntPtr result = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(S_DataPlayerPackage)));
            Marshal.StructureToPtr(sendPack, result, false);
            return result;
        }

        public Int32 getSendPackSize() { return Marshal.SizeOf(sendPack); }

        public Int32 getWrapSize() { return DataSize; }

        public void eraseProfileData(int _num)
        {
            profile[_num].player_id = _num;
            profile[_num].name = "";
        }

        /// <summary>
        /// 誰か抜けたら後ろのプレイヤーを前詰めしていく関数
        /// </summary>
        public void sortPlayerList()
        {
            for (int i = 0; i < MAXPLAYER; i++)
            {
                if (profile[i].name == "")
                {
                    for (int j = i + 1; j < MAXPLAYER; j++)
                    {
                        if (profile[j].name != "")
                        {
                            profile[i] = profile[j];
                            profile[i].player_id = i;
                            IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(profile[i]));
                            Marshal.StructureToPtr(profile[i], ptr, false);
                            eraseProfileData(j);

                            Marshal.FreeHGlobal(ptr);
                            break;
                        }
                    }
                }
                Console.WriteLine("Sort Profile [ {0} ] : {1} : {2}", i, profile[i].player_id, profile[i].name);
            }
        }

        public IntPtr SomeoneDeadHit(IntPtr _pay)
        {
            S_DeadHit deadHit = (S_DeadHit)Marshal.PtrToStructure(_pay,typeof(S_DeadHit));

            deadHit.whosby_id = (int)shot[deadHit.bullet_id].whos_shot;

            Console.WriteLine("Shot No.{0} Shot by No.{1} Player", deadHit.bullet_id, shot[deadHit.bullet_id].whos_shot);

            IntPtr send = Marshal.AllocCoTaskMem(Marshal.SizeOf(deadHit));
            Marshal.StructureToPtr(deadHit, send, false);

            return send;
        }

    }
}

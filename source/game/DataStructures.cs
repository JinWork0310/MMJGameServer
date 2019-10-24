using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace DataStructures
{
    enum keyLog
    {
        NONE = 0x00,
        MOVE = 0x01,
        ATTACK = 0x02
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct S_Header
    {
        public int id;
        public double time;
    }


    [StructLayout(LayoutKind.Sequential)]
    public struct S_DataProfile
    {
        public int player_id;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
        public string name;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct S_DataPlayer
    {
        public uint id;
        public float x, y;
        public float angle;
        public bool dead;
        
    }

    public struct S_DataShots
    {
        public int bullet_id;
        public uint whos_shot;
        public float x, y;
        public float angle;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct S_StartingData
    {
        [MarshalAs(UnmanagedType.ByValArray,SizeConst = 4)]
        public int[] spawnid;
        public uint sumplayer;
        public int stageid;
    }


    [StructLayout(LayoutKind.Sequential)]
    public struct S_DataPlayerPackage
    {
        public S_DataPlayer data0;
        public S_DataPlayer data1;
        public S_DataPlayer data2;
        public S_DataPlayer data3;
    }


    // スタート時送信用構造体
    public struct S_StartingPackage
    {
        [MarshalAs(UnmanagedType.LPStruct)]
        public S_Header header;
        [MarshalAs(UnmanagedType.LPStruct)]
        public S_StartingData data;
    }

    // 被弾死用データ
    [StructLayout(LayoutKind.Sequential)]
    public struct S_DeadHit
    {
        public int player_id;
        public int bullet_id;
        public int whosby_id;
    }
}
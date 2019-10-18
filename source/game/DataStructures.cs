using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace DataStructures
{
    [StructLayout(LayoutKind.Sequential)]
    public struct S_Header
    {
        public int id;
        public double time;
    }


    [StructLayout(LayoutKind.Sequential)]
    public struct S_DataProfile
    {
        //[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
        //public string name;

        public int player_id;
        public int spawn_id;
    }

    public struct S_DataPlayer
    {
        public uint id;
        public float x, y;
        public float angle;
    }

    public struct S_DataShots
    {
        public float x, y;
        public float angle;
        public int id;
    }

    public struct S_StartingData
    {
        [MarshalAs(UnmanagedType.ByValArray,SizeConst = 4)]
        public int[] spawnid;
        public uint sumplayer;

    }

}
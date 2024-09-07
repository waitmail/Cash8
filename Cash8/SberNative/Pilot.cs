using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Cash8
{
    public static class Pilot
    {
        [DllImport("SberDll/pilot_nt.dll", EntryPoint = "_TestPinpad", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int TestPinpad();

        [DllImport("SberDll/pilot_nt.dll", EntryPoint = "_GetTerminalID", CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetTerminalID(StringBuilder terminalID);

        [DllImport("SberDll/pilot_nt.dll", EntryPoint = "_ReadTrack2", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int ReadTrack2(StringBuilder track2);

        [DllImport("SberDll/pilot_nt.dll", EntryPoint = "_SuspendTrx", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int SuspendTransaction(int amount, string track2);

        [DllImport("SberDll/pilot_nt.dll", EntryPoint = "_CommitTrx", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int CommitTransaction(int amount, string track2);

        [DllImport("SberDll/pilot_nt.dll", EntryPoint = "_RollbackTrx", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int RollBackTransaction(int amount, string track2);

        [DllImport("SberDll/pilot_nt.dll", EntryPoint = "_card_authorize13", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int CardAuthorize(string track2, ref AuthAnswer13 authAnswer);

        [DllImport("SberDll/pilot_nt.dll", EntryPoint = "_close_day", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int CloseDay(ref AuthAnswer authAnswer);

        [DllImport("kernel32.dll")]
        internal static extern IntPtr GlobalFree(IntPtr hMem);

        [DllImport("SberDll/pilot_nt.dll", EntryPoint = "_get_statistics", CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr GetStatistics(ref AuthAnswer authAnswer);

    }
}

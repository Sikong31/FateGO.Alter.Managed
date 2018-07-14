using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
namespace FateGO.Alter.Managed
{
    //Single-threaded
    class FGOAlterLog
    {
        /*   log   */
        public static void print(string s)
        {
            using (StreamWriter sw = File.AppendText(Application.persistentDataPath + "/log.txt"))
            {
                sw.WriteLine("[{0}] {1}", DateTime.Now.ToString("yy/MM/dd hh:mm:ss"), s);
            }
        }

        public static void print(string format, Vector3 vec)
        {
            print(format, vec.x, vec.y, vec.z);
        }

        public static void print(string format, params object[] args)
        {
            print(string.Format(format, args));
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.IO;

namespace ShareLib17
{

    public interface IFolderAction
    {
        bool execute(string Dir);
    }

    public class MyIO
    {

        public static void ActOnFolder(string Dir, bool recursive, IFolderAction act)
        {
            act.execute(Dir);
            if (!recursive)
                return;
            string[] dirs = Directory.GetDirectories(Dir);
            foreach (string dir in dirs)
                ActOnFolder(dir, true, act);
        }



    }

}
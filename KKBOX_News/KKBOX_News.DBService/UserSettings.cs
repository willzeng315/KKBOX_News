using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KKBOX_News.AppService
{
    public class UserSettings
    {
        private static UserSettings _instance;

        private UserSettings() { }

        public static UserSettings Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new UserSettings();
                return _instance;
            }
        }
        public Int32 UserID;
        public Boolean IsOpenExternalWeb;
        public Boolean IsOpenAutoUpdate;
        public Int32 UpdateInterval;
    }
}

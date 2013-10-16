using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KKBOX_News.AppService
{
    public class LoginSettings
    {
        private static LoginSettings _instance;

        private LoginSettings() { }

        public static LoginSettings Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new LoginSettings();
                return _instance;
            }
        }

        public String LastAccount;
        public String LastPassword;
        public String CurrentAccount;
        public Boolean IsSaveAccountAndPassword;
        public Boolean Login;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KKBOX_News.DBService
{
    public class ArticleNavigationPasser
    {
        private static ArticleNavigationPasser _instance;

        private ArticleNavigationPasser() { }

        public static ArticleNavigationPasser Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new ArticleNavigationPasser();
                return _instance;
            }
        }
        public List<ArticleItem> Articles = new List<ArticleItem>();
        
    }
}

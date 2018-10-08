using System.Collections.Generic;
using System.Linq;
using model.ad;
using System.Configuration;
using lib.LibDal;
using LibGetAdData;
using NLog;
using System;

namespace GetAdData
{
    class GetAdData
    {
        public static string database_path { get; set; }
        public static string domain_path { get; set; }
        public static string domain_name { get; set; }

        public static string json_path { get; set; }
        public static string img_path { get; set; }

        private static Logger logger = LogManager.GetCurrentClassLogger();


        static void Main(string[] args)
        {
            try
            { 
                logger.Info("START");
                Init();
                Get_Users();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Main");
            }

        }

        static void Init()
        {
            database_path = ConfigurationManager.AppSettings["database_path"].ToString();
            domain_path = ConfigurationManager.AppSettings["domain_path"].ToString();
            domain_name = ConfigurationManager.AppSettings["domain_name"].ToString();

            json_path = ConfigurationManager.AppSettings["json_path"].ToString();
            img_path = ConfigurationManager.AppSettings["img_path"].ToString();
            logger.Info("Initalisation OK");
        }

        static void Get_Users()
        {
            // GET ACTIVE DIRECTORY USERS
            GetActiveDirectory _objADinfo = new GetActiveDirectory(domain_path, domain_name);
            List<user> Users = _objADinfo.Get_Users();
            List<user> NormalUsers = Users.Where(x => x.isNormalAccount == true).ToList<user>();
            List<user> NormalUsersNotDisabled = NormalUsers.Where(x => x.isAccountDisabled == false).ToList<user>();
            _objADinfo = null;

            // SAVE DATA TO LITEDB DATABASE
            context _userContext = new context(database_path + "user.db");
            _userContext.Upsert<user>(NormalUsersNotDisabled);


            //test export to Json
            _userContext.ExportToJson<user>(Users);
        }
        static void Get_Hosts()
        {
            // GET ACTIVE DIRECTORY USERS
            GetActiveDirectory _objADinfo = new GetActiveDirectory(domain_path, domain_name);
            List<user> Users = _objADinfo.Get_Users();
            List<user> NormalUsers = Users.Where(x => x.isNormalAccount == true).ToList<user>();
            List<user> NormalUsersNotDisabled = NormalUsers.Where(x => x.isAccountDisabled == false).ToList<user>();
            _objADinfo = null;

            // SAVE DATA TO LITEDB DATABASE
            context _userContext = new context(database_path + "user.db");
            _userContext.Upsert<user>(NormalUsersNotDisabled);


            //test export to Json
            _userContext.ExportToJson<user>(Users);
        }

    }
}

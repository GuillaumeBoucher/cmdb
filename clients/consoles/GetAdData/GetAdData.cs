using System.Collections.Generic;
using System.Linq;
using model.ad;
using System.Configuration;
using lib.LibDal;
using LibGetAdData;

namespace GetAdData
{
    class GetAdData
    {        
        static void Main(string[] args)
        {
            string database_path = ConfigurationManager.AppSettings["database_path"].ToString();
            string domain_path = ConfigurationManager.AppSettings["domain_path"].ToString();
            string domain_name = ConfigurationManager.AppSettings["domain_name"].ToString();

            string json_path = ConfigurationManager.AppSettings["json_path"].ToString();
            string img_path = ConfigurationManager.AppSettings["img_path"].ToString();

            // GET ACTIVE DIRECTORY USERS
            GetActiveDirectory _objADinfo = new GetActiveDirectory(domain_path, domain_name, true, true);
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

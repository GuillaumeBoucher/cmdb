using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace model.hosts
{
    public class host_log
    {
        //constructeur
        public host_log()
        {
            this.app_TopTenErreur = new List<host_log_item>();
            this.app_TopTenWarrning = new List<host_log_item>();
            this.sys_TopTenErreur = new List<host_log_item>();
            this.sys_TopTenWarrning = new List<host_log_item>();
        }
        public int app_nb_erreur { get; set; }
        public int app_nb_warrning { get; set; }

        public int sys_nb_erreur { get; set; }
        public int sys_nb_warrning { get; set; }

        public List<host_log_item> app_TopTenErreur { get; set; }
        public List<host_log_item> app_TopTenWarrning { get; set; }

        public List<host_log_item> sys_TopTenErreur { get; set; }
        public List<host_log_item> sys_TopTenWarrning { get; set; }

    }
    public class host_log_item
    {
        public int count { get; set; }
        public string message { get; set; }
    }
}

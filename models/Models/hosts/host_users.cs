using System;
using System.Collections.Generic;
using System.DirectoryServices;

namespace model.hosts
{
    public class host_users
    {
        public host_users()
        {
            this.profiles = new List<string>();
            this.localGroups = new List<string>();
            this.localUsers = new List<string>();
        }
        public String CurentLogUserName { get; set; }
        public Boolean IsCurentLogUserIsAdmin { get; set; }
        public List<String>  profiles { get; set; }
        public List<String> localGroups { get; set; }
        public List<String> localUsers { get; set; }

     

    }
}

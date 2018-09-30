using System;
using System.Collections.Generic;


namespace model.hosts
{
    public class host_users
    {
        public host_users()
        {
            this.profiles = new List<string>();
        }
        public String CurentLogUserName { get; set; }
        public List<String>  profiles { get; set; }
    }
}

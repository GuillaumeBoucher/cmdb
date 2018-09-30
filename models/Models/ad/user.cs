using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace model.ad
{
   public class user
    {
        public int id { get; set; }        
        public Boolean isNormalAccount { get; set; }
        public Boolean AccoutIsLock { get; set; }
        public Boolean isAccountExpire { get; set; }
        public Boolean isAccountDisabled { get; set; }
        

        public string nom { get; set; }
        public string prenom { get; set; }
        public string mail { get; set; }
        public string tel { get; set; }
        public string login { get; set; }
        public string photo { get; set; }
        public string thumbnailphoto { get; set; }

        public Byte[] b_photo { get; set; }
        public Byte[] b_thumbnailphoto { get; set; }

        public DateTime whenchanged { get; set; }
        public DateTime whencreated { get; set; }
        public Int32 admincount { get; set; }
        public Int32 badpwdcount { get; set; }
        public Int32 logoncount { get; set; }
        public DateTime accountexpires { get; set; }
        public Int64 badpasswordtime { get; set; }
        public Int64 lastlogoff { get; set; }
        public DateTime lastlogon { get; set; }
        public DateTime lastlogontimestamp { get; set; }
        public Int64 lockouttime { get; set; }
        public DateTime pwdlastset { get; set; }
        public List<String> memberof { get; set; }
        public String adspath { get; set; }
        public String adspath_file { get; set; }
        public String altrecipient { get; set; }
        public String cn { get; set; }
        public String comment { get; set; }
        public String company { get; set; }
        public String department { get; set; }
        public String description { get; set; }
        public String displayname { get; set; }
        public String displaynameprintable { get; set; }
   //     public String distinguishedname { get; set; }
        public String extensionattribute1 { get; set; }
        public String facsimiletelephonenumber { get; set; }
        public String givenname { get; set; }
        public String homemdb { get; set; }
        //public String homemta { get; set; }
        public String info { get; set; }
        public String initials { get; set; }
        public String ipphone { get; set; }
      //  public String legacyexchangedn { get; set; }        
        public String mailnickname { get; set; }
        public String manager { get; set; }

        public String name { get; set; }
        public String objectcategory { get; set; }
        public String physicaldeliveryofficename { get; set; }
        public String postalcode { get; set; }
        public String postofficebox { get; set; }
        public String samaccountname { get; set; }
        public String streetaddress { get; set; }
        public String targetaddress { get; set; }
        public String title { get; set; }
        public String userparameters { get; set; }
        public String userprincipalname { get; set; }
        public String wwwhomepage { get; set; }
        public String sn { get; set; }
        public String l { get; set; }
        public String telephonenumber { get; set; }



    }
}

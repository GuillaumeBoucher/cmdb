using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

using model.ad;
using NLog;
using NLog.Config;

namespace LibGetAdData
{
    public class GetActiveDirectory
    {
        // private members
        // ***********************************************
        private DirectoryEntry _entryRoot;
        private string adName;
        private string adPath;        
        private Logger logger = LogManager.GetCurrentClassLogger();

        private enum Enum_UserAccountControl : int
        {
            /// <summary>
            /// The logon script is executed. 
            ///</summary>
            SCRIPT = 0x00000001,

            /// <summary>
            /// The user account is disabled. 
            ///</summary>
            ACCOUNTDISABLE = 0x00000002,

            /// <summary>
            /// The home directory is required. 
            ///</summary>
            HOMEDIR_REQUIRED = 0x00000008,

            /// <summary>
            /// The account is currently locked out. 
            ///</summary>
            LOCKOUT = 0x00000010,

            /// <summary>
            /// No password is required. 
            ///</summary>
            PASSWD_NOTREQD = 0x00000020,

            /// <summary>
            /// The user cannot change the password. 
            ///</summary>
            /// <remarks>
            /// Note:  You cannot assign the permission settings of PASSWD_CANT_CHANGE by directly modifying the UserAccountControl attribute. 
            /// For more information and a code example that shows how to prevent a user from changing the password, see User Cannot Change Password.
            // </remarks>
            PASSWD_CANT_CHANGE = 0x00000040,

            /// <summary>
            /// The user can send an encrypted password. 
            ///</summary>
            ENCRYPTED_TEXT_PASSWORD_ALLOWED = 0x00000080,

            /// <summary>
            /// This is an account for users whose primary account is in another domain. This account provides user access to this domain, but not 
            /// to any domain that trusts this domain. Also known as a local user account. 
            ///</summary>
            TEMP_DUPLICATE_ACCOUNT = 0x00000100,

            /// <summary>
            /// This is a default account type that represents a typical user. 
            ///</summary>
            NORMAL_ACCOUNT = 0x00000200,

            /// <summary>
            /// This is a permit to trust account for a system domain that trusts other domains. 
            ///</summary>
            INTERDOMAIN_TRUST_ACCOUNT = 0x00000800,

            /// <summary>
            /// This is a computer account for a computer that is a member of this domain. 
            ///</summary>
            WORKSTATION_TRUST_ACCOUNT = 0x00001000,

            /// <summary>
            /// This is a computer account for a system backup domain controller that is a member of this domain. 
            ///</summary>
            SERVER_TRUST_ACCOUNT = 0x00002000,

            /// <summary>
            /// Not used. 
            ///</summary>
            Unused1 = 0x00004000,

            /// <summary>
            /// Not used. 
            ///</summary>
            Unused2 = 0x00008000,

            /// <summary>
            /// The password for this account will never expire. 
            ///</summary>
            DONT_EXPIRE_PASSWD = 0x00010000,

            /// <summary>
            /// This is an MNS logon account. 
            ///</summary>
            MNS_LOGON_ACCOUNT = 0x00020000,

            /// <summary>
            /// The user must log on using a smart card. 
            ///</summary>
            SMARTCARD_REQUIRED = 0x00040000,

            /// <summary>
            /// The service account (user or computer account), under which a service runs, is trusted for Kerberos delegation. Any such service 
            /// can impersonate a client requesting the service. 
            ///</summary>
            TRUSTED_FOR_DELEGATION = 0x00080000,

            /// <summary>
            /// The security context of the user will not be delegated to a service even if the service account is set as trusted for Kerberos delegation. 
            ///</summary>
            NOT_DELEGATED = 0x00100000,

            /// <summary>
            /// Restrict this principal to use only Data Encryption Standard (DES) encryption types for keys. 
            ///</summary>
            USE_DES_KEY_ONLY = 0x00200000,

            /// <summary>
            /// This account does not require Kerberos pre-authentication for logon. 
            ///</summary>
            DONT_REQUIRE_PREAUTH = 0x00400000,

            /// <summary>
            /// The user password has expired. This flag is created by the system using data from the Pwd-Last-Set attribute and the domain policy. 
            ///</summary>
            PASSWORD_EXPIRED = 0x00800000,

            /// <summary>
            /// The account is enabled for delegation. This is a security-sensitive setting; accounts with this option enabled should be strictly 
            /// controlled. This setting enables a service running under the account to assume a client identity and authenticate as that user to 
            /// other remote servers on the network.
            ///</summary>
            TRUSTED_TO_AUTHENTICATE_FOR_DELEGATION = 0x01000000,

            /// <summary>
            /// 
            /// </summary>
            PARTIAL_SECRETS_ACCOUNT = 0x04000000,

            /// <summary>
            /// 
            /// </summary>
            USE_AES_KEYS = 0x08000000
        }

        //private string LogMessage;
        private List<string> ListLogMessage = new List<string>();

        // public members
        // ***********************************************

        //     public List<base_string> ad_string_SchemaClassName = new List<base_string>();


        //    public List<ad_computer> ad_Computers = new List<ad_computer>();
        //    public List<ad_group> ad_Groups = new List<ad_group>();
        //    public List<ad_organizationalUnit> ad_orgUnits = new List<ad_organizationalUnit>();

        // Constructeur
        // ***********************************************
        public GetActiveDirectory(string path, string name)
        {
            this.adName = name;
            this.adPath = path;
            this.GetFirstRoot();            

        }

        // Private Méthodes
        // ***********************************************
        private void GetFirstRoot()
        {
            string currentUserSid = WindowsIdentity.GetCurrent().User.Value;

            PrincipalContext ctx = new PrincipalContext(ContextType.Domain, this.adName);
            UserPrincipal up = UserPrincipal.FindByIdentity(ctx, IdentityType.Sid, currentUserSid);
            DirectoryEntry entry = up.GetUnderlyingObject() as DirectoryEntry;
            entry.Path = this.adPath;
            this._entryRoot = entry;
        }
        private LogLevel GetLogLevel(string level)
        {
            //Trace - very detailed logs, which may include high - volume information such as protocol payloads.This log level is typically only enabled during development
            //Debug - debugging information, less detailed than trace, typically not enabled in production environment.
            //Info - information messages, which are normally enabled in production environment
            //Warn - warning messages, typically for non - critical issues, which can be recovered or which are temporary failures
            //Error - error messages - most of the time these are Exceptions
            //Fatal - very serious errors!
            LogLevel _ret = LogLevel.Info;
            switch (level.ToLower())
            {
                case "trace":
                    {
                        _ret = LogLevel.Trace;
                        break;
                    }
                case "debug":
                    {
                        _ret = LogLevel.Debug;
                        break;
                    }
                case "info":
                    {
                        _ret = LogLevel.Info;
                        break;
                    }
                case "warn":
                    {
                        _ret = LogLevel.Warn;
                        break;
                    }
                case "error":
                    {
                        _ret = LogLevel.Error;
                        break;
                    }
                case "fatal":
                    {
                        _ret = LogLevel.Fatal;
                        break;
                    }
            }

            return _ret;
        }
        private void LogFileRotate()
        {
            //Store the number of days after which you want to delete the logs.
            string log_rotate_nombre_de_jours_max = ConfigurationManager.AppSettings["log_rotate_nombre_de_jours_max"].ToString();
            int Days = Convert.ToInt32(log_rotate_nombre_de_jours_max);

            // Storing the path of the directory where the logs are stored.
            String strpath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase).Substring(6) + "\\Logs\\";
            if (File.Exists(strpath))
            {
                //Fetching all the files from the folder.
                String[] strFiles = Directory.GetFiles(strpath);
                foreach (string files in strFiles)
                {
                    //For each file checking the creation date with the current date.
                    FileInfo objFile = new FileInfo(files);
                    if (objFile.CreationTime <= DateTime.Now.AddDays(-Days))
                    {
                        //Delete the file.
                        objFile.Delete();
                    }
                }

                //If folder contains no file then delete the folder also.
                if (Directory.GetFiles(strpath).Length == 0)
                {
                    DirectoryInfo objSubDir = new DirectoryInfo(strpath);
                    //Delete the folder.
                    objSubDir.Delete();
                }
            }
        }
       
        private SearchResultCollection FindObjectCategory(string ObjectCategory)
        {
            DirectoryEntry startingPoint = this._entryRoot;
            DirectorySearcher searcher = new DirectorySearcher(startingPoint);
            searcher.Filter = "(objectCategory=" + ObjectCategory + ")";
            if (ObjectCategory == "user")
            {
                searcher.PropertiesToLoad.Add("samaccountname");
                searcher.PropertiesToLoad.Add("displayname");
                searcher.PropertiesToLoad.Add("mail");
                searcher.PropertiesToLoad.Add("telephoneNumber");
                searcher.PropertiesToLoad.Add("department");
                searcher.PropertiesToLoad.Add("title");
            }
            SearchResultCollection ResultList = searcher.FindAll();
            return ResultList;
        }
        private List<string> PrintObjectClass(SearchResult res, List<string> ObjectClass)
        {

            //GetPropertyName
            List<string> ListPropertyName = new List<string>();
            foreach (string s in res.Properties.PropertyNames)
            {
                if (s.Contains("-"))
                {
                    string s1 = s.Replace('-', '_');
                    ListPropertyName.Add(s1);
                }
                else
                {
                    ListPropertyName.Add(s);
                }
            }

            foreach (string s in ListPropertyName)
            {
                string ClassMember = "\t public ";
                string returnValue = "";
                string search_property_name = "";

                search_property_name = s;

                if (search_property_name.Contains("_"))
                {
                    search_property_name = search_property_name.Replace("_", "-");
                }

                if (res.Properties[search_property_name] != null)
                {
                    if (res.Properties[search_property_name].Count == 1)
                    {
                        var ret_v = res.Properties[search_property_name][0];

                        string property_type = ret_v.GetType().Name;
                        ClassMember = ClassMember + " " + property_type + " " + s + " { get; set; }";
                        returnValue = res.Properties[search_property_name][0].ToString();

                    }
                    else
                    {
                        List<string> ret = new List<string>();
                        string property_type = "";

                        var ret_v = res.Properties[search_property_name][0];
                        property_type = ret_v.GetType().Name;
                        returnValue = res.Properties[search_property_name][0].ToString();
                        ret.Add(returnValue);

                        ClassMember = ClassMember + " List<" + property_type + "> " + s + " { get; set; }";

                    }

                    if (!ObjectClass.Contains(ClassMember))
                    {
                        ObjectClass.Add(ClassMember);
                    }
                }

            }
            return ObjectClass;

        }
        private List<string> RemoveDoublon(List<String> data)
        {
            List<string> _ret = new List<string>();

            List<string[]> ListArray = new List<string[]>();
            foreach (string s in data)
            {
                string[] Tag = s.Split(' ');
                ListArray.Add(Tag);
            }

            int NbMemebers = data.Count;

            // Add only List Member
            for (int i = 0; i < NbMemebers; i++)
            {
                string[] Tag = ListArray[i];
                string type = Tag[3];
                if (type.Contains("List"))
                {
                    _ret.Add(data[i]);
                }
            }

            // Add other member only if member doesnt exist allready !
            for (int i = 0; i < NbMemebers; i++)
            {
                string[] Tag = ListArray[i];
                string type = Tag[3];
                string memeberName = Tag[4];
                if (!type.Contains("List"))
                {
                    string findDouble = _ret.Find(x => x.Contains(memeberName));
                    if (!(findDouble is null))
                    {
                        string[] TagfindDouble = findDouble.Split(' ');
                        string typeDouble = Tag[3];
                        if (typeDouble == type)
                        {
                            // on ne fait rien pour ne pas créer un doublon
                        }
                        else
                        {
                            // est-ce qu'on change le type du list<?>                            
                        }

                    }
                    else
                    {
                        // On ajoute l'entré car ce n'est pas un doublon
                        _ret.Add(data[i]);
                    }

                }
            }

            //on trie dans l'ordre alphabetique
            _ret = _ret.OrderBy(q => q).ToList();
            return _ret;
        }
        private T ConvertResultToList<T>(SearchResult res, T newObjet)
        {

            ResultPropertyCollection myResultPropColl = res.Properties;

            //Get Definition du type de sortie    
            Type myType = typeof(T);
            PropertyInfo[] pi = myType.GetProperties();
            int Dest___NbMember = pi.Count<PropertyInfo>();
            int Source_NbMember = res.Properties.PropertyNames.Count;
            if (Source_NbMember > Dest___NbMember)
            {
                Console.WriteLine("il manque des membres dans la classe de destination !");
            }

            foreach (PropertyInfo s in pi)
            {
                string attribut = s.Name;
                string type_attribut = s.PropertyType.Name;


                ResultPropertyValueCollection obj = myResultPropColl[attribut];
                if (obj.Count == 0)
                {
                    newObjet.GetType().GetProperty(attribut).SetValue(newObjet, null);
                }
                bool isMultipleValueMember = false;
                if (obj.Count == 1)
                {
                    isMultipleValueMember = false;
                    switch (type_attribut)
                    {
                        case "Boolean":
                            {
                                IEnumerable<Boolean> er = obj.OfType<Boolean>();
                                Boolean b = er.Single<Boolean>();
                                newObjet.GetType().GetProperty(attribut).SetValue(newObjet, b);
                                break;
                            }
                        case "Byte[]":
                            {
                                IEnumerable<Byte[]> er = obj.OfType<Byte[]>();
                                Byte[] b = er.Single<Byte[]>();
                                newObjet.GetType().GetProperty(attribut).SetValue(newObjet, b);
                                break;
                            }
                        case "DateTime":
                            {
                                IEnumerable<DateTime> er = obj.OfType<DateTime>();
                                DateTime b = er.Single<DateTime>();
                                newObjet.GetType().GetProperty(attribut).SetValue(newObjet, b);

                                break;
                            }
                        case "Int32":
                            {
                                IEnumerable<Int32> er = obj.OfType<Int32>();
                                Int32 b = er.Single<Int32>();
                                newObjet.GetType().GetProperty(attribut).SetValue(newObjet, b);
                                break;
                            }
                        case "Int64":
                            {
                                IEnumerable<Int64> er = obj.OfType<Int64>();
                                Int64 b = er.Single<Int64>();
                                newObjet.GetType().GetProperty(attribut).SetValue(newObjet, b);
                                break;
                            }
                        case "String":
                            {
                                IEnumerable<String> er = obj.OfType<String>();
                                String b = er.Single<String>();
                                newObjet.GetType().GetProperty(attribut).SetValue(newObjet, b);
                                break;
                            }
                        case "List`1":
                            {
                                isMultipleValueMember = true;
                                break;
                            }
                        default:
                            {
                                Console.WriteLine(type_attribut + " NON GERER LIGNE 345");
                                break;
                            }

                    }

                }
                if (obj.Count > 1 || isMultipleValueMember)
                {

                    Type DestinationType = s.PropertyType;
                    String soustype = DestinationType.GenericTypeArguments[0].Name;
                    String DestinationTypeString = "List<" + soustype + ">";
                    switch (DestinationTypeString)
                    {
                        case "List<DateTime>":
                            {
                                List<DateTime> _ret = new List<DateTime>();
                                for (int j = 0; j < obj.Count; j++)
                                {
                                    DateTime obj2 = (DateTime)myResultPropColl[attribut][j];
                                    _ret.Add(obj2);
                                }
                                newObjet.GetType().GetProperty(attribut).SetValue(newObjet, _ret);
                                break;
                            }
                        case "List<Int32>":
                            {
                                List<Int32> _ret = new List<Int32>();
                                for (int j = 0; j < obj.Count; j++)
                                {
                                    Int32 obj2 = (Int32)myResultPropColl[attribut][j];
                                    _ret.Add(obj2);
                                }
                                newObjet.GetType().GetProperty(attribut).SetValue(newObjet, _ret);
                                break;
                            }
                        case "List<Byte[]>":
                            {
                                List<Byte[]> _ret = new List<Byte[]>();
                                for (int j = 0; j < obj.Count; j++)
                                {
                                    Byte[] obj2 = (Byte[])myResultPropColl[attribut][j];
                                    _ret.Add(obj2);
                                }
                                newObjet.GetType().GetProperty(attribut).SetValue(newObjet, _ret);
                                break;
                            }
                        case "List<String>":
                            {
                                List<String> _ret = new List<String>();
                                for (int j = 0; j < obj.Count; j++)
                                {
                                    String obj2 = myResultPropColl[attribut][j].ToString();
                                    _ret.Add(obj2);
                                }
                                newObjet.GetType().GetProperty(attribut).SetValue(newObjet, _ret);
                                break;
                            }
                        default:
                            {
                                Console.WriteLine(DestinationTypeString + " NON GERER LIGNE 345");
                                break;
                            }
                    }
                }
            }
            return newObjet;
        }

        // Public Méthodes
        // ***********************************************
        public void PrintClass(string className)
        {
            SearchResultCollection resultCollection = this.FindObjectCategory(className);
            List<string> ObjectClass = new List<string>();
            foreach (SearchResult res in resultCollection)
            {
                ObjectClass = this.PrintObjectClass(res, ObjectClass);
            }
            //on trie dans l'ordre alphabetique
            ObjectClass = ObjectClass.OrderBy(q => q).ToList();

            //on suprime les doublon
            ObjectClass = this.RemoveDoublon(ObjectClass);

            //print to console
            Console.WriteLine("public class ad_" + className + ": base_core");
            Console.WriteLine("{");
            foreach (string ligne in ObjectClass)
            {
                Console.WriteLine("\t " + ligne);
            }
            Console.WriteLine("}");
            Console.ReadKey();
        }

        public List<string> Get_OU()
        {
            List<string> _ret = new List<string>();

            DirectoryEntry startingPoint = this._entryRoot;
            DirectorySearcher searcher = new DirectorySearcher(startingPoint);
            searcher.Filter = "(objectCategory=organizationalUnit)";
            foreach (SearchResult res in searcher.FindAll())
            {
                string _obj = "";
                _obj = res.Path;
                _ret.Add(_obj);
            }
            return _ret;
        }
        //public List<base_string> Get_String_SchemaClassNames()
        //{
        //    DirectoryEntries entries = this._entryRoot.Children;
        //    foreach (DirectoryEntry dr in entries)
        //    {
        //        base_string SchemaClassName = new base_string();
        //        SchemaClassName._name = dr.SchemaClassName;
        //        if (!this.ad_string_SchemaClassName.Contains(SchemaClassName))
        //        {
        //            this.ad_string_SchemaClassName.Add(SchemaClassName);
        //        }
        //    }
        //    return this.ad_string_SchemaClassName;
        //}

        public List<user> Get_Users()
        {
            List<user> _ret = new List<user>();
            DirectoryEntry startingPoint = this._entryRoot;
            DirectorySearcher searcher = new DirectorySearcher(startingPoint);

            searcher.Filter = "(objectCategory=user)";
            searcher.PropertiesToLoad.Add("jpegphoto");
            searcher.PropertiesToLoad.Add("thumbnailphoto");
            searcher.PropertiesToLoad.Add("mail");
            searcher.PropertiesToLoad.Add("title");
            searcher.PropertiesToLoad.Add("whenchanged");
            searcher.PropertiesToLoad.Add("whencreated");
            searcher.PropertiesToLoad.Add("admincount");
            searcher.PropertiesToLoad.Add("badpwdcount");
            searcher.PropertiesToLoad.Add("logoncount");
            searcher.PropertiesToLoad.Add("accountexpires");
            searcher.PropertiesToLoad.Add("badpasswordtime");
            searcher.PropertiesToLoad.Add("lastlogoff");
            searcher.PropertiesToLoad.Add("lastlogon");
            searcher.PropertiesToLoad.Add("lastlogontimestamp");
            searcher.PropertiesToLoad.Add("lockouttime");
            searcher.PropertiesToLoad.Add("pwdlastset");
            searcher.PropertiesToLoad.Add("memberof");
            searcher.PropertiesToLoad.Add("adspath");
            searcher.PropertiesToLoad.Add("altrecipient");
            searcher.PropertiesToLoad.Add("cn");
            searcher.PropertiesToLoad.Add("comment");
            searcher.PropertiesToLoad.Add("company");
            searcher.PropertiesToLoad.Add("department");
            searcher.PropertiesToLoad.Add("description");
            searcher.PropertiesToLoad.Add("displayname");
            searcher.PropertiesToLoad.Add("displaynameprintable");
            //searcher.PropertiesToLoad.Add("distinguishedname");
            searcher.PropertiesToLoad.Add("extensionattribute1");
            searcher.PropertiesToLoad.Add("facsimiletelephonenumber");
            searcher.PropertiesToLoad.Add("givenname");
            searcher.PropertiesToLoad.Add("homemdb");
            //searcher.PropertiesToLoad.Add("homemta");
            searcher.PropertiesToLoad.Add("info");
            searcher.PropertiesToLoad.Add("initials");
            searcher.PropertiesToLoad.Add("ipphone");
            //searcher.PropertiesToLoad.Add("legacyexchangedn");
            searcher.PropertiesToLoad.Add("mailnickname");
            searcher.PropertiesToLoad.Add("manager");
            searcher.PropertiesToLoad.Add("name");
            searcher.PropertiesToLoad.Add("objectcategory");
            searcher.PropertiesToLoad.Add("physicaldeliveryofficename");
            searcher.PropertiesToLoad.Add("postalcode");
            searcher.PropertiesToLoad.Add("postofficebox");
            searcher.PropertiesToLoad.Add("samaccountname");
            searcher.PropertiesToLoad.Add("streetaddress");
            searcher.PropertiesToLoad.Add("targetaddress");
            searcher.PropertiesToLoad.Add("userparameters");
            searcher.PropertiesToLoad.Add("userprincipalname");
            searcher.PropertiesToLoad.Add("wwwhomepage");
            searcher.PropertiesToLoad.Add("sn");
            searcher.PropertiesToLoad.Add("l");
            searcher.PropertiesToLoad.Add("telephonenumber");
            searcher.PropertiesToLoad.Add("userAccountControl");






            searcher.PageSize = 1000;

            SearchResultCollection ResultList = searcher.FindAll();
            if (ResultList != null)
            {
                foreach (SearchResult result in ResultList)
                {
                    user _usr = new user();
                    foreach (DictionaryEntry property in result.Properties)
                    {
                        string k = property.Key.ToString();
                        if (k == "memberof")
                        {
                            //TODO gerer les membres
                            //int lkll = 0;
                        }
                        string v = "";
                        foreach (var val in (property.Value as ResultPropertyValueCollection))
                        {
                            v = val.ToString().ToLower(); ;
                        }

                        switch (k)
                        {
                            case "jpegphoto":
                            case "thumbnailphoto":
                                {

                                    //Get Byte[] data from active directory
                                    ResultPropertyCollection myResultPropColl = result.Properties;
                                    ResultPropertyValueCollection obj = myResultPropColl[k];
                                    IEnumerable<Byte[]> er = obj.OfType<Byte[]>();
                                    Byte[] imageBytes = er.Single<Byte[]>();



                                    //string byte_array = "";
                                    //foreach(byte b in imageBytes)
                                    //{
                                    //    byte_array = byte_array + "|" + b.ToString();
                                    //}
                                    //byte_array = byte_array.Substring(1, byte_array.Length - 1);


                                    if (k == "jpegphoto")
                                    {
                                        _usr.b_photo = imageBytes;
                                    }

                                    if (k == "thumbnailphoto")
                                    {
                                        _usr.b_thumbnailphoto = imageBytes;
                                    }


                                    break;
                                }
                            case "useraccountcontrol":
                                {

                                    int userAccountControlValue = Convert.ToInt32(v);
                                    Enum_UserAccountControl userAccountControl = (Enum_UserAccountControl)userAccountControlValue;

                                    // This gets a comma separated string of the flag names that apply.
                                    string userAccountControlFlagNames = userAccountControl.ToString();

                                    // This is how you test for an individual flag.
                                    bool isNormalAccount = (userAccountControl & Enum_UserAccountControl.NORMAL_ACCOUNT) == Enum_UserAccountControl.NORMAL_ACCOUNT;
                                    bool isAccountDisabled = (userAccountControl & Enum_UserAccountControl.ACCOUNTDISABLE) == Enum_UserAccountControl.ACCOUNTDISABLE;
                                    bool isAccountLockedOut = (userAccountControl & Enum_UserAccountControl.LOCKOUT) == Enum_UserAccountControl.LOCKOUT;
                                    bool isAccountExpire = (userAccountControl & Enum_UserAccountControl.DONT_EXPIRE_PASSWD) == Enum_UserAccountControl.DONT_EXPIRE_PASSWD;
                                    bool isAccountPassCantChange = (userAccountControl & Enum_UserAccountControl.PASSWD_CANT_CHANGE) == Enum_UserAccountControl.PASSWD_CANT_CHANGE;
                                    bool isAccountPassExpired = (userAccountControl & Enum_UserAccountControl.PASSWORD_EXPIRED) == Enum_UserAccountControl.PASSWORD_EXPIRED;

                                    _usr.isNormalAccount = isNormalAccount;
                                    _usr.AccoutIsLock = isAccountLockedOut;
                                    _usr.isAccountExpire = isAccountExpire;
                                    _usr.isAccountDisabled = isAccountDisabled;

                                    break;
                                }
                            case "mail": { _usr.mail = v; break; }
                            case "title": { _usr.title = v; break; }
                            case "whenchanged": { _usr.whenchanged = Convert.ToDateTime(v); break; }
                            case "whencreated": { _usr.whencreated = Convert.ToDateTime(v); break; }
                            case "admincount": { _usr.admincount = Convert.ToInt32(v); break; }
                            case "badpwdcount": { _usr.badpwdcount = Convert.ToInt32(v); break; }
                            case "logoncount": { _usr.logoncount = Convert.ToInt32(v); break; }
                            case "accountexpires":
                                {
                                    Int64 _accountexpires = Convert.ToInt64(v);
                                    if (_accountexpires == 9223372036854775807 || _accountexpires == 0)
                                    {
                                        //do not expire
                                        _usr.accountexpires = new DateTime(9999, 1, 1);

                                    }
                                    else
                                    {
                                        _usr.accountexpires = DateTime.FromFileTime(_accountexpires);
                                        _usr.accountexpires = _usr.accountexpires.AddDays(-1);
                                    }

                                    break;
                                }
                            case "badpasswordtime": { _usr.badpasswordtime = Convert.ToInt64(v); break; }
                            case "lastlogoff": { _usr.lastlogoff = Convert.ToInt64(v); break; }
                            case "lastlogon":
                                {
                                    Int64 _lastlogon = Convert.ToInt64(v);

                                    _usr.lastlogon = DateTime.FromFileTime(_lastlogon);
                                    break;
                                }
                            case "lastlogontimestamp": { Int64 _lastlogontimestamp = Convert.ToInt64(v); _usr.lastlogontimestamp = DateTime.FromFileTime(_lastlogontimestamp); break; }
                            case "lockouttime": { _usr.lockouttime = Convert.ToInt64(v); break; }
                            case "pwdlastset": { Int64 _pwdlastset = Convert.ToInt64(v); _usr.pwdlastset = DateTime.FromFileTime(_pwdlastset); break; }
                            case "memberof":
                                {

                                    _usr.memberof = null;
                                    break;
                                }
                            case "adspath": { _usr.adspath = v; _usr.adspath_file = this.ConvertAdsPath2File(v); break; }
                            case "altrecipient": { _usr.altrecipient = v; break; }
                            case "cn": { _usr.cn = v; break; }
                            case "comment": { _usr.comment = v; break; }
                            case "company": { _usr.company = v; break; }
                            case "department": { _usr.department = v; break; }
                            case "description": { _usr.description = v; break; }
                            case "displayname": { _usr.displayname = v; break; }
                            case "displaynameprintable": { _usr.displaynameprintable = v; break; }
                            //            case "distinguishedname": { _usr.distinguishedname = v; break; }
                            case "extensionattribute1": { _usr.extensionattribute1 = v; break; }
                            case "facsimiletelephonenumber": { _usr.facsimiletelephonenumber = v; break; }
                            case "givenname": { _usr.givenname = v; break; }
                            case "homemdb":
                                {
                                    string[] homemdbData = v.Split(',');
                                    homemdbData = homemdbData[0].Split('=');
                                    _usr.homemdb = "database: " + homemdbData[1];
                                    break;
                                }
                            //       case "homemta": { _usr.homemta = v; break; }
                            case "info": { _usr.info = v; break; }
                            case "initials": { _usr.initials = v; break; }
                            case "ipphone": { _usr.ipphone = v; break; }
                            //    case "legacyexchangedn": { _usr.legacyexchangedn = v; break; }
                            case "mailnickname": { _usr.mailnickname = v; break; }
                            case "manager": { _usr.manager = v; break; }
                            case "name": { _usr.name = v; break; }
                            case "objectcategory": { _usr.objectcategory = v; break; }
                            case "physicaldeliveryofficename": { _usr.physicaldeliveryofficename = v; break; }
                            case "postalcode": { _usr.postalcode = v; break; }
                            case "postofficebox": { _usr.postofficebox = v; break; }
                            case "samaccountname": { _usr.samaccountname = v; break; }
                            case "streetaddress": { _usr.streetaddress = v; break; }
                            case "targetaddress": { _usr.targetaddress = v; break; }
                            case "userparameters": { _usr.userparameters = v; break; }
                            case "userprincipalname": { _usr.userprincipalname = v; break; }
                            case "wwwhomepage": { _usr.wwwhomepage = v; break; }
                            case "sn": { _usr.sn = v; break; }
                            case "l": { _usr.l = v; break; }
                            case "telephonenumber": { _usr.telephonenumber = v; _usr.tel = v; break; }
                        }
                    }
                    Console.WriteLine(_usr.samaccountname);
                    _ret.Add(_usr);
                }
            }
            ResultList.Dispose();
            searcher.Dispose();
            return _ret;
        }

        private string ConvertAdsPath2File(string adspath)
        {
            string _ret = "";

            string[] adspathdata = adspath.Split(',');
            int nbItem = adspathdata.Length;
            for (int i = nbItem - 1; i > 0; i--)
            {
                string[] type_and_ou = adspathdata[i].Split('=');
                if (type_and_ou[0] == "dc")
                {
                    _ret = type_and_ou[1] + "." + _ret;
                }
                if (type_and_ou[0] == "ou")
                {
                    _ret = _ret + "\\" + type_and_ou[1];
                }
            }

            adspathdata = _ret.Split('\\');
            //string old_ret = _ret;
            _ret = "";
            foreach (string s in adspathdata)
            {
                int j = s.Length;
                if (j > 0)
                {
                    string last_caractere = s.Substring(j - 1, 1);
                    if (last_caractere == ".")
                    {
                        _ret = s.Substring(0, j - 1);
                    }
                    else
                    {
                        _ret = _ret + "\\" + s;
                    }
                }
            }



            return _ret;
        }

        //public List<ad_computer> Get_Computers()
        //{
        //    SearchResultCollection resultCollection = this.FindObjectCategory("computer");

        //    foreach (SearchResult res in resultCollection)
        //    {
        //        ad_computer _newComputer = new ad_computer();
        //        _newComputer = ConvertResultToList<ad_computer>(res, _newComputer);
        //        Console.WriteLine(_newComputer.cn);
        //        this.ad_Computers.Add(_newComputer);
        //    }

        //    return this.ad_Computers;
        //}
        //public List<ad_group> Get_Groups()
        //{
        //    SearchResultCollection resultCollection = this.FindObjectCategory("group");

        //    foreach (SearchResult res in resultCollection)
        //    {
        //        ad_group _newGroup = new ad_group();
        //        _newGroup = ConvertResultToList<ad_group>(res, _newGroup);
        //        Console.WriteLine(_newGroup.cn);
        //        this.ad_Groups.Add(_newGroup);
        //    }

        //    return this.ad_Groups;
        //}
        //public List<ad_organizationalUnit> Get_orgUnits()
        //{
        //    SearchResultCollection resultCollection = this.FindObjectCategory("organizationalUnit");

        //    foreach (SearchResult res in resultCollection)
        //    {
        //        ad_organizationalUnit _neworgUnit = new ad_organizationalUnit();
        //        _neworgUnit = ConvertResultToList<ad_organizationalUnit>(res, _neworgUnit);
        //        Console.WriteLine(_neworgUnit.ou);
        //        this.ad_orgUnits.Add(_neworgUnit);
        //    }

        //    return this.ad_orgUnits;
        //}



    }
}

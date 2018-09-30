using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Drawing;


namespace lib.LibDal
{
    public class context
    {
     
        public  string databaseFilePath { get; set; }
        private LiteRepository _repo;

        public context(string databasePath)
        {
            this.databaseFilePath = databasePath;
            this._repo = new LiteRepository(this.databaseFilePath);

            //List<string> CollectionNames = _repo.Database.GetCollectionNames().ToList<string>();
            //List<ad_user> mdata =  _repo.Query<ad_user>("ad_users").ToList();
        }
        public List<string> GetDataBaseNames()
        {
            List<string> _ret = new List<string>();
            _ret = _repo.Database.GetCollectionNames().ToList<string>();
            return _ret;
        }
        public List<T> GetData<T>()
        {
            string CollectionName = typeof(T).Name;
            List<T> _ret = new List<T>();
            _ret = _repo.Query<T>(CollectionName).ToList();
            return _ret;
        }


        private void extractPhoto(Byte[] imageBytes, string path,string samaccountname, bool isTumb)
        {

            // Convert byte[] to Image
            MemoryStream ms = new MemoryStream(imageBytes, 0, imageBytes.Length);
            ms.Write(imageBytes, 0, imageBytes.Length);
            Image image = System.Drawing.Image.FromStream(ms, true);

            //save images
            string image_file_path = "";
            if (isTumb)
            {
               //todo change json param photo path
                image_file_path = path + "\\" + samaccountname + ".jpg";
                
            }
            else
            {
                //todo change json param photo path
                image_file_path = path + "\\" + samaccountname + ".thumb.jpg";
            }
            image.Save(image_file_path, System.Drawing.Imaging.ImageFormat.Jpeg);
            ms.Dispose(); // vide la mémoire
        }

        private bool IsJtokenIsNullOrEmpty(JToken token)
        {
            return (token == null) ||
                   (token.Type == JTokenType.Array && !token.HasValues) ||
                   (token.Type == JTokenType.Object && !token.HasValues) ||
                   (token.Type == JTokenType.String && token.ToString() == String.Empty) ||
                   (token.Type == JTokenType.Null);
        }


        public void ExportToJson<T>(List<T> dataItem)
        {
            //Création d'une Array JSON
            string json = JsonConvert.SerializeObject(dataItem, Formatting.None);
            JArray JSON_Array = JArray.Parse(json);
            foreach (JObject j in JSON_Array)
            {
                //Récupération des paramètres 
                JToken Token_adspath_file = j.GetValue("adspath_file");
                JToken Token_samaccountname = j.GetValue("samaccountname");
                JToken Token_b_photo = j.GetValue("b_photo");
                JToken Token_b_thumbnailphoto = j.GetValue("b_thumbnailphoto");

                bool Bool_adspath_file = this.IsJtokenIsNullOrEmpty(Token_adspath_file);
                bool Bool_samaccountname = this.IsJtokenIsNullOrEmpty(Token_samaccountname);
                bool Bool_b_photo = this.IsJtokenIsNullOrEmpty(Token_b_photo);
                bool Bool_b_thumbnailphoto = this.IsJtokenIsNullOrEmpty(Token_b_thumbnailphoto);

                string adspath_file = "";
                string samaccountname = "";
                string b_photo = "";
                string b_thumbnailphoto = "";
                string destination_folder = "";
                string destination_folder_file = "";
                Byte[] Byte_array_b_photo;
                Byte[] Byte_array_b_thumbnailphoto;

                if (!Bool_adspath_file)
                {
                    adspath_file = Token_adspath_file.Value<string>();
                }
                if (!Bool_samaccountname)
                {
                    samaccountname = Token_samaccountname.Value<string>();
                }
              
                //Définir et créer le repertoire de destination
                if ((adspath_file.Length > 0) && (samaccountname.Length > 0))
                {
                    string[] databasePathTab = this.databaseFilePath.Split('\\');

                    destination_folder = databasePathTab[0] + "\\" + databasePathTab[1] + "\\json\\" + adspath_file;
                    if (!Directory.Exists(destination_folder))
                    {
                        Directory.CreateDirectory(destination_folder);
                    }

                    //destination du fichier json     
                    destination_folder_file = destination_folder + "\\" + samaccountname + ".json";

                    //Export du fichier JSON
                    string jsonObject = j.ToString(Formatting.Indented);
                    StreamWriter sr = File.CreateText(destination_folder_file);
                    sr.Write(jsonObject);
                    sr.Dispose();

                    //Export Images
                    //photo
                    if (!Bool_b_photo)
                    {
                        b_photo = Token_b_photo.Value<string>();
                        Byte_array_b_photo = Convert.FromBase64String(b_photo);
                        this.extractPhoto(Byte_array_b_photo, destination_folder, samaccountname,false);
                    }
                    //tumbnail
                    if (!Bool_b_thumbnailphoto)
                    {
                        b_thumbnailphoto = Token_b_thumbnailphoto.Value<string>();
                        Byte_array_b_thumbnailphoto = Convert.FromBase64String(b_thumbnailphoto);
                        this.extractPhoto(Byte_array_b_thumbnailphoto, destination_folder, samaccountname,true);
                    }
                }
            }




            //string ParameterType = typeof(T).Name.ToLower();

            //switch(ParameterType)
            //{
            //    case "user":
            //        {
            //            List<user> USERS2 = dataItem.Cast<user>().ToList<user>();

            //            string json2 = JsonConvert.SerializeObject(USERS, Formatting.Indented);


            //            JArray ja = new JArray(
            //                                      from u in USERS2
            //                                      orderby u.id
            //                                      select new JObject(
            //                                          new JProperty("cn", u.cn),
            //                                          new JProperty("whenchanged", u.whenchanged),
            //                                          new JProperty("description", u.description)
            //                                      )
            //                                     );
            //            string a = ja.ToString(Formatting.Indented);
            //            break;
            //        }                
            //}







            //folderPath = folderPath + "\\" + DataType;

            //if (!Directory.Exists(folderPath))
            //{
            //    Directory.CreateDirectory(folderPath);
            //}



            //string result = JsonConvert.SerializeObject(Data, Formatting.Indented);
            //StreamWriter sr = File.CreateText(folderPath + "\\data2.json");
            //sr.Write(result);
            //sr.Close();


        }
        public int Insert<T>(List<T> Data)
        {           
            string CollectionName = typeof(T).Name;            
            int _ret = -1;
            _ret = _repo.Insert<T>(Data, CollectionName);

            return _ret;
        }
        public int Upsert<T>(List<T> Data)
        {
            string CollectionName = typeof(T).Name;
            int _ret = -1;
            _ret = _repo.Upsert<T>(Data, CollectionName);
            return _ret;
        }
        public bool Upsert<T>(T Data)
        {
            string CollectionName = typeof(T).Name;
            bool _ret = false;
            _ret = _repo.Upsert<T>(Data, CollectionName);
            return _ret;
        }
        public int Update<T>(List<T> Data)
        {
            string CollectionName = typeof(T).Name;
            int _ret = -1;
            _ret = _repo.Update<T>(Data, CollectionName);
            return _ret;
        }
        public bool Update<T>(T Data)
        {
            string CollectionName = typeof(T).Name;
            bool _ret = false;
            _ret = _repo.Update<T>(Data, CollectionName);
            return _ret;
        }
        
        public int Delete<T>(List<T> ListEntity)
        {
            int _Iret = 0;
            foreach(T EntityItem in ListEntity)
            { 
                string sID = EntityItem.GetType().GetProperty("id").GetValue(EntityItem).ToString();
                int iID = Convert.ToInt32(sID);
                bool _ret = this._repo.Delete<T>(iID);
                if(_ret)
                {
                    _Iret++;
                }
            }
            return _Iret;
        }
        public bool Delete<T>(T EntityItem)
        {
            string sID = EntityItem.GetType().GetProperty("id").GetValue(EntityItem).ToString();
            int iID = Convert.ToInt32(sID);
            bool  _ret = this._repo.Delete<T>(iID);
            return _ret;
        }

        public bool Delete<T>(int id)
        {
            bool _ret = false;
            BsonValue bsID = (BsonValue)id;
            _ret = _repo.Delete<T>(id, "ad_user");

            return _ret;

        }

    }
}

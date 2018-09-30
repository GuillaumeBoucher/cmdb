using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace model.hosts
{
    public class host 
    {
        //constructeur
        public host()
        {
            this.os = new host_os();
            this.Model = new host_model();
            this.logs = new host_log();
            this.users = new host_users(); ;
            this.cpu = new host_cpu();
            this.disk = new List<host_disk>();
            this.video_Cards = new List<host_video_card>();
            this.net_Cards = new List<host_net_card>();
            this.Monitors = new List<host_monitor>();
            this.ram = new host_ram();
            this.carte_son = new List<host_sound_card>();
            this.printers = new List<host_printer>();
            this.Recommandations = new List<host_recommandations>();
            String date = DateTime.Now.ToShortDateString();
            String time = DateTime.Now.ToShortTimeString();
            this.DateDesInfo = date + " " + time;
        }

        
        public int id { get; set; }
        public String name { get; set; }
        public String DateDesInfo { get; set; }
        public String SerialNumber { get; set; }
        public host_os os { get; set; }
        public host_model Model { get; set; }
        public host_log logs { get; set; }
        public host_users users { get; set; }
        public host_cpu cpu { get; set; }        
        public List<host_disk> disk { get; set; }        
        public List<host_video_card> video_Cards { get; set; }
        public List<host_net_card> net_Cards { get; set; }
        public List<host_monitor> Monitors { get; set; }
        public List<host_recommandations> Recommandations { get; set; }
        public host_ram ram { get; set; }
        public List<host_sound_card> carte_son { get; set; }
        public List<host_printer> printers { get; set; }

        //LOGICIELS:  


    }
}

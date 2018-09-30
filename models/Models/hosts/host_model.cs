using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace model.hosts
{
    public class host_model
    {
        public host_model()
        {
            this.isVirtualMachine = false;
        }
        public string Marque { get; set; }
        public string Model { get; set; }
        public string ServiceTag { get; set; }
        public string SerialNumber { get; set; }
        public string @type { get; set; }   //pc_utilisateur //pc_libreservice //pc_serveur //serveur //switch //routeur //firewall //stockage //autre
        public bool isVirtualMachine { get; set; }
    }
}

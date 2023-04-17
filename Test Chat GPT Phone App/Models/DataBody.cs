using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test_Chat_GPT_Phone_App.Models
{
    public class DataBody
    {
        public string model { get; set; }
        public List<Message> messages { get; set; }
    }
}

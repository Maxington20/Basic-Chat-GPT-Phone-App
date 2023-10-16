using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test_Chat_GPT_Phone_App.Models
{
    public class ImagePrompt
    {
        public string prompt { get; set; }

        // number of images
        public int n { get; set; }
        public string size { get; set; }
    }
}

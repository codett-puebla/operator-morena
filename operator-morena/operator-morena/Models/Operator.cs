using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace operator_morena.Models
{
    public class Operator
    {
        public int id { get; set; }
        public string name { get; set; }
        public string alias { get; set; }
        public string phone { get; set; }
        public string email { get; set; }
        public string observation { get; set; }
        public int score { get; set; }
        public int status { get; set; }
    }
}

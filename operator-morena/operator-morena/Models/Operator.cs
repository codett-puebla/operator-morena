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
        public string operators_key { get; set; }
        public string name { get; set; }
        public string last_name { get; set; }
        public string m_last_name { get; set; }
        public string alias { get; set; }
        public string phone { get; set; }
        public string email { get; set; }
        public string address { get; set; }
        public string observation { get; set; }
        public string municipality { get; set; }
        public int score { get; set; }
        public int status { get; set; }
        public int id_sections { get; set; }
        public string id_operators_key { get; set; }
        public string image { get; set; }
    }
}

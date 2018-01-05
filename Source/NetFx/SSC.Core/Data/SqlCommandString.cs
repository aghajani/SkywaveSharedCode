using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSC.Data
{
    public class SqlCommandString
    {
        public List<string> Params { get; set; }
        public string CommandString { get; set; }

        public string GenerateSql()
        {
            string r = "";
            //
            string sqlParams = "";
            if (Params != null)
            {
                foreach (string fe1 in Params.Distinct())
                {
                    sqlParams += string.Format(",{0}\n\t", fe1);
                }
                sqlParams = sqlParams.Trim(new char[] { ',', '\t' });
            }
            //
            if (!string.IsNullOrEmpty(sqlParams))
            {
                r += string.Format("DECLARE \n\t{0};\n", sqlParams);
            }
            //
            r += CommandString;
            //
            return r;
        }
        //
        public SqlCommandString Clone()
        {
            SqlCommandString r = new SqlCommandString();
            r.CommandString = CommandString;
            r.Params = Params.ToList();
            return r;
        }
    }
}

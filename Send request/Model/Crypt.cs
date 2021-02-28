using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Send_request.Model
{
    class Crypt
    {

        public string Crypt_Paswword(string pass)
        {
            string resPas = "";

            for (int i = 0; i < pass.Length; i++)
            {
                char tmpStr = pass[i];
                int tmpInt = tmpStr;
                if (i == pass.Length || i == 0)
                {
                    resPas += ((tmpInt * 2) - 10);
                }
                else
                {
                    resPas += "|" + ((tmpInt * 2) - 10);
                }
            }

            return resPas;
        }

        public string Encrypt_Password(string pass)
        {
            string resPas = "";
            string[] data = pass.Split('|');

            for (int i = 0; i < data.Length; i++)
            {
                int symInt = Convert.ToInt32(data[i]);
                symInt = (symInt + 10) / 2;
                char res = (char)symInt;
                resPas += res;
            }
            return resPas;
        }
    }
}

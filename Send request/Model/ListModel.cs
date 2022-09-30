using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Send_request.Model
{
    class ListModel
    {
        private string _date;                     // Дата
        private string _card;                     // Карта
        private string _sector;                   // Сектор
        private string _operations;               // Операция
        private string _summa;                    // Сумма

        public string Date
        {
            get { return _date; }
            set { _date = value; }
        }

        public string Card
        {
            get { return _card; }
            set { _card = value; }
        }

        public string Sector
        {
            get { return _sector; }
            set { _sector = value; }
        }

        public string Operations
        {
            get { return _operations; }
            set { _operations = value; }
        }

        public string Summa
        {
            get { return _summa; }
            set { _summa = value; }
        }
    }
}

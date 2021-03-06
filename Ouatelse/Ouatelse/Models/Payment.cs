﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ouatelse.Models
{
    public class Payment : BaseModel, IModel
    {
        public string Type { get; set; }

        public Payment()
        {

        }

        /// <summary>
        /// Permet d'hydrater l'objet
        /// </summary>
        /// <param name="data"></param>
        public void Hydrate(object[] data)
        {
            ArrayCursor<object> cursor = new ArrayCursor<object>(data);
            this.Id = Int32.Parse(cursor.Read().ToString());
            this.Type = cursor.Read().ToString();
        }

        public Dictionary<string, string> Fetch()
        {
            Dictionary<string, string> res = new Dictionary<string, string> {{"type", Type}};
            return res;
        }

        public static string CreationQuery()
        {
            string query = " DROP TABLE IF EXISTS \"moyen_de_paiements\";" + Environment.NewLine;
            query += " CREATE TABLE \"moyen_de_paiements\" (" + Environment.NewLine;
            query += " \"id\" INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, " + Environment.NewLine;
            query += " \"type\" TEXT(45,0) NOT NULL);";
            return query;
        }

        public static string Fixtures()
        {
            return "INSERT INTO moyen_de_paiements VALUES (1,'CB');";
        }

    }
}

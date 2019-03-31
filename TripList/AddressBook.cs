using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Data.SQLite;

namespace TripList
{
    public class AddressBook
    {
        public ObservableCollection<Address> addresses { get; set; }

        public AddressBook()
        {
            addresses = new ObservableCollection<Address>();
        }

        public void SaveXML(string filename)
        {
            if (File.Exists(filename))
            {
                File.Delete(filename);
            }

            using (FileStream fs = new FileStream(filename, FileMode.Create))
            {
                XmlSerializer xser = new XmlSerializer(typeof(AddressBook));
                xser.Serialize(fs, this);
                fs.Close();
            }
        }

        public static AddressBook LoadXML(string filename)
        {
            AddressBook loaded = new AddressBook();

            if (File.Exists(filename))
            {
                using (FileStream fs = new FileStream(filename, FileMode.Open))
                {
                    XmlSerializer xser = new XmlSerializer(typeof(AddressBook));
                    loaded = (AddressBook)xser.Deserialize(fs);
                    fs.Close();
                }
            }
            else
            {
                loaded = new AddressBook();
            }

            return loaded;
        }

        public static AddressBook LoadFromSQL()
        {
            AddressBook loaded = new AddressBook();

            SQLiteConnection m_dbConnection;
            m_dbConnection =new SQLiteConnection("Data Source=fuel.sqlite;Version=3;");
            m_dbConnection.Open();

            string sql = "select * from poi";
            SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                loaded.addresses.Add(new Address
                {
                    Id = int.Parse(reader["id"].ToString()),
                    POIName = reader["name"].ToString(),
                    Entity = reader["object"].ToString(),
                    POIAddress = reader["address"].ToString(),
                    Distance = int.Parse(reader["distance"].ToString()),
                    IsBase = Convert.ToBoolean(reader["base"]),
                    IsUse = Convert.ToBoolean(reader["use"]),
                    Comment = reader["comment"].ToString()
                });
            }

            return loaded;
        }
    }
}

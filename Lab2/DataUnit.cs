using System;

namespace Lab2
{
    public class DataUnit
    {
        public int Id { get; set; }
        public string ExtendedId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Source { get; set; }
        public string Object { get; set; }
        private string confidentiality;
        public string Confidentiality { get => confidentiality; set { confidentiality = value != "0" ? "Да" : "Нет"; } }
        private string integrity;
        public string Integrity { get => integrity; set { integrity = value != "0" ? "Да" : "Нет"; } }

        private string availability;
        public string Availability { get => availability; set { availability = value != "0" ? "Да" : "Нет"; } }
        public DateTime LastChanged { get; set; }

        public DataUnit(int id, string name, string description, string source, string obj, string confidentiality, string integrity, string availability, DateTime lastChanged)
        {
            Id = id;
            ExtendedId = "УБИ." + id;
            Name = name;
            Description = description;
            Source = source;
            Object = obj;
            Confidentiality = confidentiality;
            Integrity = integrity;
            Availability = availability;
            LastChanged = lastChanged;
        }

        public override string ToString()
        {
            return $"id: {Id}\n\n" +
                   $"Name: {Name}\n\n" +
                   $"Description: {Description}\n\n" +
                   $"Source: {Source}\n\n" +
                   $"Object: {Object}\n\n" +
                   $"Confidentiality: {Confidentiality}\n\n" +
                   $"Integrity: {Integrity}\n\n" +
                   $"Availability: {Availability}";
        }
    }
}

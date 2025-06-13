﻿namespace Models
{
    public class Person
    {
        //public int Key { get; set; }
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        public string? OptionalDescription { get; set; } = null;
    
        public Address? Address { get; set; }
    }
}

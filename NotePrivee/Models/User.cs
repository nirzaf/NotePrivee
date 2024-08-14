using System;
using System.Collections.Generic;

#nullable disable

namespace NotePrivee.Models
{
    public partial class User(int id, string username, string password)
    {
        public int Id { get; set; } = id;
        public string Username { get; set; } = username;
        public string Password { get; set; } = password;
    }
}

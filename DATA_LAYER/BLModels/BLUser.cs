using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DATA_LAYER.BLModels
{
    public class BLUser
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PwdHash { get; set; }
        public string PwdSalt { get; set; }
        public string Phone { get; set; }
        public bool IsConfirmed { get; set; }
        public string SecurityToken { get; set; }
        public string Role { get; set; }
    }
}

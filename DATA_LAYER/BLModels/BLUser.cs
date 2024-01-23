using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
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

        public DateTime CreatedAt { get; set; }

        public DateTime? DeletedAt { get; set; }

        public string Username { get; set; } = null!;

        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public string Email { get; set; } = null!;

        public bool IsConfirmed { get; set; } = true;

        public string? SecurityToken { get; set; }

        //[ValidateNever]
        //public string Role { get; set; }

        public int CountryOfResidenceId { get; set; }
    }
}

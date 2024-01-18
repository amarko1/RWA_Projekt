using DATA_LAYER.DALModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DATA_LAYER.BLModels
{
    public class BLTag
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "You have to enter a tag name")]
        [MaxLength(20, ErrorMessage = "{0} cannot be longer than 20 characters")]
        public string Name { get; set; } = null!;
    }
}

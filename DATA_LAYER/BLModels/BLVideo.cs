using DATA_LAYER.DALModels;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DATA_LAYER.BLModels
{
    public class BLVideo
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public int GenreId { get; set; }

        [ValidateNever]
        public string GenreName { get; set; }

        [Display(Name = "Total seconds")]
        public int TotalSeconds { get; set; }

        [Display(Name = "Streaming Url")]
        public string? StreamingUrl { get; set; }

        public int? imageid { get; set; }
    }
}

﻿using ServMidMan.Interface;
using System.ComponentModel.DataAnnotations;

namespace ServMidMan.Models
{
    public class Image : IImage
    {
        [Key]
        public int Id { get; set; }
        public string FileName { get; set; }
        public int ProductReferenceId { get; set; }
    }
}

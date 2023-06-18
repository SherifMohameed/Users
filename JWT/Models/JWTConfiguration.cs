﻿using JWT.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;

namespace JWT.Models
{
    [Table("JWTConfiguration", Schema ="GlobalConfiguration")]
    public class JWTConfiguration
    {
        private static JWTConfiguration _instance = null;


        [Key]
        public int ID { get; set; }
        [Required]
        public string Key { get; set; }
        [Required]
        public string Issuer { get; set; }
        [Required]
        public string Audience { get; set; }
        [Required]
        public double DurationInDays { get; set; }

        public static JWTConfiguration GetInstance(ApplicationDbContext _context)
        {
            _instance ??= _context.JWTConfigurations.FirstOrDefault(f => f.ID == 1);

            return _instance;
        }
    }
}

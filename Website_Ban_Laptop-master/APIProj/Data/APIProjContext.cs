using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using APIProj.Models;

namespace APIProj.Data
{
    public class APIProjContext : DbContext
    {
        public APIProjContext (DbContextOptions<APIProjContext> options)
            : base(options)
        {
        }

        public DbSet<APIProj.Models.UserModel> UserModel { get; set; }
        public DbSet<APIProj.Models.ImageModel> ImageModel { get; set; }

    }
}

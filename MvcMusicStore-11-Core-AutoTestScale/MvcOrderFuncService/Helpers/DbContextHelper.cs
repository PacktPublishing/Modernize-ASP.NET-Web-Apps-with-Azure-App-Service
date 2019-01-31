using Microsoft.EntityFrameworkCore;
using MvcMusicStore.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvcOrderFuncService.Helpers
{
    public class DbContextHelper
    {
        public MusicStoreEntities GetContext()
        {
            var optionsBuilder = new DbContextOptionsBuilder<MusicStoreEntities>();
            optionsBuilder.UseSqlServer(ConfigurationManager
                   .ConnectionStrings["MusicStoreEntities"].ConnectionString);

            return new MusicStoreEntities(optionsBuilder.Options);
        }
    }
}

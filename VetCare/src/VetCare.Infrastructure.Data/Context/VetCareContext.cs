using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VetCare.Infrastructure.Data.Context;

public class VetCareContext : DbContext
{
    public VetCareContext(DbContextOptions<VetCareContext> opt) : base(opt) { 
    
    }
}

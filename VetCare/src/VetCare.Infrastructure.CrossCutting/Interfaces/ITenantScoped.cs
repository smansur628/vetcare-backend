using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VetCare.Infrastructure.CrossCutting.Interfaces;

public interface ITenantScoped
{
    int ClinicaId { get; }
}

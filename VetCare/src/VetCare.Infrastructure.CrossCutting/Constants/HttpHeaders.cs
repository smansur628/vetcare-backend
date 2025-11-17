using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VetCare.Infrastructure.CrossCutting.Constants;

public static class HttpHeaders
{
    public const string CorrelationId = "X-Correlation-Id";
    public const string ClinicCode = "X-Clinic";
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VetCare.Infrastructure.CrossCutting.Options;

public class HashIdOptions
{
    public string Salt { get; set; } = "CHANGE-ME";
    public int MinLength { get; set; } = 8;
}

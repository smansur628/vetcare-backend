using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VetCare.Infrastructure.CrossCutting.Interfaces;

public interface IHashIdService
{
    string Encode(int id);
    int Decode(string code);
}

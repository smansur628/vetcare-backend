using HashidsNet;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VetCare.Infrastructure.CrossCutting.Interfaces;
using VetCare.Infrastructure.CrossCutting.Options;

namespace VetCare.Infrastructure.CrossCutting.Services;

public class HashIdService : IHashIdService
{
    private readonly IHashids _hash;
    public HashIdService(IOptions<HashIdOptions> opt)
        => _hash = new Hashids(opt.Value.Salt, opt.Value.MinLength);
    public int Decode(string code) => _hash.DecodeSingle(code);
    public string Encode(int id) => _hash.Encode(id); 
}

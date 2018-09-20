﻿namespace DEKL.CP.Domain.Entities
{ 
    public class Endereco : EntityBase
    {
        public string Logradouro { get; set; }
        public string CEP { get; set; }
        public int? Numero { get; set; }
        public string Complemento { get; set; }
        public string Bairro { get; set; }
        public string Cidade { get; set; }
        public string UF { get; set; }
    }
}
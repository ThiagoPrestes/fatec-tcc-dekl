﻿using DEKL.CP.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace DEKL.CP.Infra.Data.EF.Maps
{
    public class SimulaContaMap : EntityTypeConfiguration<SimulaConta>
    {
        public SimulaContaMap()
        {
            //Table
            ToTable(nameof(SimulaConta));

            //PK

            //Columns
        }
    }
}

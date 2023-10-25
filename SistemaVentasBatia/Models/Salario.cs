﻿using System;

namespace SistemaVentasBatia.Models
{
    public class Salario
    {
        public int IdSalario { get; set; }
        public int IdTabulador { get; set; }
        public int IdPuesto { get; set; }
        public int IdTurno { get; set; }
        public decimal SalarioI { get; set; }
        public DateTime FechaAlta { get; set; }
        public int IdPersonal { get; set; }
    }
}

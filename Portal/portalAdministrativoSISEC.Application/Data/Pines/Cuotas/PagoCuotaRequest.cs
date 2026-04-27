namespace portalAdministrativoSISEC.Application.Data.Pines.Cuotas
{
    public class PagoCuotaRequest
    {
        public string IdRunt { get; set; }
        public int IdTipoDoc { get; set; }
        public string NumDocumento { get; set; }
        public string NumPin { get; set; }
        public decimal ValorAbono { get; set; }
        public decimal ValorAliado { get; set; }
    }
}


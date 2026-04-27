namespace portalAdministrativoSISEC.Application.Data
{
    public class EntradaEnrolamiento
    {
        public long Id { get; set; }
        public string TipoDocumentoId { get; set; }
        public string NumeroDocumento { get; set; }
        public string PrimerNombre { get; set; }
        public string SegundoNombre { get; set; }
        public string PrimerApellido { get; set; }
        public string SegundoApellido { get; set; }
        public string CiudadExpedicionId { get; set; }
        public string FechaNacimiento { get; set; }
        public string FechaExpedicion { get; set; }
        public string CiudadNacimientoId { get; set; }
        public string Direccion { get; set; }
        public string LugarResidenciaId { get; set; }
        public int? IndicativoTelefono { get; set; }
        public string Telefono { get; set; }
        public string Celular { get; set; }
        public string CorreoElectronico { get; set; }
        public string SexoId { get; set; }
        public string EstadoCivilId { get; set; }
        public string RegimenAfiliacionId { get; set; }
        public string EpsId { get; set; }
        public string GradoEscolaridadId { get; set; }
        public string OcupacionId { get; set; }
        public string GrupoSanguinioId { get; set; }
        public bool Sincronizar { get; set; }
        public int? ClienteId { get; set; }
        public int? IdCentro { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public string MaquinaId { get; set; }
        public int? EstadoProcesoId { get; set; }
        public int TipoAfisId { get; set; }
        public string Fotografia { get; set; }
        public string Firma { get; set; }
        public bool procesoNuevo { get; set; }
        public string NombreCompleto { get; set; }
        public string DocumentoValidacion { get; set; }
        public string Base64DocumentoFirmadoFD { get; set; }
        public byte[] byteDocumentoValidacion { get; set; }
        public byte[] ByteDocumentoFirmadoFD { get; set; }
        public decimal ValorPin { get; set; }
        public string NumeroPin { get; set; }
    }
}


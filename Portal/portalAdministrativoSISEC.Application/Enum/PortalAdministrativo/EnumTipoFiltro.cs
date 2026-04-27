namespace portalAdministrativoSISEC.Enum.PortalAdministrativo
{
    public enum EnumTipoFiltro
    {
        NoValido = 0,
        FecIniFecFin = 1,
        FecIniFecFinCentro = 2,
        FecIniFecFinCentroCanal = 3,
        FecIniFecFinCanal = 4,
        TipoDocNumDoc = 5,
        NumDoc = 6,
        Pin = 7,
        Centro = 8
    }

    public enum EnumTipoFiltroDevoluciones
    {
        NoValido = 0,
        FecIniFecFin = 1,
        FecIniFecFinCanal = 2,
        FecIniFecFinCanalNumDoc = 3,
        FecIniFecFinCanalNumDocPin = 4,
        FecIniFecFinCanalNumDocPinAgente = 5,
        NumDoc = 6,
        Pin = 7,
		Centro = 8
	}

    public enum EnumTipoConsultaPines
    {
        Activos = 2,
        Usados = 4,
        Devoluciones = 1
    }
}

// ===================================================================
// Enums - Converted from Enum/PortalAdministrativo/*.cs and Enum/*.cs
// ===================================================================

/** Converted from Enum/PortalAdministrativo/EnumEstadoPin.cs */
export enum EstadoPin {
	Activo = 1,
	Usado = 2,
	Devuelto = 3,
	Vencido = 4,
	Anulado = 5
}

/** Converted from Enum/PortalAdministrativo/EnumTipoDocumento.cs */
export enum TipoDocumento {
	CedulaCiudadania = 1,
	CedulaExtranjeria = 2,
	Pasaporte = 3,
	TarjetaIdentidad = 4,
	NIT = 5
}

/** Converted from Enum/PortalAdministrativo/EnumTipoDocumentoAceptados.cs */
export enum TipoDocumentoAceptados {
	CC = 'CC',
	CE = 'CE',
	PA = 'PA',
	TI = 'TI',
	NIT = 'NIT'
}

/** Converted from Enum/PortalAdministrativo/EnumTramite.cs */
export enum TipoTramite {
	PrimeraVez = 1,
	Recategorizacion = 2,
	Refrendacion = 3,
	Duplicado = 4
}

/** Converted from Enum/PortalAdministrativo/EnumSexo.cs */
export enum SexoEnum {
	Masculino = 1,
	Femenino = 2
}

/** Converted from Enum/PortalAdministrativo/EnumTipoPago.cs */
export enum TipoPago {
	PSE = 1,
	TarjetaCredito = 2,
	Efectivo = 3,
	Nequi = 4,
	Bancolombia = 5
}

/** Converted from Enum/PortalAdministrativo/EnumTipoCliente.cs */
export enum TipoCliente {
	CRC = 1,
	CEA = 2,
	CDA = 3,
	Armas = 4
}

/** Converted from Enum/PortalAdministrativo/EnumOrigenPin.cs */
export enum OrigenPin {
	Portal = 1,
	Api = 2,
	Tercero = 3
}

/** Converted from Enum/PortalAdministrativo/EnumOrigenCotizacion.cs */
export enum OrigenCotizacion {
	Portal = 1,
	Api = 2
}

/** Converted from Enum/PortalAdministrativo/EnumEstadoFacturacionElectronica.cs */
export enum EstadoFacturacionElectronica {
	Pendiente = 1,
	Enviada = 2,
	Aceptada = 3,
	Rechazada = 4
}

/** Converted from Enum/PortalAdministrativo/EnumTiposFacturacion.cs */
export enum TiposFacturacion {
	FacturaVenta = 1,
	NotaCredito = 2,
	NotaDebito = 3
}

/** Converted from Enum/PortalAdministrativo/EnumAliadoRecaudo.cs */
export enum AliadoRecaudo {
	PSEColpatria = 1,
	Wompi = 2,
	PlaceToPay = 3
}

/** Converted from Enum/PortalAdministrativo/EnumTipoReferencia.cs */
export enum TipoReferencia {
	Pin = 1,
	Cuota = 2,
	Dispersion = 3
}

/** Converted from Enum/PortalAdministrativo/EnumCredenciales.cs */
export enum EnumCredenciales {
	ClientId = 1,
	ClientSecret = 2
}

/** Converted from Enum/PortalAdministrativo/EnumTipoInput.cs */
export enum TipoInput {
	Text = 1,
	Number = 2,
	Date = 3,
	Select = 4,
	Checkbox = 5
}

/** Converted from Enum/PortalAdministrativo/EnumConfigurarFacturacion.cs */
export enum ConfigurarFacturacion {
	Articulos = 1,
	Numeracion = 2,
	Credenciales = 3,
	Comportamiento = 4,
	Emision = 5,
	Estado = 6
}

/** Converted from Enum/PortalAdministrativo/EnumTipoFiltro.cs */
export enum TipoFiltro {
	Fecha = 1,
	Estado = 2,
	Documento = 3
}

/** Converted from Enum/PortalAdministrativo/EnumTipoFormBusqueda.cs */
export enum TipoFormBusqueda {
	Pin = 1,
	Documento = 2
}

/** Converted from Enum/PortalAdministrativo/EnumCategoriaBusquedaArticulos.cs */
export enum CategoriaBusquedaArticulos {
	Todos = 0,
	Activos = 1,
	Inactivos = 2
}

/** Converted from Enum/CompraPin/PasosCotizacion.cs */
export enum PasosCotizacion {
	DatosBasicos = 1,
	DatosPersonales = 2,
	Categorias = 3,
	Tramite = 4,
	Resumen = 5,
	MediosPago = 6,
	Confirmacion = 7
}

/** Converted from Enum/CompraPin/OpcionTramitesEnum.cs */
export enum OpcionTramites {
	PrimeraVez = 1,
	Recategorizacion = 2,
	Refrendacion = 3,
	Duplicado = 4
}

/** Converted from Enum/Devoluciones/PaymentType.cs */
export enum PaymentType {
	Efectivo = 1,
	PSE = 2,
	TarjetaCredito = 3
}

/** Converted from Enum/TipoTiempo.cs */
export enum TipoTiempo {
	Minutos = 1,
	Horas = 2,
	Dias = 3
}

/** Converted from Enum/EstadoAgenda.cs */
export enum EstadoAgenda {
	Disponible = 1,
	Ocupado = 2,
	Bloqueado = 3,
	Cancelado = 4
}

/** Converted from Enum/NotificationStatus.cs */
export enum NotificationStatus {
	Success = 1,
	Error = 2,
	Warning = 3,
	Info = 4
}

/** Converted from Enum/FranjaHoraria.cs */
export enum FranjaHoraria {
	Manana = 1,
	Tarde = 2,
	Noche = 3
}

/** Converted from Enum/PoliticaAgendamiento.cs */
export enum PoliticaAgendamiento {
	Libre = 1,
	Controlada = 2,
	Mixta = 3
}

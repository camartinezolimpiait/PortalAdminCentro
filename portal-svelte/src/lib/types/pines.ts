// ===================================================================
// Pines Module Types - Converted from Data/Pines/*.cs
// ===================================================================

/** Converted from Data/Pines/ConsultaCentroPorComercio.cs */
export interface ConsultaCentroPorComercio {
	idComercio: number;
	estado: number;
	fechaInicio: string;
	fechaFin: string;
	pagina: number;
	registrosPagina: number;
}

/** Converted from Data/Pines/ConsultaCentroPorId.cs */
export interface ConsultaCentroPorId {
	idCentro: number;
	estado: number;
	fechaInicio: string;
	fechaFin: string;
	pagina: number;
	registrosPagina: number;
}

/** Converted from Data/Pines/ConsultaDescarga.cs */
export interface ConsultaDescarga {
	idCentro: number;
	estado: number;
	fechaInicio: string;
	fechaFin: string;
}

/** Converted from Data/Pines/ConsultaDetallePin.cs */
export interface ConsultaDetallePin {
	idPin: number;
}

/** Converted from Data/Pines/ConsultaDispersion.cs */
export interface ConsultaDispersion {
	idCentro: number;
	fechaInicio: string;
	fechaFin: string;
	pagina: number;
	registrosPagina: number;
}

/** Converted from Data/Pines/ConsultaInfoPinEstado.cs */
export interface ConsultaInfoPinEstado {
	idCentro: number;
	estado: number;
	fechaInicio: string;
	fechaFin: string;
	pagina: number;
	registrosPagina: number;
}

/** Converted from Data/Pines/ResponseInfoPinEstado.cs */
export interface ResponseInfoPinEstado {
	id: number;
	pin: string;
	fechaCompra: string;
	fechaExpiracion: string;
	estado: string;
	valor: number;
	categoria: string;
	tramite: string;
	tipoDocumento: string;
	numeroDocumento: string;
	nombres: string;
	apellidos: string;
}

/** Converted from Data/Pines/ResponseInfoPinEstadoActivosDTO.cs */
export interface ResponseInfoPinEstadoActivos {
	totalRegistros: number;
	pagina: number;
	registrosPagina: number;
	pines: ResponseInfoPinEstado[];
}

/** Converted from Data/Pines/ResponseInfoPinEstadoUsadosDTO.cs */
export interface ResponseInfoPinEstadoUsados {
	totalRegistros: number;
	pagina: number;
	registrosPagina: number;
	pines: ResponseInfoPinEstado[];
}

/** Converted from Data/Pines/ResponseDetallePin.cs */
export interface ResponseDetallePin {
	id: number;
	pin: string;
	fechaCompra: string;
	fechaExpiracion: string;
	estado: string;
	valor: number;
	categoria: string;
	tramite: string;
	tipoDocumento: string;
	numeroDocumento: string;
	nombres: string;
	apellidos: string;
	centroId: number;
	centroNombre: string;
}

/** Converted from Data/Pines/ResponseConsultaDevolucion.cs */
export interface ResponseConsultaDevolucion {
	id: number;
	pin: string;
	fechaDevolucion: string;
	valor: number;
	motivo: string;
	estado: string;
}

/** Converted from Data/Pines/ResponseConsultaDispersion.cs */
export interface ResponseConsultaDispersion {
	id: number;
	fechaDispersion: string;
	valor: number;
	estado: string;
	referencia: string;
}

/** Converted from Data/Pines/ResponsePinesAsociados.cs */
export interface ResponsePinesAsociados {
	id: number;
	pin: string;
	fechaCompra: string;
	estado: string;
	valor: number;
	categoria: string;
}

/** Converted from Data/Pines/PinesAsociadosRequest.cs */
export interface PinesAsociadosRequest {
	idCentro: number;
	tipoDocumento: string;
	numeroDocumento: string;
}

/** Converted from Data/Pines/Paginas.cs */
export interface Paginas {
	paginaActual: number;
	totalPaginas: number;
	registrosPagina: number;
	totalRegistros: number;
}

/** Converted from Data/Pines/Cuotas/NotificacionPagoCuotas.cs */
export interface NotificacionPagoCuotas {
	idCentro: number;
	idCuota: number;
	referencia: string;
}

/** Converted from Data/Pines/Cuotas/PagoCuotaRequest.cs */
export interface PagoCuotaRequest {
	idCentro: number;
	valor: number;
	referencia: string;
}

/** Converted from Data/Pines/Cuotas/ResponseEnvioCorreoCuotas.cs */
export interface ResponseEnvioCorreoCuotas {
	esExitoso: boolean;
	mensaje: string;
}

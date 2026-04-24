// ===================================================================
// CompraPin Module Types - Converted from Data/CompraPin/*.cs
// ===================================================================

/** Converted from Data/CompraPin/DatosBasicos.cs */
export interface DatosBasicos {
	tipoDocumento: string;
	numeroDocumento: string;
	primerNombre: string;
	segundoNombre: string;
	primerApellido: string;
	segundoApellido: string;
	fechaNacimiento: string;
	sexo: string;
	correoElectronico: string;
	celular: string;
	direccion: string;
	departamento: string;
	municipio: string;
}

/** Converted from Data/CompraPin/DatosFacturacion.cs */
export interface DatosFacturacion {
	tipoDocumento: string;
	numeroDocumento: string;
	razonSocial: string;
	correoElectronico: string;
	direccion: string;
	departamento: string;
	municipio: string;
}

/** Converted from Data/CompraPin/CotizacionPin.cs */
export interface CotizacionPin {
	idCentro: number;
	idCategoria: number;
	idTramite: number;
	tipoDocumento: string;
	numeroDocumento: string;
	cantidad: number;
	valorUnitario: number;
	valorTotal: number;
	descuento: number;
	valorIva: number;
	valorNeto: number;
}

/** Converted from Data/CompraPin/PagoPin.cs */
export interface PagoPin {
	idCotizacion: number;
	medioPago: string;
	referenciaPago: string;
	estado: string;
}

/** Converted from Data/CompraPin/ComponentesCompra.cs */
export interface ComponentesCompra {
	paso: number;
	titulo: string;
	completado: boolean;
}

/** Converted from Data/CompraPin/Centro.cs */
export interface CentroCompra {
	id: number;
	nombre: string;
	codigoUso: string;
}

/** Converted from Data/CompraPin/Categoria.cs */
export interface CategoriaCompra {
	id: number;
	nombre: string;
	descripcion: string;
}

/** Converted from Data/CompraPin/MedioPago.cs */
export interface MedioPago {
	id: number;
	nombre: string;
	descripcion: string;
	activo: boolean;
	icono: string;
}

/** Converted from Data/CompraPin/CalculoCuota.cs */
export interface CalculoCuota {
	valorCuota: number;
	numeroCuotas: number;
	valorTotal: number;
	interes: number;
}

/** Converted from Data/CompraPin/ConfiguracionCuotas.cs */
export interface ConfiguracionCuotas {
	id: number;
	centroId: number;
	numeroCuotas: number;
	porcentajeInteres: number;
	activo: boolean;
}

/** Converted from Data/CompraPin/Convenio.cs */
export interface Convenio {
	id: number;
	nombre: string;
	centroId: number;
	activo: boolean;
}

/** Converted from Data/CompraPin/DiscriminadoValorPin.cs */
export interface DiscriminadoValorPin {
	concepto: string;
	valor: number;
}

/** Converted from Data/CompraPin/Banco.cs */
export interface Banco {
	id: number;
	nombre: string;
	codigo: string;
}

/** Converted from Data/CompraPin/Analytics.cs */
export interface Analytics {
	evento: string;
	categoria: string;
	etiqueta: string;
	valor: number;
}

/** Converted from Data/CompraPin/PasosCompraPin.cs */
export interface PasosCompraPin {
	paso: number;
	nombre: string;
	ruta: string;
	completado: boolean;
}

/** Converted from Data/CompraPin/SeleccionTramites.cs */
export interface SeleccionTramites {
	id: number;
	nombre: string;
	seleccionado: boolean;
}

/** Converted from Data/CompraPin/TipoDocumentoPtesaDTO.cs */
export interface TipoDocumentoPtesaDTO {
	id: number;
	nombre: string;
	abreviatura: string;
}

/** Converted from Data/CompraPin/TiposTramitesHabilitados.cs */
export interface TiposTramitesHabilitados {
	id: number;
	nombre: string;
	habilitado: boolean;
}

/** Converted from Data/CompraPin/Sexo.cs */
export interface Sexo {
	id: number;
	nombre: string;
}

/** Converted from Data/CompraPin/ParametrosCostoPin.cs */
export interface ParametrosCostoPin {
	idCategoria: number;
	idTramite: number;
	valor: number;
	descuento: number;
	iva: number;
}

/** Converted from Data/CompraPin/EntidadMensaje.cs */
export interface EntidadMensaje {
	esExitoso: boolean;
	mensaje: string;
	codigo: string;
}

/** Converted from Data/CompraPin/EntidadCuotas.cs */
export interface EntidadCuotas {
	id: number;
	numeroCuotas: number;
	valorCuota: number;
	porcentajeInteres: number;
}

/** Converted from Data/CompraPin/Models/CompraPinFormModel.cs */
export interface CompraPinFormModel {
	datosBasicos: DatosBasicos;
	datosFacturacion: DatosFacturacion | null;
	categoriaSeleccionada: number | null;
	tramiteSeleccionado: number | null;
	medioPagoSeleccionado: string | null;
}

/** Converted from Data/CompraPin/Models/TransactionInfo.cs */
export interface TransactionInfo {
	referencia: string;
	estado: string;
	valor: number;
	fecha: string;
	medioPago: string;
}

/** Converted from Data/CompraPin/Wompi/TokenAceptacion.cs */
export interface TokenAceptacion {
	token: string;
	url: string;
}

/** Converted from Data/CompraPin/MensajeRespuestaPSEColpatria.cs */
export interface MensajeRespuestaPSE {
	esExitoso: boolean;
	mensaje: string;
	urlRedireccion: string;
	referencia: string;
}

// ===================================================================
// Facturacion Module Types
// Converted from Data/DatosFacturacion.cs, Data/Credenciales.cs
// ===================================================================

export interface DatosFacturacionConfig {
	id: number;
	centroId: number;
	razonSocial: string;
	nit: string;
	direccion: string;
	telefono: string;
	correo: string;
	prefijo: string;
	resolucion: string;
	fechaResolucion: string;
	rangoInicial: number;
	rangoFinal: number;
	consecutivoActual: number;
	activo: boolean;
}

export interface ArticuloFacturacion {
	id: number;
	codigo: string;
	nombre: string;
	descripcion: string;
	valor: number;
	iva: number;
	activo: boolean;
}

export interface CredencialesFacturacion {
	id: number;
	centroId: number;
	proveedor: string;
	usuario: string;
	clave: string;
	urlServicio: string;
	activo: boolean;
}

export interface NumeracionFacturacion {
	id: number;
	centroId: number;
	prefijo: string;
	rangoInicial: number;
	rangoFinal: number;
	consecutivoActual: number;
	resolucion: string;
	fechaResolucion: string;
	activo: boolean;
}

export interface ConsultaFacturacion {
	centroId: number;
	fechaInicio: string;
	fechaFin: string;
	estado: string;
	pagina: number;
	registrosPagina: number;
}

export interface FacturaResponse {
	id: number;
	numero: string;
	fecha: string;
	razonSocial: string;
	nit: string;
	valorTotal: number;
	estado: string;
	urlPdf: string;
}

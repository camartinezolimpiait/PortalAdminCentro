// ===================================================================
// SuperTransporte Module Types
// Converted from Data/ValidaPinCEA/*.cs and SuperTransporte entities
// ===================================================================

export interface InfoBasicaCRC {
	centroId: number;
	razonSocial: string;
	nit: string;
	representanteLegal: string;
	direccion: string;
	telefono: string;
	correo: string;
	departamento: string;
	municipio: string;
}

export interface InfoBasicaCEA {
	centroId: number;
	razonSocial: string;
	nit: string;
	representanteLegal: string;
	direccion: string;
	telefono: string;
	correo: string;
	departamento: string;
	municipio: string;
}

export interface ResolucionHabilitacion {
	id: number;
	centroId: number;
	numero: string;
	fecha: string;
	entidad: string;
	vigencia: string;
	estado: string;
}

export interface Poliza {
	id: number;
	centroId: number;
	numero: string;
	aseguradora: string;
	fechaInicio: string;
	fechaFin: string;
	valor: number;
	estado: string;
}

export interface RepresentanteLegal {
	id: number;
	centroId: number;
	tipoDocumento: string;
	numeroDocumento: string;
	nombres: string;
	apellidos: string;
	correo: string;
	telefono: string;
}

export interface InfraestructuraCRC {
	id: number;
	centroId: number;
	descripcion: string;
	area: number;
	capacidad: number;
	estado: string;
}

export interface ProfesionalSalud {
	id: number;
	centroId: number;
	tipoDocumento: string;
	numeroDocumento: string;
	nombres: string;
	apellidos: string;
	especialidad: string;
	registroProfesional: string;
	activo: boolean;
}

export interface InstructorCEA {
	id: number;
	centroId: number;
	tipoDocumento: string;
	numeroDocumento: string;
	nombres: string;
	apellidos: string;
	licenciaConduccion: string;
	categorias: string;
	activo: boolean;
}

export interface VehiculoCEA {
	id: number;
	centroId: number;
	placa: string;
	marca: string;
	modelo: string;
	tipoVehiculo: string;
	soatVigente: boolean;
	rtmVigente: boolean;
	estado: string;
}

export interface ProgramaConvenioCEA {
	id: number;
	centroId: number;
	nombre: string;
	institucion: string;
	fechaInicio: string;
	fechaFin: string;
	activo: boolean;
}

export interface PQRSF {
	id: number;
	centroId: number;
	tipo: string;
	asunto: string;
	descripcion: string;
	fechaRadicacion: string;
	estado: string;
	respuesta: string;
	fechaRespuesta: string;
}

export interface AcreditacionONAC {
	id: number;
	centroId: number;
	numero: string;
	fechaExpedicion: string;
	fechaVencimiento: string;
	alcance: string;
	estado: string;
}

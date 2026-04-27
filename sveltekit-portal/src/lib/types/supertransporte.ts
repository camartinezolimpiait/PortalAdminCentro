export interface RepresentanteLegal {
  tipoDocumento: string;
  numeroDocumento: string;
  nombre: string;
  apellido: string;
  telefono: string;
  email: string;
}

export interface InfoBasicaCRC {
  id: number;
  nit: string;
  razonSocial: string;
  nombreComercial: string;
  direccion: string;
  telefono: string;
  email: string;
  departamento: string;
  municipio: string;
  representanteLegal: RepresentanteLegal;
}

export interface InfoConstitucion {
  tipoEntidad: string;
  fechaConstitucion: string;
  notaria: string;
  numeroEscritura: string;
  camaraComercio: string;
  matriculaMercantil: string;
}

export interface InfoInfraestructura {
  id: number;
  tipoSede: string;
  direccion: string;
  departamento: string;
  municipio: string;
  telefono: string;
  email: string;
  capacidadInstalada: number;
}

export interface ProfesionalSalud {
  id: number;
  tipoDocumento: string;
  numeroDocumento: string;
  nombre: string;
  especialidad: string;
  registroProfesional: string;
  estado: string;
}

export interface ProfesionalCertificador {
  id: number;
  tipoDocumento: string;
  numeroDocumento: string;
  nombre: string;
  tipoCertificacion: string;
  estado: string;
}

export interface AcreditacionONAC {
  id: number;
  numeroAcreditacion: string;
  fechaExpedicion: string;
  fechaVencimiento: string;
  alcance: string;
  estado: string;
}

export interface Poliza {
  id: number;
  aseguradora: string;
  numeroPoliza: string;
  tipoPoliza: string;
  fechaInicio: string;
  fechaVencimiento: string;
  valorAsegurado: number;
  estado: string;
}

export interface ResolucionHabilitacion {
  id: number;
  numero: string;
  fecha: string;
  entidadEmisora: string;
  fechaVencimiento: string;
  estado: string;
}

export interface RegistroREPS {
  id: number;
  codigoHabilitacion: string;
  nombrePrestador: string;
  estado: string;
}

export interface InterconexionRUNT {
  id: number;
  estado: string;
  fechaConexion: string;
  ultimaSincronizacion: string;
}

export interface InfoHomologado {
  id: number;
  equipo: string;
  marca: string;
  modelo: string;
  serial: string;
  fechaHomologacion: string;
  estado: string;
}

export interface Vigilado {
  id: number;
  nit: string;
  razonSocial: string;
  tipo: string;
  estado: string;
  departamento: string;
  municipio: string;
}

export interface VigiladoFilter {
  tipo?: string;
  estado?: string;
  departamento?: string;
  municipio?: string;
  busqueda?: string;
}

export interface CentroSuper {
  id: number;
  nombre: string;
  tipo: string;
  estado: string;
  nit: string;
  direccion: string;
}

// CEA-specific types
export interface InfoBasicaCEA extends InfoBasicaCRC {}

export interface InfoVehiculoCEA {
  id: number;
  placa: string;
  marca: string;
  modelo: string;
  tipo: string;
  soatVigente: boolean;
  rtmVigente: boolean;
  fechaVencimientoSoat: string;
  fechaVencimientoRtm: string;
}

export interface InstructorCEA {
  id: number;
  tipoDocumento: string;
  numeroDocumento: string;
  nombre: string;
  categoriaLicencia: string;
  estado: string;
}

export interface PropietarioCEA {
  id: number;
  tipoDocumento: string;
  numeroDocumento: string;
  nombre: string;
  participacion: number;
}

export interface ProgramaConvenioCEA {
  id: number;
  nombre: string;
  entidad: string;
  fechaInicio: string;
  fechaFin: string;
  estado: string;
}

export interface PQRSF {
  id: number;
  tipo: string;
  asunto: string;
  descripcion: string;
  fechaCreacion: string;
  estado: string;
  prioridad: string;
}

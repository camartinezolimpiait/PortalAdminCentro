export interface PinEstadoInfo {
  pin: string;
  estado: string;
  fechaCreacion: string;
  fechaVencimiento: string;
  centro: string;
  categoria: string;
  tramite: string;
  valorPin: number;
}

export interface PinDetalle {
  pin: string;
  estado: string;
  fechaCreacion: string;
  fechaVencimiento: string;
  centro: string;
  categoria: string;
  tramite: string;
  valorPin: number;
  comprador: string;
  tipoDocumento: string;
  numeroDocumento: string;
  medioPago: string;
  transaccionId: string;
}

export interface ConsultaDispersion {
  fechaInicio: string;
  fechaFin: string;
  estado?: string;
  centro?: string;
}

export interface DispersionResponse {
  id: number;
  fecha: string;
  valor: number;
  estado: string;
  centro: string;
  cantidad: number;
}

export interface PinAsociado {
  pin: string;
  estado: string;
  fechaAsociacion: string;
  centro: string;
}

export interface DevolucionInfo {
  id: number;
  pin: string;
  estado: string;
  motivo: string;
  fecha: string;
  valor: number;
}

export interface CentroResponse {
  id: number;
  nombre: string;
  codigoRunt: number;
  estado: string;
}

export interface PagoCuotaRequest {
  pin: string;
  medioPago: string;
  valor: number;
}

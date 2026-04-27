export interface ConfiguracionHorario {
  id: number;
  dia: string;
  horaInicio: string;
  horaFin: string;
  activo: boolean;
  cuposDisponibles: number;
}

export interface ConfiguracionAgendamiento {
  id: number;
  tiempoMinimo: number;
  tiempoMaximo: number;
  anticipacionDias: number;
  cancelacionHoras: number;
  reagendamientoPermitido: boolean;
}

export interface ConfiguracionCupoRegla {
  id: number;
  tipoCita: string;
  cuposMaximos: number;
  duracionMinutos: number;
  prioridad: number;
  activo: boolean;
}

export interface AgendaResponse {
  id: number;
  fecha: string;
  hora: string;
  tipoCita: string;
  estado: string;
  paciente: string;
  documento: string;
  observaciones: string;
}

export interface ParametrizacionHorario {
  id: number;
  dia: number;
  nombreDia: string;
  horaInicio: string;
  horaFin: string;
  intervaloMinutos: number;
  activo: boolean;
}

export interface TipoCita {
  id: number;
  nombre: string;
  duracion: number;
  activo: boolean;
}

export interface NuevaAgenda {
  fecha: string;
  hora: string;
  tipoCitaId: number;
  tipoDocumento: string;
  numeroDocumento: string;
  nombre: string;
  telefono: string;
  email: string;
  observaciones?: string;
}

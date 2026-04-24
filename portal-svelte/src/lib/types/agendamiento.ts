// ===================================================================
// Agendamiento Module Types
// Converted from Data/Agendamiento/*.cs, Data/AgendaDTO.cs
// ===================================================================

/** Converted from Data/AgendaDTO.cs */
export interface AgendaDTO {
	id: number;
	fechaCita: string;
	horaCita: string;
	tipoDocumento: string;
	numeroDocumento: string;
	nombres: string;
	apellidos: string;
	estado: string;
	centroId: number;
	tramite: string;
	categoria: string;
}

/** Converted from Data/NuevaAgendaMLDTO.cs */
export interface NuevaAgendaDTO {
	centroId: number;
	fechaCita: string;
	horaCita: string;
	tipoDocumento: string;
	numeroDocumento: string;
	nombres: string;
	apellidos: string;
	tramiteId: number;
	categoriaId: number;
	celular: string;
	correo: string;
}

/** Converted from Data/Agendamiento/RequestCancelationNotification.cs */
export interface RequestCancelationNotification {
	idCita: number;
	motivo: string;
}

/** Converted from Data/Agendamiento/RequestNotificacionCitas.cs */
export interface RequestNotificacionCitas {
	centroId: number;
	fecha: string;
}

/** Converted from Data/Agendamiento/ResponseNotificacionCitas.cs */
export interface ResponseNotificacionCitas {
	totalCitas: number;
	citasPendientes: number;
	citasAtendidas: number;
	citasCanceladas: number;
}

/** Scheduling slot */
export interface IntervaloHorario {
	hora: string;
	cuposDisponibles: number;
	cuposOcupados: number;
	bloqueado: boolean;
}

/** Configuration for scheduling */
export interface ConfiguracionAgendamiento {
	id: number;
	centroId: number;
	duracionCita: number;
	cuposPorIntervalo: number;
	horaInicio: string;
	horaFin: string;
	diasHabiles: number[];
	activo: boolean;
}

/** Scheduling policy */
export interface PoliticaAgendamientoConfig {
	id: number;
	centroId: number;
	tipo: number;
	diasAnticipacion: number;
	permiteCancelacion: boolean;
	tiempoCancelacion: number;
}

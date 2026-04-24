// ===================================================================
// TypeScript Models - Converted from C# DTOs
// Maps: Data/*.cs, Data/Auth/*.cs, Data/CompraPin/*.cs, Data/Pines/*.cs
// ===================================================================

// --- Auth & Session ---

/** Converted from Data/Auth/LoginResponse.cs */
export interface LoginResponse {
	tokenBearer: string;
	message: string;
}

/** Converted from Data/Auth/SisecCredentials.cs */
export interface SisecCredentials {
	userName: string;
	passWord: string;
	plataforma: string;
}

/** Converted from Data/TokenBearer.cs */
export interface TokenBearer {
	tokenBearer: string;
	message: string;
}

/** Converted from Data/UserData.cs */
export interface UserData {
	userName: string;
	passWord: string;
	plataforma: string;
}

/** Converted from Data/UserData.cs */
export interface EmailRecoverPassResponse {
	userName: string;
	email: string;
	emailConfirmed: boolean;
}

// --- Session / App State ---

/** Session user info exposed to client via $page.data */
export interface SessionUser {
	userId: string;
	userName: string;
	firstName: string;
	plataforma: string;
	centroId: number;
	codigoRunt: number | null;
	clienteId: number;
	perfilId: number | null;
	roles: string[];
	menuItems: MenuItem[];
	menuPrincipal: MenuItem[];
	subMenuItems: MenuItem[];
	authenticated: boolean;
}

/** Converted from Data/MenuInfo.cs */
export interface MenuItem {
	id: number;
	padre: number;
	aplicacionId: number;
	esOpcionMenu: string;
	ruta: string;
	orden: number;
	activo: string;
	pagina: string;
	userId: string;
	pageName: string;
	menuName: string;
	iconoNombre: string;
}

/** Converted from Data/Modulo.cs */
export interface Modulo {
	id: number;
	padre: number | null;
	aplicacionId: number;
	esOpcionMenu: boolean;
	ruta: string;
	orden: number;
	activo: boolean;
	pagina: string;
}

/** Converted from Data/Perfil.cs */
export interface Perfil {
	perfilId: number;
	nombre: string;
	clienteId: number;
	aplicacionId: number;
	esPerfilSistema: boolean;
	activo: boolean;
	transaccionGuid: string;
}

/** Converted from Data/ClienteDTO.cs */
export interface ClienteDTO {
	id: number;
	nombre: string;
	applicationId: string;
}

// --- User / Centro ---

/** Converted from Data/User.cs (Centro info) */
export interface Centro {
	idCentro: number;
	nombre: string;
	codigoUso: string;
	idComercio: number;
	idDepartamento: number;
	idMunicipio: number;
	idZona: number;
	direccion: string;
	email: string;
	fijo: string;
	movil: string;
	latitud: number;
	longitud: number;
	concurrencia: number;
	patrimonioAutonomo: boolean | null;
	activoRUNT: boolean | null;
	activo: boolean | null;
	created: string;
	createdBy: string;
	modified: string;
	modifiedBy: string;
	rowVersion: number;
	applicationUser: string | null;
	matriculaMercantil: string;
	resolucion: string;
	numeroRegistro: string;
	capacidadMaxima: number;
	convenioBanco: string;
	valorSugerido: string;
	valorComision: string;
	porcentajeIva: string | null;
	ipAutorizada: string | null;
	recaudo: boolean;
	permitirACH: boolean;
	cupo: boolean;
	valorMinPago: number | null;
	valorMaxPago: number | null;
	codigoRUNT: number | null;
	activarCV: number;
	activoFacturacion: boolean;
	paramScoreHuella: number;
	validaCotejoHuellas: number;
	saveHuellas: number;
	antFraude: number;
	ocr: number;
	poActivo: number;
	version: number | null;
	valorCD: number;
	validaLockCentro: number;
	onDispositivos: number;
	alertaValMan: number;
	onValManual: number;
}

// --- Catalogs / Master Data ---

/** Converted from Data/TipoDocumentoDTO.cs */
export interface TipoDocumentoDTO {
	id: number;
	nombre: string;
	abreviatura: string;
	activo: boolean;
}

/** Converted from Data/DepartamentoDTO.cs */
export interface DepartamentoDTO {
	id: number;
	nombre: string;
	codigoDane: string;
}

/** Converted from Data/Municipio.cs + Data/CiudadDTO.cs */
export interface MunicipioDTO {
	id: number;
	nombre: string;
	departamentoId: number;
	codigoDane: string;
}

/** Converted from Data/CategoriaDTO.cs */
export interface CategoriaDTO {
	id: number;
	nombre: string;
	descripcion: string;
	activo: boolean;
}

/** Converted from Data/SexoDTO.cs */
export interface SexoDTO {
	id: number;
	nombre: string;
}

/** Converted from Data/GrupoSanguineoDTO.cs */
export interface GrupoSanguineoDTO {
	id: number;
	nombre: string;
}

/** Converted from Data/EpsDTO.cs */
export interface EpsDTO {
	id: number;
	nombre: string;
}

/** Converted from Data/EstadoCivilDTO.cs */
export interface EstadoCivilDTO {
	id: number;
	nombre: string;
}

/** Converted from Data/GradoEscolaridadDTO.cs */
export interface GradoEscolaridadDTO {
	id: number;
	nombre: string;
}

/** Converted from Data/OcupacionDTO.cs */
export interface OcupacionDTO {
	id: number;
	nombre: string;
}

/** Converted from Data/RegimenAfiliacionDTO.cs */
export interface RegimenAfiliacionDTO {
	id: number;
	nombre: string;
}

/** Converted from Data/TipoCitaDTO.cs */
export interface TipoCitaDTO {
	id: number;
	nombre: string;
}

/** Converted from Data/TramiteDTO.cs */
export interface TramiteDTO {
	id: number;
	nombre: string;
	descripcion: string;
	activo: boolean;
}

/** Converted from Data/MotivoDTO.cs */
export interface MotivoDTO {
	id: number;
	nombre: string;
}

/** Converted from Data/OrigenPinDTO.cs */
export interface OrigenPinDTO {
	id: number;
	nombre: string;
}

// --- Credenciales ---

/** Converted from Data/Credenciales.cs */
export interface Credenciales {
	userName: string;
	userPassword: string;
}

// --- API Response ---

/** Converted from Data/ApiResponse.cs / Data/Response.cs */
export interface ApiResponse<T = unknown> {
	success: boolean;
	message: string;
	data: T;
	statusCode: number;
}

/** Converted from Data/ResponseBody.cs */
export interface ResponseBody {
	response: string;
	statusCode: number;
}

/** Converted from Data/ResponseResult.cs */
export interface ResponseResult {
	result: string;
}

/** Converted from Data/RespGeneralModel.cs */
export interface RespGeneralModel {
	esExitoso: boolean;
	mensaje: string;
	resultado: unknown;
}

export interface DatosBasicosCompra {
  tipoDocumento: string;
  numeroDocumento: string;
  nombre: string;
  apellido: string;
  email: string;
  telefono: string;
  departamento: string;
  municipio: string;
}

export interface CotizacionPin {
  categoria: string;
  tramite: string;
  valorBase: number;
  valorIva: number;
  valorTotal: number;
  descuento: number;
}

export interface CostoCuota {
  numeroCuota: number;
  valorCuota: number;
  fechaVencimiento: string;
}

export interface MedioPago {
  id: number;
  nombre: string;
  tipo: string;
  activo: boolean;
  logoUrl?: string;
}

export interface Banco {
  id: number;
  nombre: string;
  codigo: string;
}

export interface PagoPin {
  pin: string;
  medioPagoId: number;
  bancoId?: number;
  valorTotal: number;
  referencia: string;
}

export interface SeleccionTramite {
  id: number;
  nombre: string;
  tipo: string;
  requiereCategoria: boolean;
}

export interface CategoriaPorCentro {
  id: number;
  nombre: string;
  codigo: string;
  activa: boolean;
}

export interface PasoCompraPin {
  paso: number;
  nombre: string;
  completado: boolean;
  activo: boolean;
}

export interface Convenio {
  id: number;
  nombre: string;
  descuento: number;
  fechaInicio: string;
  fechaFin: string;
  activo: boolean;
}

export interface DiscriminadoValorPin {
  concepto: string;
  valor: number;
  porcentaje: number;
}

export interface ValidateCouponRequest {
  codigo: string;
  centroId: number;
  categoriaId: number;
}

export interface ValidateCouponResponse {
  valido: boolean;
  descuento: number;
  mensaje: string;
  empresa: string;
}

export interface TransactionInfo {
  referencia: string;
  estado: string;
  fecha: string;
  valor: number;
  medioPago: string;
}

export interface WompiResponse {
  referencia: string;
  urlPago: string;
  estado: string;
}

export interface DatosBasicosCDA extends DatosBasicosCompra {
  tipoVehiculo?: string;
  placa?: string;
}

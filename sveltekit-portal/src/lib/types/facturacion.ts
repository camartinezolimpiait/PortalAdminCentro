export interface ConfiguracionFacturacion {
  id: number;
  proveedorFacturacion: string;
  estado: string;
  fechaConfiguracion: string;
}

export interface DatosEmision {
  id: number;
  resolucionDian: string;
  prefijo: string;
  rangoInicio: number;
  rangoFin: number;
  fechaResolucion: string;
  fechaVencimiento: string;
}

export interface DatosArticulo {
  id: number;
  codigo: string;
  nombre: string;
  descripcion: string;
  precio: number;
  impuesto: number;
  activo: boolean;
}

export interface CredencialesProveedor {
  id: number;
  proveedor: string;
  apiKey: string;
  apiSecret: string;
  ambiente: string;
  estado: string;
}

export interface DatosNumeracion {
  id: number;
  prefijo: string;
  consecutivoActual: number;
  rangoInicio: number;
  rangoFin: number;
}

export interface DatosComportamiento {
  id: number;
  emisionAutomatica: boolean;
  envioCorreo: boolean;
  copiaOculta: string;
  formatoImpresion: string;
}

export interface EstadoFacturacion {
  id: number;
  nombre: string;
  descripcion: string;
  activo: boolean;
}

export interface DetalleFacturacion {
  id: number;
  numeroFactura: string;
  fecha: string;
  cliente: string;
  nit: string;
  total: number;
  estado: string;
  pin: string;
}

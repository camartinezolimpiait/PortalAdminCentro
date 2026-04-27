/** Generic API response - mapped from RespuestaServicios/ApiResponse */
export interface ApiResponse<T = unknown> {
  exito: boolean;
  mensaje: string;
  datos: T;
  codigo?: number;
}

export interface BasicResponse {
  success: boolean;
  message: string;
}

export interface PaginatedResult<T> {
  items: T[];
  totalItems: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

export interface FileContentResult {
  content: string;
  contentType: string;
  fileName: string;
}

export interface ColumnDefinition {
  field: string;
  header: string;
  sortable?: boolean;
  filterable?: boolean;
  width?: string;
  type?: 'text' | 'number' | 'date' | 'boolean' | 'currency';
}

export interface RequestFilter {
  page: number;
  pageSize: number;
  sortField?: string;
  sortDirection?: 'asc' | 'desc';
  filters?: Record<string, string>;
}

export interface FileBase64 {
  fileName: string;
  base64Content: string;
  contentType: string;
}

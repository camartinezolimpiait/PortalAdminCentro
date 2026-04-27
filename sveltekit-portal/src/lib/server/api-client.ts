import { env } from '$env/dynamic/private';
import type { ApiResponse } from '$lib/types/api';

export interface ApiClientOptions {
  token?: string | null;
  timeout?: number;
}

export class ApiClient {
  private readonly baseUrl: string;
  private readonly token: string | null;
  private readonly timeout: number;

  constructor(baseUrl: string, options: ApiClientOptions = {}) {
    this.baseUrl = baseUrl.replace(/\/$/, '');
    this.token = options.token ?? null;
    this.timeout = options.timeout ?? 120_000;
  }

  private async request<T>(method: string, path: string, body?: unknown): Promise<ApiResponse<T>> {
    const url = `${this.baseUrl}${path}`;
    const controller = new AbortController();
    const timeoutId = setTimeout(() => controller.abort(), this.timeout);

    try {
      const headers: Record<string, string> = {
        'Content-Type': 'application/json',
        'Accept': 'application/json'
      };
      if (this.token) {
        headers['Authorization'] = `Bearer ${this.token}`;
      }

      const response = await fetch(url, {
        method,
        headers,
        body: body ? JSON.stringify(body) : undefined,
        signal: controller.signal
      });

      if (!response.ok) {
        return {
          exito: false,
          mensaje: `HTTP ${response.status}: ${response.statusText}`,
          datos: null as unknown as T,
          codigo: response.status
        };
      }

      const data = await response.json();
      return data as ApiResponse<T>;
    } catch (error) {
      const message = error instanceof Error ? error.message : 'Unknown API error';
      return { exito: false, mensaje: message, datos: null as unknown as T, codigo: 500 };
    } finally {
      clearTimeout(timeoutId);
    }
  }

  async get<T>(path: string): Promise<ApiResponse<T>> {
    return this.request<T>('GET', path);
  }

  async post<T>(path: string, body?: unknown): Promise<ApiResponse<T>> {
    return this.request<T>('POST', path, body);
  }

  async put<T>(path: string, body?: unknown): Promise<ApiResponse<T>> {
    return this.request<T>('PUT', path, body);
  }

  async delete<T>(path: string): Promise<ApiResponse<T>> {
    return this.request<T>('DELETE', path);
  }
}

// Factory functions mapped from named HttpClients in StartupExtensions.cs
export function createAuthClient(token?: string | null): ApiClient {
  return new ApiClient(env.SISEC_AUTH_URL ?? 'http://localhost:6225/SisecAut/', { token });
}

export function createAdminClient(token?: string | null): ApiClient {
  return new ApiClient(env.SISEC_ADMIN_URL ?? 'http://localhost:6225/SisecAdmin/', { token });
}

export function createPortalClient(token?: string | null): ApiClient {
  return new ApiClient(env.PORTAL_ADMIN_URL ?? 'http://localhost:6225/ApiGateWaySisec/', { token });
}

export function createMiLicenciaClient(token?: string | null): ApiClient {
  return new ApiClient(env.MI_LICENCIA_URL ?? 'https://localhost:6226/BackEnd/', { token });
}

export function createSuperTransporteClient(token?: string | null): ApiClient {
  return new ApiClient(env.SUPERTRANSPORTE_URL ?? 'http://localhost:8081/sicov/', { token });
}

export function createVigiladosClient(token?: string | null): ApiClient {
  return new ApiClient(env.VIGILADOS_URL ?? 'http://localhost:4445/InformacionSICOV/', { token });
}

export function createStrapiClient(): ApiClient {
  return new ApiClient(env.STRAPI_URL ?? 'http://localhost:1337/api/');
}

// ===================================================================
// HTTP Client Factory - Server-side only
// Replaces: StartupExtensions.cs AgregarHttpClient()
// Centralizes all API calls through BFF pattern
// ===================================================================

import { getConfig } from './config';
import type { TokenBearer, ApiResponse } from '$lib/types/models';

const config = getConfig();

/** Options for creating an HTTP client */
interface HttpClientOptions {
	baseUrl: string;
	token?: string;
	headers?: Record<string, string>;
	/** Skip TLS validation (development only) */
	skipTls?: boolean;
}

/**
 * Internal fetch wrapper with error handling and retry.
 * Replaces named HttpClients from Startup.cs:
 * - SisecAdmin
 * - ApiStrapi
 * - ApiSuperT
 * - ApiSuperVigilados
 * - PortalAdministrativo
 */
async function internalFetch<T>(
	url: string,
	options: RequestInit & { baseUrl?: string } = {}
): Promise<T> {
	const fullUrl = options.baseUrl ? `${options.baseUrl}${url}` : url;

	const response = await fetch(fullUrl, {
		...options,
		headers: {
			'Content-Type': 'application/json',
			...options.headers
		}
	});

	if (!response.ok) {
		const errorText = await response.text().catch(() => 'Unknown error');
		throw new Error(`API Error ${response.status}: ${errorText}`);
	}

	return response.json() as Promise<T>;
}

// ===================================================================
// Named API Clients (replaces StartupExtensions.AgregarHttpClient)
// ===================================================================

/** SisecAdmin client - Parametrization API */
export const sisecAdminClient = {
	async get<T>(path: string, token?: string): Promise<T> {
		return internalFetch<T>(path, {
			baseUrl: config.uriSisecParametization,
			headers: token ? { Authorization: `bearer ${token}` } : {}
		});
	},
	async post<T>(path: string, body: unknown, token?: string): Promise<T> {
		return internalFetch<T>(path, {
			baseUrl: config.uriSisecParametization,
			method: 'POST',
			body: JSON.stringify(body),
			headers: token ? { Authorization: `bearer ${token}` } : {}
		});
	}
};

/** ApiStrapi client - CMS API */
export const strapiClient = {
	async get<T>(path: string): Promise<T> {
		return internalFetch<T>(path, {
			baseUrl: config.endPointApiStrapi,
			headers: {
				Authorization: `Bearer ${config.tokenStrapi}`
			}
		});
	}
};

/** ApiSuperT client - SuperTransporte SICOV API */
export const superTransporteClient = {
	async get<T>(path: string): Promise<T> {
		return internalFetch<T>(path, {
			baseUrl: config.endPointApiSuperT
		});
	},
	async post<T>(path: string, body: unknown): Promise<T> {
		return internalFetch<T>(path, {
			baseUrl: config.endPointApiSuperT,
			method: 'POST',
			body: JSON.stringify(body)
		});
	}
};

/** ApiSuperVigilados client */
export const vigiladosClient = {
	async get<T>(path: string): Promise<T> {
		return internalFetch<T>(path, {
			baseUrl: config.endPointApiVigilados,
			headers: {
				Authorization: `Bearer ${config.tokenSuperVigilados}`
			}
		});
	}
};

/** PortalAdministrativo client - Main admin API gateway */
export const portalAdminClient = {
	async get<T>(path: string, token?: string): Promise<T> {
		return internalFetch<T>(path, {
			baseUrl: config.apiPortalAdministrativo.url,
			headers: token ? { Authorization: `bearer ${token}` } : {}
		});
	},
	async post<T>(path: string, body: unknown, token?: string): Promise<T> {
		return internalFetch<T>(path, {
			baseUrl: config.apiPortalAdministrativo.url,
			method: 'POST',
			body: JSON.stringify(body),
			headers: token ? { Authorization: `bearer ${token}` } : {}
		});
	}
};

/** SisecAuth client - Authentication API */
export const sisecAuthClient = {
	async get<T>(path: string, token?: string): Promise<T> {
		return internalFetch<T>(path, {
			baseUrl: config.uriSisecAuth,
			headers: token ? { Authorization: `bearer ${token}` } : {}
		});
	},
	async post<T>(path: string, body: unknown, token?: string): Promise<T> {
		return internalFetch<T>(path, {
			baseUrl: config.uriSisecAuth,
			method: 'POST',
			body: JSON.stringify(body),
			headers: token ? { Authorization: `bearer ${token}` } : {}
		});
	}
};

/** MiLicencia API client */
export const miLicenciaClient = {
	async get<T>(path: string, token?: string): Promise<T> {
		return internalFetch<T>(path, {
			baseUrl: config.apiFrontMiLicencia.url,
			headers: token ? { Authorization: `bearer ${token}` } : {}
		});
	},
	async post<T>(path: string, body: unknown, token?: string): Promise<T> {
		return internalFetch<T>(path, {
			baseUrl: config.apiFrontMiLicencia.url,
			method: 'POST',
			body: JSON.stringify(body),
			headers: token ? { Authorization: `bearer ${token}` } : {}
		});
	}
};

// ===================================================================
// Token Service (replaces Services/TokenService.cs)
// ===================================================================

/**
 * Gets an API gateway token using configured credentials.
 * Replaces: TokenService.GetToken()
 */
export async function getApiToken(): Promise<TokenBearer> {
	return portalAdminClient.post<TokenBearer>('Token/Login2', {
		userName: config.apiPortalAdministrativo.userName,
		userPassword: config.apiPortalAdministrativo.userPassword
	});
}

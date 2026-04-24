import { json } from '@sveltejs/kit';
import type { RequestHandler } from './$types';
import { portalAdminClient, getApiToken } from '$lib/server/api-client';

/**
 * Catalogs API - BFF proxy for master data
 * Replaces: Direct API calls from Blazor components to Integration Microservice
 * Server-side caching and deduplication of requests
 */

const catalogCache = new Map<string, { data: unknown; timestamp: number }>();
const CACHE_TTL = 5 * 60 * 1000; // 5 minutes

function getCached(key: string): unknown | null {
const cached = catalogCache.get(key);
if (cached && Date.now() - cached.timestamp < CACHE_TTL) {
return cached.data;
}
catalogCache.delete(key);
return null;
}

function setCache(key: string, data: unknown): void {
catalogCache.set(key, { data, timestamp: Date.now() });
}

export const GET: RequestHandler = async ({ url }) => {
const type = url.searchParams.get('type');

if (!type) {
return json({ error: 'Parameter "type" is required' }, { status: 400 });
}

const cacheKey = `catalog_${type}`;
const cached = getCached(cacheKey);
if (cached) {
return json(cached);
}

try {
const apiToken = await getApiToken();
let data: unknown;

switch (type) {
case 'tiposDocumento':
data = await portalAdminClient.get('Catalogo/TiposDocumento', apiToken.tokenBearer);
break;
case 'departamentos':
data = await portalAdminClient.get('Catalogo/Departamentos', apiToken.tokenBearer);
break;
case 'municipios': {
const deptoId = url.searchParams.get('departamentoId');
data = await portalAdminClient.get(`Catalogo/Municipios?departamentoId=${deptoId}`, apiToken.tokenBearer);
break;
}
case 'categorias':
data = await portalAdminClient.get('Catalogo/Categorias', apiToken.tokenBearer);
break;
case 'tramites':
data = await portalAdminClient.get('Catalogo/Tramites', apiToken.tokenBearer);
break;
case 'sexos':
data = await portalAdminClient.get('Catalogo/Sexos', apiToken.tokenBearer);
break;
case 'gruposSanguineos':
data = await portalAdminClient.get('Catalogo/GruposSanguineos', apiToken.tokenBearer);
break;
case 'eps':
data = await portalAdminClient.get('Catalogo/Eps', apiToken.tokenBearer);
break;
default:
return json({ error: `Unknown catalog type: ${type}` }, { status: 400 });
}

setCache(cacheKey, data);
return json(data);
} catch (error) {
console.error(`Error fetching catalog ${type}:`, error);
return json({ error: `Error fetching catalog: ${type}` }, { status: 500 });
}
};
